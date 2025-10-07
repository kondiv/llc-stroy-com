using System.ComponentModel.DataAnnotations;

namespace LLCStroyCom.Domain.Requests;

public class DefectCreateRequest
{
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Description { get; set; } = null!;
}