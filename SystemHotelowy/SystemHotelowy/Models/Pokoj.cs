using System.Text.Json.Serialization;
namespace HotelReservation.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "TypPokoju")]
[JsonDerivedType(typeof(PokojJednoosobowy), typeDiscriminator: "jedynka")]
[JsonDerivedType(typeof(PokojDwuosobowy), typeDiscriminator: "dwojka")]
[JsonDerivedType(typeof(Apartament), typeDiscriminator: "apartament")]
public abstract class Pokoj
{
    public int Id { get; set; }
    public int Numer { get; set; }
    public int LiczbaMiejsc { get; set; }

    public abstract decimal ObliczCeneZaDobe();
}