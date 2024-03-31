using DragaliaAPI.Shared.Serialization;

namespace DragaliaAPI.Shared.Test.Serialization;

public class CustomSnakeCaseNamingPolicyTest
{
    [Theory]
    [InlineData("UserData", "user_data")]
    [InlineData("Ability1Level", "ability_1_level")]
    [InlineData("GetTime", "gettime")]
    [InlineData("EquipCrestSlotType1CrestId1", "equip_crest_slot_type_1_crest_id_1")]
    public void CustomSnakeCaseNamingPolicy_ConvertName_ProducesExpected(
        string input,
        string expected
    )
    {
        CustomSnakeCaseNamingPolicy.Instance.ConvertName(input).Should().Be(expected);
    }
}
