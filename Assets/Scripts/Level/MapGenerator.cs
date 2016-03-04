using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public class MapGenerator
    {
        public enum TileType
        {
            Wall,
            Free,
            Exit,
            Border // special case for room maps
        }

        private struct Position
        {
            public int X;
            public int Y;

            public static Position operator +(Position p1, Position p2)
            {
                return new Position { X = p1.X + p2.X, Y = p1.Y + p2.Y };
            }
        }

        public class RoomInfo
        {
            public int MinWidth;
            public int MaxWidth;
            public int MinHeight;
            public int MaxHeight;
            public int MinExits;
            public int MaxExits;
            public bool Border;
            public bool CleanDeadEnds;
            public RoomMap RoomMap;
            public int ProbWeight;

            public RoomInfo(int minExits, int maxExits)
            {
                MinExits = minExits;
                MaxExits = maxExits;
                ProbWeight = 1;
            }

            public RoomInfo(int minWidth, int maxWidth, int minHeight, int maxHeight, int minExits, int maxExits, bool border = false)
                : this(minExits, maxExits)
            {
                MinWidth = minWidth;
                MaxWidth = maxWidth;
                MinHeight = minHeight;
                MaxHeight = maxHeight;
                Border = border;
            }
        }

        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }
        public IList<RoomInfo> Rooms { get; set; }

        private readonly Random _random = new Random();
        private readonly Queue<Position> _queue = new Queue<Position>();
        private readonly List<Position> _exitsToCheck = new List<Position>();

        private TileType[,] _tileMap;
        private readonly int _maxTriesToBuildNewRoom = 10;
        private readonly int _maxAdditionalExitsCount = 100;
        private readonly float _minFreeTileRatio;

        private readonly IList<Position> _directions = new List<Position>
        {
            new Position {X = 0, Y = 1},
            new Position {X = 0, Y = -1},
            new Position {X = 1, Y = 0},
            new Position {X = -1, Y = 0},
        };

        public MapGenerator(int width, int height, float minFreeTileRatio)
        {
            if (width < 30 || height < 30) throw new ArgumentOutOfRangeException("width and height must be over 30");

            MapWidth = width;
            MapHeight = height;
            _minFreeTileRatio = minFreeTileRatio;
        }

        public TileType[,] GenerateMap()
        {
            if (Rooms == null || Rooms.Count == 0)
            {
                GenerateDefaultRooms();
            }

            if (Rooms == null) return null;

            FillMapWithWalls();

            BuildCenterRoom();

            BuildAllRooms();

            ClearDeadEnds();

            return _tileMap;
        }

        private void BuildAllRooms()
        {
            int freeTilesCount;
            int count = 0;
            var minFreeTilesCount = MapHeight * MapWidth *_minFreeTileRatio;
            do
            {
                while (_queue.Count > 0)
                {
                    var pos = _queue.Dequeue();

                    var choice = _random.Next(0, Rooms.Count);
                    var roomInfo = Rooms[choice];

                    var counter = 0;
                    var roomBuilt = false;
                    while (counter < _maxTriesToBuildNewRoom && roomBuilt == false)
                    {
                        roomBuilt = BuildRoom(pos, roomInfo);
                        counter++;
                    }
                }

                count++;
                freeTilesCount = GetFreeTilesCount();
                if (freeTilesCount < minFreeTilesCount)
                {
                    AddExtraExit();
                }
            }
            while (freeTilesCount < minFreeTilesCount && count < _maxAdditionalExitsCount);
        }

        private void AddExtraExit()
        {
            var x = _random.Next(0, MapWidth);
            var y = _random.Next(0, MapHeight);
            while (!IsTileWall(x, y) || !HasFreeNeighbor(new Position { X =  x, Y = y}))
            {
                x++;
                if (x == MapWidth)
                {
                    x = 0;
                    y = (y + 1)%MapHeight;
                }
            }

            var pos = new Position {X = x, Y = y};
            AddExitAtPosition(pos);
            _exitsToCheck.Add(pos);
        }

        private bool HasFreeNeighbor(Position pos)
        {
            var freeAround = _directions
                .Select(dir => pos + dir)
                .Count(newPos => !IsPositionOutOfBounds(newPos) && _tileMap[newPos.X, newPos.Y] == TileType.Free);
            return freeAround > 0;
        }

        private int GetFreeTilesCount()
        {
            return ForEachTile(t => t == TileType.Free ? 1 : 0).Sum();
        }

        private IEnumerable<T> ForEachTile<T>(Func<TileType, T> func)
        {
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    yield return func(_tileMap[i, j]);
                }
            }
        }

        private void FillMapWithWalls()
        {
            _tileMap = new TileType[MapWidth, MapHeight];
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    _tileMap[i, j] = TileType.Wall;
                }
            }
        }

        private void ClearDeadEnds()
        {
            foreach (var exit in _exitsToCheck)
            {
                var wallsAround = _directions
                    .Select(dir => exit + dir)
                    .Count(newPos => !IsPositionOutOfBounds(newPos) && _tileMap[newPos.X, newPos.Y] == TileType.Free);

                if (wallsAround < 2)
                {
                    _tileMap[exit.X, exit.Y] = TileType.Wall;
                }

                //var exitDirections = _directions.Select(dir => exit + dir).ToList();
                //var isDoor = (IsTileFree(exitDirections[0]) && IsTileFree(exitDirections[1]))
                //             || (IsTileFree(exitDirections[2]) && IsTileFree(exitDirections[3]));

                //if (!isDoor)
                //{
                //    _tileMap[exit.X, exit.Y] = TileType.Wall;
                //}
            }

            _exitsToCheck.Clear();
        }

        private void GenerateDefaultRooms()
        {
            Rooms = new List<RoomInfo>
            {
                new RoomInfo(3, 5, 3, 5, 2, 4, true) { CleanDeadEnds = true}, // small room
                new RoomInfo(6, 12, 6, 12, 3, 5, true) { CleanDeadEnds = true}, // large room
                new RoomInfo(1, 2, 4, 7, 1, 2, true) { CleanDeadEnds = true}, // small corridor
                new RoomInfo(4, 7, 1, 2, 1, 2, true) { CleanDeadEnds = true}, // small corridor
                new RoomInfo(3, 5, 6, 12, 1, 5, true) { CleanDeadEnds = true}, // large corridor
                new RoomInfo(6, 12, 3, 5, 1, 5, true) { CleanDeadEnds = true},  // large corridor
                new RoomInfo(1, 3) { CleanDeadEnds = true, RoomMap = new RoomMap { // round room
                    Height = 7, 
                    Width = 7, 
                    Map = new[,]
                {
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Border,TileType.Wall,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Border,TileType.Free,TileType.Border,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Border, TileType.Free,TileType.Free,TileType.Free,TileType.Border,TileType.Wall},
                    {TileType.Border, TileType.Free, TileType.Free,TileType.Free,TileType.Free,TileType.Free,TileType.Border},
                    {TileType.Wall, TileType.Border, TileType.Free,TileType.Free,TileType.Free,TileType.Border,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Border,TileType.Free,TileType.Border,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Border,TileType.Wall,TileType.Wall,TileType.Wall}
                }}}
            };
        }

        private bool BuildRoomFromRoomMap(Position pos, RoomInfo room)
        {
            var map = room.RoomMap;
            var entrancePositions = GetEntrancePositionsFromRoomMap(map);
            var start = _random.Next(0, entrancePositions.Count);
            for (int i = 0; i < entrancePositions.Count; i++)
            {
                var entranceNumber = (start + i) % (entrancePositions.Count);
                var entrance = entrancePositions[entranceNumber];
                var roomPosition = new Position { X = pos.X - entrance.X, Y = pos.Y - entrance.Y };
                var isRoomAvailable = IsRoomAvailable(roomPosition, map);

                if (isRoomAvailable)
                {
                    CreateRoom(roomPosition, map);
                    FindAndQueueExits(_random.Next(room.MinExits, room.MaxExits + 1), roomPosition, entrancePositions, room.CleanDeadEnds);
                    return true;
                }
            }

            return false;
        }

        private IList<Position> GetEntrancePositionsFromRoomMap(RoomMap map)
        {
            var result = new List<Position>();
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    if (map.Map[i, j] == TileType.Border)
                    {
                        result.Add(new Position { X = i, Y = j });
                    }
                }
            }

            return result;
        }

        private RoomMap BuildRoomMap(int width, int height, bool useBorder)
        {
            var adjWidth = useBorder ? width + 2 : width;
            var adjHeight = useBorder ? height + 2 : height;
            var map = new RoomMap
            {
                Width = adjWidth,
                Height = adjHeight,
                Map = new TileType[adjWidth, adjHeight]
            };

            for (int i = 0; i < adjWidth; i++)
            {
                for (int j = 0; j < adjHeight; j++)
                {
                    map.Map[i, j] = TileType.Free;
                }
            }

            if (useBorder)
            {
                map.Map[0, 0] = TileType.Wall;
                map.Map[0, adjHeight - 1] = TileType.Wall;
                map.Map[adjWidth - 1, 0] = TileType.Wall;
                map.Map[adjWidth - 1, adjHeight - 1] = TileType.Wall;

                for (int i = 1; i < adjWidth - 1; i++)
                {
                    map.Map[i, 0] = TileType.Border;
                    map.Map[i, adjHeight - 1] = TileType.Border;
                }

                for (int i = 1; i < adjHeight - 1; i++)
                {
                    map.Map[0, i] = TileType.Border;
                    map.Map[adjWidth - 1, i] = TileType.Border;
                }
            }

            return map;
        }

        private bool BuildRoom(Position pos, RoomInfo room)
        {
            var width = _random.Next(room.MinWidth, room.MaxWidth + 1);
            var height = _random.Next(room.MinHeight, room.MaxHeight + 1);

            if (room.RoomMap == null)
            {
                room.RoomMap = BuildRoomMap(width, height, room.Border);
            }

            return BuildRoomFromRoomMap(pos, room);
        }

        private void FindAndQueueExits(int number, Position leftTop, IList<Position> entrancePositions, bool addForDeadEndCheck)
        {
            for (var n = 0; n < number; n++)
            {
                var start = _random.Next(0, entrancePositions.Count);

                for (int i = 0; i <= entrancePositions.Count; i++)
                {
                    var entranceNumber = (start + i) % (entrancePositions.Count);
                    var entrance = entrancePositions[entranceNumber];
                    var entrancePosition = leftTop + entrance;
                    if (IsTileWall(entrancePosition))
                    {
                        AddExitAtPosition(entrancePosition);
                        if (addForDeadEndCheck)
                        {
                            _exitsToCheck.Add(entrancePosition);
                        }
                        break;
                    }
                }
            }
        }

        public class RoomMap
        {
            public TileType[,] Map { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        private bool IsRoomAvailable(Position leftTop, RoomMap roomMap)
        {
            for (int i = 0; i < roomMap.Width; i++)
            {
                for (int j = 0; j < roomMap.Height; j++)
                {
                    var newPos = leftTop + new Position { X = i, Y = j };
                    if (IsPositionOutOfBounds(newPos)) return false;
                    if ((roomMap.Map[i, j] == TileType.Free || roomMap.Map[i, j] == TileType.Border) && _tileMap[newPos.X, newPos.Y] == TileType.Free)
                        return false;
                }
            }

            return true;
        }

        private void CreateRoom(Position leftTop, RoomMap roomMap)
        {
            for (int i = leftTop.X; i < leftTop.X + roomMap.Width; i++)
            {
                for (int j = leftTop.Y; j < leftTop.Y + roomMap.Height; j++)
                {
                    if (roomMap.Map[i - leftTop.X, j - leftTop.Y] == TileType.Free)
                    {
                        _tileMap[i, j] = TileType.Free;
                    }
                }
            }
        }

        private void AddExitAtPosition(Position pos)
        {
            _tileMap[pos.X, pos.Y] = TileType.Exit;
            _queue.Enqueue(pos);
        }

        private void BuildCenterRoom()
        {
            CreateRoom(new Position { X = MapWidth / 2 - 5, Y = MapHeight / 2 - 5 }, new RoomMap {
                    Height = 11, 
                    Width = 11, 
                    Map = new[,]
                {
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Wall,TileType.Free,TileType.Free,TileType.Free,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Wall,TileType.Free,TileType.Free,TileType.Free,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Wall,TileType.Free,TileType.Free,TileType.Free,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Free, TileType.Free,TileType.Free,TileType.Free,TileType.Free,TileType.Free,TileType.Free,TileType.Free,TileType.Free,TileType.Wall},
                    {TileType.Wall, TileType.Free, TileType.Free,TileType.Free,TileType.Free,TileType.Free,TileType.Free,TileType.Free,TileType.Free,TileType.Free,TileType.Wall},
                    {TileType.Wall, TileType.Free, TileType.Free,TileType.Free,TileType.Free,TileType.Free,TileType.Free,TileType.Free,TileType.Free,TileType.Free,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Wall,TileType.Free,TileType.Free,TileType.Free,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Wall,TileType.Free,TileType.Free,TileType.Free,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Wall,TileType.Free,TileType.Free,TileType.Free,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall}
                }
            });

            var exit1 = new Position { X = MapWidth / 2 - 5, Y = MapHeight / 2 };
            var exit2 = new Position { X = MapWidth / 2 + 5, Y = MapHeight / 2 };
            var exit3 = new Position { X = MapWidth / 2, Y = MapHeight / 2 - 5 };
            var exit4 = new Position { X = MapWidth / 2, Y = MapHeight / 2 + 5 };

            AddExitAtPosition(exit1);
            AddExitAtPosition(exit2);
            AddExitAtPosition(exit3);
            AddExitAtPosition(exit4);
        }

        private bool IsTileFree(Position pos)
        {
            return !IsPositionOutOfBounds(pos) && _tileMap[pos.X, pos.Y] == TileType.Free;
        }

        private bool IsTileWall(Position pos)
        {
            return IsTileWall(pos.X, pos.Y);
        }

        private bool IsTileWall(int x, int y)
        {
            return !IsPositionOutOfBounds(x, y) && _tileMap[x, y] == TileType.Wall;
        }

        private bool IsPositionOutOfBounds(int x, int y)
        {
            return x <= 0 || x >= MapWidth - 1 || y <= 0 || y >= MapHeight - 1;
        }

        private bool IsPositionOutOfBounds(Position pos)
        {
            return IsPositionOutOfBounds(pos.X, pos.Y);
        }
    }
}
