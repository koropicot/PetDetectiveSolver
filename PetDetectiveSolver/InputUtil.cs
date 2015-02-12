using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace PetDetectiveSolver
{
    public static class PDInputUtil
    {
        public static PDSolver CreateSolver(char[][] input)
        {
            var yMax = input.Length;
            var xMax = input.Select(l => l.Length).Min() / 2;

            var graph = new Direction[yMax,xMax];

            foreach (var y in Enumerable.Range(0, yMax))
            foreach (var x in Enumerable.Range(0, xMax))
            {
                graph[y, x] = input[y][x * 2 + 1].Apply(c =>
                    c == 'L' ? Direction.Right | Direction.Up :
                    c == '|' ? Direction.Up :
                    c == '_' ? Direction.Right :
                        Direction.None);
            }

            foreach (var y in Enumerable.Range(0, yMax))
            foreach (var x in Enumerable.Range(0, xMax))
            {
                if (0 <= x - 1 && graph[y, x - 1].HasFlag(Direction.Right))
                    graph[y, x] |= Direction.Left;
                if (y + 1 < yMax && graph[y + 1, x].HasFlag(Direction.Up))
                    graph[y, x] |= Direction.Down;
            }

            var coordinatesOfPoint = new Dictionary<PDPoint, Coordinates>();
            var pets = new HashSet<PDPet>();

            foreach (var y in Enumerable.Range(0, yMax))
            foreach (var x in Enumerable.Range(0, xMax))
            {
                var c = input[y][x * 2];
                if (Char.IsWhiteSpace(c) || c == '.')
                    continue;
                var point = PDPoint.FromString(c.ToString());
                coordinatesOfPoint[point] = new Coordinates(x, y);
                point.Match(
                    () => 0,
                    p => { pets.Add(p); return 0; },
                    h => { pets.Add(h); return 0; });
            }

            var moveCost = new MoveCost(graph);

            var pointCount = coordinatesOfPoint.Count;
            var costs = coordinatesOfPoint.Keys.ToDictionary(p => p, _ => new Dictionary<PDPoint,int>());

            foreach (var from in coordinatesOfPoint.Keys)
                foreach (var to in coordinatesOfPoint.Keys)
                {
                    if (from.Equals(to) || costs[from].ContainsKey(to))
                        continue;
                    costs[from][to] = costs[to][from] = moveCost.Calc(coordinatesOfPoint[from], coordinatesOfPoint[to]);
                }

            return new PDSolver(pets, costs);
        }
    }
}
