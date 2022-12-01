---
title: Built-in tools
description: The toolset provided by MsBullet SDK combines third-party tools and specific SDK features.
weight: 20
bookCollapseSection: true
---

# Built-in tools.

The toolset provided by MsBullet SDK combines third-party tools and specific SDK features, all of these can be controlled by `UsingTool{tool-name}` properties, where _tool-name_ identifies the tool.

{{< hint info >}}

**NOTE**  
All default dependency versions are referenced to the latest major using wildcard, for further info https://docs.microsoft.com/nuget/concepts/package-versioning.

This means that if you want to use a specific of a certain dependency you must override the default variable to lock restoring to the wanted version.

{{< /hint >}}

## Opt-out configuration.{#opt-out}

Some tools supplied with the **MsBullet SDK** can be excluded by setting specific variables.

To disable the following tools, you will need to set their property to `false`.

| Tool | Property |
| --- | --- |
| [xUnit](https://xunit.net/ "xUnit") | `UsingToolXUnit` |
| [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning "Nerdbank.GitVersioning") | `UsingToolNerdbankGitVersioning` |
| [Coverlet](https://github.com/coverlet-coverage "Coverlet") | `UsingToolCoverlet` |
| [ReportGenerator](https://github.com/danielpalme/ReportGenerator "ReportGenerator") | `UsingToolReportGenerator` |
| [StyleCop](https://github.com/DotNetAnalyzers/StyleCopAnalyzers "StyleCop") | `UsingToolStyleCopAnalyzers` |
| [SonarSource](https://www.sonarsource.com/csharp/ "SonarSource") | `UsingToolSonarAnalyzer` |
