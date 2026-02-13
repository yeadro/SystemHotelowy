if (sessionStorage.getItem("isAdmin") !== "true") {
    window.location.href = "login.html";
}

function wyloguj() {
    sessionStorage.removeItem("isAdmin");
    window.location.href = "index.html";
}

document.addEventListener("DOMContentLoaded", () => {
    console.log("Panel załadowany");
    ladowanieRezerwacji();
    ladowaniePokoi();
});

async function ladowaniePokoi() {
    try {
        const res = await fetch("/api/pokoje/wszystkie?od=1900-01-01&do=1900-01-01");
        const pokoje = await res.json();

        pokoje.sort((a, b) => a.numer - b.numer);

        const tbody = document.querySelector("#tabelaPokoje tbody");
        tbody.innerHTML = "";

        pokoje.forEach(p => {
            let typBackend, typTekst;

            if (p.liczbaMiejsc === 1) {
                typBackend = "jedynka"; typTekst = "Jednoosobowy";
            } else if (p.liczbaMiejsc === 2) {
                typBackend = "dwojka"; typTekst = "Dwuosobowy";
            } else {
                typBackend = "apartament"; typTekst = "Apartament";
            }

            const tr = document.createElement("tr");

            tr.innerHTML = `
                <td>${p.numer}</td>
                <td>${typTekst}</td>
                <td>
                    <button class="btn edit-btn" onclick="wlaczTrybEdycji(${p.id}, '${p.numer}', '${typBackend}')">Edytuj</button>
                    <button class="btn danger-btn" onclick="usunPokoj(${p.id})">Usuń</button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    } catch (err) {
        console.error("Błąd ładowania pokoi:", err);
    }
}

async function zapiszPokoj() {
    console.log("Próba zapisu pokoju...");

    const idEdycji = document.getElementById("editId").value;
    const nrInput = document.getElementById("nrPokoju");
    const typInput = document.getElementById("typPokoju");

    if (!nrInput || !typInput) {
        console.error("Błąd: Nie znaleziono pól formularza w HTML!");
        return;
    }

    const nr = nrInput.value;
    const typ = typInput.value;

    if (!nr) {
        alert("Podaj numer pokoju!");
        return;
    }

    let miejsca = 0;
    switch (typ) {
        case "jedynka":
            miejsca = 1;
            break;
        case "dwojka":
            miejsca = 2;
            break;
        case "apartament":
            miejsca = 4;
            break;
        default:
            miejsca = 1;
    }

    const body = {
        "Numer": parseInt(nr),
        "typ": typ,
        "LiczbaMiejsc": miejsca
    };

    let url = "/api/admin/pokoje";
    let method = "POST";

    if (idEdycji) {
        url = `/api/admin/pokoje/${idEdycji}`;
        method = "PUT";
    }

    try {
        const res = await fetch(url, {
            method: method,
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(body)
        });

        if (res.ok) {
            console.log("Sukces!");
            ladowaniePokoi();
            anulujEdycje();
            alert("Zapisano pomyślnie!");
        } else {
            const text = await res.text();
            console.error("Błąd serwera:", text);
            alert("Serwer odrzucił żądanie: " + text);
        }
    } catch (err) {
        console.error("Błąd połączenia:", err);
        alert("Wystąpił błąd połączenia z serwerem.");
    }
}

function wlaczTrybEdycji(id, numer, typ) {
    document.getElementById("editId").value = id;
    document.getElementById("nrPokoju").value = numer;
    document.getElementById("typPokoju").value = typ;

    document.getElementById("formHeader").innerText = "Edytuj pokój nr " + numer;
    document.getElementById("submitBtn").innerText = "Zapisz zmiany";
    document.getElementById("cancelBtn").style.display = "inline-block";
    document.getElementById("formularzPokoju").classList.add("edit-mode");
}

function anulujEdycje() {
    document.getElementById("editId").value = "";
    document.getElementById("nrPokoju").value = "";
    document.getElementById("typPokoju").value = "jedynka";

    document.getElementById("formHeader").innerText = "Dodaj nowy pokój";
    document.getElementById("submitBtn").innerText = "Dodaj Pokój";
    document.getElementById("cancelBtn").style.display = "none";
    document.getElementById("formularzPokoju").classList.remove("edit-mode");
}

async function usunPokoj(id) {
    if(!confirm("Czy na pewno usunąć ten pokój?")) return;

    await fetch(`/api/admin/pokoje/${id}`, { method: "DELETE" });
    ladowaniePokoi();
    anulujEdycje();
}


async function ladowanieRezerwacji() {
    try {
        const res = await fetch("/api/admin/rezerwacje");
        const rezerwacje = await res.json();
        const tbody = document.querySelector("#tabelaRezerwacje tbody");
        tbody.innerHTML = "";

        rezerwacje.forEach(r => {
            const tr = document.createElement("tr");
            const dataOd = new Date(r.dataOd).toLocaleDateString();
            const dataDo = new Date(r.dataDo).toLocaleDateString();

            const sniadanieInfo = r.sniadanie
                ? "<span style='color:#4c60af; font-weight:bold'>Tak</span>"
                : "<span style='color:gray'>Nie</span>";

            tr.innerHTML = `
                <td>ID Pokoju: ${r.pokojId}</td>
                <td>${r.gosc.imie} ${r.gosc.nazwisko}</td>
                <td>${dataOd} - ${dataDo}</td>
                <td style="text-align: center;">${sniadanieInfo}</td> <td><button class="btn danger-btn" onclick="usunRezerwacje('${r.id}')">Usuń</button></td>
            `;
            tbody.appendChild(tr);
        });
    } catch (err) {
        console.error("Błąd ładowania rezerwacji:", err);
    }
}

async function usunRezerwacje(id) {
    if(!confirm("Usunąć rezerwację?")) return;
    await fetch(`/api/admin/rezerwacje/${id}`, { method: "DELETE" });
    ladowanieRezerwacji();
}