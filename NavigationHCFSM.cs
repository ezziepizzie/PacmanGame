using GAlgoT2530.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAlgoT2530.AI;

namespace PacmanGame
{
    public class NavigationHCFSM : HCFSM
    {
        public enum NavigationState { STOP, MOVING }
        private NavigationState _currentState = NavigationState.STOP;

        private Ghost _ghost;

        Tile _srcTile;
        Tile _destTile;
        LinkedList<Tile> _path;
        TileGraph _tileGraph;
        TiledMap _tiledMap;

        public NavigationHCFSM(Ghost ghost, NavigationState initialState) 
        {
            _ghost = ghost;
            _currentState = initialState;
        }

        public override void Initialize()
        {
            GameMap gameMap = (GameMap)GameObjectCollection.FindByName("GameMap");

            TiledMap tiledMap = gameMap.TiledMap;

            _srcTile = new Tile(gameMap.StartColumn, gameMap.StartRow);
        }

        public override void Update()
        {
            MouseState mouse = Mouse.GetState();

            GameMap gameMap = (GameMap)GameObjectCollection.FindByName("GameMap");

            _tiledMap = gameMap.TiledMap;
            _tileGraph = gameMap.TileGraph;

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
                        _path.RemoveFirst();
                        _ghost.UpdateAnimatedSprite(_srcTile, _path.First.Value);

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
                    _ghost.Position.Equals(Tile.ToPosition(_destTile, tileWidth, tileHeight))
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

                    if (_ghost.Position.Equals(nextTilePosition))
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

                        _ghost.UpdateAnimatedSprite(nextTile, newNextTile);
                    }

                    // Move the ghost to the new tile location
                    _ghost.Position = _ghost.Move(_ghost.Position, nextTilePosition, elapsedSeconds);

                /********************************************************************************
                    PROBLEM 3(C): Running the ghost animation.


                    HOWTOSOLVE : 1. Copy the code below.
                                 2. Paste it below this block comment.
                                 3. Fill in the blanks.

                    AnimatedSprite.Update(________);

                ********************************************************************************/
                    
                    _ghost.AnimatedSprite.Update(ScalableGameTime.GameTime);
                }
            }
        }

    }
}
