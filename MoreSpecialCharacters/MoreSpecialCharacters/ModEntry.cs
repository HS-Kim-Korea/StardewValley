using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using Harmony;

namespace MoreSpecialCharacters
{
    public class ModEntry : Mod
    {
        public static Mod instance;
        internal static IModHelper helper;
        private ModConfig config;
        public static string specialCharacters = "<=>@$`+";
        public override void Entry(IModHelper helper)
        {
            ModEntry.instance = this;
            ModEntry.helper = helper;
             
            config = this.Helper.ReadConfig<ModConfig>();
            if (config.Enable)
            {
                log(String.Format("Enabled special chacters"));
                specialCharacters += config.AddSpecialCharacters;
                var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
                var original = typeof(SpriteText).GetMethod("IsSpecialCharacter", BindingFlags.Static | BindingFlags.NonPublic);
                var prefix = typeof(ModEntry).GetMethod("IsSpecialCharacter_Prefix", BindingFlags.Static | BindingFlags.Public);

                harmony.Patch(original, new HarmonyMethod(prefix));
            }
        }
        public static void log(string text, LogLevel level = LogLevel.Debug)
        {
            instance.Monitor.Log(text, level);
        }

        public static bool IsSpecialCharacter_Prefix(SpriteText __instance, char c, ref bool __result)
        {
            try
            {
                if (specialCharacters.IndexOf(c) >= 0)
                {
                    __result = true;
                }
                else
                {
                    __result = false;
                }
                return false;
            }
            catch (Exception ex)
            {
                log($"Failed in {nameof(IsSpecialCharacter_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
