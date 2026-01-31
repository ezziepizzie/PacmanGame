// using var game = new PacmanGame.Game1();
using var game = new GAlgoT2530.Engine.GameEngine("Pacman Game", 696, 432);
game.AddScene("PacmanScene", new PacmanGame.PacmanScene());
game.Run();
