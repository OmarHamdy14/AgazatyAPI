using Agazaty.Data.DTOs.AccountDTOs;
using Agazaty.Data.DTOs.CasualLeaveDTOs;
using Agazaty.Data.DTOs.DepartmentDTOs;
using Agazaty.Data.DTOs.NormalLeaveDTOs;
using Agazaty.Data.DTOs.PermitLeavesDTOs;
using Agazaty.Data.DTOs.RoleDTOs;
using Agazaty.Data.DTOs.SickLeaveDTOs;
using Agazaty.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Agazaty
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<PermitLeave, CreatePermitLeaveDTO>().ReverseMap();
            CreateMap<PermitLeave, UpdatePermitLeaveDTO>().ReverseMap();
            CreateMap<PermitLeave, PermitLeaveDTO>().ReverseMap();

            CreateMap<CasualLeave, CreateCasualLeaveDTO>().ReverseMap();
            CreateMap<CasualLeave, UpdateCasualLeaveDTO>().ReverseMap();
            CreateMap<CasualLeave, CasualLeaveDTO>().ReverseMap();

            CreateMap<SickLeave, UpdateSickLeaveDTO>().ReverseMap();
            CreateMap<SickLeave, CreateSickLeaveDTO>().ReverseMap();

            CreateMap<NormalLeave, CreateNormalLeaveDTO>().ReverseMap();
            CreateMap<NormalLeave, UpdateNormalLeaveDTO>().ReverseMap();

            CreateMap<Department, CreateDepartmentDTO>().ReverseMap();
            CreateMap<Department, UpdateDepartmentDTO>().ReverseMap();

            CreateMap<ApplicationUser, CreateUserDTO>().ReverseMap();
            CreateMap<ApplicationUser, UpdateUserDTO>().ReverseMap();

            CreateMap<IdentityRole, CreateRoleDTO>().ReverseMap();
            CreateMap<IdentityRole, UpdateRoleDTO>().ReverseMap();






            //CreateMap<CreateUserDTO, ApplicationUser>()
            //    .ForMember(dest => dest.UserName, op => op.MapFrom(src => src.UserName))
            //    .ForMember(dest => dest.PhoneNumber, op => op.MapFrom(src => src.PhoneNumber))
            //    .ForMember(dest => dest.Gender, op => op.MapFrom(src => src.Gender))
            //    .ForMember(dest => dest.Email, op => op.MapFrom(src => src.Email))
            //    .ForMember(dest => dest.DateOfBirth, op => op.MapFrom(src => src.DateOfBirth))
            //    .ForMember(dest => dest.Position, op => op.MapFrom(src => src.Position))
            //    .ForMember(dest => dest.HireDate, op => op.MapFrom(src => src.HireDate))
            //    .ForMember(dest => dest.NationalID, op => op.MapFrom(src => src.NationalID))
            //    .ForMember(dest => dest.Departement_ID, op => op.MapFrom(src => src.Departement_ID));
        }
    }
}
