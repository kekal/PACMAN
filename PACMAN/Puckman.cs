using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PACMAN
{
    enum HorisontalDirection
    {
        Stay = 0,
        Left = -1,
        Right = 1
    }

    enum VerticalDirection
    {
        Stay = 0,
        Up = -1,
        Down = 1,
    }

    class Puckman : Ghost
    {
        private double _defaultSpeed;

        private HorisontalDirection _currentHorisontalDirection;
        private VerticalDirection _currentVerticalDirection;

        private HorisontalDirection _newHorisontalDirection;
        private VerticalDirection _newVerticalDirection;


        public Puckman()
        {
            //Visibility = Visibility.Hidden;
            _defaultSpeed = 1.0 / 200;
            _newHorisontalDirection = HorisontalDirection.Stay;
            _newVerticalDirection = VerticalDirection.Stay;

            Name = "Puckman";
            Width = BattlefieldCircumstantials.Squaresize;
            Height = BattlefieldCircumstantials.Squaresize;
            SnapsToDevicePixels = true;

            LayoutRoot.Children.Add(new Ellipse
            {
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Yellow"))
            });

            ChawAnimation();
        }

        private void ChawAnimation()
        {

            pathData.StrokeThickness = 0;
            pathData.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Black"));
            pathData.Data = Geometry.Parse("M0,0 M100,100 M50,50 L101,25 101,75");
            pathData.RenderTransformOrigin = new Point(0.5, 0.5);

            Panel.SetZIndex(pathData, 1);
            Panel.SetZIndex(this, 1);

            var translation = new ScaleTransform(1, 1);
            var translationName = "Chawing" + translation.GetHashCode();
            RegisterName(translationName, translation);
            pathData.RenderTransform = translation;

            var anim = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(200)))
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

        public void MoveDecision()
        {
            var currentCoordinates = BattlefieldCircumstantials.GetCoordinates(this);

            var newTarget = BattlefieldCircumstantials.FieldElementsArray[
                (int) (currentCoordinates.X) + (int) _newHorisontalDirection,
                (int) (currentCoordinates).Y + (int) _newVerticalDirection];

            var oldTarget = BattlefieldCircumstantials.FieldElementsArray[
                (int) (currentCoordinates.X) + (int) _currentHorisontalDirection,
                (int) (currentCoordinates).Y + (int) _currentVerticalDirection];


            if (newTarget == null)
            {
                _currentHorisontalDirection = _newHorisontalDirection;
                _currentVerticalDirection = _newVerticalDirection;
            }
            else if (oldTarget == null)
            {
                
            }
            else if (newTarget.GetType() == typeof(Boost))
            {
                _currentHorisontalDirection = _newHorisontalDirection;
                _currentVerticalDirection = _newVerticalDirection;
                ApplyBoost((ushort)(currentCoordinates.X + (int)_newHorisontalDirection), (ushort)(currentCoordinates.Y + (int)_newVerticalDirection));

            }
            else if (oldTarget.GetType() == typeof(Boost))
            {
                ApplyBoost((ushort)(currentCoordinates.X + (int)_currentHorisontalDirection), (ushort)(currentCoordinates.Y + (int)_currentVerticalDirection));
            }

            else
            {
                _currentHorisontalDirection = HorisontalDirection.Stay;
                _currentVerticalDirection = VerticalDirection.Stay;
            }

            if (double.IsNaN(Canvas.GetLeft(this)) || double.IsNaN(Canvas.GetTop(this))) return;

            var newPoint = new Point(currentCoordinates.X + (int)_currentHorisontalDirection,
                currentCoordinates.Y + (int)_currentVerticalDirection);

            CreatureMovement(newPoint);
        }

        private void ApplyBoost(ushort x, ushort y)
        {
            BattlefieldCircumstantials.FieldElementsArray[x, y].Visibility = Visibility.Collapsed;

            BattlefieldCircumstantials.RemoveElementFromField(x, y);
            _defaultSpeed *= 2;
            var tempTimer = new Timer(3000)
            {
                AutoReset = false
            
            };
            tempTimer.Elapsed += delegate
            {
                _defaultSpeed /= 2;
            };
            tempTimer.Enabled = true;

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

            var dur = new Duration(TimeSpan.FromMilliseconds(1/_defaultSpeed));

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

            sb.Name = "GhostAnimation";
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

        public void MoveDefine(KeyEventArgs args)
        {
            _newHorisontalDirection = HorisontalDirection.Stay;
            _newVerticalDirection = VerticalDirection.Stay;
            switch (args.Key)
            {
                case Key.Up:
                    //MessageBox.Show("Up");
                    _newVerticalDirection = VerticalDirection.Up;
                    break;

                case Key.Down:
                    //MessageBox.Show("Down");
                    _newVerticalDirection = VerticalDirection.Down;
                    break;

                case Key.Left:
                    //MessageBox.Show("Left");
                    _newHorisontalDirection = HorisontalDirection.Left;
                    break;

                case Key.Right:
                    //MessageBox.Show("Right");
                    _newHorisontalDirection = HorisontalDirection.Right;
                    break;
            }


        }
    }
}
