using GAlgoT2530.AI;
using GAlgoT2530.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content.Tiled;
using MonoGame.Extended.Tiled;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

// Acknowledgement / Honour Code:
// - Pathfinding logic as well as tile cover logic were gotten from the labs material.
// - Online help is used to figure out how to sort lists in C#.
// - AI is used to help debug issues regarding animation state.

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

        private HashSet<Tile> _coverTiles;
        private Texture2D _coverTexture;
        private Rectangle _coverTileRect;

        public GhostStealingHCFSM(GameEngine game, Ghost ghost, TiledMap map, TileGraph graph)
        {
            _path = new LinkedList<Tile>();

            _game = game;
            _ghost = ghost;
            _tiledMap = map;
            _tileGraph = graph;
            _currentPelletIndex = 0;

            _coverTiles = new HashSet<Tile>();
        }

        public override void Initialize()
        {
            _coverTexture = _game.Content.Load<Texture2D>("pacman-wall-24");
            _coverTileRect = new Rectangle(96, 0, 24, 24);

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

            CoverPelletTileWithEmptyTile(pellettile);
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

        public void CoverPelletTileWithEmptyTile(Tile pelletTileLocation)
        {
            TiledMapTileLayer _pelletLayer = _tiledMap.GetLayer<TiledMapTileLayer>("Food");

            bool hasTile = _pelletLayer.TryGetTile((ushort)pelletTileLocation.Col, (ushort)pelletTileLocation.Row, out TiledMapTile? pelletTile);

            if (hasTile)
            {
                // Pellet tile (3) or Power Pellet tile (4)
                if (pelletTile.Value.GlobalIdentifier == 3 || pelletTile.Value.GlobalIdentifier == 4)
                {
                    /********************************************************************************
                        PROBLEM 1 : Fill up the pellet tiles with empty tiles


                        HOWTOSOLVE : 1. Copy the code below.
                                     2. Paste it below this block comment.
                                     3. Fill in the blanks.

                        // Add empty tile only to cover newly discovered pellet tiles
                        if (!_coverTiles.Contains(pelletTileLocation))
                        {
                            _coverTiles.Add(________);

                            // Fires PelletsCleared event if all pellets has been cleared
                            // (i.e. number of cover tiles == number of navigable nodes in the graph)
                            if (_coverTiles.Count == _tileGraph.________.________)
                            {
                                PelletsCleared?.Invoke();
                            }

                            // Restart power pellet count down timer if one is found
                            if (pacmanTile.Value.GlobalIdentifier == 4)
                            {
                                RestartPowerPelletTime();
                            }
                        }
                    ********************************************************************************/

                    if (!_coverTiles.Contains(pelletTileLocation))
                    {
                        _coverTiles.Add(pelletTileLocation);

                        // Fires PelletsCleared event if all pellets has been cleared
                        // (i.e. number of cover tiles == number of navigable nodes in the graph)
                        //if (_coverTiles.Count == _tileGraph.Nodes.Count)
                        //{
                        //    PelletsCleared?.Invoke();
                        //}

                    }
                }
            }
        }

        public void Draw()
        {
            _game.SpriteBatch.Begin();

            // Draw all cover tiles
            foreach (Tile t in _coverTiles)
            {
                Vector2 position = Tile.ToPosition(t, _tiledMap.TileWidth, _tiledMap.TileHeight);
                _game.SpriteBatch.Draw(_coverTexture, position, _coverTileRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }

            _game.SpriteBatch.End();
        }
    }
}
