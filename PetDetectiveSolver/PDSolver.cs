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
            long searchNodes = 0;
            var ret =  BestFirstSearch.Search(new PDNode(PDPoint.Start(), pets.ToDictionary(pet => pet , _ => PDPetStatus.BeforeCarry)),
                Nexts, IsGoal, Comparer<Traceable<PDNode>>.Create(Comparer),
                _ => searchNodes++);

            Console.WriteLine(searchNodes);
            return ret;
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

        private Dictionary<Traceable<PDNode>, int> memo = new Dictionary<Traceable<PDNode>,int>();

        public int PathCost(Traceable<PDNode> path)
        {
            if(path.Parent == null)
                return 0;

            if (memo.ContainsKey(path))
                return memo[path];

            var cost = PathCost(path.Parent) + costs[path.Parent.Value.Position][path.Value.Position];
            memo[path] = cost;

            return cost;
        }

        private int HCost(PDNode node)
        {
            return node.PetStatuses.Values
                .Select(s => s == PDPetStatus.AfterCarry ? 0 : s == PDPetStatus.Carring ? 1 : 2)
                .Sum();
        }

        private bool IsGoal(PDNode node)
        {
            return node.PetStatuses.All(kvp => kvp.Value == PDPetStatus.AfterCarry);
        }
    }
}
