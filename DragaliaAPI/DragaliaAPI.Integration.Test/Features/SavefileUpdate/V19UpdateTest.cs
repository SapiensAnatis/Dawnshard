using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums.Summon;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public class V19UpdateTest : SavefileUpdateTestFixture
{
    protected override int StartingVersion => 18;

    public V19UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task V19Update_ConsolidatesStackableTickets()
    {
        await this.AddRangeToDatabase(
            [
                new DbSummonTicket() { SummonTicketId = SummonTickets.SingleSummon, Quantity = 5 },
                new DbSummonTicket() { SummonTicketId = SummonTickets.SingleSummon, Quantity = 2 },
                new DbSummonTicket() { SummonTicketId = SummonTickets.SingleSummon, Quantity = 1 },
                new DbSummonTicket() { SummonTicketId = SummonTickets.TenfoldSummon, Quantity = 7 },
                new DbSummonTicket() { SummonTicketId = SummonTickets.TenfoldSummon, Quantity = 1 },
                new DbSummonTicket() { SummonTicketId = SummonTickets.TenfoldSummon, Quantity = 1 },
                new DbSummonTicket() { SummonTicketId = SummonTickets.DragonSummon, Quantity = 1 },
                new DbSummonTicket() { SummonTicketId = SummonTickets.DragonSummon, Quantity = 1 }
            ]
        );

        await this.LoadIndex();

        this.ApiContext.PlayerSummonTickets.Should()
            .BeEquivalentTo<DbSummonTicket>(
                [
                    new()
                    {
                        ViewerId = this.ViewerId,
                        SummonTicketId = SummonTickets.SingleSummon,
                        Quantity = 8
                    },
                    new()
                    {
                        ViewerId = this.ViewerId,
                        SummonTicketId = SummonTickets.TenfoldSummon,
                        Quantity = 9
                    },
                    new()
                    {
                        ViewerId = this.ViewerId,
                        SummonTicketId = SummonTickets.DragonSummon,
                        Quantity = 2
                    }
                ],
                opts => opts.Excluding(x => x.KeyId)
            );
    }

    [Fact]
    public async Task V19Update_DoesNothingIfTicketsAlreadyStacked()
    {
        DbSummonTicket[] tickets =
        [
            new DbSummonTicket() { SummonTicketId = SummonTickets.SingleSummon, Quantity = 5 },
            new DbSummonTicket() { SummonTicketId = SummonTickets.TenfoldSummon, Quantity = 7 },
        ];

        await this.AddRangeToDatabase(tickets);

        await this.LoadIndex();

        this.ApiContext.PlayerSummonTickets.Should()
            .BeEquivalentTo<DbSummonTicket>(
                [
                    new()
                    {
                        KeyId = tickets[0].KeyId,
                        ViewerId = this.ViewerId,
                        SummonTicketId = SummonTickets.SingleSummon,
                        Quantity = 5
                    },
                    new()
                    {
                        KeyId = tickets[1].KeyId,
                        ViewerId = this.ViewerId,
                        SummonTicketId = SummonTickets.TenfoldSummon,
                        Quantity = 7
                    },
                ]
            );
    }

    [Fact]
    public async Task V19Update_DoesNothingIfNoTickets()
    {
        await this.LoadIndex();
        this.ApiContext.PlayerSummonTickets.Should().BeEmpty();
    }
}
