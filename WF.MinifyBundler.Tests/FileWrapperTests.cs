namespace WF.MinifyBundler.Tests;

[TestFixture]
public class FileWrapperTests
{
    private readonly FileWrapper _fileIo = new();

    [Test]
    public void Then_Exists_should_return_true()
    {
        Assert.That(_fileIo.Exists("Scripts/existing_file.js"), Is.True);
    }

    [Test]
    public void Then_Exists_should_return_false()
    {
        Assert.That(_fileIo.Exists("Scripts/non_existing_file.js"), Is.False);
    }

    [Test]
    public void Then_GetFiles_should_return_correct()
    {
        var actual = _fileIo.GetFiles("Scripts", "js");
        Assert.That(actual.Select(x => x.Name), Is.EquivalentTo(["existing_file.js"]));
    }

    [Test]
    public void Then_GetLastWriteTime_should_return_correct()
    {
        var fileName = $"Scripts/TestFile-{Guid.NewGuid()}.txt";
        File.Create(fileName).Flush();
        var actual = _fileIo.GetLastWriteTime(fileName).ToString("yyyy-MM-dd hh:mm:ss");
            Assert.That(actual, Is.EqualTo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));
    }

    [Test]
    public void Then_ReadAllText_should_read_whole_file()
    {
        const string expected = "someText";
        var fileName = $"Scripts/TestFile-{Guid.NewGuid()}.txt";
        File.WriteAllText(fileName, expected);
        Assert.That(_fileIo.ReadAllText(fileName), Is.EqualTo(expected));
    }

    [Test]
    public void Then_WriteAllText_should_write_correct_value_to_file()
    {
        const string expected = "someText";
        var fileName = $"Scripts/TestFile-{Guid.NewGuid()}.txt";
        _fileIo.WriteAllText(fileName, expected);

        Assert.That(_fileIo.ReadAllText(fileName), Is.EqualTo(expected));
    }
}