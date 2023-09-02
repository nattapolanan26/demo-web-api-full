using System;
using demo_api.Modal;
using demo_api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace demo_api.Controllers
{
    [Authorize]
    //[DisableCors]
    [EnableRateLimiting("fixedwindow")]
	[Route("api/[controller]")]
	[ApiController]
	public class CustomerController: ControllerBase
	{
		private readonly ICustomerService service;
		public CustomerController(ICustomerService service)
		{
			this.service = service;
		}

        [AllowAnonymous]
        //[EnableCors("corspolicy1")]
        [HttpGet("GetAll")]
		public async Task<IActionResult> GetAll()
		{
			var data = await this.service.GetAll();

			if(data == null)
			{
				return NotFound();
			}

			return Ok(data);
		}

        [DisableRateLimiting]
        [HttpGet("Getbycode")]
        public async Task<IActionResult> Getbycode(string code)
        {
            var data = await this.service.Getbycode(code);

            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CustomerModal _data)
        {
            var data = await this.service.Create(_data);

            return Ok(data);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(CustomerModal _data, string code)
        {
            var data = await this.service.Update(_data, code);

            return Ok(data);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string code)
        {
            var data = await this.service.Delete(code);

            return Ok(data);
        }
    }
}

