﻿namespace DragaliaAPI.Models.Nintendo;

// We only need to deserialize deviceAccount from the request. The rest of it is useless
public record LoginRequest(DeviceAccount? deviceAccount);
