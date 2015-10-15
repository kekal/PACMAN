using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PACMAN
{
    class Puckman : Brick
    {
        private double _defaultSpeed;

        public Puckman()
        {
            _defaultSpeed = 1.0 / 500;

            Name = "Puckman";
            Width = BattlefieldCircumstantials.Squaresize;
            Height = BattlefieldCircumstantials.Squaresize;

            LayoutRoot.Children.Add(new Ellipse
            {
                Fill = new SolidColorBrush((Color) ColorConverter.ConvertFromString("Yellow"))
            });

            pathData.StrokeThickness = 0;
            pathData.Fill = new SolidColorBrush((Color) ColorConverter.ConvertFromString("Black"));
            pathData.Data = Geometry.Parse(
                    "M0,0 M100,100 M50,50 L101,25 101,75"
                    );
            pathData.RenderTransformOrigin = new Point(0.5, 0.5);

            Panel.SetZIndex(pathData, 1);
            Panel.SetZIndex(this, 1);

            ChawAnimation();
        }

        private void ChawAnimation()
        {
            var translation = new ScaleTransform(1, 1);
            var translationName = "Chawing" + translation.GetHashCode();
            RegisterName(translationName, translation);
            pathData.RenderTransform = translation;



            var anim = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 200)))
            {
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseIn },
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = true
            };

            var sb = new Storyboard();
            sb.Children.Add(anim);

            Storyboard.SetTargetName(sb, translationName);
            Storyboard.SetTargetProperty(sb, new PropertyPath(ScaleTransform.ScaleYProperty));
            var storyboardName = "s" + sb.GetHashCode();
            Resources.Add(storyboardName, sb);

            sb.Begin();

        }
    }
}
