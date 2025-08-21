namespace BRGateway24.Helpers
{
    public static class ServiceCollectionExtension
    {
        public static TConfig ConfigureAppSettings<TConfig>(this IServiceCollection services, IConfiguration configuration) where TConfig : class, new()
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var config = new TConfig();
            configuration.Bind(config); 
            services.AddSingleton(config);
            return config;
        }
    }
}
