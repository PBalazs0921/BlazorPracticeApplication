namespace BlazorApp1.Entities.Dto;

public class BookingCreateDto
{
    public DateTime FromDate { get; set; } 
    public DateTime ToDate { get; set; } 
    public int UserId { get; set; }
    public int ItemId { get; set; }
    public int Price { get; set; }
    public string Comment { get; set; } = "";
}