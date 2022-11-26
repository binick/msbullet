---
title: Coverlet
---

# Coverlet.

This framework is provided as a built-in tool of **MsBullet SDK** to collect coverage through [coverlet.msbuild](https://www.nuget.org/packages/coverlet.msbuild).

## How to use and customization.

By default, all projects that have the `IsTestProject` property set to true collect coverage metrics.

{{< hint info >}}

**NOTE**  
To customize this tool, you can refer to the [official repository](https://github.com/coverlet-coverage/coverlet)

{{< /hint >}}

| Property | Description | Default value | Overridable | Note |
| --- | --- | --- | :---: | --- |
| `CoverletOutput` | Specifies the file path where the coverage report will be saved. | `$(ArtifactsCoverageDir)\$(MSBuildProjectName).xml` | ❌️ | |
| `CoverletOutputFormat` | Specifies the format(s) in which to generate the report. | `cobertura` | ✔️️ | |
| `CoverletMSBuildVersion` | Set the tool version, by default it use the latest available major. | `3.*` | ✔️️ | |
