# Bookstore — internetowa księgarnia z panelem administracyjnym

Aplikacja umożliwia przeglądanie i wyszukiwanie książek, obsługę koszyka, składanie zamówień oraz symulowane płatności. Administrator zarządza książkami i kategoriami, przegląda zamówienia i oznacza opłacone zamówienia jako zrealizowane.

Projekt wykonany w ramach pracy inżynierskiej na kierunku Informatyka w Akademii WIT.

## Technologie

- Backend: ASP.NET Core 8, Entity Framework Core 9, Npgsql.
- Baza danych: PostgreSQL 18.
- Frontend: React 18, React Router, Vite, nginx.
- Uwierzytelnianie: JWT, powiadomienia e-mail: SMTP.
- Środowisko uruchomieniowe: Docker i Docker Compose.

## Uruchomienie w środowisku kontenerowym

Zalecanym sposobem uruchomienia projektu jest środowisko kontenerowe Docker Compose. Wymagany jest uruchomiony Docker z obsługą kontenerów linuksowych i Docker Compose (np. Docker Desktop na Windows) oraz wolne porty 8080, 5172 i 5433. Przy pierwszym budowaniu potrzebny jest dostęp do Internetu w celu pobrania obrazów i pakietów, ale nie jest konieczna osobna instalacja .NET, Node.js ani PostgreSQL.

Polecenia należy wykonywać w głównym katalogu projektu, zawierającym `docker-compose.yml`.

**1. Przygotowanie konfiguracji.** Jeśli plik `.env` jeszcze nie istnieje, należy skopiować wzorzec:

```powershell
Copy-Item .env.example .env
```

W pliku `.env` należy ustawić własne `POSTGRES_PASSWORD` oraz losowy `JWT_KEY` o długości co najmniej 32 znaków ASCII, a wartości `POSTGRES_DB` i `POSTGRES_USER` mogą pozostać bez zmian.

**Poczta SMTP:** do sprawdzenia potwierdzeń zamówienia i płatności należy uzupełnić `SMTP_HOST`, `SMTP_PORT`, `SMTP_USERNAME`, `SMTP_PASSWORD` i `SMTP_FROM_EMAIL` danymi działającego serwera i konta nadawcy.

Do uruchomienia bez powiadomień należy ustawić `SMTP_HOST=` i pozostawić `SMTP_PORT=587`. Zamówienia i płatności będą zapisywane mimo braku wysyłki e-mail, jednak pełne sprawdzenie powiadomień wymaga poprawnej konfiguracji SMTP.

**2. Zbudowanie i uruchomienie aplikacji.** W terminalu, w głównym katalogu projektu zawierającym `docker-compose.yml`, należy wykonać:

```powershell
docker compose up -d --build
```

Przy pierwszym uruchomieniu API automatycznie tworzy strukturę bazy danych oraz dodaje dane początkowe: konto administratora z pustym koszykiem, 9 kategorii i 16 książek.

**3. Dostęp do aplikacji.** W przeglądarce należy otworzyć adres [http://localhost:8080](http://localhost:8080).

API jest dostępne pod `http://localhost:5172/api`, a baza pod `localhost:5433`. Stan usług można sprawdzić poleceniem `docker compose ps` — usługi powinny działać, a baza mieć stan `healthy`. W razie problemów logi API można odczytać poleceniem `docker compose logs --tail=50 api`.

Zatrzymanie przez `docker compose down` zachowuje dane, natomiast polecenie `docker compose down -v` **usuwa bazę wraz ze wszystkimi danymi**. W takim przypadku następne uruchomienie odtworzy dane początkowe.

## Konto administratora

Konto administratora powstaje automatycznie, a w projekcie zapisano wyłącznie skrót jego hasła. Do logowania wymagane są dane tego konta.

## Test współbieżności

Przy działającym API należy wykonać w PowerShell, w głównym katalogu projektu:

```powershell
powershell -ExecutionPolicy Bypass -File tests\test-wspolbieznosci.ps1
```

Skrypt pyta o dane administratora, tworzy lub wykorzystuje 12 kont testowych i przygotowuje ich koszyki. Hasło administratora jest wpisywane w sposób ukryty i nie jest zapisane w skrypcie.

Domyślnie test dotyczy książki o ID 9 (`$bookId`), której stan magazynowy przed uruchomieniem testu musi wynosić co najmniej jeden egzemplarz. Po zatrzymaniu skryptu należy ustawić stan książki na `1` w panelu administratora, a następnie nacisnąć Enter.

Test ma potwierdzić, że przy jednoczesnych próbach zakupu ostatniego egzemplarza tylko jedno zamówienie zostanie zapisane w bazie danych, a stan magazynowy spadnie z 1 do 0. Pozostałe żądania powinny zostać odrzucone z powodu braku dostępności lub konfliktu współbieżności.

## Struktura projektu

```text
Controllers/    obsługa żądań API
Models/         encje bazy danych
DTOs/           dane wejściowe i odpowiedzi API
Data/           kontekst bazy danych
Migrations/     migracje i dane początkowe
Services/       logika zamówień, płatności, poczty i tokenów
Profiles/       mapowania AutoMapper
Exceptions/     wyjątki aplikacji
Helpers/        klasy pomocnicze i ustawienia
frontend/       interfejs użytkownika i okładki
tests/          skrypt testu współbieżności
Program.cs      główna konfiguracja aplikacji
```
