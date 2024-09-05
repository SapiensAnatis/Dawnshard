using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Integration.Test.Features.Web.TimeAttack;

public partial class TimeAttackTest
{
    private async Task SeedTimeAttackData()
    {
        string teamJson = File.ReadAllText("Data/time_attack_party_info.json");

        DbTimeAttackClear[] clears =
        [
            new()
            {
                GameId = Guid.NewGuid().ToString(),
                QuestId = 227010105,
                Time = 22.811f,
                Players =
                [
                    new()
                    {
                        GameId = null!,
                        ViewerId = default,
                        Player = new()
                        {
                            ViewerId = 1530,
                            AccountId = Guid.NewGuid().ToString(),
                            UserData = new() { Name = "Qwerby" },
                        },
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = default,
                        Player = new()
                        {
                            ViewerId = 3108,
                            AccountId = Guid.NewGuid().ToString(),
                            UserData = new() { Name = "Leom" },
                        },
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = default,
                        Player = new()
                        {
                            ViewerId = 2104,
                            AccountId = Guid.NewGuid().ToString(),
                            UserData = new() { Name = "Shiny" },
                        },
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = default,
                        Player = new()
                        {
                            ViewerId = 1718,
                            AccountId = Guid.NewGuid().ToString(),
                            UserData = new() { Name = "poopnut" },
                        },
                        PartyInfo = teamJson,
                    },
                ],
            },
            // Should be filtered out as same players as above
            new()
            {
                GameId = Guid.NewGuid().ToString(),
                QuestId = 227010105,
                Time = 25.811f,
                Players =
                [
                    new()
                    {
                        GameId = null!,
                        ViewerId = 1530,
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = 3108,
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = 2104,
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = 1718,
                        PartyInfo = teamJson,
                    },
                ],
            },
            // New players
            new()
            {
                GameId = Guid.NewGuid().ToString(),
                QuestId = 227010105,
                Time = 31.811f,
                Players =
                [
                    new()
                    {
                        GameId = null!,
                        ViewerId = default,
                        Player = new()
                        {
                            ViewerId = 55,
                            AccountId = Guid.NewGuid().ToString(),
                            UserData = new() { Name = "Alicia" },
                        },
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = default,
                        Player = new()
                        {
                            ViewerId = 2119,
                            AccountId = Guid.NewGuid().ToString(),
                            UserData = new() { Name = "eze" },
                        },
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = default,
                        Player = new()
                        {
                            ViewerId = 2348,
                            AccountId = Guid.NewGuid().ToString(),
                            UserData = new() { Name = "OzpinXD" },
                        },
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = default,
                        Player = new()
                        {
                            ViewerId = 609,
                            AccountId = Guid.NewGuid().ToString(),
                            UserData = new() { Name = "Euden" },
                        },
                        PartyInfo = teamJson,
                    },
                ],
            },
            // Solo clear
            new()
            {
                GameId = Guid.NewGuid().ToString(),
                QuestId = 227010104,
                Time = 22.811f,
                Players =
                [
                    new()
                    {
                        GameId = null!,
                        ViewerId = 1530,
                        PartyInfo = teamJson,
                    },
                ],
            },
        ];

        this.ApiContext.TimeAttackClears.AddRange(clears);
        await this.ApiContext.SaveChangesAsync();
    }
}
