namespace WF.MinifyBundler.Tests;

[TestFixture]
public class MinifierTests
{
    [Test]
    public void Minify_ShouldRemoveComments()
    {
        const string input = """
                             
                                     // This is a comment
                                     function hello() { 
                                         /* Multi-line comment */
                                         console.log('Hello, World!'); 
                                     }
                                     
                             """;

        const string expectedOutput = "function hello(){console.log('Hello, World!');}";

        var result = Minifier.Minify(input);

        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void Minify_ShouldRemoveExtraWhitespace()
    {
        const string input = "  function   test ( )   {  console.log ( 'Hello' ) ; }   ";
        const string expectedOutput = "function test(){console.log('Hello');}";

        var result = Minifier.Minify(input);

        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void Minify_ShouldPreserveFunctionality()
    {
        const string input = "var name = \"World\"; console.log('Hello, ' + name);";
        const string expectedOutput = "var name=\"World\";console.log('Hello, '+name);";

        var result = Minifier.Minify(input);

        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void Minify_ShouldHandleEmptyInput()
    {
        Assert.Throws<ArgumentException>(() => Minifier.Minify(""));
    }

    [Test]
    public void Minify_ShouldHandleOnlyComments()
    {
        const string input = """
                             
                                     // Single-line comment
                                     /* Multi-line comment */
                                     
                             """;
        const string expectedOutput = "";

        var result = Minifier.Minify(input);

        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void Minify_ShouldHandleComplexJavaScript()
    {
        const string input = """
                             
                                     function add(a, b) { 
                                         return a + b; 
                                     }
                                     var result = add(5, 10);
                                     console.log(result);
                                     
                             """;

        const string expectedOutput = "function add(a,b){return a+b;}var result=add(5,10);console.log(result);";

        var result = Minifier.Minify(input);

        Assert.That(result, Is.EqualTo(expectedOutput));
    }
}
