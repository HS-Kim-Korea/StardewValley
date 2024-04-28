using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewModdingAPI;
using HarmonyLib;
using BmFont;

namespace FixFontOption
{
    internal class BMFontOption
    {
        private static IModHelper? Helper;
        private static IMonitor? Monitor;
        private static bool Debug = false;
        private static bool FontPixelZoomEnabled = false;
        private static float FontPixelZoomValue = 1f;
        private static bool NewCharacterMap = false;
        public BMFontOption(IModHelper? helper = null, IMonitor? monitor = null)
        {
            Helper = helper;
            Monitor = monitor;
        }
        public void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteText), "setUpCharacterMap"),
                prefix: new HarmonyMethod(typeof(BMFontOption), nameof(BMFontOption.SetUpCharacterMap_PreFix)),
                postfix: new HarmonyMethod(typeof(BMFontOption), nameof(BMFontOption.SetUpCharacterMap_PostFix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteText), "OnLanguageChange"),
                postfix: new HarmonyMethod(typeof(BMFontOption), nameof(BMFontOption.OnLanguageChange_PostFix)) 
            );
        }
        public void SetDebug(bool enabled)
        {
            Debug = enabled;
        }
        public void SetFontPixelZoom(bool? enable, float? value)
        {
            if(enable.HasValue)
            {
                FontPixelZoomEnabled = enable.Value;
            }
            if(value.HasValue)
            {
                FontPixelZoomValue = value.Value;
            }
        }
        private static void Log(string message, LogLevel level = LogLevel.Trace)
        {
            Monitor?.Log(message, level);
        }
        private static bool SetUpCharacterMap_PreFix(SpriteText __instance)
        {
            try
            {
                Dictionary<char, FontChar>? characterMap = Helper?.Reflection.GetField<Dictionary<char, FontChar>>(typeof(SpriteText), nameof(SpriteText.characterMap)).GetValue();
                NewCharacterMap = !LocalizedContentManager.CurrentLanguageLatin && characterMap == null;
                return true;
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(SetUpCharacterMap_PreFix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
        private static void SetUpCharacterMap_PostFix(SpriteText __instance)
        {
            try
            {
                if(FontPixelZoomEnabled && NewCharacterMap && LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko)
                {
                    SpriteText.fontPixelZoom = FontPixelZoomValue;
                }
                NewCharacterMap = false;
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(SetUpCharacterMap_PostFix)}:\n{ex}", LogLevel.Error);
            }
        }
        private static void OnLanguageChange_PostFix(SpriteText __instance, ref LocalizedContentManager.LanguageCode code)
        {
            try
            {
                if (FontPixelZoomEnabled && code == LocalizedContentManager.LanguageCode.ko)
                {
                    SpriteText.fontPixelZoom = FontPixelZoomValue;
                }
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(OnLanguageChange_PostFix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}
