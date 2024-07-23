using System.Xml;

const string LightThemePrefix = "Light";
const string DarkThemePrefix = "Dark";

Console.WriteLine("Put path to App.xaml file:");
// var pathToAppXaml = Console.ReadLine();
var pathToAppXaml = "/Users/vitaliioverchuk/Desktop/Projects/UpWork/semex-go/SemexGo/SemexGo/App.xaml";
if (string.IsNullOrWhiteSpace(pathToAppXaml))
{
    Console.WriteLine("Path to App.xaml file is required");
    return;
}

var appXamlXml = new XmlDocument();
appXamlXml.Load(pathToAppXaml);

var xamlNamespaceUri = appXamlXml.DocumentElement!.Attributes["xmlns"]!.Value;
var xamlXNamespaceUri = appXamlXml.DocumentElement!.Attributes["xmlns:x"]!.Value;

var systemPrefix = "system";
var systemNamespaceUri = $"xmlns:{systemPrefix}=\"clr-namespace:System;assembly=System.Runtime\"";

var preDefinedStaticColors = appXamlXml
    .GetElementsBy(element => "Color".Equals(element.Name))
    .ToDictionary(x => x.Attributes["x:Key"]!.Value, element => element);

var preDefinedStaticColorsToRemove = new List<XmlElement>();

Console.WriteLine("Put path to themes folder:");
// var pathToThemesFolder = Console.ReadLine();
var pathToThemesFolder = "/Users/vitaliioverchuk/Desktop/Projects/UpWork/semex-go/SemexGo/SemexGo/Themes";
if (string.IsNullOrWhiteSpace(pathToThemesFolder))
{
    Console.WriteLine("Path to themes folder is required");
    return;
}

var i = 0;
var themeFiles = Directory.GetFiles(pathToThemesFolder, "*.xaml");
var themeFilesXml = themeFiles.Select(
    x =>
    {
        var xml = new XmlDocument();
        xml.Load(x);
        return xml;
    }
).ToDictionary(x => themeFiles[i++], document => document);

var themeFilesDefinedColors = themeFilesXml
    .ToDictionary(
        x => x.Key,
        x => x.Value
            .GetElementsBy(element => "Color".Equals(element.Name))
            .ToDictionary(x1 => x1.Attributes["x:Key"]!.Value, element => element)
    );

Console.WriteLine("Put path to root directory:");
// var rootDirectory = Console.ReadLine()!;
var rootDirectory = "/Users/vitaliioverchuk/Desktop/Projects/UpWork/semex-go/SemexGo/SemexGo";
if (string.IsNullOrWhiteSpace(rootDirectory))
{
    Console.WriteLine("Path to root directory is required");
    return;
}

var xamlFiles = Directory
    .GetFiles(rootDirectory, "*.xaml", SearchOption.AllDirectories)
    .Where(x => !themeFiles.Contains(x))
    .ToList();

i = 0;
var xamlFilesXml = xamlFiles.Select(
    x =>
    {
        if (x.EndsWith("App.xaml"))
            return appXamlXml;

        var xml = new XmlDocument();
        xml.Load(x);
        return xml;
    }
).ToDictionary(x => xamlFiles[i++], document => document);

var appThemeBindings = xamlFilesXml
    .ToDictionary(
        x => x.Key,
        x =>
        {
            var elements = x.Value.GetElementsBy(_ => true);

            return elements
                .Select(
                    x1 =>
                    {
                        List<XmlAttribute>? appThemeBindingAttributes = null;

                        foreach (var argAttribute in x1.Attributes)
                        {
                            if (argAttribute is not XmlAttribute xmlAttribute || !xmlAttribute.InnerText.Contains("AppThemeBinding"))
                                continue;

                            var justThemeStrings = xmlAttribute.InnerText
                                .Substring(1, xmlAttribute.InnerText.Length - 2)
                                .TrimStart("AppThemeBinding ".ToCharArray())
                                .Replace(", ", ",");

                            if (!justThemeStrings.StartsWith("Light"))
                            {
                                var split = justThemeStrings.Split(",");
                                xmlAttribute.InnerText = $"{{AppThemeBinding {split[1]}, {split[0]}}}";
                            }

                            appThemeBindingAttributes ??= new List<XmlAttribute>();
                            appThemeBindingAttributes.Add(xmlAttribute);
                        }

                        return appThemeBindingAttributes;
                    }
                )
                .Where(x1 => x1 != null);
        }
    )
    .Where(x => x.Value.Any())
    .ToDictionary(x => x.Key, x => x.Value.ToList());

var sameAppThemeBindingGroups = appThemeBindings.Aggregate(
    new List<XmlAttribute>(),
    (list, pair) =>
    {
        foreach (var attrs in pair.Value!)
            list.AddRange(attrs!);
        
        return list;
    }
).GroupBy(x => x.InnerText);

