---
title: StyleCop Analyzers
---

# StyleCop Analyzers.

This analyzer is provided as a built-in tool of **MsBullet SDK** to enforce standard code conventions

## How to use and customization.

By default, there is no application of specific configurations within the SDK.

{{< hint info >}}

**NOTE**  
For customization of the rules of this tool, you can refer to the [official documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/Configuration.md)

{{< /hint >}}

| Property | Description | Default value | Overridable | Note |
| --- | --- | --- | :---: | --- |
| `StyleCopConfig` | Specifies the file path of `stylecop.json` configuration file. | `$(RepoRoot)eng/stylecop.json` | ✔️️ | |
| `ShowStyleCopConfig` | Specifies whether the `stylecop.json` file should be displayed within the project root. | `false` | ✔️️ | |
| `StyleCopAnalyzersVersion` | Set the tool version, by default it use the latest available major. | `1.*` | ✔️️ | |
