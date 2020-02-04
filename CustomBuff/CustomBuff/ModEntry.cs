using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CustomBuff
{
    public class ModEntry : Mod
    {
        public static Mod instance;
        internal static IModHelper helper;
        private const int startWhich = 70000;
        private ModConfig config;
        private ModData data;
        public override void Entry(IModHelper helper)
        {
            ModEntry.instance = this;
            ModEntry.helper = helper;
            this.config = this.Helper.ReadConfig<ModConfig>();
            if(this.config.Enable)
            {
                log(String.Format("Enabled custom buff"));
                this.data = this.Helper.Data.ReadJsonFile<ModData>("assets\\buff.json") ?? new ModData();
                log(String.Format("Skill count : {0}", data.Skills.Count.ToString()));
                helper.Events.Input.ButtonPressed    += onButtonPressed;    // trigger
                helper.Events.GameLoop.UpdateTicking += onUpdateTicking;    // update
            }
        }
        public static void log(string text, LogLevel level=LogLevel.Debug)
        {
            instance.Monitor.Log(text, level);
        }
        public static Color GetColorFromName(string name)
        {
            var prop = typeof(Color).GetProperty(name);
            if (prop != null)
                return (Color)prop.GetValue(null, null);
            return default(Color);
        }
        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
        private void onButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Game1.player.canMove || Game1.activeClickableMenu != null)
            {
                return;
            }

            int skillIndex = 0;
            foreach (ModData.Skill skill in data.Skills)
            {
                ++skillIndex;
                // enable
                if(!skill.Enable)
                {
                    continue;
                }

                // check key condition
                bool keyCondition = false;
                foreach (SButton key in skill.KeyBind)
                {
                    if(!this.Helper.Input.IsDown(key))
                    {
                        keyCondition = false;
                        break;
                    }
                    if(e.Button == key)
                    {
                        keyCondition = true;
                    }
                }
                if(!keyCondition)
                {
                    continue;
                }

                // find buff
                if(Game1.buffsDisplay.otherBuffs.Find(x => x.which == (startWhich + skillIndex)) != null)
                {
                    continue;
                }

                // not enough resources
                if ((Game1.player.health + skill.Health <= 0)
                    || (Game1.player.Stamina + skill.Stamina < 0))
                {
                    Game1.playSound("bob");
                    continue;
                }

                string displayName = GetPropValue(skill.NameLocalization, helper.Translation.Locale.Substring(0, 2)).ToString();
                displayName = (displayName == "") ? skill.Name : displayName;
                Buff buff = new Buff(
                    skill.Farming,
                    skill.Fishing,
                    skill.Mining,
                    skill.Digging,
                    skill.Luck,
                    skill.Foraging,
                    skill.Crafting,
                    skill.MaxStamina,
                    skill.MagneticRadius,
                    skill.Speed,
                    skill.Defense,
                    skill.Attack,
                    1,
                    skill.Name,
                    displayName);
                buff.millisecondsDuration = skill.Duration * 1000;
                buff.which = startWhich + skillIndex;
                Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + skill.Stamina);
                Game1.player.health = Math.Min(Game1.player.maxHealth, Game1.player.health + skill.Health);
                if (skill.ColorEffect != "")
                {
                    buff.glow = GetColorFromName(skill.ColorEffect);
                }
                if(skill.SoundEffect != "")
                {
                    Game1.playSound(skill.SoundEffect);
                }
                Game1.buffsDisplay.addOtherBuff(buff);
            }
        }
        private void onUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            if (!Game1.player.canMove || Game1.activeClickableMenu != null)
            {
                return;
            }

            int skillIndex = 0;
            foreach (ModData.Skill skill in data.Skills)
            {
                ++skillIndex;
                // enable
                if (!skill.Enable)
                {
                    continue;
                }

                // find buff
                foreach(Buff buff in Game1.buffsDisplay.otherBuffs)
                {
                    // found
                    if (buff.which == startWhich + skillIndex)
                    {
                        // key
                        bool keyCondition = true;
                        foreach (SButton key in skill.KeyBind)
                        {
                            if (!this.Helper.Input.IsDown(key))
                            {
                                keyCondition = false;
                                break;
                            }
                        }
                        if (keyCondition && buff.millisecondsDuration <= 100
                            && (Game1.player.health + skill.Health > 0)
                            && (Game1.player.Stamina + skill.Stamina >= 0))
                        {
                            buff.millisecondsDuration = skill.Duration * 1000;
                            Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + skill.Stamina);
                            Game1.player.health = Math.Min(Game1.player.maxHealth, Game1.player.health + skill.Health);
                        }
                    }
                }
            }
        }
    }
}
