using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace PetDetectiveSolver
{
    public class MoveCost
    {
        private Direction[,] graph;
        public MoveCost(Direction[,] graph)
        {
            this.graph = graph;
        }

        public int Calc(Coordinates from, Coordinates to)
        {
            return BestFirstSearch.Search(
                from,
                c =>
                    Offsets(graph[c.Y, c.X])
                    .Select(offset => c + offset)
                    .Where(n => 0 <= n.X && n.X < graph.GetLength(1) && 0 <= n.Y && n.Y < graph.GetLength(0)),
                p => p.Equals(to),
                Comparer<Traceable<Coordinates>>.Create((path1, path2) =>
                {
                    var calcCost = (Func<Traceable<Coordinates>, int>)(p => p.Trace().Count() + (Math.Abs(p.Value.X - to.X) + Math.Abs(p.Value.Y - to.Y)));
                    return calcCost(path2) - calcCost(path1);
                }))
                .Match(
                    path => path.Trace().Count() - 1,
                    () => { throw new InvalidOperationException(); });            
        }

        private static List<Coordinates> Offsets(Direction direction)
        {
            var offsets = new List<Coordinates>();
            if (direction.HasFlag(Direction.Left)) offsets.Add(new Coordinates(-1, 0));
            if (direction.HasFlag(Direction.Right)) offsets.Add(new Coordinates(1, 0));
            if (direction.HasFlag(Direction.Up)) offsets.Add(new Coordinates(0, -1));
            if (direction.HasFlag(Direction.Down)) offsets.Add(new Coordinates(0, 1));

            return offsets;
        }
    }

    public class Coordinates : IEquatable<Coordinates>
    {
        private readonly Tuple<int, int> entity;
        public int X { get { return entity.Item1; } }
        public int Y { get { return entity.Item2; } }
        public Coordinates(int x, int y)
        {
            this.entity = Tuple.Create(x, y);
        }

        public static Coordinates operator +(Coordinates p1, Coordinates p2)
        {
            return new Coordinates(p1.X + p2.X, p1.Y + p2.Y);
        }

        public bool Equals(Coordinates other)
        {
            return other == null ? false : this.entity.Equals(other.entity);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Coordinates);
        }

        public override int GetHashCode()
        {
            return entity.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", X, Y);
        }
    }

    [Flags]
    public enum Direction
    {
        None = 0,
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8
    }
}
