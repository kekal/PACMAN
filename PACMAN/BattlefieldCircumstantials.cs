using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace PACMAN
{
    static class BattlefieldCircumstantials
    {
        public static ushort size;
        //public static ushort entrance;
        public static Brick[,] BricksArray;
        public static List<Brick> BricksList;
        public static ushort entranceIndex;
        public static ushort exitIndex;


        public static void FeelField()
        {
            size = (ushort)MainWindow.Wm.Battlfield.RowDefinitions.Count;

            BricksArray = new Brick[20, 20];
            BricksList = new List<Brick>();


            addWalls();

            addCastle();


            addMaze();

            clearPath();



        }


        public static bool CornerCheck(Brick brick)
        {
            return (Grid.GetColumn(brick) == 0 || Grid.GetColumn(brick) == 19) && (Grid.GetRow(brick) == 0 || Grid.GetRow(brick) == 19);
        }

        public static void addBrick(ushort x, ushort y)
        {
            if (BricksArray[x, y] != null) return;

            var tempBrick = new Brick();
            MainWindow.Wm.Battlfield.Children.Add(tempBrick);
            Grid.SetColumn(tempBrick, x);
            Grid.SetRow(tempBrick, y);
            BricksArray[x, y] = tempBrick;
            BricksList.Add(tempBrick);
        }

        public static void addGhost(ushort x, ushort y, string command = "")
        {
            if (BricksArray[x, y] != null) return;
            Ghost tempBrick;

            switch (command)
            {
                case "yellow":
                    tempBrick = new YellowGhost();
                    break;
                case "red":
                    tempBrick = new RedGhost();
                    break;
                default:
                    tempBrick = new Ghost();
                    break;
            }

            MainWindow.Wm.Battlfield.Children.Add(tempBrick);
            Grid.SetColumn(tempBrick, x);
            Grid.SetRow(tempBrick, y);
            BricksArray[x, y] = tempBrick;
            BricksList.Add(tempBrick);
        }

        public static void addWalls()
        {
            for (ushort j = 0; j < size; j++)
            {
                addBrick(0, j);
            }

            for (ushort j = 0; j < size; j++)
            {
                addBrick((ushort)(size - 1), j);
            }

            var _size = (ushort)(size - 1);
            for (ushort i = 1; i < _size; i++)
            {
                addBrick(i, 0);
            }

            for (ushort j = 1; j < _size; j++)
            {
                addBrick(j, _size);
            }


        }

        public static void addCastle()
        {

            ushort lowCentreEdge = (ushort)(size / 2 - 3);
            ushort highCentreEdge = (ushort)(size / 2 + 3);
            ushort _lowCentreEdge = (ushort)(lowCentreEdge + 1);
            ushort _highCentreEdge = (ushort)(highCentreEdge - 1);


            for (var i = lowCentreEdge; i < highCentreEdge; i++)
            {
                addBrick(i, lowCentreEdge);
            }

            for (var i = lowCentreEdge; i < highCentreEdge; i++)
            {
                addBrick(i, _highCentreEdge);
            }

            for (ushort j = _lowCentreEdge; j < _highCentreEdge; j++)
            {
                addBrick(lowCentreEdge, j);
            }

            for (ushort j = _lowCentreEdge; j < _highCentreEdge; j++)
            {
                addBrick(_highCentreEdge, j);
            }


        }

        public static void addMaze()
        {
            var rnd = new Random();
            for (ushort i = 1; i < size - 1; i++)
            {
                for (ushort j = 1; j < size - 1; j++)
                {
                    var temp = rnd.Next(10);
                    if (temp > 5)
                    {
                        addBrick(i, j);
                    }
                }
            }
        }



        public static void clearPath()
        {
            ushort lowCentreEdge = (ushort)(size / 2 - 3);
            ushort highCentreEdge = (ushort)(size / 2 + 3);
            ushort _lowCentreEdge = (ushort)(lowCentreEdge + 1);
            ushort _highCentreEdge = (ushort)(highCentreEdge - 1);

            entranceIndex = (ushort)(new Random()).Next(size * 4 - 4);
            for (; CornerCheck(BricksList[entranceIndex]); entranceIndex = (ushort)(new Random()).Next(size * 4 - 4)) ;

            ClearWays(entranceIndex);

            exitIndex = (ushort)(new Random()).Next(size * 4 - 4);
            for (; CornerCheck(BricksList[exitIndex]) || exitIndex == entranceIndex; exitIndex = (ushort)(new Random()).Next(size * 4 - 4)) ;

            ClearWays(exitIndex);


            for (var i = _lowCentreEdge; i < _highCentreEdge; i++)
            {
                for (var j = _lowCentreEdge; j < _highCentreEdge; j++)
                {
                    RemoveBattlefieldElement(i, j);
                }
            }

            RemoveBattlefieldElement((ushort)(size / 2 - 1), _highCentreEdge);
            RemoveBattlefieldElement((ushort)(size / 2), _highCentreEdge);

            for (var i = size / 4; i < size * 3 / 4 + 1; i++)
            {
                RemoveBattlefieldElement((ushort)i, (ushort)(size / 4));
            }

            for (var i = size / 4; i < size * 3 / 4 + 1; i++)
            {
                RemoveBattlefieldElement((ushort)i, (ushort)(size * 3 / 4));
            }

            for (var j = size / 4; j < size * 3 / 4 + 1; j++)
            {
                RemoveBattlefieldElement((ushort)(size / 4), (ushort)j);
            }

            for (var j = size / 4; j < size * 3 / 4 + 1; j++)
            {
                RemoveBattlefieldElement((ushort)(size * 3 / 4), (ushort)j);
            }
        }

        public static void ClearWays(int index)
        {
            ushort lowCentreEdge = (ushort)(size / 2 - 3);
            ushort _highCentreEdge = (ushort)(size / 2 + 2);
            var _size = (ushort)(size - 1);

            var column = (ushort)Grid.GetColumn(BricksList[index]);
            var row = (ushort)Grid.GetRow(BricksList[index]);

            if (column == 0)
            {
                for (ushort i = 0; i < lowCentreEdge; i++)
                {
                    RemoveBattlefieldElement(row, i);
                }
            }

            if (column == _size)
            {
                for (ushort i = _size; i > _highCentreEdge; i--)
                {
                    RemoveBattlefieldElement(row, i);
                }
            }

            if (row == 0)
            {
                for (ushort i = 0; i < lowCentreEdge; i++)
                {
                    RemoveBattlefieldElement(column, i);
                }
            }

            if (row == _size)
            {
                for (ushort i = _size; i > _highCentreEdge; i--)
                {
                    RemoveBattlefieldElement(column, i);
                }
            }
        }

        public static bool MoveBattlefieldElement(ushort x1, ushort y1, ushort x2, ushort y2)
        {
            if (BricksArray[x2, y2].GetType() == typeof(Brick))
            {
                return false;
            }
            BricksArray[x2, y2] = BricksArray[x1, y1];
            BricksArray[x1, y1] = null;
            return true;
        }


        public static void RemoveBattlefieldElement(ushort x, ushort y)
        {
            MainWindow.Wm.Battlfield.Children.Remove(BricksArray[x, y]);
            BricksList.Remove(BricksArray[x, y]);
            BricksArray[x, y] = null;
        }

        public static void RemoveBattlefieldElement(Brick brick)
        {
            RemoveBattlefieldElement((ushort)Grid.GetColumn(brick), (ushort)Grid.GetRow(brick));
        }

        public static void AddGhostToCastle()
        {

            for (ushort i = 7; i < 12; i++)
            {
                for (ushort j = 7; j < 12; j++)
                {
                    if (BricksArray[i, j] != null) continue;

                    addGhost(i, j, "yellow");
                    return;
                }
            }
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
