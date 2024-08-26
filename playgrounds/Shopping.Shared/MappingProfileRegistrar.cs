using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;

namespace Shopping.Shared;

public sealed class MappingProfileRegistrar : Profile
{
    public MappingProfileRegistrar()
    {
        CreateMap<Cart, CartVm>()
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => $"{src.Owner.FirstName} {src.Owner.LastName}"));
    }
}