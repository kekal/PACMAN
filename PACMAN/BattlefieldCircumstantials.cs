using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;


namespace PACMAN
{
    static class BattlefieldCircumstantials
    {
        private static ushort _size = 20;
        public const ushort Squaresize = 30;

        private static ushort _entranceIndex;
        private static ushort _exitIndex;

        public static ushort _xEntrance;
        public static ushort _yEntrance;

        //references collection
        private static Brick[,] _fieldElementsArray;
        private static List<Brick> _ghostsList;
        private static List<Brick> _bricksList;


        public static void GenerateField()
        {
            _size = (ushort)(MainWindow.Wm.Battlfield.Width / Squaresize);

            _fieldElementsArray = new Brick[20, 20];
            _bricksList = new List<Brick>();
            _ghostsList = new List<Brick>();

            AddWalls();

            AddCastle();

            AddMaze();

            ClearPath();
        }

        private static void AddWalls()
        {
            for (ushort j = 0; j < _size; j++)
            {
                AddBrick(0, j);
            }

            for (ushort j = 0; j < _size; j++)
            {
                AddBrick((ushort)(_size - 1), j);
            }

            var cropedSize = (ushort)(_size - 1);
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

            var lowCentreEdge = (ushort)(_size / 2 - 3);
            var highCentreEdge = (ushort)(_size / 2 + 3);
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
            for (ushort i = 1; i < _size - 1; i++)
            {
                for (ushort j = 1; j < _size - 1; j++)
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
            _entranceIndex = (ushort)(new Random()).Next(_size * 4 - 4);
            for (; CornerCheck(_bricksList[_entranceIndex]); _entranceIndex = (ushort)(new Random()).Next(_size * 4 - 4)) ;


            for (var j = 0; j < _size; j++)
                for (var i = 0; i < _size; i++)
                {
                    if (_fieldElementsArray[i, j] != _bricksList[_entranceIndex]) continue;
                    _xEntrance = (ushort)i;
                    _yEntrance = (ushort)j;
                    
                }
        }

        private static void DefineEscapeIndex()
        {
            _exitIndex = (ushort)(new Random()).Next(_size * 4 - 4);
            for (; CornerCheck(_bricksList[_exitIndex]) || _exitIndex == _entranceIndex; _exitIndex = (ushort)(new Random()).Next(_size * 4 - 4)) ;
        }

        private static void ClearApproach(int index)
        {
            var lowCentreEdge = (ushort)(_size / 2 - 3);
            var crophighCentreEdge = (ushort)(_size / 2 + 2);
            var cropSize = (ushort)(_size - 1);

            var column = (ushort)(Canvas.GetLeft(_bricksList[index]) / Squaresize);
            var row = (ushort)(Canvas.GetTop(_bricksList[index]) / Squaresize);

            if (column == 0)
            {
                for (ushort i = 0; i < lowCentreEdge; i++)
                {
                    RemoveElementFromField(row, i);
                }
            }

            if (column == cropSize)
            {
                for (var i = cropSize; i > crophighCentreEdge; i--)
                {
                    RemoveElementFromField(row, i);
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
            var lowCentreEdge = (ushort)(_size / 2 - 3);
            var highCentreEdge = (ushort)(_size / 2 + 3);
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
            var highCentreEdge = (ushort)(_size / 2 + 3);
            var crophighCentreEdge = (ushort)(highCentreEdge - 1);

            RemoveElementFromField((ushort)(_size / 2 - 1), crophighCentreEdge);
            RemoveElementFromField((ushort)(_size / 2), crophighCentreEdge);
        }

        private static void ClearCorridor()
        {
            for (var i = _size / 4; i < _size * 3 / 4 + 1; i++)
            {
                RemoveElementFromField((ushort)i, (ushort)(_size / 4));
            }

            for (var i = _size / 4; i < _size * 3 / 4 + 1; i++)
            {
                RemoveElementFromField((ushort)i, (ushort)(_size * 3 / 4));
            }

            for (var j = _size / 4; j < _size * 3 / 4 + 1; j++)
            {
                RemoveElementFromField((ushort)(_size / 4), (ushort)j);
            }

            for (var j = _size / 4; j < _size * 3 / 4 + 1; j++)
            {
                RemoveElementFromField((ushort)(_size * 3 / 4), (ushort)j);
            }
        }

        private static bool CornerCheck(Brick brick)
        {
            return (Canvas.GetLeft(brick) == 0 || Canvas.GetLeft(brick) == 19 * Squaresize) &&
                   (Canvas.GetTop(brick) == 0 || Canvas.GetTop(brick) == 19 * Squaresize);
        }

        private static void AddBrick(ushort x, ushort y)
        {
            if (_fieldElementsArray[x, y] != null) return;

            var tempBrick = new Brick
            {
                Width = Squaresize,
                Height = Squaresize
            };
            MainWindow.Wm.Battlfield.Children.Add(tempBrick);
            Canvas.SetLeft(tempBrick, Squaresize * x);
            Canvas.SetTop(tempBrick, Squaresize * y);
            _fieldElementsArray[x, y] = tempBrick;
            _bricksList.Add(tempBrick);
        }

        private static Ghost AddGhost(ushort x, ushort y, string command = "")
        {
            if (_fieldElementsArray[x, y] != null) return null;
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
            _fieldElementsArray[x, y] = tempGhost;
            _ghostsList.Add(tempGhost);
            return tempGhost;
        }

        public static Brick AddGhostToCastle(string type)
        {
            for (ushort j = 7; j < 12; j++)
            {
                for (ushort i = 7; i < 12; i++)
                {
                    if (_fieldElementsArray[i, j] != null) continue;
                    return AddGhost(i, j, type);
                }
            }
            return null;
        }

        internal static Brick AddPuckman()
        {

            //if (_fieldElementsArray[_xEntrance, _yEntrance] != null)
            //{
            //    //MessageBox.Show(_xEntrance + ", " + _yEntrance + "\n" + _fieldElementsArray[_xEntrance, _yEntrance]);
            //    return null;
            //}

            var temp = new Puckman()
            {
                Width = Squaresize,
                Height = Squaresize
            };

            MainWindow.Wm.Battlfield.Children.Add(temp);
            Canvas.SetLeft(temp, Squaresize * _xEntrance);
            Canvas.SetTop(temp, Squaresize * _yEntrance);
            _fieldElementsArray[_xEntrance, _yEntrance] = temp;

            return temp;
        }

        public static bool MoveBattlefieldElement(ushort x1, ushort y1, ushort x2, ushort y2)
        {
            if (_fieldElementsArray[x2, y2] != null) return false;

            _fieldElementsArray[x2, y2] = _fieldElementsArray[x1, y1];
            _fieldElementsArray[x1, y1] = null;
            return true;
        }

        private static void RemoveElementFromField(ushort x, ushort y)
        {
            if (_fieldElementsArray[x, y] == null) return;

            MainWindow.Wm.Battlfield.Children.Remove(_fieldElementsArray[x, y]);

            if (_bricksList.Contains(_fieldElementsArray[x, y])) _bricksList.Remove(_fieldElementsArray[x, y]);
            if (_ghostsList.Contains(_fieldElementsArray[x, y])) _ghostsList.Remove(_fieldElementsArray[x, y]);

            _bricksList.Remove(_fieldElementsArray[x, y]);
            _fieldElementsArray[x, y] = null;
        }

        public static void RemoveElementFromField(Brick brick)
        {
            RemoveElementFromField((ushort)Grid.GetColumn(brick), (ushort)Grid.GetRow(brick));
        }


    }

    //class Turtle
    //{
    //    public List<turtlePath> pathesList = new List<turtlePath>();

    //    public void makeStep()
    //    {

    //        for (var index = 0; index < pathesList.Count; index++)
    //        {
    //            var currentPoint = pathesList[index].PointList.Last();
    //            var candidates = new List<Point>();
    //            for (int i = currentPoint.X - 1; i < currentPoint.X + 1; i++)
    //            {
    //                for (int j = currentPoint.Y - 1; j < currentPoint.Y + 1; j++)
    //                {
    //                    foreach (var turtlepath in pathesList)
    //                    {
    //                        foreach (var point in turtlepath.PointList)
    //                        {
    //                            if (point.X != currentPoint.X)
    //                            {
    //                                candidates.Add(point);
    //                            }
    //                        }
    //                    }
    //                }
    //            }

    //            foreach (var candidate in candidates)
    //            {
    //                var tempturtle = new turtlePath
    //                {
    //                    PointList = pathesList[index].PointList.Select(point => new Point(point.X, point.Y)).ToList()
    //                };
    //                pathesList.Add(tempturtle);


    //            }

    //            pathesList.RemoveAt(index);
    //            index -= 1;


    //        }
    //    }
    //}

    //class turtlePath
    //{
    //    public List<Point> PointList = new List<Point>();
    //}

    //class Point
    //{
    //    public Point(int x, int y)
    //    {
    //        Y = y;
    //        X = x;
    //    }

    //    public int X { get; set; }
    //    public int Y { get; set; }
    //}
}
