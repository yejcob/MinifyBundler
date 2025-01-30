using System.Text.RegularExpressions;

namespace WF.MinifyBundler;

public class JsMinifier
{
    // private readonly JsLexer _lexer = new JsLexer();
    //
    // public string Minify(string jsCode)
    // {
    //     if (string.IsNullOrWhiteSpace(jsCode))
    //         throw new ArgumentException("JavaScript code cannot be empty.", nameof(jsCode));
    //     
    //     var tokens = _lexer.Tokenize(jsCode);
    //     
    //     var filteredTokens = tokens
    //         .Where(t => t.Type != TokenType.Comment && t.Type != TokenType.Whitespace)
    //         .Select(t => t.Value);
    //     
    //     return string.Join("", filteredTokens);
    // }
    
    // public static string Minify(string jsCode)
    // {
    //     if (string.IsNullOrWhiteSpace(jsCode))
    //         throw new ArgumentException("JavaScript code cannot be empty.", nameof(jsCode));
    //
    //     // Remove single-line comments (//...)
    //     jsCode = Regex.Replace(jsCode, @"//.*", "");
    //
    //     // Remove multi-line comments (/* ... */)
    //     jsCode = Regex.Replace(jsCode, @"/\*[\s\S]*?\*/", "");
    //
    //     // Remove extra whitespace (multiple spaces → single space)
    //     jsCode = Regex.Replace(jsCode, @"\s+", " ");
    //
    //     // Remove spaces before and after certain characters
    //     jsCode = Regex.Replace(jsCode, @"\s*([{};,:])\s*", "$1");
    //
    //     // Trim leading and trailing whitespace
    //     return jsCode.Trim();
    // }
    
    // public static string Minify(string jsCode)
    // {
    //     if (string.IsNullOrWhiteSpace(jsCode))
    //         throw new ArgumentException("JavaScript code cannot be empty.", nameof(jsCode));
    //
    //     // Remove comments (single-line and multi-line)
    //     jsCode = Regex.Replace(jsCode, @"//.*|/\*[\s\S]*?\*/", string.Empty);
    //
    //     // Remove extra spaces (including new lines) and condense spaces inside functions
    //     jsCode = Regex.Replace(jsCode, @"\s+", " ");
    //
    //     // Remove spaces around operators (e.g., +, -, *, /)
    //     jsCode = Regex.Replace(jsCode, @"\s*([+\-*/%<>=!&|^])\s*", "$1");
    //
    //     // Remove spaces after commas
    //     jsCode = Regex.Replace(jsCode, @"\s*,\s*", ",");
    //
    //     // Remove spaces after semicolons
    //     jsCode = Regex.Replace(jsCode, @"\s*;\s*", ";");
    //
    //     // Trim leading and trailing spaces
    //     jsCode = jsCode.Trim();
    //
    //     return jsCode;
    // }
    
    // public static string Minify(string jsCode)
    // {
    //     if (string.IsNullOrWhiteSpace(jsCode))
    //         throw new ArgumentException("JavaScript code cannot be empty.", nameof(jsCode));
    //
    //     // Remove comments (single-line and multi-line)
    //     jsCode = Regex.Replace(jsCode, @"//.*|/\*[\s\S]*?\*/", string.Empty);
    //
    //     // Remove extra spaces (including new lines) and condense spaces inside functions
    //     jsCode = Regex.Replace(jsCode, @"\s+", " ");
    //
    //     // Remove spaces around operators (e.g., +, -, *, /, %, <, >, =, etc.)
    //     jsCode = Regex.Replace(jsCode, @"\s*([+\-*/%<>=!&|^])\s*", "$1");
    //
    //     // Remove spaces around curly braces {}
    //     jsCode = Regex.Replace(jsCode, @"\s*{\s*", "{");
    //     jsCode = Regex.Replace(jsCode, @"\s*}\s*", "}");
    //
    //     // Remove spaces after commas
    //     jsCode = Regex.Replace(jsCode, @"\s*,\s*", ",");
    //
    //     // Remove spaces after semicolons
    //     jsCode = Regex.Replace(jsCode, @"\s*;\s*", ";");
    //
    //     // Trim leading and trailing spaces
    //     jsCode = jsCode.Trim();
    //
    //     return jsCode;
    // }
    
