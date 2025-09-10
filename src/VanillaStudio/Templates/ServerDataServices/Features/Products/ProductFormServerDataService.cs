using {{ProjectName}}.Server.Data;
using {{ProjectName}}.ServiceContracts.Features.Products;
using Microsoft.EntityFrameworkCore;

namespace {{ProjectName}}.Server.DataServices.Features.Products
{
    internal class ProductFormServerDataService : IProductFormDataService
    {
        private readonly AppDbContext context;

        public ProductFormServerDataService(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<ProductFormBusinessModel> GetByIdAsync(string id)
        {
            return (await (from p in context.Products
                           where p.Id == id
                           select new ProductFormBusinessModel
                           {
                               Id = p.Id,
                               Name = p.Name,
                               Description = p.Description,
                               Price = p.Price,
                               ProductStatus = p.ProductStatus
                           }).FirstAsync()) ?? throw new Exception("Product not found");
        }

        public async Task<string> CreateAsync(ProductFormBusinessModel formViewModel)
        {
            if (context.Products.Any(p => p.Id == formViewModel.Id))
                throw new Exception("Product with the same Id already exists.");

            var product = new Product
            {
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system", // This should be replaced with the actual user
                Name = formViewModel.Name,
                Description = formViewModel.Description,
                Price = formViewModel.Price ?? 0,
                ProductStatus = formViewModel.ProductStatus
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();
            return product.Id;
        }

        public async Task<string> UpdateAsync(string id, ProductFormBusinessModel formViewModel)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                throw new Exception("Product not found.");
            }
            product.Name = formViewModel.Name;
            product.Description = formViewModel.Description;
            product.Price = formViewModel.Price ?? 0;
            product.ProductStatus = formViewModel.ProductStatus;
            await context.SaveChangesAsync();
            return product.Id;
        }

        public async Task<int> DeleteAsync(string id)
        {
            return await context.Products.Where(p => p.Id == id)
                .ExecuteDeleteAsync();

        }
    }
}
