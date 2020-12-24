using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddActorsToMainEvents
{
    class ModContent
    {
        public class Actor
        {
            public string name { get; set; } = "";
            public string target { get; set; } = "";
            public string layer { get; set; } = "";
            public int[] position { get; set; } = new int[2] { 0, 0 };
            public int direction { get; set; } = 0;
        }
        public string format { get; set; } = "1.0.0";
        public List<Actor> loadActors { get; set; } = new List<Actor>();
    }
}
