namespace HotelReservation.Models;

public abstract class Pokoj
{
    public int Id { get; set; }
    public int Numer { get; set; }
    public int LiczbaMiejsc { get; set; }

    public abstract decimal ObliczCeneZaDobe();
}