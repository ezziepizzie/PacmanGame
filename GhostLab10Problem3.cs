using GAlgoT2530.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PacmanGame
{
    public class GhostLab10Problem3 : AnimationGameObject // GameObject
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
        // public Texture2D Texture;

        // Visual appearance
        private Rectangle _ghostRect;
        private TiledMap _tiledMap;
        private TileGraph _tileGraph;

        public GhostLab10Problem3() : base("ghost-animations.sf")
        {
        }

        // Commented out as the Animation is now loaded in the base class
        //public override void LoadContent()
        //{
        //    Texture = _game.Content.Load<Texture2D>("pacman-sprites");
        //}

        public override void Initialize()
        {
            MaxSpeed = 100.0f;

            GameMap gameMap = (GameMap)GameObjectCollection.FindByName("GameMap");
            _tiledMap = gameMap.TiledMap;
            _tileGraph = gameMap.TileGraph;

            // Commented out as the Texture is not used anymore
            //_ghostRect = new Rectangle
            //{
            //    X = 0 * _tiledMap.TileWidth,
            //    Y = 6 * _tiledMap.TileHeight,
            //    Width = _tiledMap.TileWidth,
            //    Height = _tiledMap.TileHeight
            //};

        /********************************************************************************
            PROBLEM 3(A): Initialise the animation.


            HOWTOSOLVE : 1. Copy the code below.
                         2. Paste it below this block comment.
                         3. Fill in the blanks.

            // Initialize Animation to "ghostRedDown".
            AnimatedSprite.Set________(________);
            AnimatedSprite.TextureRegion = Sprite________.Texture________[AnimatedSprite.Controller.Current________];

        ********************************************************************************/

            AnimatedSprite.SetAnimation("ghostRedDown");
            AnimatedSprite.TextureRegion = SpriteSheet.TextureAtlas[AnimatedSprite.Controller.CurrentFrame];

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

                    if (_tileGraph.Nodes.Contains(_destTile) &&
                        !_destTile.Equals(_srcTile)
                       )
                    {
                        // Transition Actions
                        // 1. Compute an A* path
                        _path = AStar.Compute(_tileGraph, _srcTile, _destTile, AStarHeuristic.EuclideanSquared);
                        // 2. Remove the source tile from the path
                        _path.RemoveFirst();

                    /********************************************************************************
                        PROBLEM 3(C): Switch animation based on the source tile and the next tile.


                        HOWTOSOLVE : 1. Copy the code below.
                                     2. Paste it below this block comment.
                                     3. Fill in the blanks.

                        // The animation to play is determined based on difference between:
                        // (a) The tile the ghost is standing on (i.e. the source tile in this case)
                        // (b) The next tile the ghost will move towards
                        //     (i.e. the first tile in the path after the source tile is removed)
                        UpdateAnimatedSprite(________, ________);

                    ********************************************************************************/

                        UpdateAnimatedSprite(_srcTile, _path.First.Value);

                        // Change to MOVING state
                        _currentState = NavigationState.MOVING;
                    }

                    // NOTE: No action to execute for STOP state
                }
            }
            else if (_currentState == NavigationState.MOVING)
            {
                float elapsedSeconds = ScalableGameTime.DeltaTime;

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
                        Debug.WriteLine($"Reached the next tile (Col = {nextTile.Col}, Row = {nextTile.Row}).");
                        Debug.WriteLine($"Removing this tile from the path and getting the new next tile from path.");
                        

                    /********************************************************************************
                        PROBLEM 3(C): Update the animation based on the current tile and next tile .


                        HOWTOSOLVE : 1. Copy the code below.
                                     2. Paste it below this block comment.
                                     3. Fill in the blanks.

                        // Get the position of the new next tile from the path
                        _path.RemoveFirst();
                        Tile newNextTile = _path.________.________;
                        nextTilePosition = Tile.ToPosition(________, tileWidth, ________);

                        // Update the animation
                        UpdateAnimatedSprite(nextTile, ________);

                    ********************************************************************************/
                        
                        _path.RemoveFirst();
                        Tile newNextTile = _path.First.Value;
                        nextTilePosition = Tile.ToPosition(newNextTile, tileWidth, tileHeight);

                        UpdateAnimatedSprite(nextTile, newNextTile);
                    }

                    // Move the ghost to the new tile location
                    Position = Move(Position, nextTilePosition, elapsedSeconds);

                /********************************************************************************
                    PROBLEM 3(C): Running the ghost animation.


                    HOWTOSOLVE : 1. Copy the code below.
                                 2. Paste it below this block comment.
                                 3. Fill in the blanks.

                    AnimatedSprite.Update(________);

                ********************************************************************************/
                    
                    AnimatedSprite.Update(ScalableGameTime.GameTime);
                }
            }
        }

        public override void Draw()
        {
            // Draw the ghost at his position, extracting only the ghost image from the texture
            _game.SpriteBatch.Begin();

            // Commented out as Texture is not used anymore
            // _game.SpriteBatch.Draw(Texture, Position, _ghostRect, Color.White, Orientation, Origin, Scale, SpriteEffects.None, 0f);

            /********************************************************************************
                PROBLEM 3(D): Draw the animation using extended SpriteBatch.Draw() for AnimatedSprite at a given position, orientation, and scale.

                HOWTOSOLVE : 1. Copy the code below.
                             2. Paste it below this block comment.
                             3. Fill in the blanks.

                
                // _game.SpriteBatch.Draw(________, Position, ________, Scale);

            ********************************************************************************/

            _game.SpriteBatch.Draw(AnimatedSprite, Position, Orientation, Scale);

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

        // Select the ghost current animation based on:
        // (a) Which tile the ghost is standing on (ghostTile)
        // (b) Which tile the ghost is heading next (nextTile)
        //
        // Pre-conditions:
        //    The animation name is suffixed by:
        //      "NorthWest", "Up", "NorthEast", "Left", "Centre", "Right", "SouthWest", "Down", "SouthEast"
        //
        // Example:
        //    If nextTile is on the right of ghostTile, the animation to play is "ghostRedRight".
        public void UpdateAnimatedSprite(Tile ghostTile, Tile nextTile)
        {
            string[] directions = {"NorthWest", "Up"    , "NorthEast",
                                   "Left"     , "Centre", "Right"    ,
                                   "SouthWest", "Down"  , "SouthEast"};

            if (ghostTile == null || nextTile == null)
            {
                throw new ArgumentNullException("UpdateAnimatedSprite(): ghostTile or nextTile is null.");
            }
            else
            {
                /********************************************************************************
                    PROBLEM 3(B): Compute the index value that refer to the correct animation
                                  suffix in the 'directions' array based on ghost tile and next
                                  tile.


                    HOWTOSOLVE : 1. Write your own code.

                    // You may write more lines of code before the code below to compute the index.
                    int index = ?;

                ********************************************************************************/

                //Write
                //the code to calculate the index value which is used to index the directions array to
                //get the correct direction suffix to select the correct animation name.
                
                Tile difference = new Tile(nextTile.Col - ghostTile.Col, nextTile.Row - ghostTile.Row);
                int index = (difference.Col + 1) + 3 * (difference.Row + 1);

                string animationName = $"ghostRed{directions[index]}";

                if (AnimatedSprite.CurrentAnimation != animationName)
                {
                    AnimatedSprite.SetAnimation(animationName);
                    AnimatedSprite.TextureRegion = SpriteSheet.TextureAtlas[AnimatedSprite.Controller.CurrentFrame];
                }
            }
        }
    }
}
