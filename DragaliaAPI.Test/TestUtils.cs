using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using DragaliaAPI.Models.Database;
using MessagePack;
using MessagePack.Resolvers;

namespace DragaliaAPI.Test.Integration
{
    public static class TestUtils
    {
        public static void InitializeDbForTests(ApiContext db)
        {
            db.DeviceAccounts.AddRange(GetDeviceAccountsSeed());
            db.PlayerSavefiles.AddRange(GetPlayerSavefilesSeed());
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(ApiContext db)
        {
            db.DeviceAccounts.RemoveRange(db.DeviceAccounts);
            InitializeDbForTests(db);
        }

        public static List<DbDeviceAccount> GetDeviceAccountsSeed()
        {
            return new()
            {
                // Password is a hash of the string "password"
                new("id", "NMvdakTznEF6khwWcz17i6GTnDA="),
            };
        }

        public static List<DbPlayerSavefile> GetPlayerSavefilesSeed()
        {
            return new()
            {
                new() { DeviceAccountId = "id", ViewerId = 10000000001 }
            };
        }

        public static HttpContent CreateMsgpackContent(byte[] content)
        {
            ByteArrayContent result = new(content);
            result.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            return result;
        }

        public static string MsgpackBytesToPrettyJson(byte[] content)
        {
            string json = MessagePackSerializer.ConvertToJson(content, ContractlessStandardResolver.Options);
            using var jDoc = JsonDocument.Parse(json);
     
            return JsonSerializer.Serialize(jDoc, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
