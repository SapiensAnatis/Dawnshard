using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Controllers.Other;
using DragaliaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Test.Unit.Controllers;

public class DragalipatchControllerTest
{
    private readonly DragalipatchController controller;
    private readonly Mock<IOptionsMonitor<DragalipatchConfig>> mockOptions;

    public DragalipatchControllerTest()
    {
        this.mockOptions = new(MockBehavior.Strict);
        this.controller = new(this.mockOptions.Object);
    }

    [Fact]
    public void DragalipatchController_ReturnsConfiguredValues()
    {
        DragalipatchConfig expectedConfig =
            new()
            {
                Mode = "RAW",
                CdnUrl = "https://taylorswift.com",
                ConeshellKey = "key",
                UseUnifiedLogin = true,
            };

        this.mockOptions.SetupGet(x => x.CurrentValue).Returns(expectedConfig);

        ObjectResult? result = this.controller.Config() as ObjectResult;

        result.Should().NotBeNull();
        result!.Value.Should().BeEquivalentTo(expectedConfig);
    }
}
