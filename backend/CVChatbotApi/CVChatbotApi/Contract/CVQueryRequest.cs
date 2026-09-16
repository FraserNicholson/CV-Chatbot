using System.ComponentModel.DataAnnotations;

namespace CVChatbotApi.Contract;

public class CVQueryRequest
{
    [Required]
    public required string? Query { get; set; }
}