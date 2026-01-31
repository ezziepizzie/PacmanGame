using GAlgoT2530.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PacmanGame
{
    public class PathfindingTester : GameObject
    {
        enum SelectState { Begin, SourceSelected, SourceConfirmed, DestinationSelected };

        // Current state of a finite state machine for path input selection
        private SelectState _currentState = SelectState.Begin;

        // Tile graph and map
        private TileGraph _tileGraph;
        private TiledMap _tiledMap;

        // Textures
        private Texture2D _graphTexture;
        private Texture2D _pathTexture;

        // Input source and destination point
        private Point _srcPoint;
        private Point _destPoint;

        // Path
        private LinkedList<Tile> _path;

        public PathfindingTester(string name) : base(name)
        {
            _srcPoint = Point.Zero;
            _destPoint = Point.Zero;
            _path = new LinkedList<Tile>();
        }

        public override void LoadContent()
        {
            _graphTexture = _game.Content.Load<Texture2D>("graph-directions");
            _pathTexture = _game.Content.Load<Texture2D>("path");
        }

        public override void Initialize()
        {
            // Get tile graph and tiled map from game map
            GameMap gameMap = (GameMap)GameObjectCollection.FindByName("GameMap");
            _tileGraph = gameMap.TileGraph;
            _tiledMap = gameMap.TiledMap;
        }

        public override void Update()
        {
            MouseState mouseState = Mouse.GetState();

            // Check finite state machine
            if (_currentState == SelectState.Begin)
            {
                // Transition to SourceSelected state
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    _path.Clear();
                    _srcPoint = mouseState.Position;
                    _currentState = SelectState.SourceSelected;
                }
            }
            else if (_currentState == SelectState.SourceSelected)
            {
                // Transition to SourceConfirmed state
                if (mouseState.LeftButton == ButtonState.Released)
                {
                    _currentState = SelectState.SourceConfirmed;
                }
            }
            else if (_currentState == SelectState.SourceConfirmed)
            {
                // Transition to DestinationSelected state
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    _destPoint = mouseState.Position;
                    _currentState = SelectState.DestinationSelected;
                }
            }
            else if (_currentState == SelectState.DestinationSelected)
            {
                // Run A* to compute an optimum path and immediately transit to Begin state.
                if (mouseState.LeftButton == ButtonState.Released)
                {
                    // This is the undefined "DestinationConfirmed" state.
                    // In this state, A* is used to compute the path.
                    // Immediately thereafter, the state is transitioned back to Begin state.

                    try
                    {
                        /********************************************************************************
                            Problem 3 (a): Perform quantization by calculating 'srcTile' and 'destTile'
                                        using '_srcPoint' and '_destPoint' respectively.

                            HOWTOSOLVE : 1. Copy the code below.
                                         2. Paste it below this block comment.
                                         3. Fill in the blanks.

                            Tile srcTile = Tile.ToTile(________, _tiledMap.________, _tiledMap.________);
                            Tile destTile = Tile.ToTile(________, _tiledMap.________, _tiledMap.________);

                        ********************************************************************************/

                        Tile srcTile = Tile.ToTile(_srcPoint.ToVector2(), _tiledMap.TileWidth, _tiledMap.TileHeight);
                        Tile destTile = Tile.ToTile(_destPoint.ToVector2(), _tiledMap.TileWidth, _tiledMap.TileHeight);

                        /********************************************************************************
                            Problem 3(b): Compute optimum path using EuclideanSquared as heuristic function.
                                          (Check out AStarHeuristic.cs for pre-written heuristic functions)

                            HOWTOSOLVE : 1. Copy the code below.
                                         2. Paste it below this block comment.
                                         3. Fill in the blanks.

                            _path = AStar.Compute(________, ________, ________, ________);

                        ********************************************************************************/

                        _path = AStar.Compute(_tileGraph, srcTile, destTile, AStarHeuristic.EuclideanSquared);



                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error: Exception thrown when trying to compute A* path.");
                        Debug.WriteLine($"\tThe selected source or destination tile may be invalid.");
                        Debug.WriteLine($"\tMessage: {ex.Message}");
                    }

                    // Switch back to Begin state immediately
                    _currentState = SelectState.Begin;
                }
            }
        }

        public override void Draw()
        {
            _game.SpriteBatch.Begin();

            // Commented on purpose to hide tile graph.
            // DrawTileGraph();

            // Draw the AStar path (if any)
            DrawPath();

            _game.SpriteBatch.End();
        }

        private void DrawTileGraph()
        {
            int[] count = { 0, 1, 0, 8, 2, 0, 4, 0 };

            foreach (Tile tile in _tileGraph.Nodes)
            {
                Vector2 position = new Vector2(tile.Col * _tiledMap.TileWidth, tile.Row * _tiledMap.TileHeight);
                Rectangle rect = new Rectangle();

                ulong[] weights = _tileGraph.Connections[tile];
                int index = 0;

                for (int i = 0; i < weights.Length; ++i)
                    if (weights[i] != 0)
                        index += count[i];

                rect.X = index * _graphTexture.Height;
                rect.Y = 0;
                rect.Width = rect.Height = _graphTexture.Height;

                _game.SpriteBatch.Draw(_graphTexture, position, rect, Color.White);
            }
        }

        private void DrawPath()
        {
            Vector2 position = Vector2.Zero;

            Rectangle rectTexture;
            rectTexture.Width = _tiledMap.TileWidth;
            rectTexture.Height = _tiledMap.TileHeight;

            for (LinkedListNode<Tile> cur = _path.First; cur != null; cur = cur.Next)
            {
                int index = 4; // Refers to the centre tile from path texture (no direction)
                if (cur.Next != null)
                {
                    LinkedListNode<Tile> next = cur.Next;

                    int dCol = next.Value.Col - cur.Value.Col;
                    int dRow = next.Value.Row - cur.Value.Row;

                    index = (dCol + 1) + 3 * (dRow + 1);
                }

                position.X = cur.Value.Col * _tiledMap.TileWidth;
                position.Y = cur.Value.Row * _tiledMap.TileHeight;

                rectTexture.X = index * rectTexture.Width;
                rectTexture.Y = 0;

                _game.SpriteBatch.Draw(_pathTexture, position, rectTexture, Color.White);
            }
        }
    }
}
