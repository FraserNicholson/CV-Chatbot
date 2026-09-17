using CVChatbotApi.DataStore;
using CVChatbotApi.Mapping;
using CVChatbotApi.RequestHandlers;
using CVChatbotApi.Services;
using CVChatbotApi.Startup;
using Shared.Services;

namespace CVChatbotApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Logging
        services.AddLogging();
        
        // In memory chunk embeddings store
        services.AddHostedService<StartupDataInitialiser>();
        services.AddSingleton<IDataStore, InMemoryDataStore>();
        
        // Request Handlers
        services.AddTransient<ICVQueryHandler, CVQueryHandler>();
        
        // Services
        services.AddTransient<ICosineSimilarityService, CosineSimilarityService>();
        
        // Mappers
        services.AddTransient<ICosineSimilarityMapper, CosineSimilarityMapper>();
        services.AddTransient<IGeminiPromptMapper, GeminiPromptMapper>();
        
        // Gemini
        services.AddTransient<IGeminiHttpClient, GeminiHttpClient>();
        services.AddTransient<IGeminiEmbeddingService, GeminiEmbeddingService>();
        services.AddTransient<IGeminiPromptService, GeminiPromptService>();
        
        // Options
        services.Configure<GeminiOptions>(configuration.GetSection("Gemini"));
    }
}