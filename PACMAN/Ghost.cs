using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PACMAN
{
    class Ghost : Brick
    {
        public Ghost()
        {
            pathData.Data = Geometry.Parse(
                    "M0,100 L0,100 C16,66 8,0 50,0 92,0 83,67 100,100 M100,100 C91,91 91,80 75,80 57,80 67,100 50,100 33,100 40,80 25,80 9,80 0,100 0,100"
                    );


            var eyesPath = new Path
            {
                Data = Geometry.Parse(
                    "M0,0 M100,100 M33,51 C41,51 47,45 47,44 47,43 41,36 33,36 26,36 19,42 19,44 18.764171,46 27,51 33,51 z M66,51 C74,51 80,45 80,44 80,42 74,36 66,36 59,36 51,43 51,44 51,45 60,51 66,51 z"
                    ),
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Black")),
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White")),
                StrokeThickness = 0.2,
                Stretch = Stretch.Fill
            };

            LayoutRoot.Children.Add(eyesPath);

            MoveRight();
        }

        public void MoveRight()
        {
            var rightMarginAnimation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                From = Margin,
                To = new Thickness(30, 0, -30, 0)
            };
            var sb = new Storyboard();

            sb.Children.Add(rightMarginAnimation);
            //sb.Completed += MovingRight_Completed;
            Storyboard.SetTarget(rightMarginAnimation, this);
            Storyboard.SetTargetProperty(rightMarginAnimation, new PropertyPath(MarginProperty));
            sb.Begin();
        }

        private void MovingRight_Completed(object sender, EventArgs e)
        {
            BattlefieldCircumstantials.MoveBattlefieldElement(1, 1, 1, 1);
        }
    }
}
