---
title: Build extension points
description: It is possible to centrally manage the versioning of dependencies or to perform pre- or post-compilation steps.
weight: 30
---

# Build extension points

All file references below are relative to the repo root. The root is identified by the presence of `global.json` file.

## `/eng/Versions.props`: a single file listing packages versions and used tools

The file should be used as a single point of package versions for all NuGet packages used in the repository and for an opt-out for tools or features, like suck.

``` xml
<?xml version="1.0" encoding="utf-8"?>
<Project>

  <PropertyGroup>
    <MicrosoftBuildFrameworkVersion>16.5.0</MicrosoftBuildFrameworkVersion>
    <MicrosoftBuildVersion>$(MicrosoftBuildFrameworkVersion)</MicrosoftBuildVersion>
  </PropertyGroup>

  <PropertyGroup>
    <UsingToolXUnit>false</UsingToolXUnit>
  </PropertyGroup>

</Project>

```

The toolset also defines default versions for various tools and dependencies. These defaults can be overridden in the `Versions.props` file.  
For more information about tools or for a list of `UsingTool{Tool}` properties and default versions, please refer to [Built-in tools](../tools#opt-out).

## `/eng/stylecop.json`

The default path where place the configuration of [StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) overridable by property `StyleCopConfig`.

If you want to show the configuration file on project folder you can set `ShowStyleCopConfig` to `true`.
