using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using HarmonyLib;

namespace FixFontOption
{
    internal class SpriteFontOption
    {
        private static IModHelper? Helper;
        private static IMonitor? Monitor;
        private static bool Debug = false;
        private static bool HideShadow = false;
        private static bool DialogueLineSpaceEnabled = false;
        private static int DialogueLineSpaceValue = 42;
        private static bool SmallLineSpaceEnabled = false;
        private static int SmallLineSpaceValue = 26;

        public SpriteFontOption(IModHelper? helper = null, IMonitor? monitor = null)
        {
            Helper = helper;
            Monitor = monitor;
            if (Helper != null)
            {
                Helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            }
        }
        public void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.drawTextWithShadow), new Type[] { typeof(SpriteBatch), typeof(string), typeof(SpriteFont), typeof(Vector2), typeof(Color), typeof(float), typeof(float), typeof(int), typeof(int), typeof(float), typeof(int) }),
                prefix: new HarmonyMethod(typeof(SpriteFontOption), nameof(SpriteFontOption.DrawTextWithShadow_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.drawTextWithShadow), new Type[] { typeof(SpriteBatch), typeof(StringBuilder), typeof(SpriteFont), typeof(Vector2), typeof(Color), typeof(float), typeof(float), typeof(int), typeof(int), typeof(float), typeof(int) }),
                prefix: new HarmonyMethod(typeof(SpriteFontOption), nameof(SpriteFontOption.DrawTextWithShadow_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.drawTextWithColoredShadow)),
                prefix: new HarmonyMethod(typeof(SpriteFontOption), nameof(SpriteFontOption.DrawTextWithColoredShadow_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.TranslateFields)),
                postfix: new HarmonyMethod(typeof(SpriteFontOption), nameof(SpriteFontOption.TranslateFields_Postfix))
            );
        }
        private void GameLoop_SaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (HideShadow)
            {
                Game1.textShadowColor = new(0, 0, 0, 0);
            }
            else
            {
                Game1.textShadowColor = new(206, 156, 95, (int)byte.MaxValue);
            }
        }
        public void SetDebug(bool enabled)
        {
            Debug = enabled;
        }
        public void SetHideShadow(bool hide)
        {
            HideShadow = hide;
            if (HideShadow)
            {
                Game1.textShadowColor = new(0, 0, 0, 0);
            }
            else
            {
                Game1.textShadowColor = new(206, 156, 95, (int)byte.MaxValue);
            }
        }
        public void SetDialogueLineSpace(bool? enabled, int? value)
        {
            if (enabled.HasValue)
            {
                DialogueLineSpaceEnabled = enabled.Value;
            }
            if (value.HasValue)
            {
                DialogueLineSpaceValue = value.Value;
            }
        }
        public void SetSmallLineSpace(bool? enabled, int? value)
        {
            if (enabled.HasValue)
            {
                SmallLineSpaceEnabled = enabled.Value;
            }
            if (value.HasValue)
            {
                SmallLineSpaceValue = value.Value;
            }
        }
        private static void Log(string message, LogLevel level = LogLevel.Trace)
        {
            Monitor?.Log(message, level);
        }
        private static void TranslateFields_Postfix(Game1 __instance)
        {
            try
            {
                if (DialogueLineSpaceEnabled)
                {
                    Game1.dialogueFont.LineSpacing = DialogueLineSpaceValue;
                }
                if (SmallLineSpaceEnabled)
                {
                    Game1.smallFont.LineSpacing = SmallLineSpaceValue;
                }
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(TranslateFields_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
        private static bool DrawTextWithShadow_Prefix(Utility __instance, ref float shadowIntensity)
        {
            try
            {
                if (HideShadow)
                {
                    shadowIntensity = 0;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(DrawTextWithShadow_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
        private static bool DrawTextWithColoredShadow_Prefix(Utility __instance, ref Color shadowColor)
        {
            try
            {
                if (HideShadow)
                {
                    shadowColor *= 0f;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(DrawTextWithColoredShadow_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
    }
}
