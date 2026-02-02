using GAlgoT2530.AI;
using GAlgoT2530.Engine;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PacmanGame
{
    public class GhostStealingHCFSM : HCFSM
    {
        public enum State { Home, Stealing, Goal };

        public State CurrentState;

        private GameEngine _game;
        private Ghost _ghost;
        private TiledMap _tiledMap;
        private TileGraph _tileGraph;

        private Tile _srcTile;
        private Tile _destTile;
        private LinkedList<Tile> _path;

        private Vector2 _homePosition;
        private Vector2 _goalPosition;
        private List<Vector2> _pellets;

        private int _currentPelletIndex = 0;
        private Vector2 _nextTilePosition;


        public GhostStealingHCFSM(GameEngine game, Ghost ghost, TiledMap map, TileGraph graph)
        {
            _path = new LinkedList<Tile>();

            _game = game;
            _ghost = ghost;
            _tiledMap = map;
            _tileGraph = graph;
            _currentPelletIndex = 0;
        }

        public override void Initialize()
        {
            LoadWaypoints(_tiledMap);

            CurrentState = State.Stealing;

            _ghost.Position = _homePosition;

            if (_pellets.Count > 0)
            {
                CalculatePathToTarget(_pellets[_currentPelletIndex]);
            }
        }

        public override void Update()
        {
            float elapsedSeconds = ScalableGameTime.DeltaTime;

            if (_ghost.Position == _nextTilePosition)
            {
                if (Vector2.Distance(_ghost.Position, Tile.ToPosition(_destTile, _tiledMap.TileWidth, _tiledMap.TileHeight)) < 1f)
                {
                    ReachedTargetTile();
                }

                else
                {
                    MoveToNextTile();
                }
            }

            _ghost.Position = _ghost.Move(_ghost.Position, _nextTilePosition, elapsedSeconds);
            _ghost.AnimatedSprite.Update(ScalableGameTime.GameTime);
        }

        public void LoadWaypoints(TiledMap map)
        {
            TiledMapObjectLayer waypointLayer = null;

            foreach (var layer in _tiledMap.ObjectLayers)
            {
                if (layer.Name == "Waypoints")
                {
                    waypointLayer = layer;
                    break;
                }
            }

            _pellets = new List<Vector2>();

            foreach (var obj in waypointLayer.Objects)
            {
                Vector2 position = new Vector2(obj.Position.X, obj.Position.Y);

                if (obj.Name == "Home")
                {
                    _homePosition = position;
                }
                else if (obj.Name == "Goal")
                {
                    _goalPosition = position;
                }
                else
                {
                    _pellets.Add(position);
                }
            }

            _pellets = SortPellets(_homePosition, _pellets);
        }

        public List<Vector2> SortPellets(Vector2 homePosition, List<Vector2> pellets)
        {
            Tile homeTile = Tile.ToTile(homePosition, _tiledMap.TileWidth, _tiledMap.TileHeight);

            var pelletDistances = new List<(Vector2 pellet, int distance)>();

            foreach (var pellet in pellets)
            {
                Tile pelletTile = Tile.ToTile(pellet, _tiledMap.TileWidth, _tiledMap.TileHeight);
                var path = AStar.Compute(_tileGraph, homeTile, pelletTile, AStarHeuristic.EuclideanSquared);
                pelletDistances.Add((pellet, path.Count));
            }

            pelletDistances.Sort((a, b) => a.distance.CompareTo(b.distance));

            Debug.WriteLine("Power pellets sorted by distance from Home:");
            for (int i = 0; i < pelletDistances.Count; i++)
            {
                Debug.WriteLine($"  {i + 1}. Position: ({pelletDistances[i].pellet.X}, {pelletDistances[i].pellet.Y}), Path Length: {pelletDistances[i].distance}");
            }

            return pelletDistances.Select(p => p.pellet).ToList();
        }

        private void CalculatePathToTarget(Vector2 targetPosition)
        {
            _srcTile = Tile.ToTile(_ghost.Position, _tiledMap.TileWidth, _tiledMap.TileHeight);
            _destTile = Tile.ToTile(targetPosition, _tiledMap.TileWidth, _tiledMap.TileHeight);

            _path.Clear();
            _path = AStar.Compute(_tileGraph, _srcTile, _destTile, AStarHeuristic.EuclideanSquared);

            if (_path.Count > 0)
            {
                _path.RemoveFirst();

                //THISSSSSSSSSSS
                //_path.RemoveFirst();

                if (_path.Count > 0)
                {
                    Tile nextTile = _path.First.Value;
                    _nextTilePosition = Tile.ToPosition(nextTile, _tiledMap.TileWidth, _tiledMap.TileHeight);

                    if (!_srcTile.Equals(nextTile))
                    {
                        _ghost.UpdateAnimatedSprite(_srcTile, nextTile);
                    }
                }

                else
                {
                    _nextTilePosition = _ghost.Position;
                }

            }
        }

        private void MoveToNextTile()
        {
            if (_path.Count > 0)
            {
                _srcTile = Tile.ToTile(_ghost.Position, _tiledMap.TileWidth, _tiledMap.TileHeight);
                _path.RemoveFirst();

                if (_path.Count > 0)
                {
                    Tile nextTile = _path.First.Value;
                    _nextTilePosition = Tile.ToPosition(nextTile, _tiledMap.TileWidth, _tiledMap.TileHeight);
                    _ghost.UpdateAnimatedSprite(_srcTile, nextTile);
                }
            }
        }

        private void StealPellet(int index)
        {
            Vector2 pelletPosition = _pellets[index];
            Tile pellettile = Tile.ToTile(pelletPosition, _tiledMap.TileWidth, _tiledMap.TileHeight);

            //RemovePellet(pellettile);
        }

        private void ReachedTargetTile()
        {
            if (CurrentState == State.Home)
            {
                CurrentState = State.Stealing;
                CalculatePathToTarget(_pellets[_currentPelletIndex]);
            }

            else if (CurrentState == State.Stealing)
            {
                StealPellet(_currentPelletIndex);
                _currentPelletIndex++;

                if (_currentPelletIndex < _pellets.Count)
                {
                    CurrentState = State.Home;
                    CalculatePathToTarget(_homePosition);
                }
                else
                {
                    CurrentState = State.Goal;
                    CalculatePathToTarget(_goalPosition);
                }
            }

            else if (CurrentState == State.Goal)
            {
                Debug.WriteLine("Ghost reached goal position.");
            }
        }
    }
}
