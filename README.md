# Szavazórendszer

## **[Dokumentáció](https://docs.google.com/document/d/1GjcvmLN40UbFvJRpiaAiFeRGtFfDXai71Nm_cvEZwMY/edit?usp=sharing)**

### A Feladat

- Látogatói felület: készítsünk REST architektúrájú webalkalmazást és hozzá webes felületű, asztali grafikus vagy mobil kliens alkalmazást, amelyen keresztül a felhasználók az aktív kérdésekben szavazhatnak, valamint megtekinthetőek a korábbi szavazások eredményei.
- Adminisztrációs felület: Készítsünk Blazor keretrendszerre épülő kliens alkalmazást, amelyen keresztül új szavazásokat lehet kiírni a rendszerben. Bármely felhasználó írhat ki új szavazást.

### Megvalósítás

- A feladat egy C# Asp.net WebAPI szerveroldali applikációt, egy Blazor kliens applikációt és egy Windows Presentation Foundation (WPF) projektet tartalmaz magában.
- A Blazor alkalmazás elindulásával az alapértelmezett böngészőnkön megjelenik a bejelentkezési fül, ahol bármilyen felhasználó be tud jelentkezni szavazásokat létrehozni, módosítani, illetve törölni. A program engedélyezi a szabad regisztrálást az autorizáció hiányával, ugyanis nincsen arra szükség a program tekintetében. A program engedélyezi az élő szavazás lehetőségét, amellyel a szavazott felhasználók és a kiállító frissítés nélkül értesülnek a szavazás haladásáról. 
- A WPF kliens a szavazók feladatát látja el, regisztráció és bejelentkezés után a felhasználók az éppen futó szavazásokra pedig szavazatokat tudnak kiadni. Ha a kiállító engedélyezte az élő módot a szavazásra, akkor erről a kliensek is értesülnek.