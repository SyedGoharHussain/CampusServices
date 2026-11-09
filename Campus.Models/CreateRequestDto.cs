using System.ComponentModel.DataAnnotations;

namespace Campus.Models;

public class CreateRequestDto
{
    [Required, StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = "";

    [Required, StringLength(1000)]
    public string Description { get; set; } = "";

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}
