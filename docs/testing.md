# Testing

## Unit tests

Prosjektet bruker xUnit for enhetstesting.

Følgende tester er implementert:

- Opprettelse av Book og Library
- Validering med DataAnnotations ([Required])
- Relasjon mellom Book og Library (1:N)
- Controller-test for Books (Index, Create, Delete)

Database-testing gjøres med InMemory database.

Kjør tester med:

dotnet test

