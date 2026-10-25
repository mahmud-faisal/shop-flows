using Microsoft.AspNetCore.Mvc;

namespace ShopFlows.Api.Catalog;

public sealed record CreateProductRequest(string Sku, string Name, decimal Price, Guid CategoryId);
public sealed record UpdateProductRequest(string Name,decimal Price,Guid CategoryId,bool IsActive);

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<ProductDto>> List([FromServices] ListProductsHandler handler, CancellationToken cancellationToken) => await handler.HandleAsync(cancellationToken);
   
    

    
}
