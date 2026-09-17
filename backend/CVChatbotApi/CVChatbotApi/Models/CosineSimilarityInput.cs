namespace CVChatbotApi.Models;

public record CosineSimilarityInput(double[] QueryEmbedding, ChunkEmbedding[] ChunkEmbeddings, Guid requestId);