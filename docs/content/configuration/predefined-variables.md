---
title: Well-known properties
---

# Well-known properties.

MsBullet SDK provides some properties for internal use and you can used it to your build logic.

For all other mentioned variables that are not present into the table above you can find more info at the [official documentation](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-reserved-and-well-known-properties).

| Variables | Description | Default value | Overridable | Note |
|--- |--- |--- | :---: |--- |
| Configuration | Specify the current project configuration. | `Debug` | ✔ | |
| Platform | Specify the current project platform. | `AnyCPU` | ✔ | |
| PlatformName | Specify the current project platform name. | `$(Platform)` | ✔ | |
| RepoRoot | Identifying the root of repository. | N/A | ✔ | |
| ArtifactsDir | Specify the root where build system outputs are placed. | `$(RepoRoot)/artifacts/` | ❌ | |
| OutDirName | Used to specified the end folder of path where artifacts are placed | `$(MSBuildProjectName)` | ✔ | |
| BaseOutputPath | Identifying folder where binary artifacts are placed | `$(ArtifactsBinDir)/$(MSBuildProjectName)/` | ✔ | |
| OutputPath | Identifying folder where artifacts are placed | When `$(PlatformName)` is equal to `AnyCPU` `$(BaseOutputPath)/$(Configuration)/`, otherwise `$(BaseOutputPath)/$(PlatformName)/$(Configuration)/` | ❌ | |
| BaseIntermediateOutputPath | Identifying folder where [CIL](https://en.wikipedia.org/wiki/Common_Intermediate_Language) artifats are placed | `$(ArtifactsObjDir)/$(MSBuildProjectName)/` | ✔ | |
| IntermediateOutputPath | Identifying folder where artifacts are placed | When `$(PlatformName)` is equal to `AnyCPU` `$(BaseIntermediateOutputPath)/$(Configuration)/`, otherwise `$(BaseIntermediateOutputPath)/$(PlatformName)/$(Configuration)/` | ❌ | |
| UsingToolXUnit | Used to opt-out built-in features.<br/>By default we use [xUnit](https://xunit.github.io/) as test framework, that're represents the standart de-facto. | true | ✔ | |
| UsingToolNerdbankGitVersioning | Used to opt-out built-in features.<br/>By default we use [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) as semantic versioning tool. | true | ✔ | |
| StyleCopConfig | Used to customize the default rules of [StyleCopAnalyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) through the [`stylecop.json`](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/Configuration.md). | `$(RepoRoot)/eng/stylecop.json` | ✔ | |
| IsUnitTestProject | Identifying unit test project.<br/> A project is identified as unit test project when your's name end with:<br/><ul><li>`.Tests`</li><li>`.UnitTests`</li></ul> | false | ✔ | |
| IsIntegrationTestProject | Identifying integration test project.<br/> A project is identified as integration test project when your's name end with: `.IntegrationTests` | false | ✔ | |
| IsPerformanceTestProject | Identifying performance test project.<br/> A project is identified as performance test project when your's name end with: `.PerformanceTests` | false | ✔ | |
| IsTestProject | Identifying test project.<br/>A project is identified as test project when it has one of that properties as true:<br/><ul><li>`IsUnitTestProject`</li><li>`IsIntegrationTestProject`</li><li>`IsPerformanceTestProject`</li></ul> | N/A | ❌ | |
