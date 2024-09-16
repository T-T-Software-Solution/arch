using AutoMapper;
using Sample07.Basic.ConsoleApp.ModelMapping.Models;
using Sample07.Basic.ConsoleApp.ModelMapping.ViewModels;

namespace Sample07.Basic.ConsoleApp.ModelMapping;

sealed class MappingProfileRegistrar : Profile
{
    public MappingProfileRegistrar()
    {
        CreateMap<Easy, EasyVm>();

        CreateMap<Normal, NormalVm>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => DateTime.Now.Year - src.BirthDate.Year));
    }
}