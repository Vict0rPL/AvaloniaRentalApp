# AvaloniaRentalApp

Desktopowa aplikacja do zarządzania **wypożyczalnią samochodów**, zbudowana w technologii
**Avalonia UI 11.3 / .NET 8** w architekturze **MVVM (ReactiveUI)**, korzystająca z bazy danych
**MariaDB / MySQL** (dostęp przez Dapper + MySqlConnector).

Aplikacja obejmuje moduły: logowanie z blokadą konta, pulpit ze statystykami floty, zarządzanie
pojazdami i klientami, obsługę wypożyczeń i zwrotów oraz raporty finansowe i eksploatacyjne.


## Wymagania wstępne

| Składnik | Wersja                                              |
|----------|-----------------------------------------------------|
| .NET SDK | **8.0** (do uruchomienia aplikacji)                 |
| .NET SDK | **10.0** (dodatkowo, do kompilacji projektu testów) |
| MariaDB  | MariaDB 10.5+ lub MySQL 8.0+                        |
| System   | Windows (projekt Desktop jest typu `WinExe`)        |

## 1. Konfiguracja bazy danych

Utwórz schemat i załaduj dane przykładowe:

```bash
# Utworzenie bazy CarRentalDB, tabel, widoków i wyzwalaczy
mysql -u root -p < Database/init_db.sql

# Załadowanie danych przykładowych (użytkownicy, pojazdy, klienci, wypożyczenia)
mysql -u root -p CarRentalDB < Database/seed_data.sql
```

Łańcuch połączeniowy jest skonfigurowany w pliku
[AvaloniaRentalApp/appsettings.json](AvaloniaRentalApp/appsettings.json):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CarRentalDB;User=root;Password=root;AllowUserVariables=True"
  }
}
```

Jeśli baza działa pod innym hostem, użytkownikiem lub hasłem trzeba zmienic wartość
`DefaultConnection` w tym pliku (można to także zrobić z poziomu widoku **Ustawienia** w aplikacji).

## 2. Budowanie i uruchomienie

```bash
# Budowanie całego rozwiązania
dotnet build AvaloniaRentalApp.sln

# Uruchomienie aplikacji desktopowej
dotnet run --project AvaloniaRentalApp.Desktop/AvaloniaRentalApp.Desktop.csproj
```

## 3. Dane logowania

Konta utworzone przez `seed_data.sql` (wszystkie z tym samym hasłem **`Admin123!`**):

| Login           | Hasło       | Rola     |
|-----------------|-------------|----------|
| `admin`         | `Admin123!` | admin    |
| `anna.kowalska` | `Admin123!` | employee |
| `jan.nowak`     | `Admin123!` | employee |

> Po 5 nieudanych próbach logowania konto zostaje zablokowane na 15 minut.

## 4. Uruchamianie testów

```bash
# Wszystkie testy
dotnet test AvaloniaRentalApp.Tests/AvaloniaRentalApp.Tests.csproj

# Pojedyncza klasa testowa
dotnet test AvaloniaRentalApp.Tests/AvaloniaRentalApp.Tests.csproj --filter "ClassName=AddCustomerViewModelTests"

# Z pomiarem pokrycia kodu
dotnet test AvaloniaRentalApp.Tests/AvaloniaRentalApp.Tests.csproj --collect:"XPlat Code Coverage"
```

## 5. Struktura projektu

```
AvaloniaRentalApp/
├── AvaloniaRentalApp/            # Biblioteka rdzeniowa (net8.0)
│   ├── Models/                   # Encje domenowe (Car, Customer, Rental, User, ...)
│   ├── ViewModels/               # Modele widoku (MVVM / ReactiveUI)
│   ├── Views/                    # Widoki Avalonia (XAML + code-behind)
│   ├── Services/                 # DatabaseService, AuthService, PricingService, ...
│   ├── Converters/               # Konwertery wartości XAML
│   ├── App.axaml(.cs)            # Punkt kompozycji (composition root)
│   └── appsettings.json          # Łańcuch połączeniowy
├── AvaloniaRentalApp.Desktop/    # Punkt wejścia aplikacji (net8.0, WinExe)
├── AvaloniaRentalApp.Tests/      # Testy jednostkowe (net10.0, NUnit)
├── Database/
│   ├── init_db.sql               # Schemat bazy danych
│   └── seed_data.sql             # Dane przykładowe
├── DOKUMENTACJA.md               # Pełna dokumentacja techniczna + diagramy
└── README.md                     # Ten plik
```

## Uwagi dla systemu Windows

Projekt `AvaloniaRentalApp.Desktop` jest aplikacją typu `WinExe` z dołączonym `app.manifest`.
Plik `appsettings.json` jest kopiowany do katalogu wyjściowego (`PreserveNewest`), więc musi być
obecny obok pliku wykonywalnego — `dotnet run` i `dotnet build` zapewniają to automatycznie.
