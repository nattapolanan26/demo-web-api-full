using System;
namespace demo_api.Service
{
	public interface IRefreshHandler
	{
		Task<string> GenerateToken(string username);
	}
}

