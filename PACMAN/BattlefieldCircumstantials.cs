using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace PACMAN
{
    static class BattlefieldCircumstantials
    {
        internal static Puckman Puckman;

        internal static ushort Size = 20;
        internal const ushort Squaresize = 30;

        private static ushort _entranceIndex;
        private static ushort _exitIndex;

        internal static ushort XEntrance;
        internal static ushort YEntrance;

        internal static ushort XExit;
        internal static ushort YExit;

        internal static Brick[,] FieldElementsArray;
        internal static List<Brick> GhostsList;
        internal static List<Brick> BricksList;

        internal static readonly Color YellowColor = (Color)ColorConverter.ConvertFromString("Yellow");
        internal static readonly Color BlackColor = (Color)ColorConverter.ConvertFromString("Black");
        internal static readonly Color WhiteColor = (Color)ColorConverter.ConvertFromString("White");


        internal static void GenerateField()
        {
            Size = (ushort)(MainWindow.Wm.Battlfield.Width / Squaresize);

            FieldElementsArray = new Brick[Size, Size];
            BricksList = new List<Brick>();
            GhostsList = new List<Brick>();

            AddWalls();

            AddCastle();

            AddMaze();

            ClearPath();
        }

        private static void AddWalls()
        {
            for (ushort j = 0; j < Size; j++)
            {
                AddBrick(0, j);
            }

            for (ushort j = 0; j < Size; j++)
            {
                AddBrick((ushort)(Size - 1), j);
            }

            var cropedSize = (ushort)(Size - 1);
            for (ushort i = 1; i < cropedSize; i++)
            {
                AddBrick(i, 0);
            }

            for (ushort i = 1; i < cropedSize; i++)
            {
                AddBrick(i, cropedSize);
            }


        }

        private static void AddCastle()
        {

            var lowCentreEdge = (ushort)(Size / 2 - 3);
            var highCentreEdge = (ushort)(Size / 2 + 3);
            var croplowCentreEdge = (ushort)(lowCentreEdge + 1);
            var crophighCentreEdge = (ushort)(highCentreEdge - 1);


            for (var i = lowCentreEdge; i < highCentreEdge; i++)
            {
                AddBrick(i, lowCentreEdge);
            }

            for (var i = lowCentreEdge; i < highCentreEdge; i++)
            {
                AddBrick(i, crophighCentreEdge);
            }

            for (ushort j = croplowCentreEdge; j < crophighCentreEdge; j++)
            {
                AddBrick(lowCentreEdge, j);
            }

            for (ushort j = croplowCentreEdge; j < crophighCentreEdge; j++)
            {
                AddBrick(crophighCentreEdge, j);
            }


        }

        private static void AddMaze()
        {
            var rnd = new Random();
            for (ushort i = 1; i < Size - 1; i++)
            {
                for (ushort j = 1; j < Size - 1; j++)
                {
                    var temp = rnd.Next(10);
                    if (temp > 6)
                    {
                        AddBrick(i, j);
                    }
                }
            }
        }

        private static void ClearPath()
        {
            DefineEntranceIndex();

            ClearApproach(_entranceIndex);

            DefineEscapeIndex();

            ClearApproach(_exitIndex);

            ClearBailey();

            ClearCastleGates();

            ClearCorridor();
        }

        private static void DefineEntranceIndex()
        {
            _entranceIndex = (ushort)(new Random()).Next(Size * 4 - 4);
            for (; CornerCheck(BricksList[_entranceIndex]); _entranceIndex = (ushort)(new Random()).Next(Size * 4 - 4))
            {
            }

            for (var j = 0; j < Size; j++)
                for (var i = 0; i < Size; i++)
                {
                    if (FieldElementsArray[i, j] != BricksList[_entranceIndex]) continue;
                    XEntrance = (ushort)i;
                    YEntrance = (ushort)j;
                }
        }

        private static void DefineEscapeIndex()
        {
            _exitIndex = (ushort)(new Random()).Next(Size * 4 - 4);
            for (; CornerCheck(BricksList[_exitIndex]) || _exitIndex == _entranceIndex; _exitIndex = (ushort)(new Random()).Next(Size * 4 - 4))
            {
            }

            for (var j = 0; j < Size; j++)
                for (var i = 0; i < Size; i++)
                {
                    if (FieldElementsArray[i, j] != BricksList[_exitIndex]) continue;
                    XExit = (ushort)i;
                    YExit = (ushort)j;
                }
        }

        private static void ClearApproach(int index)
        {
            var lowCentreEdge = (ushort)(Size / 2 - 3);
            var crophighCentreEdge = (ushort)(Size / 2 + 2);
            var cropSize = (ushort)(Size - 1);

            var column = (ushort)(Canvas.GetLeft(BricksList[index]) / Squaresize);
            var row = (ushort)(Canvas.GetTop(BricksList[index]) / Squaresize);

            if (column == 0)
            {
                for (ushort i = 0; i < lowCentreEdge; i++)
                {
                    RemoveElementFromField(i, row);
                }
            }

            if (column == cropSize)
            {
                for (var i = cropSize; i > crophighCentreEdge; i--)
                {
                    RemoveElementFromField(i, row);
                }
            }

            if (row == 0)
            {
                for (ushort i = 0; i < lowCentreEdge; i++)
                {
                    RemoveElementFromField(column, i);
                }
            }

            if (row == cropSize)
            {
                for (var i = cropSize; i > crophighCentreEdge; i--)
                {
                    RemoveElementFromField(column, i);
                }
            }
        }

        private static void ClearBailey()
        {
            var lowCentreEdge = (ushort)(Size / 2 - 3);
            var highCentreEdge = (ushort)(Size / 2 + 3);
            var croplowCentreEdge = (ushort)(lowCentreEdge + 1);
            var crophighCentreEdge = (ushort)(highCentreEdge - 1);
            for (var i = croplowCentreEdge; i < crophighCentreEdge; i++)
            {
                for (var j = croplowCentreEdge; j < crophighCentreEdge; j++)
                {
                    RemoveElementFromField(i, j);
                }
            }
        }

        private static void ClearCastleGates()
        {
            var highCentreEdge = (ushort)(Size / 2 + 3);
            var crophighCentreEdge = (ushort)(highCentreEdge - 1);

            RemoveElementFromField((ushort)(Size / 2 - 1), crophighCentreEdge);
            RemoveElementFromField((ushort)(Size / 2), crophighCentreEdge);
        }

        private static void ClearCorridor()
        {
            for (var i = Size / 4; i < Size * 3 / 4 + 1; i++)
            {
                RemoveElementFromField((ushort)i, (ushort)(Size / 4));
            }

            for (var i = Size / 4; i < Size * 3 / 4 + 1; i++)
            {
                RemoveElementFromField((ushort)i, (ushort)(Size * 3 / 4));
            }

            for (var j = Size / 4; j < Size * 3 / 4 + 1; j++)
            {
                RemoveElementFromField((ushort)(Size / 4), (ushort)j);
            }

            for (var j = Size / 4; j < Size * 3 / 4 + 1; j++)
            {
                RemoveElementFromField((ushort)(Size * 3 / 4), (ushort)j);
            }
        }

        private static bool CornerCheck(Brick brick)
        {
            return (Math.Abs(Canvas.GetLeft(brick)) < 0.1 || Math.Abs(Canvas.GetLeft(brick) - 19 * Squaresize) < 0.1) &&
                   (Math.Abs(Canvas.GetTop(brick)) < 0.1 || Math.Abs(Canvas.GetTop(brick) - 19 * Squaresize) < 0.1);
        }

        private static void AddBrick(ushort x, ushort y)
        {
            if (FieldElementsArray[x, y] != null) return;

            var tempBrick = new Brick
            {
                Width = Squaresize,
                Height = Squaresize,
            };

            MainWindow.Wm.Battlfield.Children.Add(tempBrick);
            Canvas.SetLeft(tempBrick, Squaresize * x);
            Canvas.SetTop(tempBrick, Squaresize * y);
            FieldElementsArray[x, y] = tempBrick;
            BricksList.Add(tempBrick);
        }

        private static Ghost AddGhost(ushort x, ushort y, string command = "")
        {
            if (FieldElementsArray[x, y] != null) return null;
            Ghost tempGhost;
            try
            {
                switch (command)
                {
                    case "yellow":
                        tempGhost = new YellowGhost
                        {
                            Width = Squaresize,
                            Height = Squaresize
                        };
                        break;
                    case "red":
                        tempGhost = new RedGhost
                        {
                            Width = Squaresize,
                            Height = Squaresize
                        };
                        break;
                    default:
                        tempGhost = new Ghost
                        {
                            Width = Squaresize,
                            Height = Squaresize
                        };
                        break;
                }
            }
            catch (AlreadyInstatiated)
            {
                return null;
            }

            MainWindow.Wm.Battlfield.Children.Add(tempGhost);
            Canvas.SetLeft(tempGhost, Squaresize * x);
            Canvas.SetTop(tempGhost, Squaresize * y);

            FieldElementsArray[x, y] = tempGhost;
            GhostsList.Add(tempGhost);
            return tempGhost;
        }

        internal static void AddGhosts()
        {
            var ghosts = new[] { "red", "yellow" };

            foreach (var ghost in ghosts.Where(s => AddGhostToCastle(s) == null))
            {
                MessageBox.Show("Can't add to proper place ghost " + ghost);
            }

            foreach (var ghost in GhostsList)
            {
                ((Ghost)ghost).MoveDecision();
            }
        }

        private static Brick AddGhostToCastle(string type)
        {
            for (ushort j = 7; j < 12; j++)
            {
                for (ushort i = 7; i < 12; i++)
                {
                    if (FieldElementsArray[i, j] != null) continue;
                    return AddGhost(i, j, type);
                }
            }
            return null;
        }

        internal static void AddPuckman()
        {
            if (FieldElementsArray[XEntrance, YEntrance] != null)
            {
                MessageBox.Show(XEntrance + ", " + YEntrance + " contains:\n" +
                                FieldElementsArray[XEntrance, YEntrance]);
                throw new IndexOutOfRangeException();
            }

            var elem = new Puckman
            {
                Width = Squaresize,
                Height = Squaresize
            };

            MainWindow.Wm.Battlfield.Children.Add(elem);
            Canvas.SetLeft(elem, Squaresize * XEntrance);
            Canvas.SetTop(elem, Squaresize * YEntrance);
            FieldElementsArray[XEntrance, YEntrance] = elem;

            Puckman = elem;

            Puckman.MoveDecision();
        }

        internal static void AddCherry()
        {

            var temp = new Boost
            {
                Width = Squaresize,
                Height = Squaresize
            };

            var rnd = new Random();

            for (; ; )
            {
                var nX = (ushort)rnd.Next(18);
                var nY = (ushort)rnd.Next(18);

                if (FieldElementsArray[nX, nY] != null) continue;

                MainWindow.Wm.Battlfield.Children.Add(temp);
                Canvas.SetLeft(temp, Squaresize * nX);
                Canvas.SetTop(temp, Squaresize * nY);
                FieldElementsArray[nX, nY] = temp;
                break;
            }
        }

        internal static void MoveBattlefieldElement(ushort x1, ushort y1, ushort x2, ushort y2)
        {
            if (x1 == x2 && y1 == y2)
            {
                return;
            }

            FieldElementsArray[x2, y2] = FieldElementsArray[x1, y1];
            FieldElementsArray[x1, y1] = null;
        }

        internal static void RemoveElementFromField(ushort x, ushort y)
        {
            if (FieldElementsArray[x, y] == null) return;

            MainWindow.Wm.Battlfield.Children.Remove(FieldElementsArray[x, y]);

            if (BricksList.Contains(FieldElementsArray[x, y])) BricksList.Remove(FieldElementsArray[x, y]);
            if (GhostsList.Contains(FieldElementsArray[x, y])) GhostsList.Remove(FieldElementsArray[x, y]);

            FieldElementsArray[x, y] = null;
        }

        internal static Point GetCoordinates(Brick element)
        {
            for (var j = 0; j < FieldElementsArray.GetLength(0); j++)
            {
                for (var i = 0; i < FieldElementsArray.GetLength(1); i++)
                {
                    if (FieldElementsArray[i, j] == element)
                    {
                        return new Point(i, j);
                    }
                }
            }

            var x = Math.Round(Canvas.GetLeft(element) / Squaresize);
            var y = Math.Round(Canvas.GetTop(element) / Squaresize);
            return new Point(x, y);
        }

        internal static double FindDirectDistance(Brick brick1, Brick brick2)
        {

            var x1 = Canvas.GetLeft(brick1);
            var y1 = Canvas.GetTop(brick1);
            var x2 = Canvas.GetLeft(brick2);
            var y2 = Canvas.GetTop(brick2);

            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
    }
}