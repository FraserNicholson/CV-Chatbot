namespace CVChatbotApi.Models;

public record CosineSimilarityInput(string Query, double[] QueryEmbedding, ChunkEmbedding[] ChunkEmbeddings, Guid requestId);