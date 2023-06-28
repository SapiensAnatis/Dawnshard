using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Models
{
    [MessagePackObject]
    public class HeroParamExData
    {
        [Key(0)]
        public int sequenceNumber;

        [Key(1)]
        public int limitOverCount;

        [Key(2)]
        public int brSpecialSkillId;
    }
}
