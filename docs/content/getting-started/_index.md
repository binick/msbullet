---
title: Getting started
weight: 10
---

# Getting started

You can start using this SDK based on [MSBuild](https://docs.microsoft.com/visualstudio/msbuild) in three steps:

1. Add `global.json`:

``` json
{
  "msbuild-sdks": {
    "MsBullet.Sdk": "<MsBullet.Sdk version>"
  }
}
```

Replace `MsBullet.Sdk version` with desired NuGet package version, [https://www.nuget.org/packages/MsBullet.Sdk](https://www.nuget.org/packages/MsBullet.Sdk)

2. Add `Directory.Build.props` or copy this on `root` of your repo:

``` xml
<?xml version="1.0" encoding="utf-8"?>
<Project>

  <Import Project="Sdk.props" Sdk="MsBullet.Sdk" />

</Project>
```

3. Add `Directory.Build.targets` or copy this on `root` of your repo:

``` xml
<?xml version="1.0" encoding="utf-8"?>
<Project>

  <Import Project="Sdk.targets" Sdk="MsBullet.Sdk" />

</Project>
```
