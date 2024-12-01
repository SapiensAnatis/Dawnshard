using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Web.TimeAttack;

public partial class TimeAttackTest
{
    private async Task SeedTimeAttackData()
    {
        await this.ApiContext.TimeAttackClears.ExecuteDeleteAsync(
            TestContext.Current.CancellationToken
        );

        string teamJson = File.ReadAllText("Data/time_attack_party_info.json");

        DbPlayer player1 =
            new()
            {
                AccountId = Guid.NewGuid().ToString(),
                UserData = new() { Name = "Qwerby" },
            };

        DbPlayer player2 =
            new()
            {
                AccountId = Guid.NewGuid().ToString(),
                UserData = new() { Name = "Leom" },
            };

        DbPlayer player3 =
            new()
            {
                AccountId = Guid.NewGuid().ToString(),
                UserData = new() { Name = "Shiny" },
            };

        DbPlayer player4 =
            new()
            {
                AccountId = Guid.NewGuid().ToString(),
                UserData = new() { Name = "poopnut" },
            };
        DbPlayer player5 =
            new()
            {
                AccountId = Guid.NewGuid().ToString(),
                UserData = new() { Name = "Alicia" },
            };

        DbPlayer player6 =
            new()
            {
                AccountId = Guid.NewGuid().ToString(),
                UserData = new() { Name = "eze" },
            };

        DbPlayer player7 =
            new()
            {
                AccountId = Guid.NewGuid().ToString(),
                UserData = new() { Name = "OzpinXD" },
            };
        DbPlayer player8 =
            new()
            {
                AccountId = Guid.NewGuid().ToString(),
                UserData = new() { Name = "Euden" },
            };

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
                        Player = player1,
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = default,
                        Player = player2,
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = default,
                        Player = player3,
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = default,
                        Player = player4,
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
                        ViewerId = default,
                        Player = player1,
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = default,
                        Player = player2,
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = default,
                        Player = player3,
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = default,
                        Player = player4,
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
                        Player = player5,
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = default,
                        Player = player6,
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = default,
                        Player = player7,
                        PartyInfo = teamJson,
                    },
                    new()
                    {
                        GameId = null!,
                        ViewerId = default,
                        Player = player8,
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
                        ViewerId = default,
                        Player = player1,
                        PartyInfo = teamJson,
                    },
                ],
            },
        ];

        this.ApiContext.TimeAttackClears.AddRange(clears);
        await this.ApiContext.SaveChangesAsync();
    }
}
