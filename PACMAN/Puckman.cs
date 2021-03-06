﻿using System;
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

    enum TargetType
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
                //EasingFunction = new PowerEase { EasingMode = EasingMode.EaseIn },
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

            if (thisSquareType == TargetType.Exit)
            {
                MainWindow.Wm.PlayWin();
                return;
            }

            if (thisSquareType == TargetType.Entrance)
            {
                if (_newHorisontalDirection == _currentHorisontalDirection &&
                    _newVerticalDirection == _currentVerticalDirection)
                {
                    CreatureMovement(currentCoordinates, currentCoordinates);
                    return;
                }
            }

            var newCoordX = (int)currentCoordinates.X + (int)_newHorisontalDirection;
            var newCoordY = (int)currentCoordinates.Y + (int)_newVerticalDirection;

            var unPredictibleTargetType = DefineTargetType(newCoordX, newCoordY);

            var oldCoordX = (int)currentCoordinates.X + (int)_currentHorisontalDirection;
            var oldCoordY = (int)currentCoordinates.Y + (int)_currentVerticalDirection;

            var predictibleTargetType = DefineTargetType(oldCoordX, oldCoordY);

            if (unPredictibleTargetType == TargetType.Null || unPredictibleTargetType == TargetType.Entrance || unPredictibleTargetType == TargetType.Exit)
            {
                _currentHorisontalDirection = _newHorisontalDirection;
                _currentVerticalDirection = _newVerticalDirection;
                CreatureMovement(currentCoordinates, new Point(newCoordX, newCoordY));
                return;
            }

            if (unPredictibleTargetType == TargetType.Cherry)
            {
                _currentHorisontalDirection = _newHorisontalDirection;
                _currentVerticalDirection = _newVerticalDirection;
                ApplyBoost((ushort)newCoordX, (ushort)newCoordY);
                CreatureMovement(currentCoordinates, new Point(newCoordX, newCoordY));
                return;
            }

            if (predictibleTargetType == TargetType.Null || predictibleTargetType == TargetType.Entrance || predictibleTargetType == TargetType.Exit)
            {
                CreatureMovement(currentCoordinates, new Point(oldCoordX, oldCoordY));
                return;
            }

            if (predictibleTargetType == TargetType.Cherry)
            {
                ApplyBoost((ushort)oldCoordX, (ushort)oldCoordY);
                CreatureMovement(currentCoordinates, new Point(newCoordX, newCoordY));
                return;
            }


            CreatureMovement(currentCoordinates, currentCoordinates);
        }

        private TargetType DefineTargetType(int x, int y)
        {
            if (x >= BattlefieldCircumstantials.Size || x < 0 || y >= BattlefieldCircumstantials.Size || y < 0)
            {
                return TargetType.Error;
            }

            var target = BattlefieldCircumstantials.FieldElementsArray[x, y];

            if (BattlefieldCircumstantials.BricksList.Contains(target))
            {
                return TargetType.Brick;
            }

            if (target != null && target.GetType() == typeof(Boost))
            {
                return TargetType.Cherry;
            }

            if (Math.Abs(x - BattlefieldCircumstantials.XExit) < 0.2 && Math.Abs(y - BattlefieldCircumstantials.YExit) < 0.2)
            {
                return TargetType.Exit;
            }

            if (Math.Abs(x - BattlefieldCircumstantials.XEntrance) < 0.2 && Math.Abs(y - BattlefieldCircumstantials.YEntrance) < 0.2)
            {
                return TargetType.Entrance;
            }

            return TargetType.Null;
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

        private void CreatureMovement(Point start, Point goal)
        {
            const ushort square = BattlefieldCircumstantials.Squaresize;

            //var oldDiscreteX = Math.Round(Canvas.GetLeft(this) / square);
            //var oldDiscreteY = Math.Round(Canvas.GetTop(this) / square);

            var oldDiscreteX = start.X;
            var oldDiscreteY = start.Y;

            var oldX = oldDiscreteX * square;
            var oldY = oldDiscreteY * square;

            var newDiscreteX = goal.X;
            var newdiscreteY = goal.Y;

            var newX = newDiscreteX * square;
            var newY = newdiscreteY * square;

            

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

            Log.AddLog(
                String.Format("Decide to move from {0,2},{1,-2}  to  {2,2},{3,-2}   current coords: {4,-3},{5,-3}   ({6,-6:N3},{7,-6:N3})",
                Math.Round(oldDiscreteX), Math.Round(oldDiscreteY),
                Math.Round(newDiscreteX), Math.Round(newdiscreteY),
                Math.Round(oldX), Math.Round(oldY),
                oldDiscreteX, oldDiscreteY)
                );

            var dist = Math.Abs(Math.Sqrt(Math.Pow(newX - Canvas.GetLeft(this), 2) + Math.Pow(newY - Canvas.GetTop(this), 2)) % square);

            if (dist > 10)
            {
                dist = Math.Abs(square - dist);
            }

            if (dist > 10)
            {
                Log.AddLog("\t\t\t\t\t\t\t\t\t\t\tОшибка позиционирования пакмана: " + Math.Round(dist));
            }
           

            Canvas.SetLeft(this, oldX);
            Canvas.SetTop(this, oldY);

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
            
            switch (args.Key)
            {
                case Key.Up:
                    _newVerticalDirection = VerticalDirection.Up;
                    _newHorisontalDirection = HorisontalDirection.Stay;
                    Log.AddLog("Pressed UP");
                    break;

                case Key.Down:
                    _newVerticalDirection = VerticalDirection.Down;
                    _newHorisontalDirection = HorisontalDirection.Stay;
                    Log.AddLog("Pressed Down");
                    break;

                case Key.Left:
                    _newVerticalDirection = VerticalDirection.Stay;
                    _newHorisontalDirection = HorisontalDirection.Left;
                    Log.AddLog("Pressed Left");
                    break;

                case Key.Right:
                    _newVerticalDirection = VerticalDirection.Stay;
                    _newHorisontalDirection = HorisontalDirection.Right;
                    Log.AddLog("Pressed Right");
                    break;
            }


        }
    }
}
