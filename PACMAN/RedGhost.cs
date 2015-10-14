using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace PACMAN
{
    class RedGhost : Ghost
    {
        public RedGhost()
        {
            //"#FFE4FA1F
            Name = "RedGhost";
            var brush = new SolidColorBrush((Color) ColorConverter.ConvertFromString("Red"));
            pathData.Stroke = brush;
            pathData.Fill = brush;

        }
    }
}
