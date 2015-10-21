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

    enum targetType
    {
        Null,
        Brick,
        Cherry,
        Exit,
        Entrance,
        Error
    }

    class Puckman : Ghost
    {
        private double _defaultSpeed;

        private HorisontalDirection _currentHorisontalDirection;
        private VerticalDirection _currentVerticalDirection;

        private HorisontalDirection _newHorisontalDirection;
        private VerticalDirection _newVerticalDirection;

        internal Puckman()
        {
            _defaultSpeed = 1.0 / 200;
            _newHorisontalDirection = HorisontalDirection.Stay;
            _newVerticalDirection = VerticalDirection.Stay;

            Name = "Puckman";
            Width = BattlefieldCircumstantials.Squaresize;
            Height = BattlefieldCircumstantials.Squaresize;
            SnapsToDevicePixels = true;

            LayoutRoot.Children.Add(new Ellipse
            {
                Fill = new SolidColorBrush(BattlefieldCircumstantials.YellowColor)
            });

            ChawAnimation();
        }

        private void ChawAnimation()
        {

            PathData.StrokeThickness = 0;
            PathData.Fill = new SolidColorBrush(BattlefieldCircumstantials.BlackColor);
            PathData.Data = Geometry.Parse("M0,0 M100,100 M50,50 L101,25 101,75");
            PathData.RenderTransformOrigin = new Point(0.5, 0.5);

            Panel.SetZIndex(PathData, 1);
            Panel.SetZIndex(this, 1);

            var translation = new ScaleTransform(1, 1);
            var translationName = "Chawing" + translation.GetHashCode();
            RegisterName(translationName, translation);
            PathData.RenderTransform = translation;

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

        internal new void MoveDecision()
        {
            if (double.IsNaN(Canvas.GetLeft(this)) || double.IsNaN(Canvas.GetTop(this))) return;

            var currentCoordinates = BattlefieldCircumstantials.GetCoordinates(this);

            var coordX = (int)currentCoordinates.X;
            var coordY = (int)currentCoordinates.Y;

            var thisSquareType = DefineTargetType(coordX, coordY);

            if (thisSquareType == targetType.Exit)
            {
                MainWindow.Wm.PlayWin();
                return;
            }

            if (thisSquareType == targetType.Entrance)
            {
                if (_newHorisontalDirection == _currentHorisontalDirection &&
                    _newVerticalDirection == _currentVerticalDirection)
                {
                    CreatureMovement(currentCoordinates);
                    return;
                }
            }

            var newCoordX = (int)currentCoordinates.X + (int)_newHorisontalDirection;
            var newCoordY = (int)currentCoordinates.Y + (int)_newVerticalDirection;

            var unPredictibleTargetType = DefineTargetType(newCoordX, newCoordY);

            var oldCoordX = (int)currentCoordinates.X + (int)_currentHorisontalDirection;
            var oldCoordY = (int)currentCoordinates.Y + (int)_currentVerticalDirection;

            var predictibleTargetType = DefineTargetType(oldCoordX, oldCoordY);

            if (unPredictibleTargetType == targetType.Null || unPredictibleTargetType == targetType.Entrance || unPredictibleTargetType == targetType.Exit)
            {
                _currentHorisontalDirection = _newHorisontalDirection;
                _currentVerticalDirection = _newVerticalDirection;
                CreatureMovement(new Point(newCoordX, newCoordY));
                return;
            }

            if (unPredictibleTargetType == targetType.Cherry)
            {
                _currentHorisontalDirection = _newHorisontalDirection;
                _currentVerticalDirection = _newVerticalDirection;
                ApplyBoost((ushort)newCoordX, (ushort)newCoordY);
                CreatureMovement(new Point(newCoordX, newCoordY));
                return;
            }

            if (predictibleTargetType == targetType.Null || predictibleTargetType == targetType.Entrance || predictibleTargetType == targetType.Exit)
            {
                CreatureMovement(new Point(oldCoordX, oldCoordY));
                return;
            }

            if (predictibleTargetType == targetType.Cherry)
            {
                ApplyBoost((ushort)oldCoordX, (ushort)oldCoordY);
                CreatureMovement(new Point(newCoordX, newCoordY));
                return;
            }


            CreatureMovement(currentCoordinates);
        }

        private targetType DefineTargetType(int x, int y)
        {
            if (x >= BattlefieldCircumstantials.Size || x < 0 || y >= BattlefieldCircumstantials.Size || y < 0)
            {
                return targetType.Error;
            }

            var target = BattlefieldCircumstantials.FieldElementsArray[x, y];

            if (BattlefieldCircumstantials.BricksList.Contains(target))
            {
                return targetType.Brick;
            }

            if (target != null && target.GetType() == typeof(Boost))
            {
                return targetType.Cherry;
            }

            if (Math.Abs(x - BattlefieldCircumstantials.XExit) < 0.2 && Math.Abs(y - BattlefieldCircumstantials.YExit) < 0.2)
            {
                return targetType.Exit;
            }

            if (Math.Abs(x - BattlefieldCircumstantials.XEntrance) < 0.2 && Math.Abs(y - BattlefieldCircumstantials.YEntrance) < 0.2)
            {
                return targetType.Entrance;
            }

            return targetType.Null;
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

        internal void MoveDefine(KeyEventArgs args)
        {
            _newHorisontalDirection = HorisontalDirection.Stay;
            _newVerticalDirection = VerticalDirection.Stay;
            switch (args.Key)
            {
                case Key.Up:
                    _newVerticalDirection = VerticalDirection.Up;
                    break;

                case Key.Down:
                    _newVerticalDirection = VerticalDirection.Down;
                    break;

                case Key.Left:
                    _newHorisontalDirection = HorisontalDirection.Left;
                    break;

                case Key.Right:
                    _newHorisontalDirection = HorisontalDirection.Right;
                    break;
            }


        }
    }
}