    // public static string Minify(string jsCode)
    // {
    //     if (string.IsNullOrWhiteSpace(jsCode))
    //         throw new ArgumentException("JavaScript code cannot be empty.", nameof(jsCode));
    //
    //     // Preserve strings and handle them separately
    //     var stringRegex = new Regex(@"(['""][^'""]*['""])");
    //     var stringMatches = stringRegex.Matches(jsCode);
    //
    //     // Temporary list to store placeholders for the strings
    //     var placeholders = new List<string>();
    //     var originalStrings = new List<string>();
    //
    //     // Replace each string with a unique placeholder
    //     foreach (Match match in stringMatches)
    //     {
    //         string placeholder = $"__STRING_PLACEHOLDER_{placeholders.Count}__";
    //         placeholders.Add(placeholder);
    //         originalStrings.Add(match.Value); // Store the original string
    //         jsCode = jsCode.Replace(match.Value, placeholder);
    //     }
    //
    //     // Remove comments (single-line and multi-line)
    //     jsCode = Regex.Replace(jsCode, @"//.*|/\*[\s\S]*?\*/", string.Empty);
    //
    //     // Remove extra spaces (including new lines) and condense spaces inside functions
    //     jsCode = Regex.Replace(jsCode, @"\s+", " ");
    //
    //     // Remove spaces around operators (e.g., +, -, *, /, %, <, >, =, etc.)
    //     jsCode = Regex.Replace(jsCode, @"\s*([+\-*/%<>=!&|^])\s*", "$1");
    //
    //     // Remove spaces around curly braces {}
    //     jsCode = Regex.Replace(jsCode, @"\s*{\s*", "{");
    //     jsCode = Regex.Replace(jsCode, @"\s*}\s*", "}");
    //
    //     // Remove spaces after commas
    //     jsCode = Regex.Replace(jsCode, @"\s*,\s*", ",");
    //
    //     // Remove spaces after semicolons
    //     jsCode = Regex.Replace(jsCode, @"\s*;\s*", ";");
    //
    //     // Trim leading and trailing spaces
    //     jsCode = jsCode.Trim();
    //
    //     // Reinsert the original strings from the placeholders
    //     for (int i = 0; i < placeholders.Count; i++)
    //     {
    //         jsCode = jsCode.Replace(placeholders[i], originalStrings[i]);
    //     }
    //
    //     return jsCode;
    // }
    
    public static string Minify(string jsCode)
    {
        if (string.IsNullOrWhiteSpace(jsCode))
            throw new ArgumentException("JavaScript code cannot be empty.", nameof(jsCode));
    
        // Preserve strings and handle them separately
        var stringRegex = new Regex(@"(['""][^'""]*['""])");
        var stringMatches = stringRegex.Matches(jsCode);
    
        // Temporary list to store placeholders for the strings
        var placeholders = new List<string>();
        var originalStrings = new List<string>();
    
        // Replace each string with a unique placeholder
        foreach (Match match in stringMatches)
        {
            var placeholder = $"__STRING_PLACEHOLDER_{placeholders.Count}__";
            placeholders.Add(placeholder);
            originalStrings.Add(match.Value); // Store the original string
            jsCode = jsCode.Replace(match.Value, placeholder);
        }
    
        // Remove comments (single-line and multi-line)
        jsCode = Regex.Replace(jsCode, @"//.*|/\*[\s\S]*?\*/", string.Empty);
    
        // Remove extra spaces (including new lines) and condense spaces inside functions
        jsCode = Regex.Replace(jsCode, @"\s+", " ");
    
        // Remove spaces around operators (e.g., +, -, *, /, %, <, >, =, etc.)
        jsCode = Regex.Replace(jsCode, @"\s*([+\-*/%<>=!&|^])\s*", "$1");
        
        jsCode = Regex.Replace(jsCode, @"}(?=\s*(window|document))", "};");

    
        // Remove spaces around curly braces {}
        jsCode = Regex.Replace(jsCode, @"\s*{\s*", "{");
        jsCode = Regex.Replace(jsCode, @"\s*}\s*", "}");
    
        // Remove spaces around parentheses () and inside parentheses (e.g., function call, if condition)
        jsCode = Regex.Replace(jsCode, @"\s*\(\s*", "(");  // Remove space before opening parenthesis
        jsCode = Regex.Replace(jsCode, @"\s*\)\s*", ")");  // Remove space after closing parenthesis
        jsCode = Regex.Replace(jsCode, @"\s*,\s*", ",");   // Remove spaces around commas inside parentheses
    
        // Remove spaces after commas (general)
        jsCode = Regex.Replace(jsCode, @"\s*,\s*", ",");
    
        // Remove spaces after semicolons
        jsCode = Regex.Replace(jsCode, @"\s*;\s*", ";");
    
        // Trim leading and trailing spaces
        jsCode = jsCode.Trim();
    
        // Reinsert the original strings from the placeholders
        for (var i = 0; i < placeholders.Count; i++)
        {
            jsCode = jsCode.Replace(placeholders[i], originalStrings[i]);
        }
    
        return jsCode;
    }
}

