namespace Labyrintti
{
    internal class Program
    {
        static int[,] mazeDFS =
        {
            {0,0,1,0,0},
            {1,0,1,0,1},
            {0,0,0,0,1},
            {1,1,0,1,0},
            {0,0,0,0,0}
        };


        static bool[,] visited = new bool[5, 5];
        static List<(int, int)> path = new List<(int, int)>();
        public static bool DFS (int r, int c)
        {
            if (r < 0 || r >= 5 || c < 0 || c >= 5) return false; // Ulkona
            if (mazeDFS[r, c] == 1 || visited[r, c]) return false;     // Seinä tai käytetty
            visited[r, c] = true;
            path.Add((r, c));   // Lisätään reittiin

            if (r == 4 && c == 4) return true;  // maali löytyi

            // Suunnat: ylös, oikea, alas, vasen
            if (DFS(r - 1, c)) return true;
            if (DFS(r, c + 1)) return true;
            if (DFS(r + 1, c)) return true;
            if (DFS(r, c - 1)) return true;

            path.RemoveAt(path.Count - 1);  // Peru askeleita
            return false;
        }


        static int[,] mazeBFS =
{
            {0,0,1,0,0},
            {1,0,1,0,1},
            {0,0,0,0,1},
            {1,1,0,1,0},
            {0,0,0,0,0}
        };


        static (int, int)?[,] prev = new (int, int)?[5, 5];

        static void BFS()
        {
            Queue<(int, int)> queue = new();
            bool[,] visited = new bool[5, 5];

            queue.Enqueue((0,0));
            visited[0, 0] = true;

            while(queue.Count > 0)
            {
                var (r, c) = queue.Dequeue();
                if(r == 4 && c == 4)
                {
                    return; // Maali
                }

                var dirs = new (int, int)[] {(-1,0),(0,1),(1,0),(0,-1)};    // Suunnat: ylös, oikealle, alas, vasen

                foreach(var(dr, dc) in dirs)
                {
                    int nr = r + dr, nc = c + dc;
                    if(nr >=0 && nr < 5 && nc >= 0 && nc <5 && mazeBFS[nr,nc]== 0 && !visited[nr, nc])
                    {
                        visited[nr, nc] = true;
                        prev[nr, nc] = (r, c);  // kuka tuli tänne
                        queue.Enqueue((nr, nc));
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            DFS(0, 0);
            Console.WriteLine("DFS reitti:");
            foreach( var p in path)
            {
                Console.WriteLine(p);
            }
            BFS();
            List<(int, int)> route = new();
            var current = (4, 4);

            while(current != (0,0))
            {
                route.Add(current);
                current = prev[current.Item1, current.Item2].Value;
            }
            route.Add((0, 0));
            route.Reverse();

            Console.WriteLine("\nBFS reitti:");
            foreach(var p in route)
            {
                Console.WriteLine(p);
            }
        }
    }
}
