using System.Text.Json;
using HotelReservation.Models;

namespace HotelReservation.Services;

public class HotelService
{
    private readonly string _sciezkaPliku = "Data/rezerwacje.json";
    private List<Rezerwacja> _rezerwacje;

    public List<Pokoj> Pokoje { get; } = new();

    public HotelService()
    {
        Pokoje.Add(new PokojJednoosobowy { Id = 1, Numer = 101, LiczbaMiejsc = 1 });
        Pokoje.Add(new PokojJednoosobowy { Id = 2, Numer = 102, LiczbaMiejsc = 1 });
        Pokoje.Add(new PokojDwuosobowy { Id = 3, Numer = 103, LiczbaMiejsc = 2 });

        _rezerwacje = WczytajRezerwacje();
    }

    public List<Pokoj> PobierzDostepnePokoje(DateTime od, DateTime @do)
    {
        return Pokoje
            .Where(p => !_rezerwacje.Any(r =>
                r.PokojId == p.Id &&
                od < r.DataDo &&
                @do > r.DataOd))
            .ToList();
    }

    public bool DodajRezerwacje(Rezerwacja rezerwacja)
    {
        bool pokojZajety = _rezerwacje.Any(r =>
            r.PokojId == rezerwacja.PokojId &&
            rezerwacja.DataOd < r.DataDo &&
            rezerwacja.DataDo > r.DataOd);

        if (pokojZajety || rezerwacja.DataOd >= rezerwacja.DataDo)
            return false;

        _rezerwacje.Add(rezerwacja);
        ZapiszRezerwacje();
        return true;
    }

    private List<Rezerwacja> WczytajRezerwacje()
    {
        if (!File.Exists(_sciezkaPliku))
            return new List<Rezerwacja>();

        var json = File.ReadAllText(_sciezkaPliku);
        return JsonSerializer.Deserialize<List<Rezerwacja>>(json) ?? new();
    }

    private void ZapiszRezerwacje()
    {
        var json = JsonSerializer.Serialize(_rezerwacje, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        Directory.CreateDirectory("Data");
        File.WriteAllText(_sciezkaPliku, json);
    }
    
    public List<PokojDto> PobierzPokojeZStatusami(DateTime od, DateTime @do)
    {
        return Pokoje.Select(p =>
        {
            bool zajety = _rezerwacje.Any(r =>
                r.PokojId == p.Id &&
                od < r.DataDo &&
                @do > r.DataOd);

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
}



