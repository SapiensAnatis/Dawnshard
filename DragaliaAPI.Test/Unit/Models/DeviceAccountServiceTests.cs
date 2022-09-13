using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace DragaliaAPI.Test.Unit.Models
{
    public class DeviceAccountServiceTests
    {
        private readonly Mock<IDistributedCache> mockDistributedCache = new(MockBehavior.Strict);

        [Fact]
        public async Task RegisterAccount_CallsSetStringAsync()
        {
            mockDistributedCache.Setup(x => x.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
        }

    }
}
