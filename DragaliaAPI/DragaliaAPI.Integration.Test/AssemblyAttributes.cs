// Capture console output and report it to XUnit. Temporary replacement for MartinCostello.Logging.Xunit
// https://github.com/martincostello/xunit-logging/issues/717

using DragaliaAPI.Integration.Test;

[assembly: CaptureConsole]
[assembly: AssemblyFixture(typeof(CustomWebApplicationFactory))]
