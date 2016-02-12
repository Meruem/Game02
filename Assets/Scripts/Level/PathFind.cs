using System;
using System.Collections.Generic;
using Alg;

namespace PathFind
{
    public class Node
    {
        public Point Position;
        public double G;
        public double H;
        public double F { get { return G + H; } }
        public Node Parent;
        public bool IsInOpenList;
        public bool IsInClosedList;

        public override string ToString()
        {
            return string.Format("Pos: {0}, {1}; G: {2}; H: {3}", Position.X, Position.Y, G, H);
        }
    }

    public struct Point
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Point(int x, int y) : this()
        {
            X = x;
            Y = y;
        }
    }

    public class AStar
    {
        private readonly int[,] _map;
        private readonly Func<int, bool> _isWall;
        private readonly Node[,] _nodeMap;

        public AStar(int[,] map, Func<int, bool> isWall)
        {
            _map = map;
            _isWall = isWall;
            var height = _map.GetLength(0);
            var width = _map.GetLength(1);
            _nodeMap = new Node[height, width];
            Reset();
        }

        public void Reset()
        {
            var height = _map.GetLength(0);
            var width = _map.GetLength(1);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    _nodeMap[i, j] = new Node { G = double.MaxValue, Position = new Point(i, j) };
                }
            }
            
        }

        public List<Point> AStarFind(Point start, Point end, bool useTheta = false)
        {
            var open = new BinaryHeap<Node>(1000, (n1, n2) => -n1.F.CompareTo(n2.F));
            var startNode = _nodeMap[start.X, start.Y];
            startNode.G = 0;
            startNode.H = GetHeurestic(start, end); 

            open.Insert(startNode);
            startNode.IsInOpenList = true;

            while (open.Size > 0)
            {
                // pop node from open list
                var s = open.Pop();

                // found end node
                if (s.Position.Equals(end))
                {
                    var path = new List<Point>();

                    Node n = s;
                    while (n != null)
                    {
                        path.Add(n.Position);
                        n = n.Parent;
                    }

                    return path;
                }

                // add to closed and check neigbours
                s.IsInOpenList = false;
                s.IsInClosedList = true;

                // get all accessible neighbours
                var ngbr = GetVisNgbr(s.Position, _map);
                foreach (var position in ngbr)
                {
                    var ngNode = _nodeMap[position.X, position.Y];

                    // not in closed list
                    if (!ngNode.IsInClosedList)
                    {
                        if (!ngNode.IsInOpenList)
                        {
                            ngNode.G = s.G + GetDistance(s.Position, position);
                            ngNode.H = GetHeurestic(position, end);
                            ngNode.Parent = s;
                            open.Insert(ngNode);
                            ngNode.IsInOpenList = true;

                        }

                        if (useTheta && s.Parent != null && Line(s.Parent.Position.X, s.Parent.Position.Y, position.X, position.Y, (x, y) => _map[x, y] != 1))
                        {
                            var newG = s.Parent.G + GetDistance(s.Parent.Position, position);
                            if (newG < ngNode.G)
                            {
                                ngNode.Parent = s.Parent;
                                ngNode.G = newG;
                            }
                        }
                        else
                        {
                            var newG = s.G + GetDistance(s.Position, position);
                            if (newG < ngNode.G)
                            {
                                ngNode.G = newG;
                                ngNode.Parent = s;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static bool LineOfSight(Point s1, Point s2, int [,] grid, Func<int, bool> isWall)
        {
            var x0 = s1.X;
            var y0 = s1.Y;
            var x1 = s2.X;
            var y1 = s2.Y;
            var dy = y1 - y0;
            var dx = x1 - x0;
            int sy, sx;
            var f = 0;
            if (dy < 0)
            {
                dy = -dy;
                sy = -1;
            }
            else
            {
                sy = 1;
            }
            if (dx < 0)
            {
                dx = -dx;
                sx = -1;
            }
            else
            {
                sx = 1;
            }

            if (dx >= dy)
            {
                while (x0 != x1)
                {
                    f = f + dy;
                    if (f >= dx)
                    {
                        if (IsWall(x0 + (sx - 1) / 2, y0 + (sy - 1) / 2, grid, isWall))
                        {
                            return false;
                        }
                        y0 = y0 + sy;
                        f = f - dx;
                    }
                    if (f != 0 && IsWall(x0 + (sx - 1)/2, y0 + (sy - 1)/2, grid, isWall))
                        return false;
                    if (dy == 0 && IsWall(x0 + (sx - 1)/2, y0, grid, isWall) && IsWall(x0 + (sx - 1)/2, y0 - 1, grid, isWall))
                        return false;
                    x0 = x0 + sx;
                }
            }
            else
            {
                while (y0 != y1)
                {
                    f = f + dx;
                    if (f >= dy)
                    {
                        if (IsWall(x0 + (sx - 1) / 2, y0 + (sy - 1) / 2, grid, isWall))
                        {
                            return false;
                        }
                        x0 = x0 + sx;
                        f = f - dy;
                    }
                    if (f != 0 && IsWall(x0 + (sx - 1) / 2, y0 + (sy - 1) / 2, grid, isWall))
                        return false;
                    if (dx == 0 && IsWall(x0, y0 + (sy - 1) / 2, grid, isWall) && IsWall(x0 - 1, y0 + (sy - 1) / 2, grid, isWall))
                        return false;
                    y0 = y0 + sy;
                }               
            }

            return true;
        }

        private List<Point> GetVisNgbr(Point s, int[,] map)
        {
            var result = new List<Point>();

            var p1 = new Point(s.X - 1, s.Y - 1);
            var p2 = new Point(s.X - 1, s.Y);
            var p3 = new Point(s.X - 1, s.Y + 1);
            var p4 = new Point(s.X, s.Y - 1);
            var p5 = new Point(s.X, s.Y + 1);
            var p6 = new Point(s.X + 1, s.Y - 1);
            var p7 = new Point(s.X + 1, s.Y);
            var p8 = new Point(s.X + 1, s.Y + 1);

            AddIfInMapAndNotWall(result, p2, map);
            AddIfInMapAndNotWall(result, p4, map);
            AddIfInMapAndNotWall(result, p5, map);
            AddIfInMapAndNotWall(result, p7, map);
            AddIfInMapAndNotWallDiagonal(result, p1, p2, p4, map);
            AddIfInMapAndNotWallDiagonal(result, p3, p5, p2, map);
            AddIfInMapAndNotWallDiagonal(result, p6, p7, p4, map);
            AddIfInMapAndNotWallDiagonal(result, p8, p5, p7, map);

            return result;
        }

        private void AddIfInMapAndNotWallDiagonal(List<Point> list, Point p, Point pa, Point pb, int[,] map)
        {
            if (IsInMap(p, map) && !IsWall(p, map) && (!IsWall(pa, map) || !IsWall(pb, map))) list.Add(p);
        }

        private void AddIfInMapAndNotWall(List<Point> list, Point p, int[,] map)
        {
            if (IsInMap(p, map) && !IsWall(p, map)) list.Add(p);
        }

        private bool IsWall(Point p, int[,] map)
        {
            return IsWall(p, map, _isWall);
        }

        private static bool IsWall(Point p, int[,] map, Func<int,bool> isWall)
        {
            return !IsInMap(p, map) || isWall(map[p.X, p.Y]);
        }

        private static bool IsWall(int x, int y, int[,] map, Func<int, bool> isWall)
        {
            return !IsInMap(x, y, map) || isWall(map[x, y]);
        }

        private static bool IsInMap(int x, int y, int[,] map)
        {
            return x >= 0 && y >= 0 && x < map.GetLength(0) && y < map.GetLength(1);
        }

        private static bool IsInMap(Point p, int[,] map)
        {
            return IsInMap(p.X, p.Y, map);
        }

        private static double GetDistance(Point pos, Point target)
        {
            return Math.Sqrt(Math.Pow(pos.X - target.X, 2) + Math.Pow(pos.Y - target.Y, 2));
        }

        private static double GetHeurestic(Point pos, Point target)
        {
            return GetDistance(pos, target);
        }

        private static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }

        /// <summary>
        /// Plot the line from (x0, y0) to (x1, y10
        /// </summary>
        /// <param name="x0">The start x</param>
        /// <param name="y0">The start y</param>
        /// <param name="x1">The end x</param>
        /// <param name="y1">The end y</param>
        /// <param name="plot">The plotting function (if this returns false, the algorithm stops early)</param>
        public static bool Line(int x0, int y0, int x1, int y1, Func<int, int, bool> plot)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep) { Swap<int>(ref x0, ref y0); Swap<int>(ref x1, ref y1); }
            if (x0 > x1) { Swap<int>(ref x0, ref x1); Swap<int>(ref y0, ref y1); }
            int dX = (x1 - x0), dY = Math.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

            for (int x = x0; x <= x1; ++x)
            {
                if (!(steep ? plot(y, x) : plot(x, y))) return false;
                err = err - dY;
                if (err < 0) { y += ystep; err += dX; }
            }

            return true;
        }
    }
}
