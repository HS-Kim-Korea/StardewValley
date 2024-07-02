using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Objects;
using Harmony;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace Wetland
{
    public class ModEntry : Mod
    {
        public static Mod instance;
        internal static IModHelper helper;
        internal static ModConfig config;
        public override void Entry(IModHelper helper)
        {
            ModEntry.instance = this;
            ModEntry.helper = helper;

            config = helper.ReadConfig<ModConfig>();
            if (config.enable)
            {
                MethodInfo original, prefix;
                HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

                Log($"Enabled Wetland");
                original = typeof(HoeDirt).GetMethod("paddyWaterCheck", BindingFlags.Instance | BindingFlags.Public);
                prefix = typeof(ModEntry).GetMethod("PaddyWaterCheckPrefix", BindingFlags.Static | BindingFlags.NonPublic);
                harmony.Patch(original, new HarmonyMethod(prefix));
            }
        }
        public static void Log(string text, LogLevel level = LogLevel.Debug)
        {
            instance.Monitor.Log(text, level);
        }
        private static bool PaddyWaterCheckPrefix(HoeDirt __instance, ref bool __result, GameLocation location, Vector2 tile_location)
        {
            try
            {
                if (__instance.nearWaterForPaddy.Value >= 0)
                    return __instance.nearWaterForPaddy.Value == 1;
                if (!__instance.hasPaddyCrop())
                {
                    __instance.nearWaterForPaddy.Value = 0;
                    __result = false;
                    return false;
                }
                if (location.getObjectAtTile((int)tile_location.X, (int)tile_location.Y) is IndoorPot)
                {
                    __instance.nearWaterForPaddy.Value = 0;
                    __result = false;
                    return false;
                }
                int num = config.range;
                for (int index1 = -num; index1 <= num; ++index1)
                {
                    for (int index2 = -num; index2 <= num; ++index2)
                    {
                        if (location.isOpenWater((int)((double)tile_location.X + (double)index1), (int)((double)tile_location.Y + (double)index2)))
                        {
                            __instance.nearWaterForPaddy.Value = 1;
                            __result = true;
                            return false;
                        }
                    }
                }
                foreach(ModConfig.WaterSource source in config.waterSources)
                {
                    if(source.name == "")
                    {
                        continue;
                    }
                    num = source.range;
                    for (int index1 = -num; index1 <= num; ++index1)
                    {
                        for (int index2 = -num; index2 <= num; ++index2)
                        {
                            StardewValley.Object obj = location.getObjectAtTile((int)((double)tile_location.X + (double)index1), (int)((double)tile_location.Y + (double)index2));
                            if (obj is null ? false : source.name == obj.name)
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
            catch(Exception error)
            {
                Log($"Failed in {nameof(PaddyWaterCheckPrefix)}:\n{error}", LogLevel.Error);
                return true;
            }
        }
    }
}
