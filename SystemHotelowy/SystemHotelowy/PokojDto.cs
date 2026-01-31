namespace HotelReservation.Models;

public class PokojDto
{
    public int Id { get; set; }
    public int Numer { get; set; }
    public int LiczbaMiejsc { get; set; }
    public decimal CenaZaDobe { get; set; }
    public bool Dostepny { get; set; }
}