i = 0;
foreach (var sameAppThemeBinding in sameAppThemeBindingGroups)
{
    try
    {
        var themeColors = sameAppThemeBinding.Key
            .Substring(1, sameAppThemeBinding.Key.Length - 2)
            .TrimStart("AppThemeBinding ".ToCharArray())
            .Split(", ");

        var lightThemeColor = themeColors[0].Replace("Light=", string.Empty);
        var isLightUseResources = lightThemeColor.Contains("Resource ");
        var lightColorRef = isLightUseResources ? lightThemeColor.Split(" ")[1].TrimEnd('}') : lightThemeColor;

        var darkThemeColor = themeColors[1].Replace("Dark=", string.Empty);
        var isDarkUseResources = darkThemeColor.Contains("Resource ");
        var darkColorRef = isDarkUseResources ? darkThemeColor.Split(" ")[1].TrimEnd('}') : darkThemeColor;

        var lightColorValue = isLightUseResources ? preDefinedStaticColors[lightColorRef].InnerText : lightThemeColor;
        var darkColorValue = isDarkUseResources ? preDefinedStaticColors[darkColorRef].InnerText : darkThemeColor;

        if (isLightUseResources)
            preDefinedStaticColorsToRemove.Add(preDefinedStaticColors[lightColorRef]);

        if (isDarkUseResources)
            preDefinedStaticColorsToRemove.Add(preDefinedStaticColors[darkColorRef]);

        var dynamicResourceKey = string.Empty;

        if (isLightUseResources && isDarkUseResources)
        {
            var isLightColorThemeRelated = lightColorRef.StartsWith("Light");
            var isDarkColorThemeRelated = darkColorRef.StartsWith("Dark");
            var isBothThemeRelated = isLightColorThemeRelated && isDarkColorThemeRelated;

            var isLightColorThemeRelatedInverted = lightColorRef.StartsWith("Dark");
            var isDarkColorThemeRelatedInverted = darkColorRef.StartsWith("Light");
            var isBothThemeRelatedInverted = isLightColorThemeRelatedInverted && isDarkColorThemeRelatedInverted;

            var lightColorKey = lightColorRef.ExtractKeyFromThemeColor();
            var darkColorKey = darkColorRef.ExtractKeyFromThemeColor();

            if (isBothThemeRelated || isBothThemeRelatedInverted)
            {
                if (string.Equals(lightColorKey, darkColorKey))
                    dynamicResourceKey = isBothThemeRelatedInverted ? $"Inverted{lightColorKey}" : lightColorKey;
            }
        }

        if (string.IsNullOrEmpty(dynamicResourceKey))
        {
            var lightPart = lightColorRef.ExtractKeyFromThemeColor();
            var darkPart = darkColorRef.ExtractKeyFromThemeColor();
            dynamicResourceKey = $"Group{lightPart}{darkPart}";
        }

        foreach (var themeFilesDefinedColor in themeFilesDefinedColors)
        {
            if (themeFilesDefinedColor.Value.ContainsKey(dynamicResourceKey))
                continue;

            var themeXml = themeFilesDefinedColor.Value.First().Value.OwnerDocument;

            var newChild = themeXml.CreateElement("Color", xamlNamespaceUri);

            var keyAttribute = themeXml.CreateAttribute("x", "Key", xamlXNamespaceUri);
            keyAttribute.InnerText = dynamicResourceKey;
            newChild.SetAttributeNode(keyAttribute);

            newChild.InnerText = Path.GetFileName(themeFilesDefinedColor.Key).Contains(LightThemePrefix) ? lightColorValue : darkColorValue;
            themeXml.DocumentElement!.AppendChild(newChild);
            themeXml.Save(themeFilesDefinedColor.Key);
            themeFilesDefinedColor.Value.Add(dynamicResourceKey, newChild);
        }

        foreach (var xmlAttribute in sameAppThemeBinding)
            xmlAttribute.InnerText = $"{{DynamicResource {dynamicResourceKey}}}";
    }
    catch (Exception e)
    {
        Console.WriteLine($"Problem with {sameAppThemeBinding.Key}");
        Console.WriteLine(e);
        Console.WriteLine("------------------------------------------------");
    }
}

foreach (var xmlElement in preDefinedStaticColorsToRemove)
    xmlElement.ParentNode?.RemoveChild(xmlElement);

foreach (var xmlDocument in xamlFilesXml)
    xmlDocument.Value.Save(xmlDocument.Key);

static class Utils
{
    public static string ExtractKeyFromThemeColor(this string themeColor)
    {
        string? themePrefix = null;
        string? extraPrefix = null;

        if (themeColor.StartsWith("Light"))
        {
            themePrefix = "Light";
        }
        else if (themeColor.StartsWith("LightMode"))
        {
            themePrefix = "Light";
            extraPrefix = "Mode";
        }
        else if (themeColor.StartsWith("Dark"))
        {
            themePrefix = "Dark";
        }
        else if (themeColor.StartsWith("DarkMode"))
        {
            themePrefix = "Dark";
            extraPrefix = "Mode";
        }

        return themePrefix == null ? themeColor : themeColor.Replace(themePrefix + extraPrefix, string.Empty);
    }

    public static List<XmlElement> GetElementsBy(this XmlNode node, Predicate<XmlElement> predicate, List<XmlElement>? buffer = null)
    {
        buffer ??= new List<XmlElement>();

        foreach (XmlNode child in node)
        {
            if (child is XmlElement xmlElement && predicate(xmlElement))
                buffer.Add(xmlElement);

            child.GetElementsBy(predicate, buffer);
        }

        return buffer;
    }
}