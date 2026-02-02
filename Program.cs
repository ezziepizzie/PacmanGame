// using var game = new PacmanGame.Game1();
using var game = new GAlgoT2530.Engine.GameEngine("Pacman Game", 1224, 744);
game.AddScene("PacmanScene", new PacmanGame.PacmanScene());
game.Run();
