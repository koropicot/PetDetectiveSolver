using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace PetDetectiveSolver
{
    public class PDPosition : IEquatable<PDPosition>
    {
        private Variant<Product, int, int> entity;
        private PDPosition() { }
        public static PDPosition Start()
        {
            var ret = new PDPosition();
            ret.entity = Variant<Product, int, int>.C1(Product.Create());
            return ret;
        }
        public static PDPosition Pet(int i)
        {
            var ret = new PDPosition();
            ret.entity = Variant<Product, int, int>.C2(i);
            return ret;
        }
        public static PDPosition Home(int i)
        {
            var ret = new PDPosition();
            ret.entity = Variant<Product, int, int>.C3(i);
            return ret;
        }

        public static PDPosition FromIndex(int i)
        {
            return i == 0
                ? PDPosition.Start()
                : (i - 1) % 2 == 0
                    ? PDPosition.Pet((i - 1) / 2)
                    : PDPosition.Home((i - 1) / 2);
        }

        public T Match<T>(Func<T> Start, Func<int, T> Pet, Func<int, T> Home)
        {
            return entity.Match(_ => Start(), Pet, Home);
        }

        public int ToIndex()
        {
            return Match(() => 0, i => i * 2 + 1, i => i * 2 + 2);
        }

        public override string ToString()
        {
            return Match(() => '$', i => (char)('a' + i), i => (char)('A' + i)).ToString();
        }

        public bool Equals(PDPosition other)
        {

            return other != null ? this.ToIndex() == other.ToIndex() : false;
        }

        public override bool Equals(object obj)
        {
            return obj is PDPosition ? this.Equals((PDPosition)obj) : false;
        }

        public override int GetHashCode()
        {
            return this.ToIndex();
        }
    }
}
