using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wetland
{
    internal class ModConfig
    {
        public class WaterSource
        {
            public string name { get; set; } = "";
            public int range { get; set; } = 5;
        }
        public bool enable { get; set; } = true;
        public int range { get; set; } = 5;
        public List<WaterSource> waterSources { get; set; } = new List<WaterSource>();
    }
}
