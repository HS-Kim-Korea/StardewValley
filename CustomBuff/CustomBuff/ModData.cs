using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace CustomBuff
{
    class ModData
    {
        public class Skill
        {
            public class Localization
            {
                public string zh { get; set; } = "";
                public string fr { get; set; } = "";
                public string de { get; set; } = "";
                public string hu { get; set; } = "";
                public string it { get; set; } = "";
                public string ja { get; set; } = "";
                public string ko { get; set; } = "";
                public string pt { get; set; } = "";
                public string ru { get; set; } = "";
                public string es { get; set; } = "";
                public string tr { get; set; } = "";
            }
            public bool Enable { get; set; } = true;
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public Localization NameLocalization { get; set; } = new Localization();
            public Localization DescriptionLocalization { get; set; } = new Localization();
            public List<SButton> KeyBind { get; set; } = new List<SButton>();
            public string SoundEffect { get; set; } = "";
            public string ColorEffect { get; set; } = "";
            public int Duration { get; set; } = 1;
            public Single Stamina { get; set; } = 0;
            public int Health { get; set; } = 0;
            public int Farming { get; set; } = 0;
            public int Fishing { get; set; } = 0;
            public int Mining { get; set; } = 0;
            public int Digging { get; set; } = 0;
            public int Luck { get; set; } = 0;
            public int Foraging { get; set; } = 0;
            public int Crafting { get; set; } = 0;
            public int MaxStamina { get; set; } = 0;
            public int MagneticRadius { get; set; } = 0;
            public int Speed { get; set; } = 0;
            public int Defense { get; set; } = 0;
            public int Attack { get; set; } = 0;
        }
        public string Format { get; set; } = "1.0.0";
        public List<Skill> Skills { get; set; } = new List<Skill>();
    }
}
