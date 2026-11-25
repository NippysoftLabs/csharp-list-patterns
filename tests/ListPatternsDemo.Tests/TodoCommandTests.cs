using ListPatternsDemo;

namespace ListPatternsDemo.Tests;

public sealed class TodoCommandTests
{
    private static readonly string[]?[] Cases =
    [
        null,
        [],
        ["add", "milk"],
        ["ADD", "milk"],
        [" add", "milk"],
        ["add"],
        ["add", "milk", "now"],
        ["add", "--help", "x"],
        ["add", "--help"],
        ["--help"],
        ["done", "12"],
        ["done"],
        ["tag", "7"],
        ["tag", "7", "home"],
        ["tag", "7", "home", "urgent"],
        ["remove", "7"],
        [null!],
    ];

    public static TheoryData<string[]?> Inputs => new(Cases);

    [Theory]
    [MemberData(nameof(Inputs))]
    public void Switch_and_ifs_agree(string[]? args)
    {
        Assert.Equal(TodoCommand.DescribeWithIfs(args), TodoCommand.Describe(args));
    }

    [Fact]
    public void Add_needs_exactly_two_words()
    {
        Assert.Equal("Add task: milk", TodoCommand.Describe(["add", "milk"]));
        Assert.Equal("Unknown or incomplete command: add", TodoCommand.Describe(["add", "milk", "now"]));
    }

    [Fact]
    public void Help_counts_only_as_the_last_word()
    {
        // The pattern compares positions, not membership: "--help" in the middle is ignored.
        Assert.Equal(TodoCommand.Usage, TodoCommand.Describe(["add", "--help"]));
        Assert.Equal("Unknown or incomplete command: add", TodoCommand.Describe(["add", "--help", "x"]));
    }

    [Fact]
    public void Constant_patterns_on_strings_are_ordinal_and_case_sensitive()
    {
        Assert.Equal("Unknown or incomplete command: ADD", TodoCommand.Describe(["ADD", "milk"]));
        Assert.Equal("Unknown or incomplete command: " + (char)0xFF41 + "dd",
            TodoCommand.Describe([(char)0xFF41 + "dd", "milk"]));
    }

    [Fact]
    public void Tag_without_tags_is_incomplete()
    {
        Assert.Equal("Unknown or incomplete command: tag", TodoCommand.Describe(["tag", "7"]));
        Assert.Equal("Tag task 7: home, urgent", TodoCommand.Describe(["tag", "7", "home", "urgent"]));
    }

    [Fact]
    public void Var_pattern_accepts_a_null_element()
    {
        Assert.Equal("Add task: ", TodoCommand.Describe(["add", null!]));
    }

    [Fact]
    public void Array_and_span_slices_count_the_same_tags()
    {
        string[] args = ["tag", "7", "home", "urgent"];
        Assert.Equal(2, TodoCommand.CountTags(args));
        Assert.Equal(2, TodoCommand.CountTags(args.AsSpan()));
        Assert.Equal(0, TodoCommand.CountTags(["tag", "7"]));
        Assert.Equal(0, TodoCommand.CountTags(["done", "7", "x"]));
    }
}
