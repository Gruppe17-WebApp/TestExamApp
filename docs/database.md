# Database
Prosjektet bruker SQLite sammen med Entity Framework Core for lagring av data.

Databasen er satt opp i `Data/ApplicationDbContext.cs` og inneholder tabellene `Libraries` og `Books`.

Det er laget en en-til-mange-relasjon mellom bibliotek og bøker:
- Ett bibliotek kan ha flere bøker
- Hver bok er koblet til ett bibliotek gjennom `LibraryId`

Databasen opprettes med migreringen:

`20260504101947_InitialCreate`


For å opprette databasen kan følgende kommando brukes:
dotnet ef database update

## SeedData:
Data/SeedData.cs legger inn Drammen bibliotek og 3 bøker når databasen er tom.