async function sprawdzPokoje() {
    const od = document.getElementById("dataOd").value;
    const doDaty = document.getElementById("dataDo").value;

    const response = await fetch(`/api/pokoje/wszystkie?od=${od}&do=${doDaty}`);
    const pokoje = await response.json();

    const lista = document.getElementById("listaPokoi");
    lista.innerHTML = "";

    pokoje
        .sort((a, b) => a.cenaZaDobe - b.cenaZaDobe)
        .forEach(p => {
            const li = document.createElement("li");

            li.textContent =
                `ID: ${p.id}, Numer: ${p.numer}, ` +
                `Miejsca: ${p.liczbaMiejsc}, ` +
                `Cena/doba: ${p.cenaZaDobe} zł`;

            if (!p.dostepny) {
                li.classList.add("niedostepny");
                li.textContent += " (zajęty)";
            }

            lista.appendChild(li);
        });
    const dni = obliczLiczbeDni(od, doDaty);
    document.getElementById("cena").textContent =
        `Czas pobytu: ${dni} dni`;

}


async function zarezerwuj() {
    const rezerwacja = {
        pokojId: parseInt(document.getElementById("pokojId").value),
        gosc: {
            imie: document.getElementById("imie").value,
            nazwisko: document.getElementById("nazwisko").value
        },
        dataOd: document.getElementById("dataOd").value,
        dataDo: document.getElementById("dataDo").value
    };

    const response = await fetch("/api/rezerwacje", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(rezerwacja)
    });

    const komunikat = document.getElementById("komunikat");

    if (response.ok) {
        komunikat.textContent = "Rezerwacja została zapisana.";
        komunikat.style.color = "green";
    } else {
        komunikat.textContent = "Nie udało się utworzyć rezerwacji.";
        komunikat.style.color = "red";
    }
}

function obliczLiczbeDni(od, doDaty) {
    const start = new Date(od);
    const end = new Date(doDaty);
    return (end - start) / (1000 * 60 * 60 * 24);
}
