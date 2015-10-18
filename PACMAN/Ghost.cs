using System;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Path = System.Windows.Shapes.Path;
using Timer = System.Timers.Timer;

namespace PACMAN
{
    class Ghost : Brick
    {
        private Point _previousPosition;
        private readonly double _defaultSpeed;

        public Ghost()
        {
            _defaultSpeed = 1.0 / 500;

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

            Panel.SetZIndex(this, 1);

            #region ================movement descriptors====================
            var leftDescriptor = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(Canvas));
            leftDescriptor.AddValueChanged(this, delegate
            {
                if ((ushort)(Math.Abs(Canvas.GetLeft(this) % BattlefieldCircumstantials.Squaresize)) < 2)
                {
                    //moveDecision();
                }
            });

            var rightDescriptor = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(Canvas));
            rightDescriptor.AddValueChanged(this, delegate
            {
                if ((ushort)(Math.Abs(Canvas.GetTop(this) % BattlefieldCircumstantials.Squaresize)) < 2)
                {
                    //moveDecision();
                }
            });
            #endregion
        }

        public void GhostMovement(Point goal)
        {
            const ushort square = BattlefieldCircumstantials.Squaresize;

            var oldDiscreteX = Canvas.GetLeft(this) / square;
            var oldDiscreteY = Canvas.GetTop(this) / square;

            var oldX = Canvas.GetLeft(this);
            var oldY = Canvas.GetTop(this);

            var newX = goal.X * square;
            var newY = goal.Y * square;

            var newDiscreteX = goal.X;
            var newdiscreteY = goal.Y;

            var dur = new Duration(TimeSpan.FromMilliseconds(1 / _defaultSpeed));

            var xMovementAnimation = new DoubleAnimation
            {
                Duration = dur,
                From = oldX,
                To = newX
            };

            var yMovementAnimation = new DoubleAnimation
            {
                Duration = dur,
                From = oldY,
                To = newY
            };
            _previousPosition = new Point(oldDiscreteX, oldDiscreteY);

            BattlefieldCircumstantials.MoveBattlefieldElement(
                (ushort)Math.Round(oldDiscreteX),
                (ushort)Math.Round(oldDiscreteY),
                (ushort)Math.Round(newDiscreteX),
                (ushort)Math.Round(newdiscreteY));

            var sb = new Storyboard();
            

            sb.Children.Add(xMovementAnimation);
            sb.Children.Add(yMovementAnimation);
            sb.Completed +=
                delegate
                {
                    MoveDecision();
                };

            Storyboard.SetTarget(xMovementAnimation, this);
            Storyboard.SetTarget(yMovementAnimation, this);
            Storyboard.SetTargetProperty(xMovementAnimation, new PropertyPath("(Canvas.Left)"));
            Storyboard.SetTargetProperty(yMovementAnimation, new PropertyPath("(Canvas.Top)"));

            sb.Begin();
        }

        public void MoveDecision()
        {
            if (double.IsNaN(Canvas.GetLeft(this)) || double.IsNaN(Canvas.GetTop(this))) return;

            var ghostWay = new PathFind(BattlefieldCircumstantials.getCoordinates(this),
                new Point(BattlefieldCircumstantials._xEntrance, BattlefieldCircumstantials._yEntrance));

            if (ghostWay.Path.Count > 1)
            {
                GhostMovement(ghostWay.Path[0]);
            } else
            {
                var timer = new Timer(500)
                {
                    AutoReset = false
                };
                timer.Elapsed += Maketry;
                timer.Enabled = true;
            }

        }

        private void Maketry(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            
            MoveDecision();
            
        }

    }



    internal class AlreadyInstatiated : Exception
    {

    }
}
