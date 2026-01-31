using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacmanGame
{
    public class TileGraph
    {
        public HashSet<Tile>                Nodes;
        public Dictionary<Tile, ulong[]>    Connections;

        public TileGraph()
        {
            /********************************************************************************
                PROBLEM 4 : Initialize Nodes and Connections using their respective 
                            default constructors.

                HOWTOSOLVE : 1. Copy the code below.
                             2. Paste it below this block comment.
                             3. Fill in the blanks.

                Nodes = ________;
                Connections = ________;
            ********************************************************************************/

            Nodes = new HashSet<Tile>();
            Connections = new Dictionary<Tile, ulong[]>();
        }

        public void CreateFromTiledMapTileLayer(TiledMapTileLayer foodLayer, ushort colStart, ushort rowStart)
        {
            /********************************************************************************
                PROBLEM 5 : Verify that a tile exists in the food layer at (colStart, rowStart)
                            If yes, call BFSConstructGraph() to construct the graph.

                HOWTOSOLVE : 1. Copy the code below.
                                2. Paste it below this block comment.
                                3. Fill in the blanks.

                bool hasTile = foodLayer.TryGetTile(________, ________, out TiledMapTile? tile);

                if (hasTile && !tile.Value.IsBlank)
                {
                    BFSConstructGraph(________, ________, ________);
                }
                else
                {
                    throw new Exception($"[TileGraph]: Error: No tile found in (Col = {colStart}, Row = {rowStart}).");
                }
            ********************************************************************************/

            bool hasTile = foodLayer.TryGetTile(colStart, rowStart, out TiledMapTile? tile);

            if (hasTile && !tile.Value.IsBlank)
            {
                BFSConstructGraph(foodLayer, colStart, rowStart);
            }
            else
            {
                throw new Exception($"[TileGraph]: Error: No tile found in (Col = {colStart}, Row = {rowStart}).");
            }



        }

        private void BFSConstructGraph(TiledMapTileLayer foodLayer, ushort colStart, ushort rowStart)
        {
            int[]    MoveRow   = {-1, -1, -1,  0, 0,  1,  1,  1};
            int[]    MoveCol   = {-1,  0,  1, -1, 1, -1,  0,  1};
            ulong[]  Cost      = { 2,  1,  2,  1, 1,  2,  1,  2}; // Diagonal = 2, Non-Diagonal = 1
            int[]    Direction = { 1,  3,  4,  6}; // (Up = 1, Left = 3, Right = 4, Down = 6)

        /********************************************************************************
            PROBLEM 6(a) : Construct the start tile with the right arguments.

            HOWTOSOLVE : 1. Copy the code below.
                            2. Paste it below this block comment.
                            3. Fill in the blanks.

            Tile startTile = ________;
            Nodes.Add(________);
        ********************************************************************************/

            Tile startTile = new Tile(colStart, rowStart);
            Nodes.Add(startTile);




            /********************************************************************************
                PROBLEM 6(b) : Create a queue to contain the Tile objects for BFS traversal.
                               Which tile should be enqueued before the traversal begins?

                HOWTOSOLVE : 1. Copy the code below.
                             2. Paste it below this block comment.
                             3. Fill in the blanks.

                Queue<Tile> queue = ________;
                queue.Enqueue(________);
            ********************************************************************************/

            Queue<Tile> queue = new Queue<Tile>();
            queue.Enqueue(startTile);

            while (queue.Count > 0)
            {
                Tile currentTile = queue.Dequeue();

                foreach (int direction in Direction)
                {
                /********************************************************************************
                    PROBLEM 6(c) : Calculate the neighbour's row and column using 'currentTile',
                                   'MoveRow', and 'MoveCol'.

                    HOWTOSOLVE : 1. Copy the code below.
                                 2. Paste it below this block comment.
                                 3. Fill in the blanks.

                    int rowNeighbour = currentTile.Row + ________;
                    int colNeighbour = currentTile.Col + ________;
                ********************************************************************************/

                    int rowNeighbour = currentTile.Row + MoveRow[direction];
                    int colNeighbour = currentTile.Col + MoveCol[direction];



                    // A valid neighbour satisfies the following criteria:
                    // (1) Row and column is within the number of rows and columns respectively.
                    // (2) A tile exists in food layer at location (column, row).
                    if ((0 <= rowNeighbour && rowNeighbour < foodLayer.Height) &&
                        (0 <= colNeighbour && colNeighbour < foodLayer.Width) &&
                        foodLayer.TryGetTile((ushort)colNeighbour, (ushort)rowNeighbour, out TiledMapTile? neighbourTiledMapTile) &&
                        !neighbourTiledMapTile.Value.IsBlank)
                    {
                        // Add neighbour node and its connections to graph
                        Tile neighbourTile = new Tile(colNeighbour, rowNeighbour);

                        // If neighbour node not yet created or does not exist yet.
                        if (!Nodes.Contains(neighbourTile))
                        {
                            // Add neighbour tile to the graph (this also marks it as visited)
                            Nodes.Add(neighbourTile);

                            // Add neighbour to queue
                            queue.Enqueue(neighbourTile);
                        }

                        // Create a new connection joining the current node with the neighbour node
                        if (!Connections.TryGetValue(currentTile, out ulong[] weights))
                        {
                            weights = new ulong[Cost.Length]; // Allocate 8 ulongs
                            weights[direction] = Cost[direction];
                            Connections.Add(currentTile, weights);
                        }
                        else
                        {
                            Connections[currentTile][direction] = Cost[direction];
                        }
                    }
                }
            }
        }
    }
}
