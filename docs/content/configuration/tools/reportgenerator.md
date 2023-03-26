---
title: ReportGenerator
---

# ReportGenerator.

[ReportGenerator](https://www.nuget.org/packages/ReportGenerator) is provided as a built-in tool of **MsBullet SDK** to generate code coverage reports.

## How to use and customization.

By default, all projects that have the `IsTestProject` property set to true produce a human-readable report over collected coverage metrics by [coverlet](./../coverletmsbuild).

In addition, we produce a summary report that contains all cross-project collected metrics after each project test.

{{< hint info >}}

**NOTE**  
To customize this tool, you can refer to the [official site](https://reportgenerator.io/).

{{< /hint >}}

| Property | Description | Default value | Overridable | Note |
| --- | --- | --- | :---: | --- |
| `GenerateCoverageReportSummary` | Specifies whether to generate the summary report. | `true` | ✔️ | |
| `ReportGeneratorVersion` | Set the tool version, by default it use the latest available major. | `5.*` | ✔️️ | |
