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
        private HashSet<PDPet> pets;
        private Dictionary<PDPoint, Dictionary<PDPoint, int>> costs;

        public PDSolver(HashSet<PDPet> pets, Dictionary<PDPoint, Dictionary<PDPoint, int>> costs)
        {
            this.pets = pets;
            this.costs = costs;
        }

        public Option<Traceable<PDNode>> Solve()
        {
            return BestFirstSearch.Search(new PDNode(PDPoint.Start(), pets.ToDictionary(pet => pet , _ => PDPetStatus.BeforeCarry)),
                Nexts, IsGoal, Comparer<Traceable<PDNode>>.Create(Comparer));
        }

        private IEnumerable<PDNode> Nexts(PDNode current)
        {
            var canCarry = current.PetStatuses.Count(kvp => kvp.Value == PDPetStatus.Carring) < MaxCarry;

            return current.PetStatuses
                //運び中または車に空きがある状態で運ぶ前の動物が次の操作の対象になる
                .Where(kvp => (kvp.Value == PDPetStatus.Carring) || (canCarry && kvp.Value == PDPetStatus.BeforeCarry))
                .Select(next =>
                    next.Value == PDPetStatus.BeforeCarry
                        ? new PDNode(PDPoint.Pet(next.Key),
                            current.PetStatuses.ToDictionary(kvp => kvp.Key, kvp => kvp.Key == next.Key ? PDPetStatus.Carring : kvp.Value))
                        : new PDNode(PDPoint.Home(next.Key),
                            current.PetStatuses.ToDictionary(kvp => kvp.Key, kvp => kvp.Key == next.Key ? PDPetStatus.AfterCarry : kvp.Value)));
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
                        (acc, node) => new { cost = acc.cost + costs[acc.Position][node.Position], node.Position })
                    .Apply(res => res.cost));
        }

        private int HCost(PDNode node)
        {
            var now = node.Position;
            var unVisited = new HashSet<PDPoint>(node.PetStatuses.SelectMany(kvp =>
                kvp.Value == PDPetStatus.AfterCarry ? Enumerable.Empty<PDPoint>() :
                kvp.Value == PDPetStatus.Carring ? new[] { PDPoint.Home(kvp.Key) } :
                new[] { PDPoint.Pet(kvp.Key), PDPoint.Home(kvp.Key) }));

            return unVisited
                .Select(uv => costs[uv]
                    .Where(kvp => kvp.Key != uv && (kvp.Key == now || unVisited.Contains(kvp.Key)))
                    .Apply(cost => cost.Any() ? cost.Min(kvp => kvp.Value) : 0))
                .Sum();
        }

        private bool IsGoal(PDNode node)
        {
            return node.PetStatuses.All(kvp => kvp.Value == PDPetStatus.AfterCarry);
        }
    }
}
