# ConfigGarden
A simple modding utility to save/load json configuration files in predictable locations.

## Usage
First, create a config class with all the properties you plan to use and a ModInfo class
```csharp
using ConfigGarden;
using ConfigGarden.Interfaces;

class MyConfig
{
    public bool Enabled { get; set; } = true;
}

class ModInfo : IModInfo
{
    public string GAME_NAME => "Tiny Terry's Turbo Trip";
    public string AUTHOR_NAME => "FeluciaPS";
    public string MOD_NAME => "My Awesome Mod";
}```

You can then create a ConfigHelper anywhere in your plugin.

```csharp
public class MyPlugin
{
    public static ConfigHelper<MyConfig> config;

    public static void Awake() {
        // Loads the config from disk, or creates it with default values if it doesn't exist
        config = new ConfigHelper<MyConfig>(new ModInfo());

        // You can now access the config values through the Data property
        bool isEnabled = config.Data.Enabled;

        // And set them
        config.Data.Enabled = false;
        
        // Just don't forget to save after making changes
        config.Save();
    }
}
```

### Optional parameters
- `fileName`: defaults to `config.json`.
- `logger`: pass any class implementing `ConfigGarden.Interfaces.ILogger` to have messages logged to your console.


### Other
The library also contains a small `WineHelper.IsWine` property that you can use to detect WINE environments.

It does this by checking if `wine_get_version` is present on `ntdll.dll` which should only be the case when WINE is running.

## File locations
By default, files are saved in `[rootdir]/ModConfig/[GameName]/[PluginAuthor]/[PluginName]`.

- **Windows:** `[rootdir]` is your `My Documents` folder
- **WINE:** `[rootdir]` is `~/.config/` (or whatever is in the user's `$XDG_CONFIG_HOME` variable)

Not all WINE forks and Linux distros have been tested, so the path may vary there.

Example: `Documents/ModConfig/Tiny Terry's Turbo Trip/FeluciaPS/My Awesome Mod/`

## Credits
Created by [FeluciaPS](https://github.com/FeluciaPS). Mod icon by Frutiflow. 

Special thanks to Frutiflow for testing on WINE.