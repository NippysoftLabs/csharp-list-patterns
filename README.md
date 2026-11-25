# List patterns in C#

A small console app that parses the words of a to-do command line twice: once with `if`
statements, lengths and indexes, and once with a `switch` expression and list patterns
(C# 11 and later). A test checks that both versions return the same text for every input.

```csharp
public static string Describe(string[]? args) => args switch
{
    null or [] => Usage,
    [.., "--help"] => Usage,
    ["add", var title] => $"Add task: {title}",
    ["done", var id] => $"Complete task {id}",
    ["tag", var id, .. { Length: > 0 } tags] => $"Tag task {id}: {string.Join(", ", tags)}",
    [var command, ..] => $"Unknown or incomplete command: {command}",
};
```

## Things the code shows on purpose

- A list pattern compares **positions and length**, not membership. `[.., "--help"]` only looks
  at the last word, so `add --help x` is not a help request. Use `Contains` to ask "is it anywhere".
- `["add", var title]` needs exactly two words. A slice (`..`) is what allows more.
- A slice matches zero or more elements. `.. { Length: > 0 } tags` asks for at least one tag.
- String constants in a pattern are compared ordinally: `ADD` and a fullwidth `add` do not match `add`.
- `var` accepts a `null` element.
- The `null or []` arm is needed: a list pattern never matches `null`, and no other arm accepts an
  empty list. Without that arm the compiler warns with CS8509 and a `null` or empty input throws
  `SwitchExpressionException`. If only `null` is missing (`[] => ...`), the warning is CS8655.
- On an array, a slice with a designation (`.. var tags`) is a **new array**: on 1,000,000 tags it
  allocated 8,000,024 bytes. The same pattern over a `ReadOnlySpan<string>` allocated 0 bytes
  (see `results/probe.txt`).

## Run

Requirements: .NET SDK 10.0.401 or a later 10.0 patch.

```bash
dotnet test --solution ListPatternsDemo.slnx
dotnet run --project src/ListPatternsDemo -c Release
```

The outputs of one run are in `results/`.

## What this code does not protect

- Case or Unicode variants of a command (`ADD`, fullwidth letters, leading spaces): normalize the
  input before matching if they must be accepted.
- `null` elements inside the array: `var` captures them and the text shows an empty value.
- The meaning of the values: `done abc` is accepted as an id. Validate ids after matching.
- `IEnumerable<T>` sources: list patterns need a `Length` or `Count` property and an indexer, so
  a lazy sequence must be materialized first, which reads all of it.

## License

MIT. See `LICENSE`.
