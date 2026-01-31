using GAlgoT2530.Engine;

namespace PacmanGame
{
    public class PacmanScene : GameScene
    {
        public override void CreateScene()
        {
            // Game map
            GameMap gameMap = new GameMap("GameMap");
            gameMap.StartColumn = 18;
            gameMap.StartRow = 11;

            // Pathfinding Tester
            // PathfindingTester pathfindingTester = new PathfindingTester("PathfindingTester");

            // Ghost
            Ghost ghost = new Ghost();

            // Pac Man
            Pacman pacman = new Pacman();
            pacman.Speed = 100f;
            pacman.StartColumn = 1;
            pacman.StartRow = 1;
            pacman.NavigableTileLayerName = "Food";
        }
    }
}
