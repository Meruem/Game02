using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class MapGenerator
    {
        public enum TileType
        {
            Wall,
            Free
        }

        private readonly Random _random = new Random();
        private readonly Queue<Position> _queue = new Queue<Position>();

        private struct Position 
        {
            public int X;
            public int Y;

            public static Position operator +(Position p1, Position p2)
            {
                return new Position { X = p1.X + p2.X, Y = p1.Y + p2.Y };
            }
        }

        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }
        public TileType[,] TileMap { get; private set; }

        public MapGenerator(int width, int height)
        {
            if (width < 30 || height < 30) throw new ArgumentOutOfRangeException("limits must be over 30");

            MapWidth = width;
            MapHeight = height;

            TileMap = new TileType[width, height];
            for (int i = 0; i < width; i++ )
            {
                for (int j = 0; j < height; j++)
                {
                    TileMap[i, j] = TileType.Wall;
                }
            }

            BuildCenterRoom();

            while (_queue.Count > 0)
            {
                var pos = _queue.Dequeue();

                var choice = _random.Next(0, 6);
                switch (choice)
                {
                    case 0:
                        // small room
                        BuildRoom(pos, 3, 5, 3, 5, 1, 3);
                        break;
                    case 1:
                        // large room
                        BuildRoom(pos, 6, 12, 6, 12, 2, 5);
                        break;
                    case 2:
                        // small corridor
                        BuildRoom(pos, 1, 2, 4, 7, 1, 2);
                        break;
                    case 3:
                        // small corridor
                        BuildRoom(pos, 4, 7, 1, 2, 1, 2);
                        break;
                    case 4:
                        // large corridor
                        BuildRoom(pos, 3, 5, 6, 12, 1, 5);
                        break;
                    case 5:
                        // large corridor
                        BuildRoom(pos, 6, 12, 3, 5, 1, 5);
                        break;
                }
            }
        }


        private void BuildRoom(Position pos, int minWidth, int maxWidth, int minHeight, int maxHeight, int minExits, int maxExits, bool border = false)
        {
            var width = _random.Next(minWidth, maxWidth + 1);
            var height = _random.Next(minHeight, maxHeight + 1);

            var entrancePositions = CreateEntranceList(width, height);

            var start = _random.Next(0, (width + height) * 2);

            for (int i = 0; i <= (width + height) * 2; i++)
            {
                var entranceNumber = (start + i) % ((width + height) * 2);
                var entrance = entrancePositions[entranceNumber];

                var roomPosition = new Position { X = pos.X - entrance.X, Y = pos.Y - entrance.Y };

                if (IsRoomAvailable(roomPosition, width, height, border))
                {
                    CreateRoom(roomPosition, width, height);
                    FindAndQueueExits(_random.Next(minExits, maxExits + 1), roomPosition, width, height);
                    break;
                }
            }
        }

        private void FindAndQueueExits(int number, Position leftTop, int width, int height)
        {
            for (var n = 0; n < number; n++)
            {
                var entrancePositions = CreateEntranceList(width, height);
                var start = _random.Next(0, (width + height) * 2);

                for (int i = 0; i <= (width + height) * 2; i++)
                {
                    var entranceNumber = (start + i) % ((width + height) * 2);
                    var entrance = entrancePositions[entranceNumber];
                    var entrancePosition = leftTop + entrance;
                    if (IsTileWall(entrancePosition))
                    {
                        AddRoomAtPosition(entrancePosition);
                        break;
                    }
                }
            }
        }

        private Position[] CreateEntranceList(int width, int height)
        {
            var entrancePositions = new Position[(width + height) * 4];

            for (int i = 0; i < width; i++)
            {
                entrancePositions[i] = new Position { X = i, Y = -1 };
                entrancePositions[width + i] = new Position { X = i, Y = height };
            }

            for (int i = 0; i < height; i++)
            {
                entrancePositions[2 * width + i] = new Position { X = width, Y = i };
                entrancePositions[2 * width + height + i] = new Position { X = -1, Y = i };
            }

            return entrancePositions;
        }

        private bool IsRoomAvailable(Position leftTop, int width, int height, bool border)
        {
            var pos = border ? leftTop + new Position { X = -1, Y = -1 } : leftTop;
            var w = border ? width + 2 : width;
            var h = border ? height + 2 : height;

            for (int i = pos.X; i < pos.X + w; i++)
            {
                for (int j = pos.Y; j < pos.Y + h; j++)
                {
                    if (!IsTileWall(i,j)) return false;
                }
            }

            return true;
        }

        private void CreateRoom(Position leftTop, int width, int height)
        {
            for (int i = leftTop.X; i < leftTop.X + width; i++)
            {
                for (int j = leftTop.Y; j < leftTop.Y + height; j++)
                {
                    TileMap[i, j] = TileType.Free;
                }
            }
        }

        private void AddRoomAtPosition(Position pos)
        {
            TileMap[pos.X, pos.Y] = TileType.Free;
            _queue.Enqueue(pos);
        }

        private void BuildCenterRoom()
        {
            CreateRoom(new Position { X = MapWidth / 2 - 5, Y = MapHeight / 2 - 5}, 10, 10);

            var exit1 = new Position { X = MapWidth / 2 - 6, Y = MapHeight / 2 };
            var exit2 = new Position { X = MapWidth / 2 + 5, Y = MapHeight / 2 };
            var exit3 = new Position { X = MapWidth / 2, Y = MapHeight / 2 - 6 };
            var exit4 = new Position { X = MapWidth / 2, Y = MapHeight / 2 + 5 };

            AddRoomAtPosition(exit1);
            AddRoomAtPosition(exit2);
            AddRoomAtPosition(exit3);
            AddRoomAtPosition(exit4);
        }

        private bool IsTileWall(int x, int y)
        {
            return IsTileWall(new Position { X = x, Y = y });
        }


        private bool IsTileWall(Position pos)
        {
            return pos.X > 0 && pos.X < MapWidth - 1 && pos.Y > 0 && pos.Y < MapHeight - 1 && TileMap[pos.X, pos.Y] == TileType.Wall;
        }
    }
}
