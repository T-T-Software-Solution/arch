using AutoMapper;
using Sample16.RemoteRequest.Shared.Messages;
using Sample16.RemoteRequest.Shared.ViewModels;

namespace Sample16.RemoteRequest.Shared;

public sealed class MappingProfileRegistrar : Profile
{
    public MappingProfileRegistrar()
    {
        CreateMap<Pong, PongVm>();
    }
}