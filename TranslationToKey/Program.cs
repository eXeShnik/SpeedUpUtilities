using System.Text;

string? input;
var isKeyOnlyOutput = false;
var buffer = new List<string>();
var outputBuffer = new StringBuilder();

const string keyFlag = "-key";
const string fileBufferPathFlag = "-file";
const string quitKey = "q";

if (args.Length == 0)
{
    Console.WriteLine("Please enter translation value");
    input = Console.ReadLine();
}
else
{
    for (var index = 0; index < args.Length; index++)
    {
        var arg = args[index];

        if (keyFlag.Equals(arg))
        {
            isKeyOnlyOutput = true;
            continue;
        }

        switch (fileBufferPathFlag.Equals(arg))
        {
            case true:
            {
                if (index + 1 < args.Length)
                {
                    var filePath = args[index + 1];
                    if (File.Exists(filePath))
                    {
                        buffer.AddRange(File.ReadAllLines(filePath));
                        index++;
                    }
                }

                continue;
            }
            case false:
                buffer.Add(arg);
                break;
        }

    }

    buffer.Add("q");
    input = buffer[0];
}

var shouldUseBuffer = buffer.Count > 0;

while (!quitKey.Equals(input))
{
    if (shouldUseBuffer)
    {
        input = buffer[0];
        buffer.RemoveAt(0);
    }

    if (!string.IsNullOrWhiteSpace(input))
    {
        var key = string.Empty;
        var isNotLetterBefore = true;
        foreach (var ch in input)
        {
            if (char.IsLetter(ch))
            {
                key += isNotLetterBefore ? char.ToUpperInvariant(ch) : ch;
                isNotLetterBefore = false;
                continue;
            }

            isNotLetterBefore = true;
        }

        if (isKeyOnlyOutput)
            outputBuffer.AppendLine($"\nKey: {key}\nValue: {input}\n");
        else
            outputBuffer.AppendLine("<data name=\"" + key + "\" xml:space=\"preserve\">\n    <value>" + input + "</value>\n</data>\n");
    }

    input = Console.ReadLine();
}

var output = outputBuffer.ToString();

if (isKeyOnlyOutput)
    Console.WriteLine(output);

await TextCopy.ClipboardService.SetTextAsync(output);