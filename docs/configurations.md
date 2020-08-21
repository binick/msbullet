# Configuration.

MsBullet SDK provides some variables for internal use and you can used it to your build logic.

| Variables | Description | Default value | Overridable | Note |
|--- |--- |--- | :---: |--- |
| OutDirName | Identifying folder where artifacts are placed | `$(MSBuildProjectName)` | ✔ | |
| BaseOutputPath | Identifying folder where artifacts are placed | `$(ArtifactsBinDir)/$(MSBuildProjectName)/` | ✔ | |
| OutputPath | Identifying folder where artifacts are placed | When `$(PlatformName)` is equal to `AnyCPU` `$(BaseOutputPath)/$(Configuration)/`, otherwise `$(BaseOutputPath)/$(PlatformName)/$(Configuration)` | ✔ | |
| IsUnitTestProject | Identifying unit test project.<br/> A project is identified as unit test project when your's name end with:<br/><ul><li>`.Tests`</li><li>`.UnitTests`</li></ul> | false | ✔ | |
| IsIntegrationTestProject | Identifying integration test project.<br/> A project is identified as integration test project when your's name end with: `.IntegrationTests` | false | ✔ | |
| IsPerformanceTestProject | Identifying performance test project.<br/> A project is identified as performance test project when your's name end with: `.PerformanceTests` | false | ✔ | |
| IsTestProject | Identifying test project.<br/> A project is identified as test project when it has one of that properties as true:<br/><ul><li>`IsUnitTestProject`</li><li>`IsIntegrationTestProject`</li><li>`IsPerformanceTestProject`</li></ul> | N/A | ❌ | |
