using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public class V27UpdateTest : SavefileUpdateTestFixture
{
    protected override int StartingVersion => 26;

    public V27UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task V27Update_FixesCharasWithAbilityLevel4()
    {
        DbPlayerCharaData cleo = new(this.ViewerId, Charas.Cleo) { Ability1Level = 4 };

        await this.AddToDatabase(cleo);

        await this.LoadIndex();

        await this
            .ApiContext.PlayerCharaData.Entry(cleo)
            .ReloadAsync(TestContext.Current.CancellationToken);

        cleo.Ability1Level.Should().Be(3);
    }

    [Fact]
    public async Task V27Update_FixesAlexWithAbilityLevel3()
    {
        DbPlayerCharaData alex = new(this.ViewerId, Charas.Alex) { Ability1Level = 3 };

        await this.AddToDatabase(alex);

        await this.LoadIndex();

        await this
            .ApiContext.PlayerCharaData.Entry(alex)
            .ReloadAsync(TestContext.Current.CancellationToken);

        alex.Ability1Level.Should().Be(2);
    }

    [Fact]
    public async Task V27Update_FixesStoryCharacterWithLockedSharedSkill()
    {
        DbPlayerCharaData luca = new(this.ViewerId, Charas.Luca) { IsUnlockEditSkill = false };

        await this.AddToDatabase(luca);

        await this.LoadIndex();

        await this
            .ApiContext.PlayerCharaData.Entry(luca)
            .ReloadAsync(TestContext.Current.CancellationToken);

        luca.IsUnlockEditSkill.Should().Be(true);
    }
}
