using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetDetectiveSolver
{
    public class PDNode : IEquatable<PDNode>
    {
        public readonly PDPosition Position;
        public readonly PetStatus[] PetStatuses;

        public PDNode(PDPosition position, PetStatus[] PetStatuses)
        {
            this.Position = position;
            this.PetStatuses = PetStatuses;
        }

        public bool Equals(PDNode other)
        {
            return other != null ? this.Position.Equals(other.Position) && this.PetStatuses.SequenceEqual(other.PetStatuses) : false;
        }

        public override bool Equals(object obj)
        {
            return obj is PDNode ? this.Equals((PDNode)obj) : false;
        }

        public override int GetHashCode()
        {
            return this.Position.GetHashCode() ^ this.PetStatuses.GetHashCode();
        }
    }

    public enum PetStatus
    {
        BeforeCarry,
        Carring,
        AfterCarry
    }
}
