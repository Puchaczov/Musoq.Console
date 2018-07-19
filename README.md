# Musoq.Console

Simple client that runs Musoq queries.

# Usage

Run the `dotnet` command like so

`dotnet .Musoq.Console.Client.dll -qa "SELECT 1 FROM #system.dual()"` - calculates the query provided directly

`dotnet .Musoq.Console.Client.dll --qa "Path\To\Query.fql"` - calculates the query stored in external file.
