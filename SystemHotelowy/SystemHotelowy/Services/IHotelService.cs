using HotelReservation.Models;

namespace HotelReservation.Services;

public interface IHotelService
{
    List<Pokoj> PobierzDostepnePokoje(DateTime od, DateTime doDaty);
    bool DodajRezerwacje(Rezerwacja rezerwacja);
    List<PokojDto> PobierzPokojeZStatusami(DateTime od, DateTime doDaty);

    List<Rezerwacja> PobierzWszystkieRezerwacje();
    void UsunRezerwacje(Guid id);
    void DodajPokoj(Pokoj pokoj);
    bool EdytujPokoj(int id, Pokoj zaktualizowanyPokoj); 
    void UsunPokoj(int id);
    List<Pokoj> PobierzWszystkiePokoje();
}