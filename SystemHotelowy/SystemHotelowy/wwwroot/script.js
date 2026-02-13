let pobranePokoje = [];

async function sprawdzPokoje() {
    const od = document.getElementById("dataOd").value;
    const doDaty = document.getElementById("dataDo").value;

    if (!od || !doDaty) {
        alert("Proszę wybrać obie daty!");
        return;
    }

    try {
        const response = await fetch(`/api/pokoje/wszystkie?od=${od}&do=${doDaty}`);
        if (!response.ok) throw new Error("Błąd pobierania danych");

        pobranePokoje = await response.json();

        przerysujPokoje();

        const dni = obliczLiczbeDni(od, doDaty);
        document.getElementById("cena").textContent = `Długość pobytu: ${dni} dni`;

    } catch (err) {
        console.error(err);
        alert("Wystąpił błąd podczas pobierania pokoi.");
    }
}

function przerysujPokoje() {
    const lista = document.getElementById("listaPokoi");
    lista.innerHTML = "";

    const trybSortowania = document.getElementById("sortowanie").value;
    const filtrInput = document.getElementById("filtrCena").value;
    const maxCena = filtrInput ? parseFloat(filtrInput) : 999999;

    let wyniki = pobranePokoje.filter(p => p.cenaZaDobe <= maxCena);

    wyniki.sort((a, b) => {
        if (trybSortowania === "cena_rosnaco") return a.cenaZaDobe - b.cenaZaDobe;
        if (trybSortowania === "cena_malejaco") return b.cenaZaDobe - a.cenaZaDobe;
        if (trybSortowania === "miejsca_malejaco") return b.liczbaMiejsc - a.liczbaMiejsc;
        return 0;
    });

    if (wyniki.length === 0) {
        lista.innerHTML = "<li style='color: gray'>Brak pokoi spełniających kryteria.</li>";
        return;
    }

    wyniki.forEach(p => {
        const li = document.createElement("li");

        li.innerHTML = `
            <strong>Nr ${p.numer}</strong> 
            (${p.liczbaMiejsc}-osobowy) - 
            Cena: <strong>${p.cenaZaDobe} zł</strong>/doba
        `;

        if (!p.dostepny) {
            li.classList.add("niedostepny");
            li.innerHTML += " <span style='color:red; font-weight:bold'>(ZAJĘTY)</span>";
        } else {
            const btn = document.createElement("button");
            btn.className = "wybierz-btn";
            btn.textContent = "Wybierz";
            btn.onclick = () => {
                document.getElementById("pokojId").value = p.id;
                document.getElementById("pokojId").scrollIntoView({ behavior: "smooth", block: "center" });
            };
            li.appendChild(btn);
        }

        lista.appendChild(li);
    });
}

async function zarezerwuj() {
    const pokojId = document.getElementById("pokojId").value;
    const imie = document.getElementById("imie").value;
    const nazwisko = document.getElementById("nazwisko").value;
    const od = document.getElementById("dataOd").value;
    const doDaty = document.getElementById("dataDo").value;

    if (!pokojId || !imie || !nazwisko || !od || !doDaty) {
        alert("Uzupełnij wszystkie dane!");
        return;
    }

    const rezerwacja = {
        pokojId: parseInt(pokojId),
        gosc: {
            imie: imie,
            nazwisko: nazwisko
        },
        dataOd: od,
        dataDo: doDaty,
        sniadanie: document.getElementById("sniadanie").checked
    };

    try {
        const response = await fetch("/api/rezerwacje", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(rezerwacja)
        });

        const komunikat = document.getElementById("komunikat");

        if (response.ok) {
            komunikat.textContent = "Rezerwacja została zapisana pomyślnie!";
            komunikat.style.color = "green";
            sprawdzPokoje();
        } else {
            const errorText = await response.text();
            komunikat.textContent = "Błąd: " + (errorText || "Nie udało się utworzyć rezerwacji.");
            komunikat.style.color = "red";
        }
    } catch (err) {
        console.error(err);
        alert("Błąd połączenia z serwerem.");
    }
}

function obliczLiczbeDni(od, doDaty) {
    const start = new Date(od);
    const end = new Date(doDaty);
    const roznicaCzasu = end - start;
    const dni = roznicaCzasu / (1000 * 60 * 60 * 24);
    return dni > 0 ? dni : 0;
}