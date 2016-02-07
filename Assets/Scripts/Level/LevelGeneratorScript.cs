using System;
using Assets.Scripts.Level;
using Assets.Scripts.LevelObjectives;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class LevelGeneratorScript : MonoBehaviour
    {
        public int MapWidth;
        public int MapHeight;
        public int MonsterCount = 30;

        public int VisibilityRadius = 9;
        public int MaxRadius = 12;

        public Transform WallTile;
        public Transform GroudTile;
        public Transform Player;
        public Transform Monster;

        private readonly int _maxTries = 200;
        private TileMap _map;
        private GameObject _tilesGameObject;
        private GameObject _monstersGameObject;

        private ILevelObjective _levelObjective;
        private bool _isRestarting;

        public void Awake()
        {
            _tilesGameObject = new GameObject("Tiles");
            _tilesGameObject.transform.parent = transform;

            _monstersGameObject = new GameObject("Monsters");
            _monstersGameObject.transform.parent = transform;

            _levelObjective = new KillAllMonstersObjective(_monstersGameObject);

            Restart();
        }

        public void Update()
        {
            if (_levelObjective.IsCompleted())
            {
                Restart();
            }

            var currentTile = GetTileFromPosition(Player.position);
            ApplyTileVisibility(currentTile.X, currentTile.Y, VisibilityRadius, MaxRadius);
        }

        public void Restart()
        {
            if (_isRestarting) return;

            _isRestarting = true;
            ClearTiles();
            ClearMonsters();

            var bounds = WallTile.GetComponent<Renderer>().bounds.size;
            GenerateTiles(MapWidth, MapHeight, bounds.x, bounds.y);
            var startPosition = GetRandomFreePosition();
            Player.position = startPosition;

            GenerateMonsters();
            ApplyTileVisibility(0, 0, 0, 1000);

            _isRestarting = false;
        }

        public void ApplyTileVisibility(int x, int y, int visibilityRadius, int maxRadius)
        {
            for (int i = Math.Max(0, x - maxRadius); i < Math.Min(_map.MapWidth, x + maxRadius); i++)
            {
                for (int j = Math.Max(0, y - maxRadius); j < Math.Min(_map.MapHeight, y + maxRadius); j++)
                {
                    var r2 = (x - i)*(x - i) + (y - j)*(y - j);
                    _map[i, j].IsVisible = r2 < visibilityRadius*visibilityRadius;
                }
            }
        }

        private void ClearTiles()
        {
            for (int i = _tilesGameObject.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(_tilesGameObject.transform.GetChild(i).gameObject);
            }
        }

        private void ClearMonsters()
        {
            for (int i = _monstersGameObject.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(_monstersGameObject.transform.GetChild(i).gameObject);
            }
        }

        private void GenerateTiles(int mapWidth, int mapHeight, float tileWidth, float tileHeight)
        {
            var map = new MapGenerator(mapWidth, mapHeight);
            _map = new TileMap(map.TileMap);
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    var tile = map.TileMap[i, j];
                    Transform newTransform = null;

                    switch (tile)
                    {
                        case MapGenerator.TileType.Wall:
                            newTransform = (Transform)Instantiate(WallTile, new Vector3(i * tileWidth, j * tileHeight, 0), Quaternion.identity);

                            break;
                        case MapGenerator.TileType.Free:
                            newTransform = (Transform)Instantiate(GroudTile, new Vector3(i * tileWidth, j * tileHeight, 0), Quaternion.identity);
                            break;
                    }

                    if (newTransform != null)
                    {
                        newTransform.transform.parent = _tilesGameObject.transform;
                        _map[i, j].GameObject = newTransform.gameObject;
                        _map[i, j].AfterVisibilityChanged =
                            t =>
                            {
                                var spriteRenderer = t.GameObject.GetComponent<SpriteRenderer>();
                                if (spriteRenderer != null)
                                {
                                    spriteRenderer.enabled = t.IsVisible;
                                }
                            };
                    }
                }
            }
        }

        private void GenerateMonsters()
        {
            for (int i = 0; i < MonsterCount; i++)
            {
                var position = GetRandomFreePosition();
                var monster = (Transform)Instantiate(Monster, position, Quaternion.identity);
                monster.parent = _monstersGameObject.transform;
            }
        }

        private Tile GetTileFromPosition(Vector2 position)
        {
            var bounds = WallTile.GetComponent<Renderer>().bounds.size;
            var x = position.x / bounds.x;
            var y = position.y / bounds.y;

            return _map[(int) x, (int) y];
        }

        private Vector2 GetRandomFreePosition()
        {
            var position = Vector2.zero;
            int count = 0;
            var bounds = WallTile.GetComponent<Renderer>().bounds.size;

            while (position == Vector2.zero && count <= _maxTries)
            {
                var x = Random.Range(0, _map.MapWidth - 1);
                var y = Random.Range(0, _map.MapHeight - 1);

                if (_map[x, y].TileType == MapGenerator.TileType.Free)
                {
                    position = new Vector2(bounds.x * x, bounds.y * y);
                }

                count++;
            }

            return position;
        }
    }
}