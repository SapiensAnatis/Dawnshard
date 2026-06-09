# Build Targets: MasterAssetConverter and MissionDesigner

Post-overhaul reference (2026-06). The authoritative doc is
`DragaliaAPI/DragaliaAPI.MasterAssetConverter/README.md`; this file is a condensed note.

## Design principle

Both tools run at the **start** of the build and hand their outputs to the .NET SDK's standard
content pipeline. Nothing copies files into `bin/` manually anymore.

## MasterAssetConverter

- `MasterAssetConverter.targets`, imported by `DragaliaAPI.csproj`.
- Target `ConvertMasterAssets`: incremental (Inputs = `$(MasterAssetResources)**/*.json` minus
  `*.schema.json`; Outputs = matching `.msgpack` under `$(IntermediateOutputPath)masterassets/`).
  Partial incremental: only out-of-date JSON files are passed to the tool. Guarded with
  `Condition="'$(DesignTimeBuild)' != 'true'"`.
- Target `IncludeMasterAssetOutputs` (`BeforeTargets="AssignTargetPaths"`, always runs): globs the
  obj output and adds the files as `Content` with `TargetPath="Resources/..."` and
  `CopyToOutputDirectory="PreserveNewest"`, plus `FileWrites` for Clean.
- The SDK content pipeline then handles: copy to `$(OutDir)Resources/`, **transitive flow to every
  project referencing DragaliaAPI.csproj** (all test projects — the old `DependsOnApiMsgpackFiles`
  property and `CopyApiMsgpackFiles` target in `Directory.Build.props` were deleted), publish
  output (the old `CopyMsgpackToPublish` target was deleted).
- Gotcha encountered: inside a target, unqualified `%(RecursiveDir)` in an item attribute batches
  over pre-existing items of the type being declared (MSB4120) — metadata must be qualified via a
  staging item: `%(_MasterAssetMsgpackFiles.RecursiveDir)`.
- `$(IntermediateOutputPath)` is not defined at evaluation time in csproj-imported .targets files
  (SDK targets import later), hence all paths/items are created inside targets.

## MissionDesigner

- `MissionDesigner.targets`, imported by `DragaliaAPI.MissionDesigner.csproj` itself; target runs
  `AfterTargets="Build"` of that project.
- Now incremental: Inputs = `$(TargetPath)` (the designer assembly, proxy for the C# mission DSL)
  + `MissionNormalData.json`; Output = `MissionProgressionInfo.json`.
- Output is written into the source tree (`DragaliaAPI.Shared/Resources/Missions/`) and is
  **checked into git** (deliberate: diffable in PRs). It is NOT in `FileWrites`, so Clean leaves
  it alone.
- Ordering vs the converter: `DragaliaAPI.csproj` has `ReferenceOutputAssembly="false"`
  ProjectReferences to both tool projects, so MissionDesigner builds (and runs) before
  DragaliaAPI's conversion targets see the JSON.

## Runtime executables (unchanged)

- `MasterAssetConverter <outputDir> <resourcesPath> <jsonFiles...>` — reflects
  `[GenerateMasterAsset<T>]` / `[ExtendMasterAsset]` attributes from `MasterAsset` in
  DragaliaAPI.Shared, deserializes JSON, writes MessagePack. Creates output subdirs itself.
- `MissionDesigner <resourcesPath>` — reflects `[ContainsMissionList]` static classes, applies
  `ImplicitPropertyAttribute`s, resolves Normal-mission prerequisite chains from
  `MissionNormalData.json`, writes `MissionProgressionInfo.json`.

## Environment note (this machine)

The openSUSE distro .NET SDK (10.0.108, source-built) lacks the `PrunePackageData` folder, so any
fresh restore fails with NETSDK1226. Workaround: pass `/p:AllowMissingPrunePackageData=true`
(harmless; just disables package pruning). CI uses Microsoft-built SDKs and is unaffected.
