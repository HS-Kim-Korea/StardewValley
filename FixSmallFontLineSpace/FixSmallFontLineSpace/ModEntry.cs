using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using Harmony;


namespace FixSmallFontLineSpace
{
    public class ModEntry : Mod
    {
        public static Mod instance;
        internal static IModHelper helper;
        private static ModConfig config;
        public override void Entry(IModHelper helper)
        {
            ModEntry.instance = this;
            ModEntry.helper = helper;

            config = this.Helper.ReadConfig<ModConfig>();
            if (config.Enable)
            {
                Log(String.Format("Enabled FixSmallFontLineSpace"));

                var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
                var original = typeof(Game1).GetMethod("TranslateFields", BindingFlags.Instance | BindingFlags.NonPublic);
                var postfix = typeof(ModEntry).GetMethod("TranslateFieldsPostfix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, null, new HarmonyMethod(postfix));
            }
        }
        public static void Log(string text, LogLevel level = LogLevel.Debug)
        {
            instance.Monitor.Log(text, level);
        }
        public static void TranslateFieldsPostfix(Game1 __instance)
        {
            try
            {
                if(config.LineSpace > 0)
                {
                    Game1.smallFont.LineSpacing = config.LineSpace;
                    Log($"Small font line space : {config.LineSpace}");
                }
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(TranslateFieldsPostfix)}:\n{ex}");
            }
        }
    }
}
