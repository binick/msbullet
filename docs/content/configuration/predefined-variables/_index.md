---
title: Well-known properties
bookToc: false
---

# Well-known properties.

**MsBullet SDK** provides some properties for internal use, and you can use them in your build logic.

For all other variables mentioned that are not in the table above, you can find more information in the [official documentation](https://docs.microsoft.com/visualstudio/msbuild/msbuild-reserved-and-well-known-properties "MSBuild reserved and well-known properties").

| Property | Description | Default value | Overridable | Note |
| --- | --- | --- | :---: | --- |
| `Configuration` | Specify the current project configuration. | `Debug` | ✔️️ | |
| `Platform` | Specify the current project platform. | `AnyCPU` | ✔️ | |
| `PlatformName` | Specify the current project platform name. | `$(Platform)` | ✔️ | |
| `RepoRoot` | Identifying the root of repository. | N/A | ✔️ | Navigate up the folder tree until a `global.json` is found. |
| `ArtifactsDir` | Specify the root where build system outputs are placed. | `$(RepoRoot)/artifacts/` | ❌ | |
| `OutDirName` | Used to specified the end folder of path where artifacts are placed | `$(MSBuildProjectName)` | ✔️ | |
| `BaseOutputPath` | Identifying folder where binary artifacts are placed | `$(ArtifactsBinDir)/$(MSBuildProjectName)/` | ✔️ | |
| `OutputPath` | Identifying folder where artifacts are placed | When `$(PlatformName)` is equal to `AnyCPU` `$(BaseOutputPath)/$(Configuration)/`, otherwise `$(BaseOutputPath)/$(PlatformName)/$(Configuration)/` | ❌ | |
| `BaseIntermediateOutputPath` | Identifying folder where [CIL](https://en.wikipedia.org/wiki/Common_Intermediate_Language) artifats are placed | `$(ArtifactsObjDir)/$(MSBuildProjectName)/` | ✔️ | |
| `IntermediateOutputPath` | Identifying folder where artifacts are placed | When `$(PlatformName)` is equal to `AnyCPU` `$(BaseIntermediateOutputPath)/$(Configuration)/`, otherwise `$(BaseIntermediateOutputPath)/$(PlatformName)/$(Configuration)/` | ❌ | |
| `UsingToolXUnit` | Used to opt-out built-in features.<br/>By default we use [xUnit](https://xunit.github.io/) as test framework, that're represents the standart de-facto. | true | ✔️ | |
| `UsingToolNerdbankGitVersioning` | Used to opt-out built-in features.<br/>By default we use [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) as semantic versioning tool. | true | ✔️ | |
| `UsingToolStyleCopAnalyzers` | Used to opt-out built-in features.<br/>By default we use [StyleCopAnalyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) to enforce a set of style and consistency code rules. | true | ✔️ | |
| `UsingToolSonarAnalyzer` | Used to opt-out built-in features.<br/>By default we use [SonarSource](https://www.sonarsource.com/csharp/) as a well-established code quality standards. | true | ✔️ | |
| `StyleCopConfig` | Used to customize the default rules of [StyleCopAnalyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) through the [`stylecop.json`](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/Configuration.md). | `$(RepoRoot)/eng/stylecop.json` | ✔️ | |
| `IsUnitTestProject` | Identifying unit test project.<br/> A project is identified as unit test project when your's name end with:<br/><ul><li>`.Tests`</li><li>`.UnitTests`</li></ul> | false | ✔️ | |
| `IsIntegrationTestProject` | Identifying integration test project.<br/> A project is identified as integration test project when your's name end with: `.IntegrationTests` | false | ✔️ | |
| `IsPerformanceTestProject` | Identifying performance test project.<br/> A project is identified as performance test project when your's name end with: `.PerformanceTests` | false | ✔️ | |
| `IsTestProject` | Identifying test project.<br/>A project is identified as test project when it has one of that properties as true:<br/><ul><li>`IsUnitTestProject`</li><li>`IsIntegrationTestProject`</li><li>`IsPerformanceTestProject`</li></ul> | N/A | ❌ | |
| `DeployProjectOutput` | Mark a project to deploy (TODO: specify Deploy target) | true | ✔️ | |
| `PackageOutputPath` | Identifies where build output package will be stored. | When `$(IsShippable)` is equal to `true` `$(ArtifactsShippingPackagesDir)`, otherwise `$(ArtifactsNonShippingPackagesDir)` | ❌ | |
| `IsShippable` | Mark a project as shippable, a project should be shippable when your output is intended for the public. | When `$(IsTestProject)` is equal to `true` `false`, otherwise N/A. | ✔️ | |
| `PackageLicenseFile` | Identifies the license of the package | `License.txt` | ✔️ | That file will be added to package when `$(PackageLicenseExpressionInternal)` is not valorized and `$(IsPackable)` is equal to `true` |
| `PackageLicenseExpressionInternal` | Specifies the license to be used. | TODO | ✔️ |  |
| `IsPackable` | Identifies the project as packable. | When `$(IsTestProject)` is equal to `true` `false`, otherwise N/A. | ✔️ |  |
| `MicrosoftCodeAnalysisNetAnalyzersVersion` | Specifies the version of [Microsoft.CodeAnalysis.NetAnalyzers](https://github.com/dotnet/roslyn-analyzers#microsoftcodeanalysisnetanalyzers) | `6.*` | ✔️ | Used only for .NET version lower then .NET 5 |
| `MicrosoftVisualStudioThreadingAnalyzersVersion` |  | `16.*` | ✔️ | Used only for .NET version lower then .NET 5, will be removed in the next major release. |
| `DotNetRoot` |  |  | ✔️ |  |
| `MonoTool` |  | `mono` | ✔️ |  |
| `RepositoryEngineeringDir` | Identifies the engineering directory of the repository. | `$(RepoRoot)/eng` | ❌ |  |
| `RepositoryToolsDir` |  | `$(RepoRoot)/.tools` | ❌ |  |
| `VersionsPropsPath` | Specifies the path of `Versions.props` | `$(RepositoryEngineeringDir)Versions.props` | ❌ |  |
| `ArtifactsToolsetDir` |  | `$(ArtifactsDir)/toolset` | ❌ |  |
| `ArtifactsObjDir` |  | `$(ArtifactsDir)/obj` | ❌ |  |
| `ArtifactsBinDir` |  | `$(ArtifactsDir)/bin` | ❌ |  |
| `ArtifactsLogDir` |  | `$(ArtifactsDir)/log/$(Configuration)` | ❌ |  |
| `ArtifactsTmpDir` |  | `$(ArtifactsDir)/tmp/$(Configuration)` | ❌ |  |
| `ArtifactsTestResultsDir` |  | `$(ArtifactsDir)/TestResults/$(Configuration)` | ❌ |  |
| `ArtifactsReportDir` |  | `$(ArtifactsTestResultsDir)/Reports` | ❌ |  |
| `ArtifactsCoverageDir` |  | `$(ArtifactsTestResultsDir)/Coverage` | ❌ |  |
| `ArtifactsSymStoreDirectory` |  | `$(ArtifactsDir)/SymStore/$(Configuration)` | ❌ |  |
| `ArtifactsPackagesDir` |  | `$(ArtifactsDir)/packages/$(Configuration)` | ❌ |  |
| `ArtifactsShippingPackagesDir` |  | `$(ArtifactsPackagesDir)/Shippable` | ❌ |  |
| `ArtifactsNonShippingPackagesDir` |  | `$(ArtifactsPackagesDir)/NonShippable` | ❌ |  |
| `BuildForLiveUnitTesting` |  | TODO | ❌ |  |
| `MicrosoftNETTestSdkVersion` | Specifies the version of [Microsoft.NET.Test.Sdk](https://github.com/microsoft/vstest) | `16.*` | ✔️ | Available only when `$(IsTestProject)` is equal to `true`, will be updated to `17.*` in the next major release. |
| `TestRunnerName` |  |  | ❌ |  |
| `ExcludeFromSourceBuild` |  |  | ❌ |  |
| `VersionToolName` |  |  | ❌ |  |
| `NerdbankGitVersioningVersion` | Specifies the version of [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) | `3.*` | ✔️ |  |
| `XUnitVersion` | Specifies the version of [xUnit](https://xunit.net/) | `2.*` | ✔️ | Available only when `$(IsTestProject)` is equal to `true` |
| `XUnitAssertVersion` | Specifies the version of [xUnit](https://xunit.net/) | `$(XUnitVersion)` | ✔️ | Available only when `$(IsTestProject)` is equal to `true` |
| `XUnitAbstractionsVersion` | Specifies the version of [xUnit](https://xunit.net/) | `$(XUnitVersion)` | ✔️ | Available only when `$(IsTestProject)` is equal to `true` |
| `XUnitRunnerVisualStudioVersion` | Specifies the version of [xUnit](https://xunit.net/) | `2.4.3` | ✔️ | Available only when `$(IsTestProject)` is equal to `true` |
| `XUnitRunnerConsoleVersion` | Specifies the version of [xUnit](https://xunit.net/) | `2.4.1` | ✔️ | Available only when `$(IsTestProject)` is equal to `true` |
| `XUnitDesktopSettingsFile` | Specifies the [xUnit](https://xunit.net/) runner configuration. | `$(MSBuildThisFileDirectory)xunit.runner.json` | ❌ | Available only when `$(IsTestProject)` is equal to `true` |
| `TestRuntime` |  |  | ❌ |  |
| `TestArchitectures` |  |  | ✔️ |  |
| `GitRepoRoot` | Used by [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) to identify the `.git` folder | `$(RepoRoot)` | ✔️ |  |
