---
title: Single build output
weight: 20
---

## Single build output

All projects output will be placed into the `artifacts` folder that has structured below

``` plain
artifacts
├─ obj
│  └─ $(MSBuildProjectName)
│     └─ ($(Platform)|)
│        └─ $(Configuration)
├─ bin
│  └─ $(MSBuildProjectName)
│     └─ ($(Platform)|)
│        └─ $(Configuration)
├─ log
│  └─ $(Configuration)
├─ tmp
│  └─ $(Configuration)
├─ TestResults
│  └─ $(Configuration)
|     ├─ Coverage
│     │  └─ $(MSBuildProjectName)_$(TargetFramework|)_$(Platform|).(xml|html)
│     └─ Reports
│        ├─ $(MSBuildProjectName)
│        |  ├─ History
│        |  └─ Reports
|        └─ Summary
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
| TestResults | Reports and coverage results produced by all projects in the repo. |

Each directory is identified by the property, please refer to [Well-known properties](../configuration/predefined-variables/).
