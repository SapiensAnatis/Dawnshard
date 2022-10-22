using MessagePack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers;

[Route("debug")]
[ApiController]
public class DebugController : ControllerBase
{
    [Route("msgpack")]
    [Produces("application/octet-stream")]
    [HttpGet]
    public object GetMsgpack()
    {
        return new List<object>() { new MsgpackTest(), new MsgpackTest2() };
    }
}

[MessagePackObject]
public class MsgpackTest
{
    [Key(0)]
    public int Int { get; set; } = 1;

    [Key(2)]
    public string String { get; set; } = "lol";
}

[MessagePackObject(true)]
public class MsgpackTest2
{
    public bool Bool { get; set; } = true;

    public byte[] Bytes { get; set; } = new byte[] { 1, 2, 3, 4 };
}
