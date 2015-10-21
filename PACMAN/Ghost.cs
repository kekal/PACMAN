using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Path = System.Windows.Shapes.Path;

namespace PACMAN
{
    class Ghost : Brick
    {
        private double _defaultSpeed;
        
        internal Ghost()
        {
            _defaultSpeed = 1.0 / 300;

            SnapsToDevicePixels = true;
            PathData.Data = Geometry.Parse(
                    "M0,100 L0,100 C16,66 8,0 50,0 92,0 83,67 100,100 M100,100 C91,91 91,80 75,80 57,80 67,100 50,100 33,100 40,80 25,80 9,80 0,100 0,100"
                    );

            var eyesPath = new Path
            {
                Data = Geometry.Parse(
                    "M0,0 M100,100 M33,51 C41,51 47,45 47,44 47,43 41,36 33,36 26,36 19,42 19,44 18.764171,46 27,51 33,51 z M66,51 C74,51 80,45 80,44 80,42 74,36 66,36 59,36 51,43 51,44 51,45 60,51 66,51 z"
                    ),
                Stroke = new SolidColorBrush(BattlefieldCircumstantials.BlackColor),
                Fill = new SolidColorBrush(BattlefieldCircumstantials.WhiteColor),
                StrokeThickness = 0.2,
                Stretch = Stretch.Fill
            };

            var pupil1 = new Ellipse
            {
                SnapsToDevicePixels = false,
                Fill = new SolidColorBrush(BattlefieldCircumstantials.BlackColor),
                StrokeThickness = 0,
                Width = 1.5,
                Height = 1.5,
                Margin = new Thickness(0, 0, 10, 4),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Stretch = Stretch.Fill
            };

            var pupil2 = new Ellipse
            {
                SnapsToDevicePixels = false,
                Fill = new SolidColorBrush(BattlefieldCircumstantials.BlackColor),
                StrokeThickness = 0,
                Width = 1.5,
                Height = 1.5,
                Margin = new Thickness(9.5, 0, 0, 4),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Stretch = Stretch.Fill
            };

            LayoutRoot.Children.Add(eyesPath);
            LayoutRoot.Children.Add(pupil1);
            LayoutRoot.Children.Add(pupil2);

            
            Panel.SetZIndex(this, 1);

            #region ================movement descriptors====================
            //var leftDescriptor = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(Canvas));
            //leftDescriptor.AddValueChanged(this, delegate
            //{
            //    if ((ushort)(Math.Abs(Canvas.GetLeft(this) % BattlefieldCircumstantials.Squaresize)) < 2)
            //    {
            //        //moveDecision();
            //    }
            //});

            //var topDescriptor = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(Canvas));
            //topDescriptor.AddValueChanged(this, delegate
            //{
            //    if ((ushort)(Math.Abs(Canvas.GetTop(this) % BattlefieldCircumstantials.Squaresize)) < 2)
            //    {
            //        //moveDecision();
            //    }
            //});
            #endregion
        }

        private void CreatureMovement(Point goal)
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

        internal void MoveDecision()
        {
            if (double.IsNaN(Canvas.GetLeft(this)) || double.IsNaN(Canvas.GetTop(this))) return;
            
            if (BattlefieldCircumstantials.FindDirectDistance(this, BattlefieldCircumstantials.Puckman) < 40)
            {
                MainWindow.Wm.PlayDeath();
                return;
            }

            var currentCoordinates = BattlefieldCircumstantials.GetCoordinates(this);
            var puckmanCoordinates = BattlefieldCircumstantials.GetCoordinates(BattlefieldCircumstantials.Puckman);


            var ghostWay = new PathFind(currentCoordinates, puckmanCoordinates);

            if (ghostWay.Path.Count > 1)
            {
                CreatureMovement(ghostWay.Path[0]);
            }
            else
            {
                foreach (var ghost in BattlefieldCircumstantials.GhostsList.Where(ghost => ghost != this))
                {
                    ghostWay = new PathFind(currentCoordinates, BattlefieldCircumstantials.GetCoordinates(ghost));
                    if (ghostWay.Path.Count > 1)
                    {
                        CreatureMovement(ghostWay.Path[0]);
                        return;
                    }
                }
                CreatureMovement(currentCoordinates);
            }
        }

        internal void Stop()
        {
            _defaultSpeed = 1.0 / 1000000;
        }
    }
}
