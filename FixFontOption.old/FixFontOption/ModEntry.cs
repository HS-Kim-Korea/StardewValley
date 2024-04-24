using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using Harmony;
using StardewValley.BellsAndWhistles;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using BmFont;

namespace FixFontOption
{
    public class ModEntry : Mod
    {
        public static Mod instance;
        internal static IModHelper helper;
        private static ModConfig config;
        private static Dictionary<int, string> debugBuffer = new Dictionary<int, string>();
        private static bool setUpCharacterMap = false;
        public override void Entry(IModHelper helper)
        {
            ModEntry.instance = this;
            ModEntry.helper = helper;

            config = this.Helper.ReadConfig<ModConfig>();
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            MethodInfo original, prefix, postfix;

            if (config.HideShadow)
            {
                Log(String.Format("Enabled to remove text shadow"));

                original = typeof(Utility).GetMethod("drawTextWithShadow", new Type[] { typeof(SpriteBatch), typeof(string), typeof(SpriteFont), typeof(Vector2), typeof(Color), typeof(float), typeof(float), typeof(int), typeof(int), typeof(float), typeof(int) });
                prefix = typeof(ModEntry).GetMethod("DrawTextWithShadowPrefix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));

                original = typeof(Utility).GetMethod("drawTextWithShadow", new Type[] { typeof(SpriteBatch), typeof(StringBuilder), typeof(SpriteFont), typeof(Vector2), typeof(Color), typeof(float), typeof(float), typeof(int), typeof(int), typeof(float), typeof(int) });
                prefix = typeof(ModEntry).GetMethod("DrawTextWithShadowPrefix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));

                original = typeof(Utility).GetMethod("drawTextWithColoredShadow", BindingFlags.Static | BindingFlags.Public);
                prefix = typeof(ModEntry).GetMethod("DrawTextWithColoredShadowPrefix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));

                Game1.textShadowColor = new Color(0, 0, 0, 0);
                SpriteText.shadowAlpha = 0;

                //original = typeof(Game1).GetMethod("drawDialogueBox", new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(bool), typeof(bool), typeof(string), typeof(bool), typeof(bool), typeof(int), typeof(int), typeof(int) });
                //prefix = typeof(ModEntry).GetMethod("DrawDialogueBoxPrefix", BindingFlags.Static | BindingFlags.Public);
                //harmony.Patch(original, new HarmonyMethod(prefix));

                //original = typeof(SpriteText).GetMethod("drawString", BindingFlags.Static | BindingFlags.Public);
                //prefix = typeof(ModEntry).GetMethod("DrawStringPrefix", BindingFlags.Static | BindingFlags.Public);
                //harmony.Patch(original, new HarmonyMethod(prefix));
            }
            if (config.EnableFixFontPixelZoom)
            {
                Log(String.Format("Enabled to fix font pixel zoom"));
                original = typeof(SpriteText).GetMethod("setUpCharacterMap", BindingFlags.Static | BindingFlags.NonPublic);
                prefix = typeof(ModEntry).GetMethod("SetUpCharacterMapPreFix", BindingFlags.Static | BindingFlags.Public);
                postfix = typeof(ModEntry).GetMethod("SetUpCharacterMapPostFix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

                original = typeof(SpriteText).GetMethod("OnLanguageChange", BindingFlags.Static | BindingFlags.NonPublic);
                postfix = typeof(ModEntry).GetMethod("OnLanguageChangePostfix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, null, new HarmonyMethod(postfix));
            }
            if (config.EnableFixSmallFontLineSpace)
            {
                Log(String.Format("Enabled to fix line space for small font"));
                original = typeof(Game1).GetMethod("TranslateFields", BindingFlags.Instance | BindingFlags.Public);
                postfix = typeof(ModEntry).GetMethod("TranslateFieldsPostfix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, null, new HarmonyMethod(postfix));
            }
            if(config.EnableFixFontColor)
            {
                original = typeof(SpriteText).GetMethod("drawString", BindingFlags.Static | BindingFlags.Public);
                prefix = typeof(ModEntry).GetMethod("DrawStringPrefix2", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));

                Log(String.Format("Enabled to fix font color"));
                original = typeof(SpriteBatch).GetMethod("DrawString", new Type[] { typeof(SpriteFont), typeof(string), typeof(Vector2), typeof(Color) });
                prefix = typeof(ModEntry).GetMethod("SpriteBatchDrawStringPrefix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));

                original = typeof(SpriteBatch).GetMethod("DrawString", new Type[] { typeof(SpriteFont), typeof(StringBuilder), typeof(Vector2), typeof(Color) });
                prefix = typeof(ModEntry).GetMethod("SpriteBatchDrawStringPrefix2", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));

                original = typeof(SpriteBatch).GetMethod("DrawString", new Type[] { typeof(SpriteFont), typeof(string), typeof(Vector2), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) });
                prefix = typeof(ModEntry).GetMethod("SpriteBatchDrawStringPrefix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));

                original = typeof(SpriteBatch).GetMethod("DrawString", new Type[] { typeof(SpriteFont), typeof(StringBuilder), typeof(Vector2), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) });
                prefix = typeof(ModEntry).GetMethod("SpriteBatchDrawStringPrefix2", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));

                original = typeof(SpriteBatch).GetMethod("DrawString", new Type[] { typeof(SpriteFont), typeof(string), typeof(Vector2), typeof(Color), typeof(float), typeof(Vector2), typeof(Vector2), typeof(SpriteEffects), typeof(float) });
                prefix = typeof(ModEntry).GetMethod("SpriteBatchDrawStringPrefix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));

                original = typeof(SpriteBatch).GetMethod("DrawString", new Type[] { typeof(SpriteFont), typeof(StringBuilder), typeof(Vector2), typeof(Color), typeof(float), typeof(Vector2), typeof(Vector2), typeof(SpriteEffects), typeof(float) });
                prefix = typeof(ModEntry).GetMethod("SpriteBatchDrawStringPrefix2", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));

                original = typeof(SpriteText).GetMethod("getColorFromIndex", BindingFlags.Static | BindingFlags.Public);
                prefix = typeof(ModEntry).GetMethod("getColorFromIndexPrefix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));
            }
            if(config.EnableFixImgColor)
            {
                original = typeof(SpriteBatch).GetMethod("Draw", new Type[] { typeof(Texture2D), typeof(Rectangle), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(SpriteEffects), typeof(float) });
                prefix = typeof(ModEntry).GetMethod("SpriteBatchDrawPrefix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));

                original = typeof(SpriteBatch).GetMethod("Draw", new Type[] { typeof(Texture2D), typeof(Rectangle), typeof(Rectangle?), typeof(Color) });
                prefix = typeof(ModEntry).GetMethod("SpriteBatchDrawPrefix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));

                original = typeof(SpriteBatch).GetMethod("Draw", new Type[] { typeof(Texture2D), typeof(Rectangle), typeof(Color) });
                prefix = typeof(ModEntry).GetMethod("SpriteBatchDrawPrefix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));

                original = typeof(SpriteBatch).GetMethod("Draw", new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(Vector2), typeof(SpriteEffects), typeof(float) });
                prefix = typeof(ModEntry).GetMethod("SpriteBatchDrawPrefix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));

                original = typeof(SpriteBatch).GetMethod("Draw", new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) });
                prefix = typeof(ModEntry).GetMethod("SpriteBatchDrawPrefix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));

                original = typeof(SpriteBatch).GetMethod("Draw", new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color) });
                prefix = typeof(ModEntry).GetMethod("SpriteBatchDrawPrefix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));

                original = typeof(SpriteBatch).GetMethod("Draw", new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Color) });
                prefix = typeof(ModEntry).GetMethod("SpriteBatchDrawPrefix", BindingFlags.Static | BindingFlags.Public);
                harmony.Patch(original, new HarmonyMethod(prefix));
            }
            if (config.Debug)
            {
                helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
                debugBuffer.Clear();
            }

        }

        public static void Log(string text, LogLevel level = LogLevel.Debug)
        {
            instance.Monitor.Log(text, level);
        }
        public static bool DrawTextWithShadowPrefix(Utility __instance, ref float shadowIntensity)
        {
            try
            {
                shadowIntensity = 0;
                return true;
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(DrawTextWithShadowPrefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
        public static bool DrawTextWithColoredShadowPrefix(Utility __instance,
          ref Color shadowColor)
        {
            try
            {
                shadowColor *= 0f;
                return true;
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(DrawTextWithColoredShadowPrefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
        public static Color getColorFromIndex(int index)
        {
            switch (index)
            {
                case -1:
                    return LocalizedContentManager.CurrentLanguageLatin ? Color.White : new Color(86, 22, 12);
                case 1:
                    return Color.SkyBlue;
                case 2:
                    return Color.Red;
                case 3:
                    return new Color(110, 43, (int)byte.MaxValue);
                case 4:
                    return Color.White;
                case 5:
                    return Color.OrangeRed;
                case 6:
                    return Color.LimeGreen;
                case 7:
                    return Color.Cyan;
                case 8:
                    return new Color(60, 60, 60);
                default:
                    return Color.Black;
            }
        }
        public static bool DrawStringPrefix(SpriteText __instance)
        {
            try
            {
                SpriteText.shadowAlpha = 0;
                return true;
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(DrawStringPrefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
        public static bool DrawStringPrefix2(SpriteText __instance, string s, int color)
        {
            try
            {
                if (config.Debug)
                {
                    if (s != "")
                    {
                        Color clr = getColorFromIndex(color);
                        int key = $"DrawStringPrefix2{clr}{s}".GetHashCode();
                        string value = $"DrawStringPrefix2 : {clr} : {s}";
                        if (!debugBuffer.ContainsKey(key))
                        {
                            debugBuffer.Add(key, value);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(DrawStringPrefix2)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
        public static bool getColorFromIndexPrefix(SpriteText __instance, ref Color __result, int index)
        {
            try
            {
                Color color = getColorFromIndex(index);
                foreach (FixColor fixFontColor in config.FontColor)
                {
                    List<byte> target = fixFontColor.Target;
                    List<byte> change = fixFontColor.Change;

                    if (target.Count == 4 && change.Count == 4
                        && color.R == target[0] && color.G == target[1] && color.B == target[2] && color.A == target[3])
                    {
                        __result.R = change[0];
                        __result.G = change[1];
                        __result.B = change[2];
                        __result.A = change[3];
                        return false;
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                Log($"Failed in {nameof(getColorFromIndexPrefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
       public static bool DrawDialogueBoxPrefix(Game1 __instance)
        {
            try
            {
                Game1.textShadowColor = new Color(0, 0, 0, 0);
                return true;
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(DrawDialogueBoxPrefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
        public static void SetUpCharacterMapPreFix(SpriteText __instance)
        {
            try
            {
                var _characterMap = helper.Reflection.GetField<Dictionary<char, FontChar>>(typeof(SpriteText), "_characterMap").GetValue();

                if (!LocalizedContentManager.CurrentLanguageLatin
                    && _characterMap == null
                    && LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko)
                {
                    setUpCharacterMap = true;
                }
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(SetUpCharacterMapPreFix)}:\n{ex}", LogLevel.Error);
            }
        }
        public static void SetUpCharacterMapPostFix(SpriteText __instance)
        {
            try
            {
                if (setUpCharacterMap)
                {
                    SpriteText.fontPixelZoom = config.FontPixelZoom;
                    setUpCharacterMap = false;
                }
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(SetUpCharacterMapPostFix)}:\n{ex}", LogLevel.Error);
            }
        }
        public static void OnLanguageChangePostfix(SpriteText __instance)
        {
            try
            {
                SpriteText.fontPixelZoom = config.FontPixelZoom;
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(OnLanguageChangePostfix)}:\n{ex}", LogLevel.Error);
            }
        }
        public static void TranslateFieldsPostfix(Game1 __instance)
        {
            try
            {
                var lineSpace = config.SmallFontLineSpace;
                if (lineSpace > 0)
                {
                    Game1.smallFont.LineSpacing = lineSpace;
                }
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(TranslateFieldsPostfix)}:\n{ex}");
            }
        }
        public static void SpriteBatchDrawStringPrefix(Game1 __instance, ref Color color, string text)
        {
            try
            {
                if (config.Debug)
                {
                    int key = $"SpriteBatchDrawStringPrefix{color}{text}".GetHashCode();
                    string value = $"SpriteBatchDrawStringPrefix : {color} : {text}";
                    if (!debugBuffer.ContainsKey(key))
                    {
                        debugBuffer.Add(key, value);
                    }
                }
                foreach (FixColor fixFontColor in config.FontColor)
                {
                    List<byte> target = fixFontColor.Target;
                    List<byte> change = fixFontColor.Change;

                    if (target.Count == 4 && change.Count == 4
                        && color.R == target[0] && color.G == target[1] && color.B == target[2] && color.A == target[3])
                    {
                        color.R = change[0];
                        color.G = change[1];
                        color.B = change[2];
                        color.A = change[3];
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(SpriteBatchDrawStringPrefix)}:\n{ex}");
            }
        }
        public static void SpriteBatchDrawStringPrefix2(Game1 __instance, ref Color color, StringBuilder text)
        {
            try
            {
                if (config.Debug)
                {
                    int key = $"SpriteBatchDrawStringPrefix2{color}{text}".GetHashCode();
                    string value = $"SpriteBatchDrawStringPrefix2 : {color} : {text}";
                    if (!debugBuffer.ContainsKey(key))
                    {
                        debugBuffer.Add(key, value);
                    }
                }
                foreach (FixColor fixFontColor in config.FontColor)
                {
                    List<byte> target = fixFontColor.Target;
                    List<byte> change = fixFontColor.Change;

                    if (target.Count == 4 && change.Count == 4
                        && color.R == target[0] && color.G == target[1] && color.B == target[2] && color.A == target[3])
                    {
                        color.R = change[0];
                        color.G = change[1];
                        color.B = change[2];
                        color.A = change[3];
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(SpriteBatchDrawStringPrefix2)}:\n{ex}");
            }
        }
        public static void SpriteBatchDrawPrefix(Game1 __instance, ref Color color)
        {
            try
            {
                foreach (FixColor fixImgColor in config.ImgColor)
                {
                    List<byte> target = fixImgColor.Target;
                    List<byte> change = fixImgColor.Change;

                    if (target.Count == 4 && change.Count == 4
                        && color.R == target[0] && color.G == target[1] && color.B == target[2] && color.A == target[3])
                    {
                        color.R = change[0];
                        color.G = change[1];
                        color.B = change[2];
                        color.A = change[3];
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(SpriteBatchDrawPrefix)}:\n{ex}");
            }
        }
        static string CallStackLog()
        {
            var st = new System.Diagnostics.StackTrace(true);
            string callstack = "";
            foreach (var frame in st.GetFrames())
            {
                callstack += $"파일명:{frame.GetFileName()}, 라인:{frame.GetFileLineNumber()}, 함수명:{frame.GetMethod()}\n";
            }
            return callstack;
        }
        private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            foreach (KeyValuePair<int, string> buffer in debugBuffer)
            {
                Log($"{buffer.Value}");
            }
            debugBuffer.Clear();
        }
    }
}
