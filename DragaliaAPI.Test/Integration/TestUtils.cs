using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using MessagePack;
using MessagePack.Resolvers;

namespace DragaliaAPI.Test.Integration
{
    public static class TestUtils
    {
        public static void InitializeDbForTests(DeviceAccountContext db)
        {
            db.DeviceAccounts.AddRange(GetSeedingData());
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(DeviceAccountContext db)
        {
            db.DeviceAccounts.RemoveRange(db.DeviceAccounts);
            InitializeDbForTests(db);
        }

        public static List<DbDeviceAccount> GetSeedingData()
        {
            return new()
            {
                // Password is a hash of the string "password"
                new("id", "NMvdakTznEF6khwWcz17i6GTnDA="),
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
