using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace Labyrintti
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var graph = new Dictionary<string, List<string>>
            {
                ["0,0"] = new List<string> {"0,1"},
                ["0,1"] = new List<string> {"0,0", "1,1"},
                ["0,3"] = new List<string> {"1,3", "0,4"},
                ["0,4"] = new List<string> {"0,3"},
                ["1,1"] = new List<string> {"0,1", "2,1"},
                ["1,3"] = new List<string> {"0,3", "2,3"},
                ["2,0"] = new List<string> {"2,1"},
                ["2,1"] = new List<string> {"1,1", "2,0", "2,2"},
                ["2,2"] = new List<string> {"2,1", "2,3", "3,2"},
                ["2,3"] = new List<string> {"1,3", "2,2"},
                ["3,2"] = new List<string> {"2,2", "4,2"},
                ["4,0"] = new List<string> {"4,1"},
                ["4,1"] = new List<string> {"4,0", "4,2"},
                ["4,2"] = new List<string> {"3,2", "4,1", "4,3"},
                ["4,3"] = new List<string> {"4,2", "4,4"},
                ["4,4"] = new List<string> {"4,3"}
            };
            Console.WriteLine("DFS traversal: ");
            var visited = new HashSet<string>();
            DFS(graph, "0,0", visited);

            Console.WriteLine("\n\nShortest path using BFS: ");
            var path = BFS(graph, "0,0", "4,4");
            Console.WriteLine(string.Join(" ->", path));

        }

        static void DFS(Dictionary<string, List<string>> graph, string start, HashSet<string> visited)
        {
            if (visited.Contains(start)) return;    // stop if seen
            visited.Add(start);         // mark visited
            Console.WriteLine(start);   // print node

            foreach (var next in graph[start])
            {
                DFS(graph, next, visited);      // recurse deeper
            }
        }
        static List<string> BFS(Dictionary<string, List<string>> graph, string start, string goal)
        {
            var queue = new Queue<string>();
            var visited = new HashSet<string>();
            var parent = new Dictionary<string, string>();

            queue.Enqueue(start);
            visited.Add(start);

            while(queue.Count > 0)
            {
                var node = queue.Dequeue();
                if(node == goal)
                {
                    var path = new List<string>();
                    while(node != null)
                    {
                        path.Add(node);
                        parent.TryGetValue(node, out node);
                    }
                    path.Reverse();
                    return path;
                }
                foreach(var next in graph[node])
                {
                    if(!visited.Contains(next))
                    {
                        visited.Add(next);
                        parent[next] = node;
                        queue.Enqueue(next);
                    }
                }
            }
            return new List<string>();
        }
    }
}
