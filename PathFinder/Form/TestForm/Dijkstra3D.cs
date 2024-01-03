using System;
using System.Collections.Generic;
using System.Linq;
namespace RouteOptimizer.Form.TestForm
{
    public class Dijkstra3D
    {
        public static Dictionary<(int, int, int), List<((int, int, int), int)>> BuildGraph()
        {
            // Example: Build a 3D grid as a graph
            var graph = new Dictionary<(int, int, int), List<((int, int, int), int)>>();

            // Add nodes and edges (connections with weights)
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        var currentNode = (x, y, z);
                        var neighbors = new List<((int, int, int), int)>();

                        // Connect to neighbors (adjust conditions based on your 3D space)
                        if (x > 0) neighbors.Add(((x - 1, y, z), 1)); // Example: Connect to the node to the left
                        if (x < 2) neighbors.Add(((x + 1, y, z), 1)); // Example: Connect to the node to the right
                        if (y > 0) neighbors.Add(((x, y - 1, z), 1)); // Example: Connect to the node below
                        if (y < 2) neighbors.Add(((x, y + 1, z), 1)); // Example: Connect to the node above
                        if (z > 0) neighbors.Add(((x, y, z - 1), 1)); // Example: Connect to the node behind
                        if (z < 2) neighbors.Add(((x, y, z + 1), 1)); // Example: Connect to the node in front

                        graph[currentNode] = neighbors;
                    }
                }
            }

            return graph;
        }

        public static Dictionary<(int, int, int), int> Dijkstra(Dictionary<(int, int, int), List<((int, int, int), int)>> graph, (int, int, int) start)
        {
            var distances = new Dictionary<(int, int, int), int>();
            var priorityQueue = new PriorityQueue<(int, int, int)>();

            foreach (var node in graph.Keys)
            {
                distances[node] = int.MaxValue;
                priorityQueue.Enqueue(node, int.MaxValue);
            }

            distances[start] = 0;
            priorityQueue.Enqueue(start, 0);

            while (priorityQueue.Count > 0)
            {
                var (currentNode, currentDistance) = priorityQueue.Dequeue();

                foreach (var (neighbor, weight) in graph[currentNode])
                {
                    var newDistance = distances[currentNode] + weight;

                    if (newDistance < distances[neighbor.Item1])
                    {
                        distances[neighbor.Item1] = newDistance;
                        priorityQueue.Enqueue(neighbor.Item1, newDistance);
                    }
                }
            }

            return distances;
        }

        public static void Main()
        {
            var graph = BuildGraph();
            var startNode = (0, 0, 0);
            var distances = Dijkstra(graph, startNode);

            // Print or use the result as needed
            foreach (var (node, distance) in distances)
            {
                Console.WriteLine($"Shortest distance from {startNode} to {node}: {distance}");
            }
        }
    }

    public class PriorityQueue<T>
    {
        private readonly SortedDictionary<int, Queue<T>> _queue = new SortedDictionary<int, Queue<T>>();

        public int Count
        {
            get
            {
                var count = 0;
                foreach (var queue in _queue.Values)
                {
                    count += queue.Count;
                }
                return count;
            }
        }

        public void Enqueue(T item, int priority)
        {
            if (!_queue.TryGetValue(priority, out var queue))
            {
                queue = new Queue<T>();
                _queue[priority] = queue;
            }

            queue.Enqueue(item);
        }

        public (T, int) Dequeue()
        {
            if (_queue.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            var queue = _queue.First().Value;
            var item = queue.Dequeue();

            if (queue.Count == 0)
            {
                _queue.Remove(_queue.First().Key);
            }

            return (item, _queue.First().Key);
        }
    }
}