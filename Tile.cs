using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacmanGame
{
    public class Tile
    {
        public int Row;
        public int Col;

        public Tile(int col, int row)
        {
            Col = col;
            Row = row;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Tile rhs = (Tile)obj;
                return (Row == rhs.Row) && (Col == rhs.Col);
            }
        }

        public override int GetHashCode()
        {
            return (Row << 2) ^ Col;
        }

        public override string ToString()
        {
            return $"[Tile]: Col = {Col}, Row = {Row}";
        }
		
		public static Vector2 ToPosition(Tile tile, int tileWidth, int tileHeight)
        {
            return new Vector2(tile.Col * tileWidth, tile.Row * tileHeight);
        }

        public static Tile ToTile(Vector2 position, int tileWidth, int tileHeight)
        {
            return new Tile((int)(position.X / tileWidth), (int)(position.Y / tileHeight));
        }
    }
}
