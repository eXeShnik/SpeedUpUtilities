using System.Text;
using System.Xml;

string? pathToResxFile;
var className = "StringKeys";
var withoutOrderFlag = false;

if (args.Length == 0)
{
    Console.WriteLine("Please enter path to resx file");
    pathToResxFile = Console.ReadLine();
}
else
{
    var index = 0;
    pathToResxFile = args[index++];

    if (args.Length > 1)
        className = args[index++];

    if (args.Length > 2)
        withoutOrderFlag = "no-order".Equals(args[index]);
}

if (string.IsNullOrWhiteSpace(pathToResxFile))
{
    Console.WriteLine("Path to resx file is required");
    return;
}

var resxXml = new XmlDocument();
resxXml.Load(pathToResxFile);

var rootNode = resxXml.GetElementsByTagName("root")[0]!.ChildNodes;
var keys = new List<string>();

foreach (XmlElement node in rootNode)
{
    if (!"data".Equals(node.Name))
        continue;

    keys.Add(node.Attributes["name"]!.Value);
}

var sb = new StringBuilder();
sb.AppendLine($"public static class {className}");
sb.AppendLine("{");

var orderedKeys = !withoutOrderFlag ? keys.Order().ToArray() : keys.ToArray();

foreach (var key in orderedKeys)
    sb.AppendLine($"\tpublic const string {key} = nameof({key});");

sb.AppendLine("}");
await TextCopy.ClipboardService.SetTextAsync(sb.ToString());