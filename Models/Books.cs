using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace digital_library.Models;

public enum AvailabilityStatus
{
    Available,
    Borrowed
}

public class Book
{
    public int Id { get; set; }
    [DefaultValue("Untitled")]
    public required string Title { get; set; }
    public string? Author { get; set; }
    public string? Genre { get; set; }
    [DefaultValue(0)]
    public DateTime PublicationDate { get; set; }
    public AvailabilityStatus AvailabilityStatus { get; set; }
    public string? BorrowedBy { get; set; }
    public DateTime? BorrowedDate { get; set; }
    [Display(Name = "Cover Image URL")]
    public string? CoverUrl { get; set; }
}