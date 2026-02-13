namespace HotelReservation.Models;

public class PokojJednoosobowy : Pokoj
{
    public override decimal ObliczCeneZaDobe()
    {
        return 200m;
    }
}