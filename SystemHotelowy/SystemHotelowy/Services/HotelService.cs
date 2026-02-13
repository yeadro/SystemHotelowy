using System.Text.Json;
using HotelReservation.Models;

namespace HotelReservation.Services;

public class HotelService : IHotelService
{
    private readonly string _plikRezerwacje = "Data/rezerwacje.json";
    private readonly string _plikPokoje = "Data/pokoje.json";
    
    private List<Rezerwacja> _rezerwacje;
    private List<Pokoj> _pokoje;

    public HotelService()
    {
        _rezerwacje = WczytajDane<Rezerwacja>(_plikRezerwacje);
        _pokoje = WczytajDane<Pokoj>(_plikPokoje);

        if (!_pokoje.Any())
        {
            _pokoje.Add(new PokojJednoosobowy { Id = 1, Numer = 101, LiczbaMiejsc = 1 });
            _pokoje.Add(new PokojJednoosobowy { Id = 2, Numer = 102, LiczbaMiejsc = 1 });
            _pokoje.Add(new PokojDwuosobowy { Id = 3, Numer = 103, LiczbaMiejsc = 2 });
            ZapiszDane(_plikPokoje, _pokoje);
        }
    }


    public List<Pokoj> PobierzDostepnePokoje(DateTime od, DateTime doDaty)
    {
        return _pokoje
            .Where(p => !_rezerwacje.Any(r =>
                r.PokojId == p.Id &&
                od < r.DataDo &&
                doDaty > r.DataOd))
            .ToList();
    }

    public bool DodajRezerwacje(Rezerwacja rezerwacja)
    {
        if (rezerwacja.DataOd.Date < DateTime.Now.Date)
            return false;

        if (rezerwacja.DataDo <= rezerwacja.DataOd)
            return false;
        bool pokojZajety = _rezerwacje.Any(r =>
            r.PokojId == rezerwacja.PokojId &&
            rezerwacja.DataOd < r.DataDo &&
            rezerwacja.DataDo > r.DataOd);

        if (pokojZajety || rezerwacja.DataOd >= rezerwacja.DataDo)
            return false;

        rezerwacja.Id = Guid.NewGuid();
        _rezerwacje.Add(rezerwacja);
        ZapiszDane(_plikRezerwacje, _rezerwacje);
        return true;
    }

    public List<PokojDto> PobierzPokojeZStatusami(DateTime od, DateTime doDaty)
    {
        return _pokoje.Select(p =>
        {
            bool zajety = _rezerwacje.Any(r =>
                r.PokojId == p.Id &&
                od < r.DataDo &&
                doDaty > r.DataOd);

            return new PokojDto
            {
                Id = p.Id,
                Numer = p.Numer,
                LiczbaMiejsc = p.LiczbaMiejsc,
                CenaZaDobe = p.ObliczCeneZaDobe(),
                Dostepny = !zajety
            };
        }).ToList();
    }

    public List<Rezerwacja> PobierzWszystkieRezerwacje()
    {
        return _rezerwacje.OrderByDescending(r => r.DataOd).ToList();
    }

    public void UsunRezerwacje(Guid id)
    {
        var rezerwacja = _rezerwacje.FirstOrDefault(r => r.Id == id);
        if (rezerwacja != null)
        {
            _rezerwacje.Remove(rezerwacja);
            ZapiszDane(_plikRezerwacje, _rezerwacje);
        }
    }

    public void DodajPokoj(Pokoj pokoj)
    {
        int noweId = _pokoje.Any() ? _pokoje.Max(p => p.Id) + 1 : 1;
        pokoj.Id = noweId;
        
        _pokoje.Add(pokoj);
        ZapiszDane(_plikPokoje, _pokoje);
    }
    public bool EdytujPokoj(int id, Pokoj zaktualizowanyPokoj)
    {
        var staryPokoj = _pokoje.FirstOrDefault(p => p.Id == id);
        if (staryPokoj == null) return false;

        if (_pokoje.Any(p => p.Numer == zaktualizowanyPokoj.Numer && p.Id != id))
        {
            return false;
        }

        zaktualizowanyPokoj.Id = id;

        int index = _pokoje.IndexOf(staryPokoj);
        _pokoje[index] = zaktualizowanyPokoj;
    
        ZapiszDane(_plikPokoje, _pokoje);
        return true;
    }

    public void UsunPokoj(int id)
    {
        var pokoj = _pokoje.FirstOrDefault(p => p.Id == id);
        if (pokoj != null)
        {
            _pokoje.Remove(pokoj);
            _rezerwacje.RemoveAll(r => r.PokojId == id);
            
            ZapiszDane(_plikPokoje, _pokoje);
            ZapiszDane(_plikRezerwacje, _rezerwacje);
        }
    }

    public List<Pokoj> PobierzWszystkiePokoje()
    {
        return _pokoje;
    }
    

    private List<T> WczytajDane<T>(string sciezka)
    {
        if (!File.Exists(sciezka))
            return new List<T>();

        var json = File.ReadAllText(sciezka);
        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    private void ZapiszDane<T>(string sciezka, List<T> dane)
    {
        var opcje = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(dane, opcje);

        Directory.CreateDirectory("Data");
        File.WriteAllText(sciezka, json);
    }
}