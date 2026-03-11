# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).


## [1.2.0] - 2026-03-11

### Changed
- **Replaced `csc.rsp` file manipulation with `PlayerSettings.SetScriptingDefineSymbols`** using `NamedBuildTarget` API (Unity 2021.2+). Defines are now managed through Unity's native scripting define symbols system, making them visible in Project Settings and properly integrated with the build pipeline.
- Bumped minimum Unity version to **2021.3 LTS** (required for `NamedBuildTarget` API)
- Removed `System.IO` and `UnityEditor.Compilation` dependencies (no longer needed without `csc.rsp` file I/O)
- Removed `needRecompile` field and manual `CompilationPipeline.RequestScriptCompilation()` calls — `PlayerSettings.SetScriptingDefineSymbols` triggers recompilation automatically

### Added
- **New `Package` DomainType** for direct UPM package detection via `PackageInfo.FindForAssetPath`. Use the package name (e.g., `com.unity.render-pipelines.universal`) as the lookup value for more reliable detection than namespace scanning.
- Build target validation — warns if active build target group is unknown

### Documentation
- Updated README with Package DomainType documentation, migration notes, and updated requirements
- Updated package.json description and keywords

## [1.1.0] - 2026-03-11

### Fixed
- Fixed null reference exception in `Bootstrap()` when DirectiveDefiner asset is not found
- Fixed asset search using wrong name (`PreprocessorDirectiveManager` → `PreprocessorDirectiveDefiner`)
- Fixed `FindAssets` returning GUIDs being used directly as paths (now uses `GUIDToAssetPath`)
- Fixed hardcoded Windows path separator (`\\`) in RSP file path construction
- Fixed path construction to use `Path.Combine()` instead of string concatenation
- Fixed `needRecompile` being set to `true` when creating a new RSP file with no directives

### Improved
- Added exception handling around file I/O operations
- Added `ReflectionTypeLoadException` handling in assembly type scanning
- Made Odin Inspector dependency optional via `ODIN_INSPECTOR` conditional compilation
- Added `[ContextMenu("Apply Directives")]` for applying without Odin Inspector
- Improved error messages with `[PDM]` prefix and actionable guidance
- Reduced excessive `Debug.Log` calls; errors now use `LogWarning`/`LogError` appropriately
- Extracted RSP file update logic into separate `UpdateRspFile` method
- Added XML documentation comment to the class
- Added null/empty check for `lookUpCode` array
- Renamed `OnEnable` to `ApplyDirectives` to better reflect intent and avoid Unity lifecycle confusion
- Renamed event handler from `Events_registeredPackages` to `OnRegisteredPackages`
- Removed deprecated .NET 3.5 `mcs.rsp` code path
- Removed unhelpful `#region What`

### Documentation
- Expanded README with installation instructions, usage guide, configuration reference, and code examples
- Updated package.json with descriptive metadata and keywords
- Updated CHANGELOG with all changes

## [1.0.0] - 2021-09-12

- Initial release
- Creates CHANGELOG
