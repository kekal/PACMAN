using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Path = System.Windows.Shapes.Path;

namespace PACMAN
{
    class Ghost : Brick
    {
        public Point previousPosition;
        public DependencyPropertyDescriptor leftDescriptor;
        public DependencyPropertyDescriptor rightDescriptor;
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

            leftDescriptor = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(Canvas));
            leftDescriptor.AddValueChanged(this, delegate
            {
                if ((int)(Math.Abs(Canvas.GetLeft(this) / BattlefieldCircumstantials.Squaresize)) < 2)
                {
                    //moveDecision();
                }
            });

            rightDescriptor = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(Canvas));
            rightDescriptor.AddValueChanged(this, delegate
            {
                if ((int)(Math.Abs(Canvas.GetTop(this) / BattlefieldCircumstantials.Squaresize)) < 2)
                {
                    //moveDecision();
                }
            });
        }


        public void CheckMovementCommand(int x, int y)
        {
            var square = BattlefieldCircumstantials.Squaresize;

            if (Math.Abs(previousPosition.X - Canvas.GetLeft(this) / square) < 2 &&
                Math.Abs(previousPosition.Y - Canvas.GetTop(this) / square) < 2) return;


            var size = BattlefieldCircumstantials.Size;


            if (Math.Abs(x) + Math.Abs(y) > 1 ||
                Canvas.GetLeft(this) / square + x >= size ||
                Canvas.GetTop(this) / square + y >= size)
            {
                MessageBox.Show(GetType() + " trying to move to the (" + x + ", " + y + ").");
                throw new ArgumentOutOfRangeException();
            }

            GhostMovement(x, y);

        }

        public void GhostMovement(int x, int y)
        {
            var square = BattlefieldCircumstantials.Squaresize;

            var left = Canvas.GetLeft(this);
            var xMovementAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(1 / _defaultSpeed)),
                From = left,
                To = left + x * square
            };

            var top = Canvas.GetTop(this);
            var yMovementAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(1 / _defaultSpeed)),
                From = top,
                To = top + y * square
            };

            var sb = new Storyboard();
            //sb.Duration = new Duration(TimeSpan.FromMilliseconds(1 / _defaultSpeed));

            sb.Children.Add(xMovementAnimation);
            sb.Children.Add(yMovementAnimation);
            sb.Completed +=
                delegate
                {
                    BattlefieldCircumstantials.MoveBattlefieldElement(
                        (ushort)(left / square),
                        (ushort)(top / square),
                        (ushort)(left / square + x),
                        (ushort)(top / square + y));

                    previousPosition = new Point(left / square + x, top / square + y);

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


            CheckMovementCommand(0, 1);
        }
    }



    internal class AlreadyInstatiated : Exception
    {

    }
}
