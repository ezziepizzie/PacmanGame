using GAlgoT2530.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tiled;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Drawing;

namespace PacmanGame
{
    public class GhostLab09 : GameObject
    {
        // FSM for navigation
        enum NavigationState { STOP, MOVING };

        // Navigation current state
        private NavigationState _currentState = NavigationState.STOP;

        // Navigation
        private Tile _srcTile;
        private Tile _destTile;
        private LinkedList<Tile> _path;

        // Attributes
        public float MaxSpeed;
        public Texture2D Texture;

        // Visual appearance
        private Rectangle _ghostRect;
        private TiledMap _tiledMap;
        private TileGraph _tileGraph;

        public GhostLab09() : base()
        {
        }

        public override void LoadContent()
        {
            Texture = _game.Content.Load<Texture2D>("pacman-sprites");
        }

        public override void Initialize()
        {
            MaxSpeed = 100.0f;

            GameMap gameMap = (GameMap)GameObjectCollection.FindByName("GameMap");
            _tiledMap = gameMap.TiledMap;
            _tileGraph = gameMap.TileGraph;

            /********************************************************************************
                PROBLEM 4: Determine the location of the red ghost in "pacman-sprites.png".


                HOWTOSOLVE : 1. Copy the code below.
                             2. Paste it below this block comment.
                             3. Fill in the blanks.


                _ghostRect = new Rectangle
                {
                    X       = ________ * _tiledMap.TileWidth,
                    Y       = ________ * _tiledMap.TileHeight,
                    Width   = _tiledMap.________,
                    Height  = _tiledMap.________
                };
            ********************************************************************************/

            //Since the red ghost is just a region within the pacman-sprites.png file, we must specify the
            //rectangle area of the red ghost in the image to display it.

            _ghostRect = new Rectangle
            {
                X       = 0 * _tiledMap.TileWidth,
                Y       = 6 * _tiledMap.TileHeight,
                Width   = _tiledMap.TileWidth,
                Height  = _tiledMap.TileHeight
            };



            // Initialize Source Tile
            _srcTile = new Tile(gameMap.StartColumn, gameMap.StartRow);

            // Initialize Position
            Position = Tile.ToPosition(_srcTile, _tiledMap.TileWidth, _tiledMap.TileHeight);
        }

        public override void Update()
        {
            MouseState mouse = Mouse.GetState();

            int tileWidth = _tiledMap.TileWidth;
            int tileHeight = _tiledMap.TileHeight;

            // Implement the movement behaviour
            if (_currentState == NavigationState.STOP)
            {
                // Left mouse button pressed
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    // Get destination tile as the mouse-selected tile
                    _destTile = Tile.ToTile(mouse.Position.ToVector2(), tileWidth, tileHeight);

				/********************************************************************************
					PROBLEM 5: Fill in the blanks based on the logic below:

								IF (1) The destination tile is in the tile graph.
								   (2) The destination tile is not the source tile.

					HOWTOSOLVE : 1. Fill in the blanks (Codes already copied).

					if (_tileGraph.________.________(________) &&
						!________.Equals(________)
					   )
				********************************************************************************/

                    if (_tileGraph.Nodes.Contains(_destTile) &&
                        !_destTile.Equals(_srcTile)
                       )
                    {
                        // Transition Actions
                        // 1. Compute an A* path
                        _path = AStar.Compute(_tileGraph, _srcTile, _destTile, AStarHeuristic.EuclideanSquared);
                        // 2. Remove the source tile from the path
                        _path.RemoveFirst();

                        // Change to MOVING state
                        _currentState = NavigationState.MOVING;
                    }

                    // NOTE: No action to execute for STOP state
                }
            }
            else if (_currentState == NavigationState.MOVING)
            {
                float elapsedSeconds = ScalableGameTime.DeltaTime;

            /********************************************************************************
                PROBLEM 5: Fill in the blanks based on the logic below:

                            IF (1) The A* path is empty, OR
                               (2) The ghost has reached the destination tile
                                    (i.e., both positions of the ghost and the destination tile match).

                HOWTOSOLVE : 1. Fill in the blanks (Codes already copied).

                if (_path.Count == ________ ||
                    ________.Equals(Tile.ToPosition(________, tileWidth, ________))
                    )
            ********************************************************************************/

                if (_path.Count == 0 ||
                    Position.Equals(Tile.ToPosition(_destTile, tileWidth, tileHeight))
                    )
                {
                    // Update source tile to destination tile
                    _srcTile = _destTile;
                    _destTile = null;

                    // Change to STOP state
                    _currentState = NavigationState.STOP;
                }

                // Action to execute on the MOVING state
                else
                {
                    Tile nextTile = _path.First.Value; // throw exception if path is empty

                    Vector2 nextTilePosition = Tile.ToPosition(nextTile, tileWidth, tileHeight);

                    if (Position.Equals(nextTilePosition))
                    {
                        Debug.WriteLine($"Reach the next tile (Col = {nextTile.Col}, Row = {nextTile.Row}).");

                        
                        Debug.WriteLine($"Removing the head tile of the path. Getting the next tile from path.");
                        _path.RemoveFirst();

                        // Get the next destination position
                        nextTile = _path.First.Value; // throw exception if path is empty
                    }

                    // Move the ghost to the new tile location
                    Position = Move(Position, nextTilePosition, elapsedSeconds);
                }
            }
        }

        public override void Draw()
        {
            // Draw the ghost at his position, extracting only the ghost image from the texture
            _game.SpriteBatch.Begin();

        /********************************************************************************
            PROBLEM 4: Fill in the blanks based on the logic below:

            HOWTOSOLVE : 1. Copy the code below.
                         2. Paste it below this block comment.
                         3. Fill in the blanks.
                
            
            _game.SpriteBatch.Draw(________, Position, ________, Color.White, Orientation, Origin, Scale, SpriteEffects.None, 0f);

        ********************************************************************************/

            _game.SpriteBatch.Draw(Texture, Position, _ghostRect, Color.White, Orientation, Origin, Scale, SpriteEffects.None, 0f);




            _game.SpriteBatch.End();
        }

        // Given source (src) and destination (dest) locations, and elapsed time, 
        //     try to move from source to destination at the given speed within elapsed time.
        // If cannot reach dest within the elapsed time, return the location where it will reach.
        // If can reach or overshoot the dest, the return dest.
        public Vector2 Move(Vector2 src, Vector2 dest, float elapsedSeconds)
        {
            Vector2 dP = dest - src;
            float distance = dP.Length();
            float step = MaxSpeed * elapsedSeconds;

            if (step < distance)
            {
                dP.Normalize();
                return src + (dP * step);
            }
            else
            {
                return dest;
            }
        }
    }
}
