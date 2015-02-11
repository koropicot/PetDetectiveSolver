using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace PetDetectiveSolver
{
    public class PDSolver
    {
        const int MaxCarry = 4;
        private int petCount;
        private int[][] costs;

        public PDSolver(int[][] costs)
        {
            this.petCount = (costs.Length - 1) / 2;
            this.costs = costs;
        }

        public Option<Traceable<PDNode>> Solve()
        {
            return BestFirstSearch.Search(new PDNode(PDPosition.Start(), Enumerable.Repeat(PetStatus.BeforeCarry, petCount).ToArray()),
                Nexts, IsGoal, Comparer<Traceable<PDNode>>.Create(Comparer));
        }

        private IEnumerable<PDNode> Nexts(PDNode current)
        {
            var canCarry = current.PetStatuses.Count(status => status == PetStatus.Carring) < MaxCarry;

            return current.PetStatuses
                .Select((status, i) => new { i, status })
                //運び中または車に空きがある状態で運ぶ前の動物が次の操作の対象になる
                .Where(p => (p.status == PetStatus.Carring) || (canCarry && p.status == PetStatus.BeforeCarry))
                .Select(p =>
                    p.status == PetStatus.BeforeCarry
                        ? new PDNode(PDPosition.Pet(p.i), current.PetStatuses.Select((s, i) => i == p.i ? PetStatus.Carring : s).ToArray())
                        : new PDNode(PDPosition.Home(p.i), current.PetStatuses.Select((s, i) => i == p.i ? PetStatus.AfterCarry : s).ToArray()));
        }

        private int Comparer(Traceable<PDNode> path1, Traceable<PDNode> path2)
        {
            var cost1 = PathCost(path1) + HCost(path1.Value);
            var cost2 = PathCost(path2) + HCost(path2.Value);

            return cost2 - cost1;
        }

        public int PathCost(Traceable<PDNode> path)
        {
            return path.Trace()
                .Apply(list => list.Skip(1)
                    .Aggregate(
                        new { cost = 0, list.First().Position },
                        (acc, node) => new { cost = acc.cost + costs[acc.Position.ToIndex()][node.Position.ToIndex()], node.Position })
                    .Apply(res => res.cost));
        }

        private int HCost(PDNode node)
        {
            var now = node.Position.ToIndex();
            var unVisited = new HashSet<int>(node.PetStatuses.SelectMany((status, i) =>
                status == PetStatus.AfterCarry ? Enumerable.Empty<int>() :
                status == PetStatus.Carring ? new[] { PDPosition.Home(i).ToIndex() } :
                new[] { PDPosition.Pet(i).ToIndex(), PDPosition.Home(i).ToIndex() }));

            return unVisited.Select(to => costs[to].Where((c, from) => from != to && (from == now || unVisited.Contains(from))).Min()).Sum();
        }

        private bool IsGoal(PDNode node)
        {
            return node.PetStatuses.All(status => status == PetStatus.AfterCarry);
        }
    }
}
