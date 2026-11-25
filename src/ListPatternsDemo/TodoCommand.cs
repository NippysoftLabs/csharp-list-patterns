namespace ListPatternsDemo;

/// <summary>
/// Turns the words of a small to-do command line into a description of what would run.
/// The same rules are written twice: with if statements and indexes, and with list patterns.
/// </summary>
public static class TodoCommand
{
    public const string Usage = "Usage: todo add <title> | done <id> | tag <id> <tag>...";

    /// <summary>Before: length checks and indexes, one if per rule.</summary>
    public static string DescribeWithIfs(string[]? args)
    {
        if (args == null || args.Length == 0)
        {
            return Usage;
        }

        if (args[args.Length - 1] == "--help")
        {
            return Usage;
        }

        if (args.Length == 2 && args[0] == "add")
        {
            return $"Add task: {args[1]}";
        }

        if (args.Length == 2 && args[0] == "done")
        {
            return $"Complete task {args[1]}";
        }

        if (args.Length >= 3 && args[0] == "tag")
        {
            string[] tags = args[2..];
            return $"Tag task {args[1]}: {string.Join(", ", tags)}";
        }

        return $"Unknown or incomplete command: {args[0]}";
    }

    /// <summary>After: one switch arm per rule, each arm states the shape it expects.</summary>
    public static string Describe(string[]? args) => args switch
    {
        null or [] => Usage,
        // Only the last word is compared: "--help" anywhere else is an ordinary word.
        [.., "--help"] => Usage,
        // Exactly two words: "add milk now" does not match this arm.
        ["add", var title] => $"Add task: {title}",
        ["done", var id] => $"Complete task {id}",
        // The slice keeps every word after the id; the property pattern asks for at least one.
        ["tag", var id, .. { Length: > 0 } tags] => $"Tag task {id}: {string.Join(", ", tags)}",
        // Any other non-empty input: only the first word is read.
        [var command, ..] => $"Unknown or incomplete command: {command}",
    };

    /// <summary>
    /// Counts the tags of a "tag" command. On an array, the slice is a new array with copied elements.
    /// </summary>
    public static int CountTags(string[] args) => args is ["tag", _, .. var tags] ? tags.Length : 0;

    /// <summary>
    /// The same count over a span: the slice is a view of the same memory, so nothing is copied.
    /// </summary>
    public static int CountTags(ReadOnlySpan<string> args) => args is ["tag", _, .. var tags] ? tags.Length : 0;
}
