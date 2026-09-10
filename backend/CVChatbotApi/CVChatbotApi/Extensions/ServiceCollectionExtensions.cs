using CVChatbotApi.DataStore;
using CVChatbotApi.Startup;
using Shared.ExternalServices;

namespace CVChatbotApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // In memory chunk embeddings store
        services.AddHostedService<StartupDataInitialiser>();
        services.AddSingleton<IDataStore, InMemoryDataStore>();
        
        // Gemini
        services.AddTransient<IGeminiHttpClient, GeminiHttpClient>();
        
        // Options
        services.Configure<GeminiOptions>(configuration.GetSection("Gemini"));
    }
}