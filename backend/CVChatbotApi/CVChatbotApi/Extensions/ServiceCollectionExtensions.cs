using System.Threading.RateLimiting;
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
        
        // Add rate limiting
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: "global", // For now, one rate limit for all users is fine, gemini has 15 rpm limit anyway
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 10,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(1)
                    }));
        });
        
        // In memory chunk embeddings store
        services.AddHostedService<StartupDataInitialiser>();
        services.AddSingleton<IDataStore, InMemoryDataStore>();
        
        // Request Handlers
        services.AddTransient<ICvQueryHandler, CvQueryHandler>();
        
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