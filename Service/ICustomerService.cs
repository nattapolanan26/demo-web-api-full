using System;
using demo_api.Helper;
using demo_api.Modal;
using LearnAPI.Repos.Models;

namespace demo_api.Service
{
	public interface ICustomerService
	{
        Task<List<CustomerModal>> GetAll();

        Task<CustomerModal> Getbycode(string code);

        Task<APIResponse> Delete(string code);

        Task<APIResponse> Create(CustomerModal data);

        Task<APIResponse> Update(CustomerModal data, string code);
    }
}

