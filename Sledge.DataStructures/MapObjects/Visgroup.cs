﻿using System.Linq;
using System.Text;
using System.Drawing;
using Sledge.DataStructures.GameData;

namespace Sledge.DataStructures.MapObjects
{
    public class Visgroup
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Visible { get; set; }
        public Color Colour { get; set; }

        public Visgroup Clone()
        {
            return new Visgroup { ID = ID, Name = Name, Visible = Visible, Colour = Colour};
        }
    }
}
