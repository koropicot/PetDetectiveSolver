using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetDetectiveSolver
{
    public class PDNode : IEquatable<PDNode>
    {
        public readonly PDPoint Position;
        public readonly Dictionary<PDPet, PDPetStatus> PetStatuses;

        public PDNode(PDPoint position, Dictionary<PDPet, PDPetStatus> PetStatuses)
        {
            this.Position = position;
            this.PetStatuses = PetStatuses;
        }

        public bool Equals(PDNode other)
        {
            return other == null ? false : this.Position.Equals(other.Position) && this.PetStatuses.SequenceEqual(other.PetStatuses);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PDNode);
        }

        public override int GetHashCode()
        {
            return this.Position.GetHashCode() ^ this.PetStatuses.GetHashCode();
        }
    }

    public enum PDPetStatus
    {
        BeforeCarry,
        Carring,
        AfterCarry
    }
}
