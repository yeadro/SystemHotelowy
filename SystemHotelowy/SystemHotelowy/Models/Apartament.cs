namespace HotelReservation.Models;

public class Apartament : Pokoj
{
    public Apartament()
    {
        LiczbaMiejsc = 4;
    }
    public override decimal ObliczCeneZaDobe()
    {
        return 500m; 
    }
}