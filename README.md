# LaLaDancer

This project is a mod for Rift of the NecroDancer which provides miscellaneous quality of life improvements and fixes various minor bugs.

> [!WARNING]
> BepInEx mods are <ins>**not officially supported**</ins> by Rift of the NecroDancer. If you encounter any issues with this mod, please open an issue on this GitHub repository, and do not submit reports to Brace Yourself Games!

The current version is <ins>**v0.1.1**</ins>. Downloads for the latest version can be found [here](https://github.com/96-LB/LaLaDancer/releases/latest). The changelog can be found [here](Changelog.md).

## Installation

1. Install the latest version of BepInEx 5 and Rift of the NecroManager. You can find detailed directions on the [Rift of the NecroManager](https://github.com/96-LB/RiftOfTheNecroManager) GitHub page!

2. Navigate to the latest release of LaLaDancer [here](https://github.com/96-LB/LaLaDancer/releases/latest).

> [!CAUTION]
> Do NOT download the source code using the button at the top of this page. If you're downloading a `.zip` file, you are at the wrong place.

3. Expand the "Assets" tab at the bottom and download `LaLaDancer.dll`.

4. Place `LaLaDancer.dll` in the `BepInEx/plugins` directory inside the Rift of the NecroDancer game folder.

> [!TIP]
> You can find this folder by right clicking on the game in your Steam library and clicking 'Properties'. Then navigate to 'Installed Files' and click 'Browse'.

## Usage

By default, all bugfixes are applied as soon as the mod is installed, but all quality-of-life changes are opt-in. All options can be configured in the settings menu. The full list of features can be seen below.

### Bugfixes
- Enemies which are affected by bounce traps or portals near the action row now have much improved sound effect prediction for their hitsounds.
- Armadillos have hitsounds which now match their timing when used on subdivisions not divisible by 3.
- The sound effect for blademasters now matches the chart's current tempo instead of the BPM at the beginning of the chart.
- The score display can now display more than 7 digits.
- Custom particles on custom charts will now display all sprites from the spritesheet, instead of just the first 75%.
- The practice mode countdown now matches the tempo of the section you start at instead of the BPM at the beginning of the chart.

## Quality of life
- You can now automatically skip the splash screen on the game startup.
