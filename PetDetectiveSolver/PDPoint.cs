using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace PetDetectiveSolver
{
    public class PDPoint : IEquatable<PDPoint>
    {
        private Variant<Product, PDPet, PDPet> entity;
        private PDPoint() { }
        public static PDPoint Start()
        {
            var ret = new PDPoint();
            ret.entity = Variant<Product, PDPet, PDPet>.C1(Product.Create());
            return ret;
        }
        public static PDPoint Pet(PDPet p)
        {
            var ret = new PDPoint();
            ret.entity = Variant<Product, PDPet, PDPet>.C2(p);
            return ret;
        }
        public static PDPoint Home(PDPet p)
        {
            var ret = new PDPoint();
            ret.entity = Variant<Product, PDPet, PDPet>.C3(p);
            return ret;
        }

        public static PDPoint FromString(string s)
        {
            return Char.IsSymbol(s[0])
                ? PDPoint.Start()
                : Char.IsLower(s[0])
                    ? PDPoint.Pet(new PDPet(s))
                    : PDPoint.Home(new PDPet(s));
        }

        public T Match<T>(Func<T> Start, Func<PDPet, T> Pet, Func<PDPet, T> Home)
        {
            return entity.Match(_ => Start(), Pet, Home);
        }

        public override string ToString()
        {
            return Match(() => "$", p => p.ToString().ToLower(), h => h.ToString().ToUpper());
        }

        public bool Equals(PDPoint other)
        {

            return other == null ? false : this.Match(
                () => other.Match(() => true, _ => false, _ => false),
                pt => other.Match(() => false, po => pt.Equals(po), _ => false),
                ht => other.Match(() => false, _ => false, ho => ht.Equals(ho)));
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PDPoint);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
