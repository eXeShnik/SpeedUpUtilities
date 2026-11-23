// See https://aka.ms/new-console-template for more information

var regexFullStr = Console.ReadLine();

if (string.IsNullOrWhiteSpace(regexFullStr))
{
    Console.WriteLine("Regex string is required");
    return;
}

var split = regexFullStr.Split("|", StringSplitOptions.RemoveEmptyEntries);

for (var i = 0; i < split.Length; i++)
{
    var ctx = split[i];

    if (string.IsNullOrWhiteSpace(ctx))
        continue;

    if (ctx.StartsWith("^") && ctx.EndsWith("\\z"))
        continue;
    
    split[i] = $"^{ctx}\\z";
}

Console.WriteLine(string.Join("|", split));