# NoMoreMountains

`NoMoreMountains` is a BepInEx mod for MineMogul that cleans up loose idle ore by dropping forgotten surface piles through the world once they have sat still long enough.

## What It Does

- Targets loose `Ore`, `OreCluster`, `Gem`, and `Crushed` `OrePiece` objects.
- Ignores ore that is in baskets, attached to a magnet tool, or already marked for destruction.
- When an eligible piece stays idle long enough, the mod:
  - sets `BaseSellValue` to `0`
  - enables its colliders as triggers
  - nudges it downward so it can fall through terrain and clear surface clutter
- When pooled ore respawns, the original collider state and sell value are restored.

## Configuration

Config section: `General`

- `Enabled` default `true`
- `IdleSecondsBeforeDrop` default `30`
- `ScanIntervalSeconds` default `0.25`
- `MovementThreshold` default `0.03`
- `VelocityThreshold` default `0.05`

Config section: `Debug`

- `VerboseLogging` default `false`

## Build

This project uses the shared MineMogul template paths in the `.csproj`.

Default fallback path:

```text
F:\SteamLibrary\steamapps\common\MineMogul
```

If your game is installed somewhere else, build with a custom property:

```powershell
dotnet build .\NoMoreMountains.csproj -c Release /p:MineMogulDir="D:\SteamLibrary\steamapps\common\MineMogul"
```

## Install

1. Build the project in `Release`.
2. Copy `bin\Release\net472\NoMoreMountains.dll` into your MineMogul `BepInEx\plugins` folder.

## Project Layout

- `NoMoreMountains.cs` plugin bootstrap and Harmony setup
- `ModInfo.cs` central mod metadata
- `Config/` config binding
- `Core/` runtime scan loop
- `Services/` cleanup logic and state management
- `Data/` small state models
- `Patches/` Harmony patch entry points
- `Properties/` assembly metadata
