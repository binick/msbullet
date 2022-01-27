---
title: Artifact folder structure
---

# Single build output.

All projects output will be places into the `artifacts` folder 

```
root
└─ artifacts
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
