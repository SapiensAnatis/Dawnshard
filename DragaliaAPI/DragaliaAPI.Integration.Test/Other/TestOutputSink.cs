using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace DragaliaAPI.Integration.Test.Other;

/// <summary>
/// Serilog sink that routes log events emitted by the shared test web server to the
/// <see cref="ITestOutputHelper"/> of the test that triggered them.
/// </summary>
/// <remarks>
/// A single web server is shared across all tests, which may run in parallel, so
/// <see cref="TestContext.Current"/> is not reliable when a log is emitted on a request-handling
/// thread. Tests register their output helper via <see cref="Register"/> and tag their requests
/// with an <c>Xunit-Test-Id</c> header (see <see cref="HttpClientExtensions"/>); the sink uses that
/// header to resolve the correct helper for each event.
/// </remarks>
internal sealed class TestOutputSink : ILogEventSink
{
    private const string TestIdHeader = "Xunit-Test-Id";

    private static readonly ConcurrentDictionary<string, ITestOutputHelper> OutputHelpers = new();

    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly MessageTemplateTextFormatter formatter = new(
        "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}"
    );

    public TestOutputSink(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Associates a test's unique ID with its output helper so that logs emitted while handling that
    /// test's requests can be routed back to it.
    /// </summary>
    public static void Register(string testId, ITestOutputHelper outputHelper) =>
        OutputHelpers[testId] = outputHelper;

    public void Emit(LogEvent logEvent)
    {
        ITestOutputHelper? outputHelper = this.ResolveOutputHelper();
        if (outputHelper is null)
        {
            return;
        }

        using StringWriter writer = new();
        this.formatter.Format(logEvent, writer);

        try
        {
            outputHelper.Write(writer.ToString());
        }
        catch (InvalidOperationException)
        {
            // The test owning this output helper has already finished; drop the late log rather than
            // crashing the shared server.
        }
    }

    private ITestOutputHelper? ResolveOutputHelper()
    {
        // Logs emitted on the test's own thread (e.g. during setup) carry the ambient test context.
        if (TestContext.Current is { TestOutputHelper: { } helper })
        {
            return helper;
        }

        // Logs emitted while handling a request run on a server thread with no ambient test context,
        // so fall back to the test ID that the client stamped onto the request.
        if (
            this.httpContextAccessor.HttpContext is { } httpContext
            && httpContext.Request.Headers.TryGetValue(TestIdHeader, out StringValues testIds)
            && testIds is [{ } testId]
            && OutputHelpers.TryGetValue(testId, out ITestOutputHelper? requestHelper)
        )
        {
            return requestHelper;
        }

        return null;
    }
}
