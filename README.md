# Game-Of-Life

Conway's Game of Life ([Wikipedia](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life)) implemented in C# (.NET 10) as a console app.

Screenshot of the console application running, in this case with an [R_Pentomino](https://www.conwaylife.com/wiki/R-pentomino) starting pattern:

![](Game-Of-Life_recording.gif)

## Getting Started

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet build                                    # build the solution
dotnet test Game-Of-Life.Library                # run all tests
dotnet run --project Game-Of-Life.Console       # start the simulation
```

## Coverage

Tools are pinned in `.config/dotnet-tools.json`. Restore them once after cloning:

```bash
dotnet tool restore
```

Run tests with coverage collection, then generate the HTML report:

```bash
dotnet tool run dotnet-coverage collect --settings coverage.settings.xml "dotnet test Game-Of-Life.Library" -f cobertura -o coverage/coverage.cobertura.xml
dotnet tool run reportgenerator "-reports:coverage/coverage.cobertura.xml" "-targetdir:coverage/report" -reporttypes:Html
```

Open `coverage/report/index.html` to view the results.
