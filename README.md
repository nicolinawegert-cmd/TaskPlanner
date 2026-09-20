# Task Planner – Backend

Ett API byggt med ASP.NET Core. Det hanterar uppgifter och filer åt
[webbappen](https://github.com/nicolinawegert-cmd/TaskPlanner.Web#readme).

Uppgifterna sparas i en SQLite-databas med hjälp av Entity Framework Core.

## Starta projektet

Du behöver Git och [.NET SDK 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).
Du behöver inte installera någon separat databasserver.

Klona projektet:

```sh
git clone https://github.com/nicolinawegert-cmd/TaskPlanner.git TaskPlanner.Api
cd TaskPlanner.Api
```

Kommandot ger projektmappen namnet `TaskPlanner.Api`.

Installera verktyget för databasens migrationer om du inte redan har det:

```sh
dotnet tool install --global dotnet-ef --version 10.0.12
```

Om du har en äldre version, använd i stället
`dotnet tool update --global dotnet-ef --version 10.0.12`.
Du kan kontrollera versionen med `dotnet ef --version`.

Kör sedan i projektmappen:

```sh
dotnet restore
dotnet ef database update
dotnet run
```

- `dotnet restore` installerar projektets paket.
- `dotnet ef database update` skapar databasen och dess tabeller.
- `dotnet run` startar API:t på port `5035`.

Databasen följer inte med när repot klonas, så databassteget behövs vid första
starten. Filerna i `Migrations/` beskriver vilka tabeller som ska skapas.

Öppna [http://localhost:5035/api/tasks](http://localhost:5035/api/tasks)
för att kontrollera att API:t fungerar. En ny databas är tom och ger svaret `[]`.

Stoppa API:t med `Ctrl+C`. Vid nästa start räcker
`dotnet run` i projektmappen.
Låt API:t vara igång när du använder webbappen.

## Endpoints

| Metod | Adress | Vad den gör |
| --- | --- | --- |
| GET | `/api/tasks` | Hämtar uppgifterna |
| POST | `/api/tasks` | Lägger till en uppgift |
| PUT | `/api/tasks/{id}` | Uppdaterar en uppgift |
| DELETE | `/api/tasks/{id}` | Tar bort en uppgift |
| POST | `/api/tasks/{id}/file` | Laddar upp en fil till en uppgift |
| GET | `/uploads/{filnamn}` | Hämtar en uppladdad fil |

POST och PUT för uppgifter tar emot JSON. Titel krävs. Status kan vara
`NotStarted`, `InProgress` eller `Completed`, och slutdatum är valfritt.
Filuppladdningen använder ett formulärfält som heter `file`.

Om uppgiften saknas returneras `404`. En lyckad borttagning ger `204`,
vilket betyder att svaret är tomt.

## Hur koden är uppdelad

- `Program.cs` innehåller inställningarna och startar appen.
- `Controllers/TaskController.cs` tar emot anropen och skickar tillbaka svar.
- `Services/TaskService.cs` hämtar och sparar uppgifterna i databasen.
- `Models/` beskriver en uppgift och dess status.
- `Data/AppDbContext.cs` kopplar modellerna till databasen.

Uppdelningen gör att allt inte behöver ligga i `Program.cs`.
SQLite används för att spara uppgifterna även efter omstart och för att göra
projektet enkelt att köra lokalt. Entity Framework Core gör att databasen kan
hanteras med C#-kod.

## Databas och filer

Databasen heter `taskplanner.db`. Inställningen finns i `appsettings.json`.

Uppladdade filer sparas i `Uploads/`, som skapas automatiskt. Varje fil får ett
unikt prefix i namnet för att undvika namnkonflikter. Filnamnet sparas på uppgiften
i databasen.

En uppgift visar en fil åt gången. Om en ny fil laddas upp, eller om uppgiften
tas bort, ligger den gamla filen fortfarande kvar på disk.

## Testning och felsökning

API:t har testats för att lista, skapa, uppdatera och ta bort uppgifter samt
ladda upp och hämta filer. Även saknade uppgifter och ogiltig titel har kontrollerats.
