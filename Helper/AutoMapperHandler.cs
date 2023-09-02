using System;
using AutoMapper;
using demo_api.Modal;
using LearnAPI.Repos.Models;

namespace demo_api.Helper
{
	public class AutoMapperHandler: Profile
	{
        public AutoMapperHandler()
        {
            CreateMap<TblCustomer, CustomerModal>().ForMember(item => item.Statusname, opt => opt.MapFrom(
                item => (item.IsActive != null && item.IsActive.Value) ? "Active" : "In active")).ReverseMap();
        }
    }
}

