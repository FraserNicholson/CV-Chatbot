using System.ComponentModel.DataAnnotations;

namespace CVChatbotApi.Contract;

public class CvQueryRequest
{
    [Required]
    public required string? Query { get; set; }
}