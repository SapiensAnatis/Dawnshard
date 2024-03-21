using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Integration.Test.Features.Reward;

public class RewardServiceTest : TestFixture
{
    public RewardServiceTest(
        CustomWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper
    )
        : base(factory, testOutputHelper) { }

    [Theory]
    [InlineData(SummonTickets.SingleSummon)]
    [InlineData(SummonTickets.TenfoldSummon)]
    [InlineData(SummonTickets.AdventurerSummon)]
    [InlineData(SummonTickets.DragonSummon)]
    [InlineData(SummonTickets.AdventurerSummonPlus)]
    [InlineData(SummonTickets.DragonSummonPlus)]
    public async Task GrantSummoningTickets_Stackable_IncrementsExistingRow(
        SummonTickets ticketType
    )
    {
        DbSummonTicket ticket = new() { SummonTicketId = ticketType, Quantity = 1, };
        DbPlayerPresent present =
            new()
            {
                EntityType = EntityTypes.SummonTicket,
                EntityQuantity = 5,
                EntityId = (int)ticketType
            };

        await this.AddRangeToDatabase([ticket, present]);

        await this.Client.PostMsgpack(
            "/present/receive",
            new PresentReceiveRequest() { PresentIdList = [(ulong)present.PresentId] }
        );

        this.ApiContext.PlayerSummonTickets.Should()
            .BeEquivalentTo(
                [
                    new DbSummonTicket()
                    {
                        KeyId = ticket.KeyId,
                        ViewerId = this.ViewerId,
                        SummonTicketId = ticketType,
                        Quantity = 6,
                    }
                ]
            );
    }

    [Theory]
    [InlineData(SummonTickets.SingleSummon)]
    [InlineData(SummonTickets.TenfoldSummon)]
    [InlineData(SummonTickets.AdventurerSummon)]
    [InlineData(SummonTickets.DragonSummon)]
    [InlineData(SummonTickets.AdventurerSummonPlus)]
    [InlineData(SummonTickets.DragonSummonPlus)]
    public async Task GrantSummoningTickets_Stackable_NoRow_CreatesNewRow(SummonTickets ticketType)
    {
        DbPlayerPresent present =
            new()
            {
                EntityType = EntityTypes.SummonTicket,
                EntityQuantity = 5,
                EntityId = (int)ticketType
            };

        await this.AddToDatabase(present);

        await this.Client.PostMsgpack(
            "/present/receive",
            new PresentReceiveRequest() { PresentIdList = [(ulong)present.PresentId] }
        );

        this.ApiContext.PlayerSummonTickets.Should()
            .BeEquivalentTo(
                [
                    new DbSummonTicket()
                    {
                        ViewerId = this.ViewerId,
                        SummonTicketId = ticketType,
                        Quantity = 5,
                    }
                ],
                opts => opts.Excluding(x => x.KeyId)
            );
    }
}
