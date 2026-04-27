using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace digital_library.Models;

public class Book
{
    public int Id { get; set; }
    [DefaultValue("Untitled")]
    public required string Title { get; set; }
    public string? Author { get; set; }
    public string? Genre { get; set; }
    [DefaultValue(0)]
    public DateTime PublicationDate { get; set; }
    [DefaultValue(true)]  
    public required Enum AvailabilityStatus { get; set; }
}