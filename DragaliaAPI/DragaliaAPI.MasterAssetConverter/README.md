# Master asset build pipeline

Two custom tools run during the build to produce the master data ("master asset") files that
DragaliaAPI loads at runtime. Both run at the **start** of the build, before the .NET SDK's
content-copy pipeline, so their outputs are handled by standard build machinery from then on.

```
DragaliaAPI.MissionDesigner            (runs as part of its own Build)
   [ContainsMissionList] C# DSL ──► DragaliaAPI.Shared/Resources/Missions/MissionProgressionInfo.json
                                       (written into the source tree, checked into git)
                                                      │
                                                      ▼
DragaliaAPI.Shared/Resources/**/*.json    (all master data, including the file above)
                                                      │
                                                      ▼
DragaliaAPI.MasterAssetConverter       (runs early in DragaliaAPI's build, BeforeTargets=AssignTargetPaths)
   JSON ──► obj/<cfg>/<tfm>/masterassets/**/*.msgpack
                                                      │
                registered as Content items with CopyToOutputDirectory=PreserveNewest
                                                      │
              ┌───────────────────────────────────────┼──────────────────────────────────┐
              ▼                                       ▼                                  ▼
   DragaliaAPI/bin/.../Resources/        test project bin/.../Resources/        publish/Resources/
   (own output)                          (flows transitively to anything        (`dotnet publish`,
                                          referencing DragaliaAPI.csproj —       used by Docker)
                                          no opt-in property needed)
```

## Ordering

- `DragaliaAPI.csproj` has `ProjectReference`s (with `ReferenceOutputAssembly="false"`) to both
  tool projects, so MSBuild builds them — and, for MissionDesigner, runs it — before DragaliaAPI's
  own targets execute.
- Inside DragaliaAPI's build, conversion is hooked `BeforeTargets="AssignTargetPaths"`: after
  `ResolveProjectReferences` (tools exist), before the content pipeline (outputs get picked up).

## Incrementality

- **MasterAssetConverter** (`MasterAssetConverter.targets`): declares each `.json` as an input and
  the corresponding `obj/.../masterassets/*.msgpack` as an output. On a warm build the target is
  skipped; when some JSON files change, MSBuild passes only those files to the tool (partial
  incremental build).
- **MissionDesigner** (`MissionDesigner.targets`): inputs are the freshly built designer assembly
  (a proxy for the C# mission DSL) and `MissionNormalData.json`; output is
  `MissionProgressionInfo.json`. The tool only reruns when the DSL or that file changed.

## Things that fall out of using the content pipeline (no extra code needed)

- Test projects get `Resources/**/*.msgpack` in their output directory simply by referencing
  `DragaliaAPI.csproj` (transitive content copy). The old `DependsOnApiMsgpackFiles` opt-in
  property and manual copy target are gone.
- `dotnet publish` includes the files automatically.
- `Clean` removes the generated files from both `obj/` and `bin/` (via `FileWrites`), but never
  touches `MissionProgressionInfo.json` — that is a checked-in source file.

## Invoking the converter manually

```
MasterAssetConverter <outputDir> <resourcesPath> <jsonFile1> [<jsonFile2> ...]
```

For each JSON file it finds the matching `[GenerateMasterAsset<T>]` (or `[ExtendMasterAsset]`)
attribute on the `MasterAsset` class in DragaliaAPI.Shared, deserializes the JSON, and writes a
MessagePack binary to `<outputDir>/<relative-path>.msgpack`.
