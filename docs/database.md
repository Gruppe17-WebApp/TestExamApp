# Database:
Prosjektet bruker SQLite og Entity Framework Core.

## DbContext:
Data/ApplicationDbContext.cs

## Tabeller:
- Libraries
- Books

## Relasjon:
Library (1) -> Books (many)

## Book har fremmednøkkel:
LibraryId

## Migration:
20260504101947_InitialCreate

## Opprett database:
dotnet ef database update

## SeedData:
Data/SeedData.cs legger inn Drammen bibliotek og 3 bøker når databasen er tom.