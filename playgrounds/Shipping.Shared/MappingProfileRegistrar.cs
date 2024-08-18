using AutoMapper;
using Shipping.Shared.Entities;
using Shipping.Shared.Entities.ViewModels;

namespace Shipping.Shared;

public sealed class MappingProfileRegistrar : Profile
{
    public MappingProfileRegistrar()
    {
        CreateMap<Cart, CartVm>()
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => $"{src.Owner.FirstName} {src.Owner.LastName}"));
    }
}