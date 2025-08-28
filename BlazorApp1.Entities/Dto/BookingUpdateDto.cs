namespace BlazorApp1.Entities.Dto;

public class BookingUpdateDto
{
    public int Id { get; set; }
    public DateTime FromDate { get; set; } 
    public DateTime ToDate { get; set; } 
    public int UserId { get; set; }
    public int ItemId { get; set; }

    public string Comment { get; set; } = "";
}