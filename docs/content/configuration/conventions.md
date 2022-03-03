---
title: Conventions
---

# Recommended conventions.

### File Naming.

Filenames should be all lowercase, for instance: `build.cmd` or `nuget.config`. Only exceptions are for files that needs to be cased a particular way for an existing set of tools to read them (Example: `Directory.Build.props` which MSBuild expects exactly that case on Linux).

Filenames with multiple words should use kebab-casing like `common-variables.ps1`.

MSBuild based targets and props files for a particular library should match the exact casing of the library package they belong to, like `Microsoft.Common.targets`.

### Dependent Packages Version.

Package versions are stored in MSBuild properties in the [`eng\Versions.props`](https://github.com/binick/msbullet/blob/main/eng/Versions.props) file. Use these properties to include the correct version of the package. New properties will be included as needed. 

If your project depend on a package which is also part of the .NET SDK used by MsBullet (check `global.json` to see which version is currently in use) the project should use the version of the package available in the SDK. Otherwise, the latest stable version of the package should be used. For instance, the `Microsoft.SourceLink.GitHub` (version 1.0.0) is present on the .NET SDK and the version is exposed in MsBullet through the `$(MicrosoftSourceLinkGitHubVersion)` property in [`eng\Versions.props`](https://github.com/binick/msbullet/blob/main/eng/Versions.props). Therefore, to include Newtonsoft.Json in your project do the following:

`<PackageReference Include="Microsoft.SourceLink.GitHub" Version="$(MicrosoftSourceLinkGitHubVersion)" />`
