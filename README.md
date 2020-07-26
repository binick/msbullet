![Logo](assets/icon.png)

# MsBullet SDK 

The project is inspired to [Arcade](https://github.com/dotnet/arcade) and your intent is that to avoid the tedious configuration of tools related to development cycle such as testing, versioning and other best practices.

## How to use

You can start using this SDK based on [MSBuild](https://docs.microsoft.com/en-us/visualstudio/msbuild) in three steps:

1. Add [`global.json`](global.json) or copy this on root of your repo:

``` json
{
  "tools": {
    "dotnet": "[dotnet sdk version]"
  },
  "msbuild-sdks": {
    "MsBullet.Sdk": "[MsBullet.Sdk version]"
  }
}

```

2. Add [`Directory.Build.props`](Directory.Build.props) or copy this on `root` of your repo:

``` xml
<?xml version="1.0" encoding="utf-8"?>
<Project>

  <Import Project="Sdk.props" Sdk="MsBullet.Sdk" />

</Project>
```

3. Add [`Directory.Build.targets`](Directory.Build.targets) or copy this on `root` of your repo:

``` xml
<?xml version="1.0" encoding="utf-8"?>
<Project>

  <Import Project="Sdk.targets" Sdk="MsBullet.Sdk" />

</Project>
```

The steps below are optionally, but reccomended.

1. Copy to `/eng/common/build.ps1` and this contents [build.ps1](eng/common/build.ps1)
2. Copy to `/eng/common/tools.ps1` this contents [tools.ps1](eng/common/tools.ps1)
3. Copy on repo `root`  content [build.cmd](build.cmd)
