---
title: Artifact folder structure
weight: 5
---

```
.
└─ $(ArtifactsDir)
   ├─ toolset
   ├─ obj
   ├─ bin
   ├─ log
   │  └─ $(Configuration)
   ├─ tmp
   │  └─ $(Configuration)
   ├─ TestResults
   │  └─ $(Configuration)
   ├─ Reports
   ├─ Coverage
   ├─ SymStore
   │  └─ $(Configuration)
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
| SymStore | Storage for converted Windows PDBs |
| log | Build binary log and other logs. |
| tmp | Temp files generated during build. |
| toolset | Files generated during toolset restore. |
