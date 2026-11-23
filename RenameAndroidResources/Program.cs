Console.Write("Enter directory: ");

var dir = Console.ReadLine();

if (!Directory.Exists(dir))
{
    Console.WriteLine("Directory does not exist.");
    return;
}

var files = Directory.GetFiles(dir);

foreach (var file in files)
{
    string newName;

    var extension = Path.GetExtension(file);
    var name = Path.GetFileNameWithoutExtension(file);

    if (string.IsNullOrWhiteSpace(extension))
        newName = $"{name}.mp3";
    else
    {
        if (name.Length > 2 && name[0] == '_' && name[1] == '_')
            newName = $"{name[1..]}{extension}";
        else
        {
            if (!name.Contains(' ') && !name.Contains('-') && !name.Contains('.'))
            {
                var firstChar = name[0];

                if (char.IsAsciiLetterUpper(firstChar) || char.IsAsciiLetterLower(firstChar) || firstChar == '_')
                {
                    continue;
                }
            }

            newName = $"_{name.Replace(' ', '_').Replace('.', '_').Replace('-', '_')}{extension}";
        }
    }

    var newPath = Path.Combine(dir, newName);
    File.Move(file, newPath);
}