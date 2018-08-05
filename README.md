# Musoq.Console

Simple client that runs Musoq queries.

# How do I make query

Run the `dotnet` command like so

`dotnet .Musoq.Console.Client.dll -q "SELECT 1 FROM #system.dual()"` - calculates the query provided directly

`dotnet .Musoq.Console.Client.dll --qs "Path\To\Query.fql"` - calculates the query stored in external file.

Those are switches you can use

    --q "your query here" - Put your query here.
    --qs "your .fql file here" - Put file here if you want to load your query from file instead of passing it through the argument.
    --sd "your file here" - If you want to store the score of processed file as CSV file.
    --compileOnly - compiles query without running it.
    --outputTranslatedQuery "Path/To/File.cs" - Translated query will be stored on disk.

