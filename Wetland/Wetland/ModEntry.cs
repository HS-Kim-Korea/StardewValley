using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace Wetland
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        public static Mod? instance;
        public static ModConfig? Config;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            instance = this;
            Config = helper.ReadConfig<ModConfig>();
            Harmony harmony = new(ModManifest.UniqueID);

            if(Config.enable)
            {
                Log($"Enabled Wetland", LogLevel.Debug);
                harmony.Patch(
                    original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.paddyWaterCheck)),
                    prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.PaddyWaterCheck_Prefix))
                );
            }
        }
        private static void Log(string message, LogLevel level = LogLevel.Trace)
        {
            instance?.Monitor?.Log(message, level);
        }
        private static bool PaddyWaterCheck_Prefix(HoeDirt __instance, ref bool __result, bool forceUpdate)
        {
            try
            {
                if (!forceUpdate && __instance.nearWaterForPaddy.Value >= 0)
                {
                    return true;
                }
                if (!__instance.hasPaddyCrop())
                {
                    return true;
                }
                Vector2 tile = __instance.Tile;
                if (__instance.Location.getObjectAtTile((int)tile.X, (int)tile.Y) is IndoorPot)
                {
                    return true;
                }
                int num = Config?.range ?? 5;
                for (int i = -num; i <= num; i++)
                {
                    for (int j = -num; j <= num; j++)
                    {
                        if (__instance.Location.isWaterTile((int)(tile.X + (float)i), (int)(tile.Y + (float)j)))
                        {
                            __instance.nearWaterForPaddy.Value = 1;
                            __result = true;
                            return false;
                        }
                    }
                }
                foreach(ModConfig.WaterSource source in Config?.waterSources ?? new List<ModConfig.WaterSource>())
                {
                    if(source.name == "")
                    {
                        continue;
                    }
                    num = source.range;
                    for(int i = -num; i <= num; i++)
                    {
                        for(int j = -num; j <= num; j++)
                        {
                            StardewValley.Object obj = __instance.Location.getObjectAtTile((int)(tile.X + (float)i), (int)(tile.Y + (float)j));
                            if (obj is not null && source.name == obj.name)
                            {
                                __instance.nearWaterForPaddy.Value = 1;
                                __result = true;
                                return false;
                            }
                        }
                    }
                }
                __instance.nearWaterForPaddy.Value = 0;
                __result = false;
                return false;
            }
            catch (Exception error)
            {
                Log($"Failed in {nameof(ModEntry.PaddyWaterCheck_Prefix)}:\n{error}", LogLevel.Error);
                return true;
            }
        }
    }
}