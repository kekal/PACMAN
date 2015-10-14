using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace PACMAN
{
    class YellowGhost : Ghost
    {
        public YellowGhost()
        {
            //"#FFE4FA1F
            Name = "YellowGhost";
            var brush = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFE4FA1F"));
            pathData.Stroke = brush;
            pathData.Fill = brush;

        }
    }
}
