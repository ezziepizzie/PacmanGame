using GAlgoT2530.AI;
using GAlgoT2530.Engine;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using System.Collections.Generic;
using System.Diagnostics;

namespace PacmanGame
{
    public class GhostHCFSM : HCFSM
    {
        public enum State { Roam, Chase, Evade };

        public State CurrentState;

        private GameEngine _game;
        private Ghost _ghost;
        private TiledMap _tiledMap;
        private TileGraph _tileGraph;

        private Tile _srcTile;
        private Tile _destTile;
        private LinkedList<Tile> _path;

        // For Roam State only
        private Vector2 _destTilePosition;

        // For Chase State only
        private GameObject _pacman;
        private Tile _nextTile;
        private Vector2 _nextTilePosition;
		
		// For transitions between Roam and Chase states
        public float RoamMaxSeconds;
        public float ChaseMaxSeconds;
        private float _roamRemainingSeconds;
        private float _chaseRemainingSeconds;

        // Lab 10: Constructor
        // public GhostHCFSM(GameEngine game, Ghost ghost, TiledMap map, TileGraph graph)
        //{
        //    _game = game;
        //    _ghost = ghost;
        //    _tiledMap = map;
        //    _tileGraph = graph;
        //}

        // Lab 11 (Problem 3): New Constructor
        public GhostHCFSM(GameEngine game, Ghost ghost, TiledMap map, TileGraph graph, Pacman pacman)
        {
            _path = new LinkedList<Tile>();

            _game = game;
            _ghost = ghost;
            _tiledMap = map;
            _tileGraph = graph;
            _pacman = pacman;
        }

        // Lab 10: Initialize()
        //public override void Initialize()
        //{
        //    CurrentState = State.Roam;

        //    Roam_Initialize();
        //}

        // Lab 11 (Problem 3): New Initialize()
        public override void Initialize()
        {
            CurrentState = State.Roam;

            // Comment out this line to test the Chase state below.
            // Roam_Initialize();

			// Comment out for Lab 11 (Problem 4)
            // Chase_Initialize();

            // Lab 11 (Problem 4) only
            RoamMaxSeconds = 3f;
            ChaseMaxSeconds = 5f;
            _roamRemainingSeconds = 0f;
            _chaseRemainingSeconds = 0f;
			
            Roam_Initialize();
        }

        // Lab 10 : Update()
        //public override void Update()
        //{
        //    Roam_Action();
        //}

        // Lab 11 (Problem 3): New Update()
        public override void Update()
        {
            // Comment out this line to test Chase action
            // Roam_Action();

            // Lab 11 (Problem 3)
            // Chase_Action();

            // Lab 11 (Problem 4)
            /********************************************************************************
            PROBLEM 4 : Finite State Machine Update

            HOWTOSOLVE : 1. Copy the code below.
                         2. Paste it below this block comment.
                         3. Fill in the blanks.

            if (CurrentState == ________)
            {
                if (TransitionTriggered_RoamToChase())
                {
                    Chase_Initialize();
                    CurrentState = ________;
                }
                else
                {
                    Roam_Action();
                }
            }
            else if (CurrentState == ________)
            {
                if (TransitionTriggered_ChaseToRoam())
                {
                    ________();
                    CurrentState = ________;
                }
                else
                {
                    ________();
                }
            }
        ********************************************************************************/

            if (CurrentState == State.Roam)
            {
                if (TransitionTriggered_RoamToChase())
                {
                    Chase_Initialize();
                    CurrentState = State.Chase;
                }
                else
                {
                    Roam_Action();
                }
            }
            else if (CurrentState == State.Chase)
            {
                if (TransitionTriggered_ChaseToRoam())
                {
                    Roam_Initialize();
                    CurrentState = State.Roam;
                }
                else
                {
                    Chase_Action();
                }
            }


        }

        /// ROAM STATE /// <summary>
        #region ROAM STATE METHODS
        /// </summary>
        private void Roam_Initialize()
        {
            // Set the ghost Speed
            _ghost.MaxSpeed = 80.0f;

            // Initialize source tile from owner's current position
            _srcTile = Tile.ToTile(_ghost.Position, _tiledMap.TileWidth, _tiledMap.TileHeight);
        }

