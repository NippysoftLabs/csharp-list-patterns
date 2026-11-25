using ListPatternsDemo;

// Runs every input through both versions and prints the outputs side by side.
// A FULLWIDTH LATIN SMALL LETTER A (U+FF41) is built from its code so the source stays ASCII.
string fullwidthAdd = (char)0xFF41 + "dd";

var inputs = new (string Label, string[]? Words)[]
{
    ("null", null),
    ("empty array", []),
    ("add milk", ["add", "milk"]),
    ("ADD milk (upper case)", ["ADD", "milk"]),
    ("U+FF41 dd milk (fullwidth a)", [fullwidthAdd, "milk"]),
    ("' add' milk (leading space)", [" add", "milk"]),
    ("add (no title)", ["add"]),
    ("add milk now (one word too many)", ["add", "milk", "now"]),
    ("add --help x (--help not last)", ["add", "--help", "x"]),
    ("add --help (--help last)", ["add", "--help"]),
    ("done abc (id is not a number)", ["done", "abc"]),
    ("tag 7 (no tags)", ["tag", "7"]),
    ("tag 7 home urgent", ["tag", "7", "home", "urgent"]),
    ("[null] (null element)", [null!]),
    ("add null (null title)", ["add", null!]),
};

Console.WriteLine($"runtime {Environment.Version}");
foreach (var (label, words) in inputs)
{
    string after = TodoCommand.Describe(words);
    string before = TodoCommand.DescribeWithIfs(words);
    string same = after == before ? "same" : "DIFFERENT";
    Console.WriteLine($"{label} => {after} [{same}]");
}

// Allocation of the slice: an array slice copies, a span slice does not.
const int Words = 1_000_002;
string[] large = new string[Words];
large[0] = "tag";
large[1] = "7";
Array.Fill(large, "x", 2, Words - 2);

TodoCommand.CountTags(large);
TodoCommand.CountTags(large.AsSpan());

long start = GC.GetAllocatedBytesForCurrentThread();
int arrayCount = TodoCommand.CountTags(large);
long arrayBytes = GC.GetAllocatedBytesForCurrentThread() - start;

start = GC.GetAllocatedBytesForCurrentThread();
int spanCount = TodoCommand.CountTags(large.AsSpan());
long spanBytes = GC.GetAllocatedBytesForCurrentThread() - start;

Console.WriteLine($"array slice of {arrayCount} tags: {arrayBytes} bytes allocated");
Console.WriteLine($"span slice of {spanCount} tags: {spanBytes} bytes allocated");
