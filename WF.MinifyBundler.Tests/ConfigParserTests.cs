namespace WF.MinifyBundler.Tests;

[TestFixture]
public class ConfigParserTests
{
    [Test]
    public void ParseJson_should_return_correct_config()
    {
        var expected =  new CompilerOptions{FileType = "js", OutputPath = "./test.js", SourceFolders = ["Scripts"]};
        
        var actual = ConfigParser.ParseJson("""[{"fileType":"js", "outputPath":"./test.js","sourceFolders":["Scripts"]}]""");
        Assert.Multiple(() =>
        {
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().OutputPath, Is.EqualTo(expected.OutputPath));
            Assert.That(actual.First().FileType, Is.EqualTo(expected.FileType));
            Assert.That(actual.First().SourceFolders, Is.EqualTo(expected.SourceFolders));
        });
    }
}