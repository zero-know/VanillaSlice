using Microsoft.Extensions.DependencyInjection;

namespace {{ProjectName}}.Server.DataServices.Extensions
{
    public static class FeaturesRegistrationExt
    {
        public static void AddServerSideFeatureServices(this IServiceCollection services)
        {
            // Products
            services.AddScoped<ServiceContracts.Features.Products.IProductListingDataService, Features.Products.ProductListingServerDataService>();
            services.AddScoped<ServiceContracts.Features.Products.IProductFormDataService, Features.Products.ProductFormServerDataService>();

            //##ServerDataService##
        }
    }
}
