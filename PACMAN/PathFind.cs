using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PACMAN
{
    class PathFind
    {
        private readonly Point _start;
        private readonly Point _finish;

        public List<Point> Path;

        private Dictionary<Point, int> _closed;
        private Dictionary<Point, int> _border;
        private readonly Dictionary<Point, int> _candidates;

        public PathFind(Point start, Point finish)
        {
            _closed = new Dictionary<Point, int>();
            _border = new Dictionary<Point, int>();
            _candidates = new Dictionary<Point, int>();

            _start = start;
            _finish = finish;

            _border[start] = 0;

            Search();


            RevealPath();
        }


        private void Search()
        {
            for (; ; )
            {
                foreach (var pointValuePair in _border)
                {
                    LookForCandidates(pointValuePair.Value + 1);
                }

                if (_candidates.Count < 1)
                {
                    MessageBox.Show("Packman is unreachable");
                    throw new PackmanUnreachable();
                }

                _closed = _closed.Concat(_border).ToDictionary(x => x.Key, x => x.Value);

                _border.Clear();


                _border = _border.Concat(_candidates).ToDictionary(x => x.Key, x => x.Value);

                _candidates.Clear();


                if (_border.ContainsKey(_finish))
                {
                    return;
                }
            }
        }

        private void LookForCandidates(int value)
        {
            foreach (var pointValuePair in _border)
            {
                var newCandidates = new List<Point>
                {
                    new Point(pointValuePair.Key.X + 1, pointValuePair.Key.Y),
                    new Point(pointValuePair.Key.X, pointValuePair.Key.Y + 1),
                    new Point(pointValuePair.Key.X - 1, pointValuePair.Key.Y),
                    new Point(pointValuePair.Key.X, pointValuePair.Key.Y - 1)
                };

                for (var i = 0; i < newCandidates.Count; i++)
                {
                    if (
                        (newCandidates[i].Y < 0) ||
                        (newCandidates[i].X < 0) ||
                        (newCandidates[i].Y >= BattlefieldCircumstantials.Size) ||
                        (newCandidates[i].X >= BattlefieldCircumstantials.Size) ||
                        (_border.ContainsKey(newCandidates[i])) ||
                        (BattlefieldCircumstantials._bricksList.Contains(
                            BattlefieldCircumstantials._fieldElementsArray[(int)(newCandidates[i].X), (int)newCandidates[i].Y])) ||
                        (_closed.ContainsKey(newCandidates[i]))
                        )
                    {
                        newCandidates.RemoveAt(i);
                        i -= 1;
                        continue;
                    }

                    _candidates[newCandidates[i]] = value;
                }
            }
        }

        private void RevealPath()
        {
            Path = new List<Point> { _finish };

            for (; ; )
            {
                var around = new List<Point>
                {
                    new Point(Path.Last().X + 1, Path.Last().Y),
                    new Point(Path.Last().X, Path.Last().Y + 1),
                    new Point(Path.Last().X - 1, Path.Last().Y),
                    new Point(Path.Last().X, Path.Last().Y - 1)
                };

                if (around.Contains(_start))
                {
                    Path.Reverse();
                    return;
                }

                Path.Add(around.Where(point => _closed.ContainsKey(point))
                        .ToDictionary(t => t, t => _closed[t])
                        .Aggregate((l, r) => l.Value < r.Value ? l : r)
                        .Key);
            }
        }
    }

    class PackmanUnreachable : Exception { }
}
