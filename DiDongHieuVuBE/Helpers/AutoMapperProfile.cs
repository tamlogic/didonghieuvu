using AutoMapper;
using ManageEmployee.Entities;
using ManageEmployee.Models;
using ManageEmployee.ViewModels;

namespace ManageEmployee.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            AllowNullCollections = true;
            AllowNullDestinationValues = true;
            CreateMap<RegisterModel, User>();
            CreateMap<UserModel, User>();
            CreateMap<RegisterLaborWebModel, User>();
            CreateMap<Province, ProvinceModel>(); 
            CreateMap<ProvinceModel, Province>();
            CreateMap<Menu, MenuViewModel>().ReverseMap();
            CreateMap<MenuRole, MenuRoleViewModel>().ReverseMap();
            CreateMap<User, UserActiveModel>();
            CreateMap<Print, PrintViewModel>();
        }
    }
}

