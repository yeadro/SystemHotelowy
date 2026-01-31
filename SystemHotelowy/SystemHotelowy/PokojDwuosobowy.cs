namespace HotelReservation.Models;

public class PokojDwuosobowy : Pokoj
{
    public override decimal ObliczCeneZaDobe()
    {
        return 350m;
    }
}