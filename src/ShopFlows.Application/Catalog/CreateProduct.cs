namespace ShopFlows.Application.Catalog;

public sealed record CreateProductCommand(string Sku, string Name, decimal price, Guid CategoryId);

public sealed class CreateProductHandler
{
    public CreateProductHandler(IProductRepository products, IRepository<Category> categories, IUnitOfWork unitOfWork, IOptions<StoreOptions>)
    {

        public async Task<ProductDto> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        if (!await categories.ExistsAsync(command.CategoryId, CancellationToken))
            throw new RequestValidationException("$Category `{command.CategoryId}` doesn't exist.");

        var price = new Money(command.Price, storeOptions.Value.Currency);
        
        // var product=

        }

    }
    
}
