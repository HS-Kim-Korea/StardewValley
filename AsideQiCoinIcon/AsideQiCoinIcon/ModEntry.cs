using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Minigames;

namespace AsideQiCoinIcon
{
    public class ModEntry : Mod
    {
        public static Mod instance;
        internal static IModHelper helper;
        private ModConfig config;
        private string pad;
        private IMinigame lastgame;
        public override void Entry(IModHelper helper)
        {
            ModEntry.instance = this;
            ModEntry.helper = helper;
            this.config = this.Helper.ReadConfig<ModConfig>();
            if(this.config.enable)
            {
                helper.Events.Player.Warped += this.onWarped;
                helper.Events.Display.Rendering += onRendering;
                pad = "".PadLeft(Math.Abs(this.config.buffer), ' ');
                lastgame = null;
                string msg = string.Format("Enabled");
                log(msg);
            }
        }
        private void onRendering(object sender, RenderingEventArgs e)
        {
            IMinigame curgame = Game1.currentMinigame;

            if(curgame != lastgame)
            {
                List<string> gamelist = new List<string> { "Slots", "CalicoJack" };
                if(curgame != null && gamelist.Contains(curgame.minigameId()))
                {
                    // 켜짐
                    IReflectedField<string> coinBuffer = this.Helper.Reflection.GetField<string>(curgame, "coinBuffer");
                    coinBuffer.SetValue(pad);
                    string msg = string.Format("StartGame");
                    log(msg);
                }
                lastgame = curgame;
            }
        }
        private void onWarped(object sender, WarpedEventArgs e)
        {
            GameLocation location = e.NewLocation;
            if(location.Name == "Club")
            {
                IReflectedField<string> coinBuffer = this.Helper.Reflection.GetField<string>(location, "coinBuffer");
                coinBuffer.SetValue(pad);
                string s = string.Format("onWarped - Old:{0}, New:{1}", e.OldLocation.Name, e.NewLocation.Name);
                log(s);
            }
        }
        public static void log(string text)
        {
            instance.Monitor.Log(text, LogLevel.Debug);
        }

    }
}
