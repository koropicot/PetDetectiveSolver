using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetDetectiveSolver
{
    public class PDPet : IEquatable<PDPet>
    {
        public readonly string Symbol;
        public PDPet(string symbol)
        {
            this.Symbol = symbol.ToLower();
        }

        public override string ToString()
        {
            return Symbol;
        }

        public bool Equals(PDPet other)
        {
            return other == null ? false : this.Symbol == other.Symbol;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PDPet);
        }

        public override int GetHashCode()
        {
            return Symbol.GetHashCode();
        }

}
}
