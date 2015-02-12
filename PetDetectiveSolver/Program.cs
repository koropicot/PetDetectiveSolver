using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extensions;

namespace PetDetectiveSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = new List<string>();
            while (true)
            {
                var line = Console.ReadLine();
                if (line == "")
                    break;
                lines.Add(line);
            }

            var solver = PDInputUtil.CreateSolver(lines.Select(s => s.ToArray()).ToArray());

            solver.Solve().Match(
                tr => solver.PathCost(tr).ToString() + ":" +  tr.Trace().Select(node => node.Position.ToString()).Apply(list => string.Join(" ", list)),
                () => "ERROR")
                .Act(Console.WriteLine);

            Console.ReadLine();
        }

    }


}
