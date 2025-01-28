using Microsoft.Build.Framework;
using NSubstitute;

namespace WF.MinifyBundler.Tests;

[TestFixture]
public class BundlerTests
{
    private const string CompilerSettingsFile = "compilerSettings.json";
    private const string Destination = "wwwroot/bundler.min.js";
    private const string SourceDirectory = "Scripts";
    private readonly DateTime _destinationTime = DateTime.Parse("2025-01-16 14:20").ToUniversalTime();
    private readonly DateTime _newContentTime = DateTime.Parse("2025-01-16 15:20").ToUniversalTime();
    private readonly DateTime _oldContentTime = DateTime.Parse("2025-01-15 12:20").ToUniversalTime();
    private readonly Bundler _bundler = new(){CompilerSettingsFile = CompilerSettingsFile};
    private readonly IBuildEngine _buildEngine = Substitute.For<IBuildEngine>();
    private readonly IFileWrapper _fileWrapper = Substitute.For<IFileWrapper>();

    [OneTimeSetUp]
    public void CreateBuildEngine()
    {
        _bundler.BuildEngine = _buildEngine;
        _bundler.FileWrapper = _fileWrapper;
        _fileWrapper.Exists(CompilerSettingsFile).Returns(true);
        _fileWrapper.ReadAllText(CompilerSettingsFile).Returns(File.ReadAllText(CompilerSettingsFile));
        _fileWrapper.GetLastWriteTime(Destination).Returns(_destinationTime);
        _fileWrapper.GetFiles(SourceDirectory, "js").Returns([new FileInfo("Scripts/existing_file.js") { LastWriteTime = _oldContentTime }]);
        _fileWrapper.ReadAllText(Arg.Any<string>()).Returns("""
                                                            function F(){
                                                                console.log('hejsan');
                                                            }
                                                            
                                                            /*function K(){
                                                                console.log('hello');
                                                            }*/
                                                            
                                                            function K(){
                                                                console.log('hello');
                                                            }
                                                            """);
    }

    [SetUp]
    public void ResetReceivedCalls()
    {
        _fileWrapper.ClearReceivedCalls();
    }

    [Test]
    public void Should_create_file_when_not_existing()
    {
        _fileWrapper.Exists(Destination).Returns(false);

        _bundler.Execute();

        _fileWrapper.Received(1).WriteAllText(Destination, Arg.Any<string>());
        Assert.That(_bundler.GeneratedFiles.Length, Is.EqualTo(1));
    }

    [Test]
    public void Should_do_nothing_when_file_is_up_to_date()
    {
        _bundler.TimeProvided = _oldContentTime;
        _fileWrapper.Exists(Destination).Returns(true);

        _bundler.Execute();

        _fileWrapper.DidNotReceive().WriteAllText(Destination, Arg.Any<string>());
    }

    [Test]
    public void Should_update_file_when_existing_and_has_new_content()
    {
        _bundler.TimeProvided = _newContentTime;
        _fileWrapper.Exists(Destination).Returns(true);

        _bundler.Execute();

        _fileWrapper.Received(1).WriteAllText(Destination, Arg.Any<string>());
    }
}