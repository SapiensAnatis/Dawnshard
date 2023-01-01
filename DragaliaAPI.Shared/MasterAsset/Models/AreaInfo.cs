﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public class AreaInfo
{
    public AreaInfo(string scenePath, string areaName)
    {
        this.ScenePath = scenePath;
        this.AreaName = areaName;
    }

    public string ScenePath { get; set; }

    public string AreaName { get; set; }
}
