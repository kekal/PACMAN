using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PACMAN
{
    class Boost : Brick
    {
        internal Boost()
        {
            LayoutRoot.RenderTransformOrigin = new Point(0.5, 0.5);
            LayoutRoot.RenderTransform = new ScaleTransform(1, 1.27);

            Width = BattlefieldCircumstantials.Squaresize;
            Height = BattlefieldCircumstantials.Squaresize;
            

            PathData.Name="Sticks";
            PathData.Data =
                Geometry.Parse(
                    "M0,0 M215,274 M62.9,161.7 C62.9,161.7 143.915,98.666 148.5,68.1 153.085,37.533998 181.752,152.988 162.9,183.3");
            PathData.HorizontalAlignment = HorizontalAlignment.Left;
            PathData.VerticalAlignment = VerticalAlignment.Top;
            PathData.Stretch = Stretch.Fill;
            PathData.RenderTransformOrigin = new Point(0.5, 0.5);
            PathData.Stroke = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF7AA20D"));
            PathData.SnapsToDevicePixels = true;
            PathData.StrokeStartLineCap = PenLineCap.Round;
            PathData.StrokeEndLineCap = PenLineCap.Round;


            var leaf = new Path
            {
                Name = "Sticks",
                Data = Geometry.Parse(
                    "M0,0 M215,274 M139.743,60.227 C138.401,54.149 140.147,47.303 145.505,40.546 156.684,26.448 169.087,23.007 186.3,22.7 186.307,22.7 208.415,22.304 213.014,2.669 210.155,14.874 207.434,26.837 203.24,38.714 196.423,58.015 180.841,87.241 155.609,76.742 146.819,73.085 141.273,67.156 139.743,60.227 z"),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Stretch = Stretch.Fill,
                RenderTransformOrigin = new Point(0.5, 0.5),
                Fill = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF7AA20D")),
                SnapsToDevicePixels = true,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };

            var rCherry = new Path
            {
                Name = "RigthtCherr",
                Data = Geometry.Parse(
                    "M0,0 M215,274 M163.351,178.548 C165.105,178.428 166.96,178.063 169,177.4 177.512,174.632 190.185,170.415 197.181,180.552 212.257,202.397 202.945,236.428 178.794,247.075 151.184,259.247 119.832,230.128 122.411,202.07 123.677,188.291 130.574,166.12 148.547,174.592 153.975,177.15 158.23,178.897 163.351,178.548 z"),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Stretch = Stretch.Fill,
                RenderTransformOrigin = new Point(0.5, 0.5),
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFED6E46")),
                SnapsToDevicePixels = true,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };

            var lCherry = new Path
            {
                Name = "LeftCherr",
                Data = Geometry.Parse(
                    "M0,0 M215,274 M67.237,158.065 C68.973,158.338 70.863,158.393 73,158.2 81.914,157.391 95.206999,156.092 99.777999,167.529 109.629,192.176 92.994002,223.29 67.082,228.309 37.459,234.048 13.354,198.695 22.097,171.91 26.391,158.756 38.037,138.669 53.681,150.92 58.405,154.621 62.166,157.268 67.237,158.065 z"),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Stretch = Stretch.Fill,
                RenderTransformOrigin = new Point(0.5, 0.5),
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFED6E46")),
                SnapsToDevicePixels = true,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };

            LayoutRoot.Children.Add(leaf);
            LayoutRoot.Children.Add(rCherry);
            LayoutRoot.Children.Add(lCherry);

            Panel.SetZIndex(this, 1);
        }
    }
}
