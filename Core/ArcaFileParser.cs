using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace Arca.Core;

public class ArcaFileParser {

    private readonly string file;
    private ArcaDefinition result;

    public ArcaFileParser(string file)
    {
        this.file = file;
        result = new ArcaDefinition();
    }

    public ArcaDefinition Parse() {

        result ??= new ArcaDefinition();

        var lines = File.ReadAllLines(file);
        var currentSection = Section.UNKNOWN;

        for(int i = 0; i < lines.Length; i++) {
            var line = lines[i];
            
            if(SkipLine(line)) {
                continue;
            }

            if(line.Trim().StartsWith("#define")) {
                currentSection = Section.DEFINE;
                continue;
            }

            if(line.Trim().StartsWith("#enifed")) {
                currentSection = Section.PAIRS;
                continue;
            }

            if(currentSection == Section.DEFINE) {
                var spl = line.Split("=");
                var constant = new Constant(spl[0].Trim(), spl[1].Trim());
                result.Constants.Add(constant.Key, constant);
                continue;
            }

            if(line.Trim().StartsWith("*")) {
                currentSection = Section.PAIRS;
                i = ParsePairs(lines, i);
            }
        }

        return result;
    }

    private void FillWithConstantValues(CallDefinition definition) {
        definition.Url = ReplaceValues(definition.Url);
        definition.Body = ReplaceValues(definition.Body);
    }

    private string ReplaceValues(string? inputText)
    {
        if(string.IsNullOrEmpty(inputText)) {
            return string.Empty;
        }

        foreach (var constant in result.Constants.Values)
        {
            // Build the regular expression pattern for each placeholder
            string pattern = @"\$" + Regex.Escape(constant.Key) + @"\$";

            // Replace the placeholder with the replacement value using Regex
            inputText = Regex.Replace(inputText, pattern, constant.Value);
        }

        return inputText;
    }

    private static bool SkipLine(string? line) {
        return string.IsNullOrWhiteSpace(line)
            || line.Trim().StartsWith(@"//");
    }

    private int ParsePairs(string[] lines, int i)
    {
        var name = lines[i].Replace("*", "").Trim();

        CallDefinition onA = null;
        CallDefinition onB = null;
        for(; i < lines.Length; i++) {
            var line = lines[i];
            if(SkipLine(line)) {
                continue;
            }

            line = line.Trim();
            var spl = line.Split(" ");
            var on = spl[0];
            if(on == "onA:") {
                onA = new CallDefinition(i, ParseMethod(spl[1]), spl[2], spl.Length > 3 ? spl[3] : null);
                FillWithConstantValues(onA);
                continue;
            }

            if(on == "onB:") {
                onB = new CallDefinition(i, ParseMethod(spl[1]), spl[2], spl.Length > 3 ? spl[3] : null);
                FillWithConstantValues(onB);
                continue;
            }

            if(onA != null && onB != null) {
                result.Calls.Add(new CallPair(name, onA, onB));
                return i;
            }
        }

        if(onA != null && onB != null) {
            result.Calls.Add(new CallPair(name, onA, onB));
            return i;
        }

        throw new Exception("Pair not well-formatted");
    }

    private static HttpMethod ParseMethod(string methodName) {
        return methodName.ToUpper() switch
        {
            "GET" => HttpMethod.Get,
            "POST" => HttpMethod.Post,
            "PUT" => HttpMethod.Put,
            "DELETE" => HttpMethod.Delete,
            "PATCH" => HttpMethod.Patch,
            _ => throw new ArgumentException($"Unsuspported Http Method {methodName}"),
        };
    }
}

file enum Section {
    UNKNOWN = 0,
    DEFINE = 1,
    PAIRS = 2,
    SKIP_PAIR = 3
}