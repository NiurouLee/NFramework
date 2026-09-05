# NFramework DDD

This package contains the `Assets` content of the DDD Unity project, arranged so it
can be installed through the Unity Package Manager.

## Install

### From a Git URL

Add this package from its Git repository URL in:

`Window > Package Manager > + > Add package from git URL...`

If this folder is a subfolder of a larger repository, append the path to this
folder, for example:

```
https://github.com/you/your-repo.git?path=/com.nframework.ddd#0.1.0
```

### From a local folder

`Window > Package Manager > + > Add package from disk...` and select this folder.

## Required project dependencies

Install the following packages in the destination project **before** adding this
package, otherwise some assemblies will fail to resolve:

```json
{
  "dependencies": {
    "com.code-philosophy.hybridclr": "https://gitee.com/focus-creative-games/hybridclr_unity.git#v8.8.0",
    "com.code-philosophy.luban": "https://gitee.com/focus-creative-games/luban_unity.git#v1.1.1"
  },
  "scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.tuyoogame.yooasset"
      ]
    }
  ]
}
```

Commercial / per-seat plugins such as DOTween Pro, Odin Inspector and EasyTouch
are included in this snapshot for convenience. Check each asset's license before
redistributing the package.

## Layout

- `Assets/Framework` - framework core, UI system and third-party snapshots
- `Assets/GameLogic` - game logic / hotfix code
- `Assets/PackageAssets` - prefabs and art assets
- `Assets/StreamingAssets` - streaming content (UPM does not copy these into the
  destination project's StreamingAssets; consume them with AssetBundle /
  Addressables instead)

## Note

UPM only compiles scripts that belong to an assembly definition (`.asmdef`).
Scripts left loose inside `Assets` are copied by the package but will not be
compiled by Unity.
