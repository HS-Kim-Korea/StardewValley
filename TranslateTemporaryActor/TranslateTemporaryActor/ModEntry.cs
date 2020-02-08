using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using Harmony;

namespace TranslateTemporaryActor
{
    public class ModEntry : Mod
    {
        public static Mod instance;
        internal static IModHelper helper;
        private static ModConfig config;
        private static ModData data;
        public override void Entry(IModHelper helper)
        {
            ModEntry.instance = this;
            ModEntry.helper = helper;

            config = this.Helper.ReadConfig<ModConfig>();
            if (config.Enable)
            {
                log(String.Format("Enabled special chacters"));
                data = this.Helper.Data.ReadJsonFile<ModData>("assets\\NPCNames.json") ?? new ModData();
                var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
                var original = typeof(NPC).GetMethod("translateName", BindingFlags.Instance | BindingFlags.NonPublic);
                var prefix = typeof(ModEntry).GetMethod("translateName_Prefix", BindingFlags.Static | BindingFlags.Public);

                harmony.Patch(original, new HarmonyMethod(prefix));
            }
        }
        public static void log(string text, LogLevel level = LogLevel.Debug)
        {
            instance.Monitor.Log(text, level);
        }
        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
        public static bool translateName_Prefix(NPC __instance, string name, ref string __result)
        {
            try
            {
                foreach(ModData.NPCName npc in data.NPCNames)
                {
                    if(npc.Name == name)
                    {
                        string localized = GetPropValue(npc.NameLocalization, helper.Translation.Locale.Substring(0, 2)).ToString();
                        if(localized != "")
                        {
                            __result = localized;
                            return false;
                        }
                        return true;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                log($"Failed in {nameof(translateName_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
