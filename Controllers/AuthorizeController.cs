using System;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Text;
using demo_api.Modal;
using demo_api.Service;
using LearnAPI.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace demo_api.Controllers
{
	public class AuthorizeController: ControllerBase
	{
		private readonly LearndataContext context;
		private readonly JwtSettings jwtSettings;
		private readonly IRefreshHandler refresh;

        public AuthorizeController(LearndataContext context, IOptions<JwtSettings> options, IRefreshHandler refresh)
		{
			this.context = context;
			this.jwtSettings = options.Value;
			this.refresh = refresh;
		}

		[HttpPost("GenerateToken")]
		public async Task<IActionResult> GenerateToken([FromBody] UserCred data)
		{

            var user = await this.context.TblUsers.FirstOrDefaultAsync(item => item.Code == data.username && item.Password == data.password);
			if (user != null)
			{
				// geneate token
				var tokenhandler = new JwtSecurityTokenHandler();
				var tokenKey = Encoding.UTF8.GetBytes(this.jwtSettings.securityKey);
				var tokendesc = new SecurityTokenDescriptor
				{
					Subject = new ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.Name, user.Code),
						new Claim(ClaimTypes.Role, user.Role)
					}),
					Expires = DateTime.UtcNow.AddSeconds(30),
					SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
				};
				var token = tokenhandler.CreateToken(tokendesc);
				var finaltoken = tokenhandler.WriteToken(token);

				return Ok(new TokenResponse() {  Token = finaltoken, RefreshToken = await this.refresh.GenerateToken(data.username) });

			}
			else
			{
				return Unauthorized();
			}
		}


        [HttpPost("GenerateRefreshToken")]
        public async Task<IActionResult> GenerateRefreshToken([FromBody] TokenResponse token)
        {

            var _refreshToken = await this.context.TblRefreshtokens.FirstOrDefaultAsync(item => item.Refreshtoken == token.RefreshToken);
            if (_refreshToken != null)
            {
                // geneate token
                var tokenhandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(this.jwtSettings.securityKey);

				SecurityToken securityToken;

				var principal = tokenhandler.ValidateToken(token.Token, new TokenValidationParameters()
				{
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out securityToken);

				var _token = securityToken as JwtSecurityToken;
				if (_token != null && _token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
				{
					string username = principal.Identity?.Name;

					var _existdata = await this.context.TblRefreshtokens.FirstOrDefaultAsync(item => item.Userid == username
					&& item.Refreshtoken == token.RefreshToken);

					if(_existdata != null)
					{
						var _newtoken = new JwtSecurityToken(
							claims: principal.Claims.ToArray(),
							expires: DateTime.Now.AddSeconds(30),
							signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtSettings.securityKey)), SecurityAlgorithms.HmacSha256)
						);

						var _finalToken = tokenhandler.WriteToken(_newtoken);
						return Ok(new TokenResponse() { Token = _finalToken, RefreshToken = await this.refresh.GenerateToken(username) });
					}
					else
					{
						return Unauthorized();
					}
				}
				else
				{
					return Unauthorized();
				}
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}

