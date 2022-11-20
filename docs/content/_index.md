---
title: MsBullet SDK
---

# MsBullet SDK

MsBullet SDK is a set of [MSBuild](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild) files that provide common build features of [MSBuild SDK](https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-use-project-sdk) distributed through NuGet package.

The pillars on which MsBullet SDK is based are:

{{< columns >}}

**Control and ownership**

The developer teams are owners of their repos and they should be feel free to use whatever tools they want.

<--->

**Knowledge sharing**

Business value is the main goal.  
The functionality one team has developed can contribute to another's success.

{{< /columns >}}

{{< details "**Special thanks go to [Arcade](https://github.com/dotnet/arcade)🙏**" >}}

This project is highly inspired by [Arcade](https://github.com/dotnet/arcade) and takes some implementations from it.

{{< /details >}}

## Getting started

You can start using this SDK based on [MSBuild](https://docs.microsoft.com/visualstudio/msbuild) in three steps:

1. Add `global.json`:

``` json
{
  "tools": {
    "dotnet": "[dotnet sdk version]"
  },
  "msbuild-sdks": {
    "MsBullet.Sdk": "[MsBullet.Sdk version]"
  }
}

```

2. Add `Directory.Build.props` or copy this on `root` of your repo:

``` xml
<?xml version="1.0" encoding="utf-8"?>
<Project>

  <Import Project="Sdk.props" Sdk="MsBullet.Sdk" />

</Project>
```

3. Add `Directory.Build.targets` or copy this on `root` of your repo:

``` xml
<?xml version="1.0" encoding="utf-8"?>
<Project>

  <Import Project="Sdk.targets" Sdk="MsBullet.Sdk" />

</Project>
```

## Single build output

All projects output will be placed into the `artifacts` folder that has structured below

``` plain
artifacts
├─ obj
│ └─ $(MSBuildProjectName)
│   └─ ($(Platform)|)
│     └─ $(Configuration)
├─ bin
│ └─ $(MSBuildProjectName)
│   └─ ($(Platform)|)
│     └─ $(Configuration)
├─ log
│ └─ $(Configuration)
├─ tmp
│ └─ $(Configuration)
├─ TestResults
│ └─ $(Configuration)
│   └─ $(MSBuildProjectName)_$(TargetFramework)_$(Platform).(xml|html)
└─ packages
    └─ $(Configuration)
        ├─ Shippable
        └─ NonShippable
```

| Directory | Description |
| --- | --- |
| bin | Build output of each project. |
| obj | Intermediate directory for each project. |
| packages | NuGet packages produced by all projects in the repo. |
| log | Build binary log and other logs. |
| tmp | Temp files generated during build. |

Each directory is identified by a property, please refer to [Well-known properties](./configuration/predefined-variables/).

## Built-in tools and extensibility points

The toolset provided by MsBullet SDK combines third-party tools and specific SDK's features, all of these can be controlled by `UsingTool{tool-name}` properties, where _tool-name_ identify the tool.

{{< hint info >}}

**NOTE**  
All default dependency versions are referenced to the latest major using wildcard, for further info https://docs.microsoft.com/nuget/concepts/package-versioning.

This means that if you want to use a specific of a certain dependency you must override the default variable to lock restoring to the wanted version.

{{< /hint >}}
All file references below are relative to repo root. The root is identified by presence of `.git` folder.

### `/eng/Versions.props`: a single file listing packages versions and used tools

The file should be used as single point of package versions for all NuGet packages used in the repository and for opt-out for tools or features, like suck.

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
For more information about tools, please refer to [Built-in tools](./configuration/tools).

See [`DefaultVersions.props`](../../src/MsBullet.Sdk/tools/DefaultVersions.props) for a list of UsingTool properties and default versions.

### `/eng/stylecop.json`

The default path where place the configuration of [StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) overridable by property `StyleCopConfig`.

If you want to show the configuration file on project folder you can set `ShowStyleCopConfig` to `true`.

<!-- 
## Why you need to use or promote this in your organization.

One of the major problems that afflict the teamwork is the sharing of knowledge. This problem is hugely amplified when your organization has multiple teams.
Commonly the communication across teams is poor and this often leads to reinventing the wheel, or when to the things go in well mode the copy and paste is accepts as the "non plus ultra" solution. 

For this reason, the milestones of MsBullet SDK are:

 - being able to share functionality across team and repo is one of the peace to grow and increase the organization's revenues.
 - offer to dev and team or repo owners a modular and sustainable solution to manage what tools that are needed, and which aren't, for yours daily work. 
-->
