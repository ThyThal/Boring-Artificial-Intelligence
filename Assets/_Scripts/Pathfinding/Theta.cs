using System;
using System.Collections.Generic;
using UnityEngine;

public class Theta<T>
{
    public delegate List<T> GetNeighbours(T current);
    public delegate bool Satisfies(T current);
    public delegate float GetCost(T point, T current);
    public delegate float Heuristic(T current);

    public List<T> Run(T start, Satisfies satisfies, GetNeighbours getNeighbours, GetCost getCost, Heuristic heuristic, int watchdog = 100)
    {
        Dictionary<T, T> parents = new Dictionary<T, T>();
        PriorityQueue<T> pending = new PriorityQueue<T>();
        Dictionary<T, float> cost = new Dictionary<T, float>();
        HashSet<T> visited = new HashSet<T>();

        pending.Enqueue(start, 0);
        cost.Add(start, 0);

        while (!pending.IsEmpty)
        {
            watchdog--;
            if (watchdog <= 0)
            {
                return new List<T>();
            }

            T current = pending.Dequeue();
            if (satisfies(current))
            {
                return ConstructPath(current, parents);
            }

            visited.Add(current);
            List<T> neighbours = getNeighbours(current);
            for (int i = 0; i < neighbours.Count; i++)
            {
                var item = neighbours[i];
                if (visited.Contains(item))
                {
                    continue;
                }

                float totalCost = cost[current] + getCost(current, item);
                if (cost.ContainsKey(item) && cost[item] < totalCost)
                {
                    continue;
                }

                cost[item] = totalCost;
                parents[item] = current;
                pending.Enqueue(item, totalCost + heuristic(item));
            }
        }
        return new List<T>();
    }

    List<T> ConstructPath(T end, Dictionary<T,T> parents)
    {
        var path = new List<T>();
        path.Add(end);

        while (parents.ContainsKey(path[path.Count - 1]))
        {
            var lastNode = path[path.Count - 1];
            path.Add(parents[lastNode]);
        }

        path.Reverse();
        return path;
    }

}
