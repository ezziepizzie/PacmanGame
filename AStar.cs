using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Text;

namespace PacmanGame
{
    public class AStar
    {
        // None = Not seen
        // Opened = Seen and is in the Priority Queue (PQ)
        // Closed = Seen, was in the PQ, and has been removed from the PQ.
        private enum State {None = 0, Opened, Closed};

        // AStarData is designed solely for selecting
        // current graph node to traverse from the priority queue
        private class AStarData : IComparable<AStarData>
        {
            public Tile self;
            public Tile parent;
            public ulong g; // cost so far from the start node to this node
            public ulong h; // heuristic fron this node to the end node
            public State state;

            public AStarData(Tile self, ulong g, ulong h)
            {
                this.self = self;
                this.parent = null;
                this.g = g;
                this.h = h;
                this.state = State.None;
            }

            public int CompareTo(AStarData rhs)
            {
                ulong f1 = this.g + this.h;
                ulong f2 = rhs.g + rhs.h;
                return (int)(f1 - f2);
            }

            public override string ToString()
            {
                return self.ToString();
            }
        }

        // A "function pointer" pointing to a heuristic function
        public delegate ulong Heuristic(Tile start, Tile end);

        public static LinkedList<Tile> Compute(TileGraph graph, Tile start, Tile end, Heuristic heuristic)
        {
            // Create node records to store data used by AStar
            Dictionary<Tile, AStarData> astarNodeDictionary = new Dictionary<Tile, AStarData>();

        /********************************************************************************
            Problem 1(a) : For each node, initialize its 'g' and 'h' value.
                        (For the start node, 'g' is 0; for other nodes. 
                         For other nodes, 'g' is the largest possible value.
                         In C#, T.MaxValue.is the largest value for a numeric type T.)

            HOWTOSOLVE : 1. Copy the code below.
                         2. Paste it below this block comment.
                         3. Fill in the blanks.

            foreach (Tile node in ________)
                astarNodeDictionary.Add(node, new AStarData(________, ulong.MaxValue, heuristic(________, end)));
            astarNodeDictionary[start] = new AStarData(start, ________, heuristic(________, end));

        ********************************************************************************/
            
            foreach (Tile node in graph.Nodes)
                astarNodeDictionary.Add(node, new AStarData(node, ulong.MaxValue, heuristic(node, end)));

            astarNodeDictionary[start] = new AStarData(start, 0, heuristic(start, end));




            // Priority Queue (PQ) for deciding which node to process
            PriorityQueue<AStarData> pq = new PriorityQueue<AStarData>();

            // Push the node record for start node to the priority queue
            AStarData startNode = astarNodeDictionary[start];
            pq.Push(startNode);
            startNode.state = State.Opened;

            // Current node and neighbour node to be used in A*
            AStarData curData, neighbourData;

            // For constructing neighbour node
            int[] MoveRow = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] MoveCol = { -1, 0, 1, -1, 1, -1, 0, 1 };

            while (!pq.Empty())
            {
                // Get current node data
                curData = pq.Top();
                pq.Pop();
                curData.state = State.Closed;

                /********************************************************************************
                    Problem 1(b) : Stop A* algorithm if the current node is the end.

                    HOWTOSOLVE : 1. Copy the code below.
                                 2. Paste it below this block comment.
                                 3. Fill in the blanks.

                    if (curData.self == ________)
                        break;
                ********************************************************************************/

                if (curData.self == null)
                    break;
                
				


                // Get the weights to all neighbours of current node
                ulong[] weights = graph.Connections[curData.self];

                // For each neighbour ... 
                for (int i = 0; i < weights.Length; ++i)
                {
                    // ... if there is a connection
                    if (weights[i] != 0)
                    {
                        int colNeighbour = curData.self.Col + MoveCol[i];
                        int rowNeighbour = curData.self.Row + MoveRow[i];

                        Tile neighbourTile = new Tile(colNeighbour, rowNeighbour);
                        neighbourData = astarNodeDictionary[neighbourTile];
                    }
                    else
                    {
                        // NOTE: For Pacman, the diagonal connections are not used,
                        // so their weights are 0.
                        continue;
                    }

                    // Ignore neighbours in Closed state
                    if (neighbourData.state == State.Closed)
                    {
                        continue;
                    }

                /********************************************************************************
                    Problem 1(c) : Update 'g' and the parent node if a shorter 'g' value is found.

                    HOWTOSOLVE : 1. Copy the code below.
                                 2. Paste it below this block comment.
                                 3. fill in the blanks.

                    ulong gNew = ________ + weights[i];
                    if (neighbourData.g > ________)
                    {
                        neighbourData.g = gNew;
                        neighbourData.parent = ________;
                    }
                ********************************************************************************/

                        ulong gNew = curData.g + weights[i];

                    if (neighbourData.g > gNew)
                    {
                        neighbourData.g = gNew;
                        neighbourData.parent = curData.self;
                    }




                    // Request PQ to update the neighbourData's location in PQ
                    // amidst any update on its 'g' value.
                    // (Consult your instructor if you are unsure what this means)
                    if (neighbourData.state == State.Opened)
                    {
                        pq.Update(neighbourData);
                    }
                    else // here, the neighbourData.state should be State.None
                    {
                        pq.Push(neighbourData);
                        neighbourData.state = State.Opened;
                    }
                }
            }

            return ConstructPath(astarNodeDictionary, start, end);
        }

        private static LinkedList<Tile> ConstructPath(Dictionary<Tile, AStarData> astarNodeDictionary, Tile start, Tile end)
        {
            LinkedList<Tile> path = new LinkedList<Tile>();

        /********************************************************************************
            Problem 2 : Construct the optimum path by linking the tiles
                        from end to start.
            HOWTOSOLVE : Uncomment the code block below and fill in the blanks.

            AStarData cur = ________;
            while (cur.parent != ________)
            {
                path.________(cur.self);
                cur = ________;
            }
            path.AddFirst(________);
        ********************************************************************************/

            AStarData cur = astarNodeDictionary[end];
            while (cur != null) {
                path.AddFirst(cur.self);
                cur = cur.parent != null ? astarNodeDictionary[cur.parent] : null;
            }
            path.AddFirst(start);

            return path;
        }
    }
}
