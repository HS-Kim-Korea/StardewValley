using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace TranslateTemporaryActor
{
    class ModData
    {
        public string Format { get; set; } = "1.0.0";
        public class NPCName
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
            public string Name { get; set; } = "";
            public Localization NameLocalization { get; set; } = new Localization();
        }
        public List<NPCName> NPCNames { get; set; } = new List<NPCName>();
    }
}
