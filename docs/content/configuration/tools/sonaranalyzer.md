---
title: SonarSource Analyzer
---

# SonarSource Analyzer.

This analyzer is provided as a built-in tool of **MsBullet SDK** to help developers maintain high security and code quality.

## How to use and customization.

By default, there is no application of specific configurations within the SDK.

{{< hint info >}}

**NOTE**  
For customization of the rules of this tool, you can refer to the [official repository](https://github.com/SonarSource/sonar-dotnet)

{{< /hint >}}

| Property | Description | Default value | Overridable | Note |
| --- | --- | --- | :---: | --- |
| `SonarAnalyzerCSharpVersion` | Set the tool version, by default it use the latest available major. | `8.*` | ✔️️ | |
