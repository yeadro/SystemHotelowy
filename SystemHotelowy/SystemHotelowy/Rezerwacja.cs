namespace HotelReservation.Models;

public class Rezerwacja
{
    public int PokojId { get; set; }
    public Gosc Gosc { get; set; } = new();
    public DateTime DataOd { get; set; }
    public DateTime DataDo { get; set; }
}