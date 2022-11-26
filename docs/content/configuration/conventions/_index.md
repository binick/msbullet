---
title: Conventions
description: There are some useful conventions for keeping behavior consistent across multiple operating systems.
weight: 10
---

# Conventions.

## Project Naming.

When a project name ends with `.Tests`, `.UnitTests`, `.IntegrationTests` or `.PerformanceTests` are detected as test projects automatically and the `IsTestProject` property will be evaluated as true.

In addition, for the following suffixes the propriety identifying their specific type will be valued:

 - for `.Tests` or `.UnitTests`, `IsUnitTestProject` will be true
 - for `.IntegrationTests`, `IsIntegrationTestProject` will be true
 - for `.PerformanceTests`, `IsPerformanceTestProject` will be true

## Recommended conventions.

### File Naming.

Filenames should be all lowercase, for instance: `build.cmd` or `nuget.config`. Only exceptions are for files that need to be cased a particular way for an existing set of tools to read them (Example: `Directory.Build.props` which *MSBuild* expects exactly that case on Linux).

Filenames with multiple words should use kebab-casing like `common-variables.ps1`.

*MSBuild* based targets and props files for a particular library should match the exact casing of the library package they belong to, like `Microsoft.Common.targets`.

### Dependent Packages Version.

Package versions are stored in *MSBuild* properties in the `eng\Versions.props` file, like [this](https://github.com/binick/msbullet/blob/main/eng/Versions.props). Use these properties to include the correct version of the package. New properties will be included as needed.

The property version should be named with this pattern `{PackageName}Version` where `PackageName` shouldn't have any punctuation characters, for example for `Microsoft.NET.Test.Sdk` the property version will be `MicrosoftNETTestSdkVersion`.

If your project depends on a package that is also part of the .NET SDK used by **MsBullet** (check `global.json` to see which version is currently in use) the project should use the version of the package available in the SDK. Otherwise, the latest stable version of the package should be used. For instance, the `Microsoft.SourceLink.GitHub` (version 1.0.0) is present on the .NET SDK and the version is exposed in **MsBullet** through the `$(MicrosoftSourceLinkGitHubVersion)` property in [`eng\Versions.props`](https://github.com/binick/msbullet/blob/main/eng/Versions.props). Therefore, to include `Newtonsoft.Json` in your project do the following:

`<PackageReference Include="Newtonsoft.Json" Version="$(NewtonsoftJsonVersion)" />`
