---
title: Built-in tools
---

# Opt-out tools.

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
