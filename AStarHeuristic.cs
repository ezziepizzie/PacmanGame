using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacmanGame
{
    public class AStarHeuristic
    {
        public static ulong Manhattan(Tile start, Tile end)
        {
            int dCol = end.Col - start.Col;
            int dRow = end.Row - start.Row;
            return (ulong)(Math.Abs(dCol) + Math.Abs(dRow));
        }

        public static ulong EuclideanSquared(Tile start, Tile end)
        {
            int dCol = end.Col - start.Col;
            int dRow = end.Row - start.Row;
            return (ulong)(dCol * dCol + dRow * dRow);
        }
    }
}
