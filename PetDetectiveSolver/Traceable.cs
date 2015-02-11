using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace PetDetectiveSolver
{
    public class Traceable<T>
    {
        public readonly T Value;
        public readonly Traceable<T> Parent;

        public Traceable(T value, Traceable<T> parent)
        {
            this.Value = value;
            this.Parent = parent;
        }
    }

    public static class Traceable
    {
        public static Traceable<T> Create<T>(T value, Traceable<T> parent)
        {
            return new Traceable<T>(value, parent);
        }

        public static IEnumerable<T> Trace<T>(this Traceable<T> goal)
        {
            return Ex.Unfold(goal,
                tr => tr == null
                    ? Option.None<Product<T, Traceable<T>>>()
                    : Option.Some(Product.Create(tr.Value, tr.Parent))).Reverse();
        }
    }
}
