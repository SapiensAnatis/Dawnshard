using DragaliaAPI.Controllers.Other;
using DragaliaAPI.Models.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Test.Controllers;

public class DragalipatchControllerTest
{
    private readonly DragalipatchController controller;
    private readonly Mock<IOptionsMonitor<DragalipatchOptions>> mockPatchOptions;
    private readonly Mock<IOptionsMonitor<LoginOptions>> mockLoginOptions;

    public DragalipatchControllerTest()
    {
        this.mockPatchOptions = new(MockBehavior.Strict);
        this.mockLoginOptions = new(MockBehavior.Strict);

        this.controller = new(this.mockPatchOptions.Object, this.mockLoginOptions.Object);
    }

    [Fact]
    public void DragalipatchController_ReturnsConfiguredValues()
    {
        DragalipatchOptions expectedPatchConfig =
            new()
            {
                Mode = "RAW",
                CdnUrl = "https://taylorswift.com",
                ConeshellKey = "key",
            };

        LoginOptions expectedLoginConfig = new() { UseBaasLogin = true, };

        this.mockPatchOptions.SetupGet(x => x.CurrentValue).Returns(expectedPatchConfig);
        this.mockLoginOptions.SetupGet(x => x.CurrentValue).Returns(expectedLoginConfig);

        ObjectResult? result = this.controller.Config().Result as ObjectResult;

        result.Should().NotBeNull();
        result!.Value.Should().BeEquivalentTo(expectedPatchConfig);
    }
}
