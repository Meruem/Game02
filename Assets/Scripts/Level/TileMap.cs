using System;
using UnityEngine;

namespace Assets.Scripts.Level
{
    public class Tile
    {
        private bool _isVisible;
        public int X { get; private set; }
        public int Y { get; private set; }
        public MapGenerator.TileType TileType { get; private set; }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value == _isVisible) return;
                _isVisible = value;
                if (AfterVisibilityChanged != null)
                {
                    AfterVisibilityChanged(this);
                }
            }
        }

        public Action<Tile> AfterVisibilityChanged { get; set; }

        public GameObject GameObject { get; set; }

        public Tile(int x, int y, MapGenerator.TileType tileType)
        {
            X = x;
            Y = y;
            TileType = tileType;
            IsVisible = true;
        }
    }

    public class TileMap
    {
        private readonly Tile[,] _tiles;

        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }

        public TileMap(MapGenerator.TileType[,] map)
        {
            MapWidth = map.GetLength(0);
            MapHeight = map.GetLength(1);
            _tiles = new Tile[MapWidth, MapHeight];
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    _tiles[i, j] = new Tile(i, j, map[i,j]);
                }
            }
        }

        public Tile this[int x, int y]
        {
            get { return _tiles[x, y]; }
        }

    }
}
