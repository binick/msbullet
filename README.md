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

The steps below are optionally, but recommended.

1. Copy to `/eng/common/build.ps1` and this contents [build.ps1](eng/common/build.ps1)
2. Copy to `/eng/common/build.sh` and this contents [build.sh](eng/common/build.sh)
3. Copy to `/eng/common/tools.ps1` this contents [tools.ps1](eng/common/tools.ps1)
4. Copy to `/eng/common/tools.sh` this contents [tools.sh](eng/common/tools.sh)
5. Copy to `build.cmd` content [build.cmd](build.cmd)
6. Copy to `build.sh` content [build.sh](build.sh)

## Contributing

**NET 7.0 workaround**  
As reported in the problem [#16400 [net7.0-rc2] Creating a cross-target library with net6.0-mac fails with an obscure error](https://github.com/xamarin/xamarin-macios/issues/16400) at the moment there is a problem that makes it mandatory to install the following workloads:

 - `dotnet workload install ios`
 - `dotnet workload install maccatalyst`
 - `dotnet workload install macos`
 - `dotnet workload install tvos`
