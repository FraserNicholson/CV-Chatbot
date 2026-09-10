using CVChatbotApi.DataStore;
using CVChatbotApi.Startup;

namespace CVChatbotApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // In memory chunk embeddings store
        services.AddHostedService<StartupDataInitialiser>();
        services.AddSingleton<IDataStore, InMemoryDataStore>();
    }
}