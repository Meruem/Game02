using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts
{
    public class LevelGenerator : MonoBehaviour
    {
        private readonly int _maxTries = 200;
        private readonly Random _random = new Random();
        private MapGenerator _map;

        public Transform Tile;
        public int MapWidth;
        public int MapHeight;
        public Transform Player;
        public Transform Monster;
        public int MonsterCount = 30;

        public void Awake()
        {
            var bounds = Tile.renderer.bounds.size;
            GenerateTiles(MapWidth, MapHeight, bounds.x, bounds.y);
            var startPosition = GetRandomFreePosition();
            Player.position = startPosition;

            GenerateMonsters();
        }

        private void GenerateTiles(int mapWidth, int mapHeight, float tileWidth, float tileHeight)
        {
            var levelTransform = GameObject.Find("Level").transform;

            _map = new MapGenerator(mapWidth, mapHeight);
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    if (_map.TileMap[i, j] == MapGenerator.TileType.Wall)
                    {
                        var tile = (Transform)Instantiate(Tile, new Vector3(i * tileWidth, j * tileHeight, 0), Quaternion.identity);
                        tile.transform.parent = levelTransform;
                    }
                }
            }
        }

        private void GenerateMonsters()
        {
            for (int i = 0; i < MonsterCount; i++)
            {
                var position = GetRandomFreePosition();
                Instantiate(Monster, position, Quaternion.identity);
            }
        }

        private Vector2 GetRandomFreePosition()
        {
            var position = Vector2.zero;
            int count = 0;
            var bounds = Tile.renderer.bounds.size;

            while (position == Vector2.zero && count <= _maxTries)
            {
                var x = _random.Next(0, _map.MapWidth);
                var y = _random.Next(0, _map.MapHeight);

                if (_map.TileMap[x, y] == MapGenerator.TileType.Free)
                {
                    position = new Vector2(bounds.x * x, bounds.y * y);
                }

                count++;
            }

            return position;
        }
    }
}