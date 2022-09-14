using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Models;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

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
                new() { Id = "id", HashedPassword = "NMvdakTznEF6khwWcz17i6GTnDA=" },
            };
        }
    }
}
