// See https://aka.ms/new-console-template for more information

using System.Text;
using SixLabors.ImageSharp;

var baseSize = 4f;

var directoryPath = string.Empty;

if (args.Length > 0)
{
    directoryPath = args[0];
    
    if (args.Length > 1)
        baseSize = float.Parse(args[1]);
}
else
{
    Console.WriteLine("Please provide a directory path.");
    directoryPath = Console.ReadLine();
}

var files = Directory.GetFiles(directoryPath, "*.png", SearchOption.TopDirectoryOnly);

var sb = new StringBuilder(); 

foreach (var file in files)
{
    var image = Image.Load(file);
    var fileName = Path.GetFileName(file);
    var baseWidth = image.Width / baseSize;
    var baseHeight = image.Height / baseSize;
    var mauiImage = $"<MauiImage Include=\"Resources\\Images\\{fileName}\" BaseSize=\"{Math.Floor(baseWidth)},{Math.Floor(baseHeight)}\" />";
    Console.WriteLine(mauiImage);
    sb.AppendLine(mauiImage);
}

await TextCopy.ClipboardService.SetTextAsync(sb.ToString());