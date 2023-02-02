#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable IDE0052 // Remove unread private members

using MessagePack;
using MessagePack.AspNetCoreMvcFormatter;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace DragaliaAPI.MessagePack;

/// <summary>
/// Wrapper to MessagePack formatters that accept application/octet-stream.
/// </summary>
public class CustomMessagePackOutputFormatter : MessagePackOutputFormatter
{
    public const string ContentType = "application/octet-stream";
    private readonly MessagePackSerializerOptions options;

    public CustomMessagePackOutputFormatter()
        : this(null) { }

    public CustomMessagePackOutputFormatter(MessagePackSerializerOptions options)
        : base(options)
    {
        this.options = options;

        this.SupportedMediaTypes.Add(ContentType);
        this.SupportedMediaTypes.Add("application/x-msgpack");
    }

    public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context) =>
        base.WriteResponseBodyAsync(context);
}

public class CustomMessagePackInputFormatter : MessagePackInputFormatter
{
    private const string ContentType = "application/octet-stream";
    private readonly MessagePackSerializerOptions options;

    public CustomMessagePackInputFormatter()
        : this(null) { }

    public CustomMessagePackInputFormatter(MessagePackSerializerOptions options)
        : base(options)
    {
        this.options = options;

        this.SupportedMediaTypes.Add(ContentType);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(
        InputFormatterContext context
    ) => await base.ReadRequestBodyAsync(context);
}
