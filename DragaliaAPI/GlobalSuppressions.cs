﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Style",
    "IDE1006:Naming Styles",
    Scope = "namespaceanddescendants",
    Target = "~N:DragaliaAPI.Models.Generated",
    Justification = "msgpack/json object fields should start with lowercase to match real keys"
)]
