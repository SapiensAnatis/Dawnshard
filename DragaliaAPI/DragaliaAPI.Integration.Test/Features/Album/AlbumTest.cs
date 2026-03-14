using DragaliaAPI.Database.Entities;
using DragaliaAPI.Infrastructure.Results;

namespace DragaliaAPI.Integration.Test.Features.Album;

public class AlbumTest : TestFixture
{
    public AlbumTest(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
        : base(factory, testOutputHelper) { }

    [Fact]
    public async Task Index_ReturnsMedals()
    {
        await this.AddRangeToDatabase([
            new DbPlayerCharaHonor() { CharaId = Charas.ThePrince, HonorId = 100101 },
            new DbPlayerCharaHonor() { CharaId = Charas.ThePrince, HonorId = 100401 },
            new DbPlayerCharaHonor() { CharaId = Charas.BondforgedPrince, HonorId = 100501 },
        ]);

        DragaliaResponse<AlbumIndexResponse> res =
            await this.Client.PostMsgpack<AlbumIndexResponse>(
                "/album/index",
                cancellationToken: TestContext.Current.CancellationToken
            );

        res.Data.CharaHonorList.Should()
            .BeEquivalentTo([
                new AtgenCharaHonorList()
                {
                    CharaId = Charas.ThePrince,
                    HonorList = [new AtgenHonorList(100401), new AtgenHonorList(100101)],
                },
                new AtgenCharaHonorList()
                {
                    CharaId = Charas.BondforgedPrince,
                    HonorList = [new AtgenHonorList(100501)],
                },
            ]);
    }
}
