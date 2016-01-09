using Assets.Scripts.LevelObjectives;
using UnityEngine;

namespace Assets.Scripts
{
    public class LevelGenerator : MonoBehaviour
    {
        public int MapWidth;
        public int MapHeight;
        public int MonsterCount = 30;

        public Transform WallTile;
        public Transform GroudTile;
        public Transform Player;
        public Transform Monster;

        private readonly int _maxTries = 200;
        private MapGenerator _map;
        private GameObject _tilesGameObject;
        private GameObject _monstersGameObject;

        private ILevelObjective _levelObjective;
        private bool _isRestarting;

        public void Awake()
        {
            var levelTransform = GameObject.Find("Level").transform;

            _tilesGameObject = new GameObject("Tiles");
            _tilesGameObject.transform.parent = levelTransform;

            _monstersGameObject = new GameObject("Monsters");
            _monstersGameObject.transform.parent = levelTransform;

            _levelObjective = new KillAllMonstersObjective(_monstersGameObject);

            Restart();
        }

        public void Update()
        {
            if (_levelObjective.IsCompleted())
            {
                Restart();
            }
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

            _isRestarting = false;
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
            _map = new MapGenerator(mapWidth, mapHeight);
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    var tile = _map.TileMap[i, j];

                    switch (tile)
                    {
                        case MapGenerator.TileType.Wall:
                            var tileGameObject = (Transform)Instantiate(WallTile, new Vector3(i * tileWidth, j * tileHeight, 0), Quaternion.identity);
                            tileGameObject.transform.parent = _tilesGameObject.transform;
                            break;
                        case MapGenerator.TileType.Free:
                            var woodGameObject = (Transform)Instantiate(GroudTile, new Vector3(i * tileWidth, j * tileHeight, 0), Quaternion.identity);
                            woodGameObject.transform.parent = _tilesGameObject.transform;
                            break;
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

        private Vector2 GetRandomFreePosition()
        {
            var position = Vector2.zero;
            int count = 0;
            var bounds = WallTile.GetComponent<Renderer>().bounds.size;

            while (position == Vector2.zero && count <= _maxTries)
            {
                var x = Random.Range(0, _map.MapWidth - 1);
                var y = Random.Range(0, _map.MapHeight - 1);

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