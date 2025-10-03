using System.ComponentModel.DataAnnotations;

namespace LLCStroyCom.Domain.Dto;

public class DefectPatchDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
}