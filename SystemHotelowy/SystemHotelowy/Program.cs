using System.Text.Json;
using HotelReservation.Models;
using HotelReservation.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IHotelService, HotelService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/pokoje", (IHotelService service, DateTime od, DateTime @do) =>
{
    return service.PobierzDostepnePokoje(od, @do);
});

app.MapGet("/api/pokoje/wszystkie", (IHotelService service, DateTime od, DateTime @do) =>
{
    return service.PobierzPokojeZStatusami(od, @do);
});

app.MapPost("/api/rezerwacje", (IHotelService service, Rezerwacja rezerwacja) =>
{
    bool sukces = service.DodajRezerwacje(rezerwacja);
    return sukces ? Results.Ok() : Results.BadRequest("Błąd rezerwacji");
});

app.MapGet("/api/admin/rezerwacje", (IHotelService service) =>
{
    return service.PobierzWszystkieRezerwacje();
});

app.MapDelete("/api/admin/rezerwacje/{id}", (IHotelService service, Guid id) =>
{
    service.UsunRezerwacje(id);
    return Results.Ok();
});

app.MapPost("/api/admin/pokoje", (IHotelService service, [FromBody] JsonElement json) =>
{
    string typ = json.GetProperty("typ").GetString();
    Pokoj nowyPokoj;

    if (typ == "jedynka")
        nowyPokoj = json.Deserialize<PokojJednoosobowy>();
    else
        nowyPokoj = json.Deserialize<PokojDwuosobowy>();

    if (nowyPokoj != null)
    {
        service.DodajPokoj(nowyPokoj);
        return Results.Ok();
    }
    return Results.BadRequest();
});

app.MapPut("/api/admin/pokoje/{id}", (IHotelService service, int id, [FromBody] JsonElement json) =>
{
    string typ = json.GetProperty("typ").GetString();
    Pokoj zaktualizowanyPokoj;

    if (typ == "jedynka")
        zaktualizowanyPokoj = json.Deserialize<PokojJednoosobowy>();
    else
        zaktualizowanyPokoj = json.Deserialize<PokojDwuosobowy>();

    if (zaktualizowanyPokoj != null)
    {
        bool sukces = service.EdytujPokoj(id, zaktualizowanyPokoj);
        return sukces ? Results.Ok() : Results.BadRequest("Nie można edytować pokoju.");
    }
    return Results.BadRequest();
});

app.MapDelete("/api/admin/pokoje/{id}", (IHotelService service, int id) =>
{
    service.UsunPokoj(id);
    return Results.Ok();
});

app.Run();