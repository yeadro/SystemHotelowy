using HotelReservation.Services;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

var hotelService = new HotelService();

app.MapGet("/api/pokoje", (DateTime od, DateTime @do) =>
{
    return hotelService.PobierzDostepnePokoje(od, @do);
});

app.MapPost("/api/rezerwacje", (HotelReservation.Models.Rezerwacja rezerwacja) =>
{
    bool sukces = hotelService.DodajRezerwacje(rezerwacja);
    return sukces
        ? Results.Ok()
        : Results.BadRequest("Pokój jest zajęty lub dane są niepoprawne");
});

app.MapGet("/api/pokoje/wszystkie", (DateTime od, DateTime @do) =>
{
    return hotelService.PobierzPokojeZStatusami(od, @do);
});


app.Run();