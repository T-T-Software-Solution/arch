using AutoMapper;
using Shipping.Shared.Entities;
using Shipping.Shared.Entities.ViewModels;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.WebApi.Biz.Products;

public sealed record ListProducts : IRequesting<IEnumerable<ProductVm>>;

internal sealed class ListProductHandler(IRepository<Product> repository, IMapper mapper) : RequestHandler<ListProducts, IEnumerable<ProductVm>>
{
    public override IEnumerable<ProductVm> Handle(ListProducts request)
    {
        return repository.Get().Select(mapper.Map<ProductVm>);
    }
}