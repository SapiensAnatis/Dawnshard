using System.Collections.Generic;
using DragaliaAPI.Photon.Plugin.Plugins.GameLogic.Events;
using FluentAssertions;
using Xunit;

namespace DragaliaAPI.Photon.Plugin.Test
{
    public class MemberCountTableTest
    {
        [Theory]
        [ClassData(typeof(BuildMemberCountTableData))]
        public void BuildMemberCountTable_ReturnsExpectedResult(
            List<(int ActorNr, int HeroParamCount)> actorData,
            Dictionary<int, int> expectedTable
        ) =>
            MemberCountHelper
                .BuildMemberCountTable(actorData)
                .Should()
                .BeEquivalentTo(expectedTable);

        private class BuildMemberCountTableData
            : TheoryData<List<(int ActorNr, int HeroParamCount)>, Dictionary<int, int>>
        {
            public BuildMemberCountTableData()
            {
                this.Add(
                    new List<(int ActorNr, int HeroParamCount)> { (1, 4), (2, 4), (3, 4), (4, 4) },
                    new Dictionary<int, int>
                    {
                        { 1, 1 },
                        { 2, 1 },
                        { 3, 1 },
                        { 4, 1 }
                    }
                );

                this.Add(
                    new List<(int ActorNr, int HeroParamCount)> { (1, 4), (2, 4) },
                    new Dictionary<int, int> { { 1, 2 }, { 2, 2 } }
                );

                this.Add(
                    new List<(int ActorNr, int HeroParamCount)> { (1, 4), (2, 1) },
                    new Dictionary<int, int> { { 1, 3 }, { 2, 1 } }
                );

                this.Add(
                    new List<(int ActorNr, int HeroParamCount)> { (1, 1), (2, 4) },
                    new Dictionary<int, int> { { 1, 1 }, { 2, 3 } }
                );

                this.Add(
                    new List<(int ActorNr, int HeroParamCount)> { (1, 1), (2, 1), (3, 2) },
                    new Dictionary<int, int>
                    {
                        { 1, 1 },
                        { 2, 1 },
                        { 3, 2 }
                    }
                );

                this.Add(
                    new List<(int ActorNr, int HeroParamCount)> { (1, 1), (2, 1) },
                    new Dictionary<int, int> { { 1, 1 }, { 2, 1 } }
                );

                this.Add(
                    new List<(int ActorNr, int HeroParamCount)> { (1, 1), (2, 1), (3, 1) },
                    new Dictionary<int, int>
                    {
                        { 1, 1 },
                        { 2, 1 },
                        { 3, 1 }
                    }
                );
            }
        }
    }
}