        private void Roam_Action()
        {
			// Lab11 (Problem 4): Update roam countdown timer
        /********************************************************************************
            PROBLEM 4 : Finite State Machine Update

            HOWTOSOLVE : 1. Copy the code below.
                         2. Paste it below this block comment.
                         3. Fill in the blanks.

            // Update the remaining seconds for roam state.
            _roamRemainingSeconds -= ________;
        ********************************************************************************/

            _roamRemainingSeconds -= ScalableGameTime.DeltaTime;



            if (Roam_IsPathEmpty())
            {
				// Update source tile
			    _srcTile = Tile.ToTile(_ghost.Position, _tiledMap.TileWidth, _tiledMap.TileHeight);

			    // Get new random path and remove the source tile from the path
			    _path = Roam_GenerateRandomPath(_tileGraph, _srcTile);
			    _path.RemoveFirst();
                _path.RemoveFirst();

                // Update destination tile and its position
                _destTile = _path.Last.Value;
			    _destTilePosition = Tile.ToPosition(_destTile, _tiledMap.TileWidth, _tiledMap.TileHeight);

			    // Change animation
			    Tile nextTile = _path.First.Value;
			    _ghost.UpdateAnimatedSprite(_srcTile, nextTile);
            }

            Roam_Moving();
        }

        private bool Roam_IsPathEmpty()
        {
            return _path == null || _path.Count == 0;
        }

        private LinkedList<Tile> Roam_GenerateRandomPath(TileGraph graph, Tile srcTile)
        {
			// Randomly select a navigable tile as destination
            Tile randomTile = new Tile(-1, -1);
            while (!_tileGraph.Nodes.Contains(randomTile) ||
                   randomTile.Equals(srcTile)
                  )
            {
                randomTile.Col = _game.Random.Next(0, _tiledMap.Width);
                randomTile.Row = _game.Random.Next(0, _tiledMap.Height);
            }

            // Compute an A* path
            return AStar.Compute(_tileGraph, srcTile, randomTile, AStarHeuristic.EuclideanSquared);
        }

        private void Roam_Moving()
        {
            float elapsedSeconds = ScalableGameTime.DeltaTime;

            int tileWidth = _tiledMap.TileWidth;
            int tileHeight = _tiledMap.TileHeight;
			
			Tile headTile = _path.First.Value;
            Vector2 headTilePosition = Tile.ToPosition(headTile, tileWidth, tileHeight);

            if (_ghost.Position.Equals(headTilePosition))
            {
                Debug.WriteLine($"Reach head tile (Col = {headTile.Col}, Row = {headTile.Row}).");

                // Remove the head tile from path
                Debug.WriteLine($"Removing head tile. Get next node from path.");
                _path.RemoveFirst();

                // Update animation
                if (!Roam_IsPathEmpty())
                {
                    Tile nextTile = _path.First.Value;
                    _ghost.UpdateAnimatedSprite(headTile, nextTile);
                }
            }
		
            // Move the ghost to the new tile location
            _ghost.Position = _ghost.Move(_ghost.Position, headTilePosition, elapsedSeconds);
            _ghost.AnimatedSprite.Update(ScalableGameTime.GameTime);
        }
        #endregion

        //// CHASE STATE ////
        #region CHASE STATE METHODS
        private void Chase_Initialize()
        {
            _ghost.MaxSpeed = 120.0f;

            // Initialize the next tile position
            _nextTilePosition = _ghost.Position;
            _nextTile = Tile.ToTile(_nextTilePosition, _tiledMap.TileWidth, _tiledMap.TileHeight);
        }

        private void Chase_Action()
        {
            float elapsedSeconds = ScalableGameTime.DeltaTime;

            // Lab11 (Problem 4): Update chase countdown timer
        /********************************************************************************
            PROBLEM 4 : Finite State Machine Update

            HOWTOSOLVE : 1. Copy the code below.
                         2. Paste it below this block comment.
                         3. Fill in the blanks.

            // Update the remaining seconds for roam state.
            _chaseRemainingSeconds -= ________;
        ********************************************************************************/

            _chaseRemainingSeconds -= ScalableGameTime.DeltaTime;

            // Reach the next tile
            if (_ghost.Position.Equals(_nextTilePosition))
            {
                if (Chase_ShouldRecalculatePathTowardsPacman())
                {
                    Chase_RecalculatePathTowardsPacman();
                }
                else
                {
                    // Get the next tile and update animation
                    _srcTile = _nextTile;
                    _path.RemoveFirst();
                    if (!_srcTile.Equals(_destTile))
                    {
                        _nextTile = _path.First.Value;
                        _nextTilePosition = Tile.ToPosition(_nextTile, _tiledMap.TileWidth, _tiledMap.TileHeight);
                        _ghost.UpdateAnimatedSprite(_srcTile, _nextTile);
                    }
                }
            }

            // Move the ghost to the new tile location
            _ghost.Position = _ghost.Move(_ghost.Position, _nextTilePosition, elapsedSeconds);
            _ghost.AnimatedSprite.Update(ScalableGameTime.GameTime);
        }

