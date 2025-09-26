using Microsoft.Extensions.DependencyInjection;

namespace {{ProjectName}}.ClientShared.Extensions
{
    public static class FeaturesRegistrationExt
    {
        public static void AddClientSideFeatureServices(this IServiceCollection services)
        {
            // Products
            services.AddScoped<ServiceContracts.Features.Products.IProductListingDataService, Features.Products.ProductListingClientDataService>();
            services.AddScoped<ServiceContracts.Features.Products.IProductFormDataService, Features.Products.ProductFormClientDataService>();

            //##ClientDataService##

        }
    }
}
