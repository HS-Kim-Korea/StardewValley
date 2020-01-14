using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using SFarmer = StardewValley.Farmer;
using PyTK.CustomTV;

namespace RestoreChannelName
{
    public class ModEntry : Mod
    {
        public static Mod instance;
        internal static IModHelper helper;
        private ModConfig config;

        private string weatherString;
        private string fortuneString;
        private string queenString;
        private string landString;
        private string rerunString;

        /*
         * Public methods
         */
        /// <summary>
        /// The mode entry point, called after the mod is first loaded.
        /// </summary>
        /// <param name="helper">Provides simlified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModEntry.instance = this;
            ModEntry.helper = helper;
            this.config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.SaveLoaded += onGameSaveLoaded;
            
        }

        /*
         * Private methods
         */
         /// <summary>
         /// 채널 정보 복구 
         /// </summary>
        private void Restore()
        {
            log("Changed channel name");
            weatherString = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13105");
            fortuneString = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13107");
            queenString = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13114");
            landString = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13111");
            rerunString = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13117");

            CustomTVMod.addChannel("weather", weatherString, showOriginalProgram);
            CustomTVMod.addChannel("fortune", fortuneString, showOriginalProgram);
            CustomTVMod.addChannel("land", landString, showOriginalProgram);
            CustomTVMod.addChannel("queen", queenString, showOriginalProgram);
            CustomTVMod.addChannel("rerun", rerunString, showOriginalProgram);
        }
        private static void showOriginalProgram(TV tv, TemporaryAnimatedSprite sprite, SFarmer who, string a)
        {
            switch (a)
            {
                case "weather": a = "Weather"; break;
                case "queen": a = "The"; break;
                case "rerun": a = "The"; break;
                case "land": a = "Livin'"; break;
                case "fortune": a = "Fortune"; break;
                default: a = "Weather"; break;
            }
            tv.selectChannel(who, a);

        }

        /*
         * Events
         */
        /// <summary>
        /// 게임 시작
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onGameSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (this.config.RestoreChannelName)
                Restore();
        }

        /*
         * Utils
         */
        public static void log(string text)
        {
            instance.Monitor.Log(text, LogLevel.Debug);
        }
    }
}
