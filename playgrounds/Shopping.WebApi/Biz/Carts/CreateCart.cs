﻿using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Carts;

public sealed record CreateCart : IHttpRequesting<CartVm>;

internal sealed class CreateCartHandler(ICorrelationContext context,
    IRepository<User> userRepository,
    IRepository<Cart> cartRepository,
    IMapper mapper)
    : HttpRequestHandlerAsync<CreateCart, CartVm>
{
    public override async Task<IHttpResponse<CartVm>> HandleAsync(CreateCart request, CancellationToken cancellationToken = default)
    {
        if (context.CurrentUserId is null)
        {
            return Response(HttpStatusCode.Unauthorized);
        }

        var user = await userRepository.GetByIdAsync(context.CurrentUserId, cancellationToken);
        if (user is null)
        {
            return Response(HttpStatusCode.NotFound, "User not found");
        }

        var entity = new Cart
        {
            Id = Guid.NewGuid().ToString(),
            Owner = user,
        };
        await cartRepository.InsertAsync(entity, cancellationToken);
        return Response(HttpStatusCode.Created, mapper.Map<CartVm>(entity));
    }
}