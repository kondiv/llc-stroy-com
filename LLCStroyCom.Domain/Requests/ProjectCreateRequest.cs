using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace LLCStroyCom.Domain.Requests;

public class ProjectCreateRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
    [MaxLength(255, ErrorMessage = "Name must not exceed 255 characters")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "City is required")]
    [MinLength(2, ErrorMessage = "City must be at least 1 character")]
    [MaxLength(180, ErrorMessage = "City must not exceed  characters")]
    public string City { get; set; }
}