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

            // Pathfinding Tester (Commented for Problem 4 and 5)
            // PathfindingTester pathfindingTester = new PathfindingTester("PathfindingTester");

            // Ghost (For Problem 4 and 5)
            Ghost ghost = new Ghost();
        }
    }
}