        private bool Chase_ShouldRecalculatePathTowardsPacman()
        {
            if (_path != null)
            {
                // Get the pacman's tile as destination tile
                _destTile = Tile.ToTile(_pacman.Position, _tiledMap.TileWidth, _tiledMap.TileHeight);

                // Clean up the path from last node to first node
                //   until the last node either contains the destination tile
                LinkedListNode<Tile> destTileNode = _path.Find(_destTile);
                while (_path.Last != destTileNode)
                {
                    _path.RemoveLast();
                }

                return destTileNode == null;
            }
            else
            {
                return true;
            }
        }

        private void Chase_RecalculatePathTowardsPacman()
        {
            // Precondition:
            // 1. Owner must be position exactly on the source tile
            // Postconditions:
            // 1. Updates srcTile, destTile, path, nextTile, nextTilePosition

            _srcTile = Tile.ToTile(_ghost.Position, _tiledMap.TileWidth, _tiledMap.TileHeight);
            _destTile = Tile.ToTile(_pacman.Position, _tiledMap.TileWidth, _tiledMap.TileHeight);

            /********************************************************************************
                PROBLEM 3 : Calculate path and animate.

                HOWTOSOLVE : 1. Copy the code below.
                             2. Paste it below this block comment.
                             3. Fill in the blanks.

                // 1. Compute an A* path
                _path.Clear();
                _path = AStar.Compute(_tileGraph, ________, ________, AStarHeuristic.EuclideanSquared);

                // 2. Remove the source tile from the path 
                _path.RemoveFirst();

                // 3. Update ghost animation
                if (!_srcTile.Equals(_destTile))
                {
                    // Calculate next tile for the purpose of playing the correct animation.
                    _nextTile = _path.First.Value;
                    _nextTilePosition = Tile.ToPosition(________, _tiledMap.TileWidth, _tiledMap.TileHeight);

                    // Ensure animated starts at the correct direction.
                    _ghost.UpdateAnimatedSprite(_srcTile, ________);
                }
            ********************************************************************************/

            _path.Clear();
            _path = AStar.Compute(_tileGraph, _srcTile, _destTile, AStarHeuristic.EuclideanSquared);

            _path.RemoveFirst();
            _path.RemoveFirst();

            if (!_srcTile.Equals(_destTile))
            {
                _nextTile = _path.First.Value;
                _nextTilePosition = Tile.ToPosition(_nextTile, _tiledMap.TileWidth, _tiledMap.TileHeight);
                _ghost.UpdateAnimatedSprite(_srcTile, _nextTile);
            }
        }
        #endregion

        //// TRANSITIONS ////
        //// Lab11 (Problem 4)
		private bool TransitionTriggered_RoamToChase()
        {
		/********************************************************************************
            PROBLEM 4 : Set the condition for transition from Roam to Chase.

            HOWTOSOLVE : 1. Copy the code below.
                         2. Paste it below this block comment.
                         3. Fill in the blanks.

            // Roaming has no seconds remaining
            if (_roamRemainingSeconds <= ________)
            {
                // Reset roaming seconds
                _roamRemainingSeconds = ________;
                return ________;
            }

            return ________;
        ********************************************************************************/

            if (_roamRemainingSeconds <= 0f)
            {
                // Reset roaming seconds
                _roamRemainingSeconds = RoamMaxSeconds;
                return true;
            }

            return false;
        }

        private bool TransitionTriggered_ChaseToRoam()
        {
		/********************************************************************************
            PROBLEM 4 : Set the condition for transition from Chase to Roam.

            HOWTOSOLVE : 1. Copy the code below.
                         2. Paste it below this block comment.
                         3. Fill in the blanks.

            // Chasing has no seconds remaining
            if (_chaseRemainingSeconds <= ________)
            {
                _chaseRemainingSeconds = ________;
                return ________;
            }

            return ________;
        ********************************************************************************/
            
            if (_chaseRemainingSeconds <= 0f)
            {
                _chaseRemainingSeconds = ChaseMaxSeconds;
                return true;
            }

            return false;
        }
    }
}
