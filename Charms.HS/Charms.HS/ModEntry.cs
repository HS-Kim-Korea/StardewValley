using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Charms.HS
{
    public class ModEntry : Mod
    {
        public static Mod instance;
        internal static IModHelper helper;
        internal ModConfig config;
        internal Dictionary<string, ModContent.Life> lives = new Dictionary<string, ModContent.Life>();

        public override void Entry(IModHelper helper)
        {
            ModEntry.instance = this;
            ModEntry.helper = helper;

            config = this.Helper.ReadConfig<ModConfig>();
            if(config.enable)
            {
                Log($"Enabled Charms", LogLevel.Info);
                GetContentPacks();
                helper.Events.GameLoop.UpdateTicking += onUpdateTicking;
                //helper.Events.GameLoop.OneSecondUpdateTicking += onOneSecondUpdateTickingEventArgs;
            }
        }
        public static void Log(string text, LogLevel level = LogLevel.Debug)
        {
            instance.Monitor.Log(text, level);
        }
        private void GetContentPacks()
        {
            foreach(IContentPack contentPack in helper.ContentPacks.GetOwned())
            {
                DirectoryInfo dir = new DirectoryInfo(contentPack.DirectoryPath);
                if (dir.Exists)
                {
                    Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version}", LogLevel.Info);
                    ModContent content = contentPack.ReadJsonFile<ModContent>("charms.json") ?? new ModContent();
                    Version format = new Version(content.format);
                    if(format.CompareTo(new Version("1.0.0")) == 0)
                    {
                        foreach(ModContent.Life life in content.lives)
                        {
                            string key = life.name;
                            if(!lives.ContainsKey(key))
                            {
                                Log($"Attached charm : {key}", LogLevel.Info);
                                lives.Add(key, life);
                            }
                            else
                            {
                                Log($"Duplicated charm : {key}", LogLevel.Warn);
                            }
                        }
                    }
                }
            }
        }
        private void onUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            // Death
            if (Game1.player.health <= 0)
            {
                foreach(KeyValuePair<string, ModContent.Life> life in lives)
                {
                    bool exist = Game1.player.hasItemInInventoryNamed(life.Key);
                    if(exist)
                    {
                        int health = life.Value.health > 0 ? life.Value.health : 1;
                        int yobablessing = life.Value.yobablessing;

                        // heal
                        Game1.player.health = health;

                        // buff
                        if (yobablessing > 0)
                        {
                            Buff buff = new Buff(Buff.yobaBlessing);
                            buff.millisecondsDuration = yobablessing * 1000;
                            Game1.buffsDisplay.addOtherBuff(buff);
                        }

                        // remove item
                        removeItemFromInventory(life.Key);
                        Game1.playSound("flameSpellHit");
                        break;
                    }
                }
            }
        }
        private void onOneSecondUpdateTickingEventArgs(object sender, OneSecondUpdateTickingEventArgs e)
        {
            // for debug
            bool exist = Game1.player.hasItemInInventoryNamed("Guardian Stone");
            Log($"onOneSecondUpdateTickingEventArgs : {exist}");
            if(exist)
            {
                Item item = Game1.player.hasItemWithNameThatContains("Guardian Stone");
                int stack = 1;
                if(item.Stack > stack)
                {
                    item.Stack -= stack;
                }
                else
                {
                    Game1.player.removeItemFromInventory(item);
                }
            }
            
        }
        private void removeItemFromInventory(string name, int stack = 1)
        {
            Item item = Game1.player.hasItemWithNameThatContains("Guardian Stone");
            if(item != null)
            {
                if(item.Stack > stack)
                {
                    item.Stack -= stack;
                }
                else
                {
                    Game1.player.removeItemFromInventory(item);
                }
            }
        }
    }
}
