namespace WF.MinifyBundler.Tests;

[TestFixture]
public class CompressorTests
{
    [TestCase("var a = 1;  ", "var a= 1;")]
    [TestCase("function test() {  return true; }", "function test(){ return true;}")]
    [TestCase("var x = 'Hello'; // Comment", "var x='Hello';")]
    public void Compress_ShouldMinifyJavaScript(string input, string expected)
    {
        var result = Compressor.Compress(input);
        
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Compress_ShouldRemoveSingleLineComments()
    {
        const string input = "var x = 5; // This is a comment";
        const string expected = "var x= 5;";
        var result = Compressor.Compress(input);
        
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Compress_ShouldRemoveMultiLineComments()
    {
        const string input = "var x = 5; /* This is a \n multiline comment */ var y = 10;";
        const string expected = "var x= 5; var y= 10;";
        var result = Compressor.Compress(input);
        
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Compress_ShouldPreserveStringLiterals()
    {
        const string input = "var str = \"Hello  World\";";
        const string expected = "var str=\"Hello  World\";"; // Spaces inside strings should be preserved
        var result = Compressor.Compress(input);
        
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Compress_ShouldThrowExceptionForUnterminatedString()
    {
        const string input = "var str = \"Hello;";
        
        var ex = Assert.Throws<Compressor.CompressionException>(() => Compressor.Compress(input));
        Assert.That(ex.Message, Contains.Substring("Unterminated string"));
    }

    [Test]
    public void Compress_ShouldThrowExceptionForUnterminatedComment()
    {
        const string input = "var x = 5; /* This is an unterminated comment ";
        
        var ex = Assert.Throws<Compressor.CompressionException>(() => Compressor.Compress(input));
        Assert.That(ex.Message, Is.EqualTo("Unterminated comment."));
    }

    [Test]
    public void Compress_ShouldHandleEmptyInput()
    {
        const string input = "";
        const string expected = "";
        var result = Compressor.Compress(input);
        
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Compress_ShouldNotModifyValidRegularExpressions()
    {
        const string input = "var regex = /\\s+/g;";
        const string expected = "var regex=/\\s+/g;";
        var result = Compressor.Compress(input);
        
        Assert.That(result, Is.EqualTo(expected));
    }
}
