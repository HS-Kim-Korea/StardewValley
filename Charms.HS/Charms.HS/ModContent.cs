using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charms.HS
{
    class ModContent
    {
        public class Life
        {
            public string name { get; set; } = "";
            public int health { get; set; } = 10;
            public int yobablessing { get; set; } = 2;
        }
        public string format { get; set; } = "1.0.0";
        public List<Life> lives { get; set; } = new List<Life>();
    }
}
