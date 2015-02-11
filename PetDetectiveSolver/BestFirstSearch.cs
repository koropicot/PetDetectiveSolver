using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace PetDetectiveSolver
{
    public class BestFirstSearch
    {
        /// <summary>
        /// 探索
        /// </summary>
        /// <typeparam name="T">ノードの型</typeparam>
        /// <param name="start">最初のノード</param>
        /// <param name="nexts">ノードの展開関数</param>
        /// <param name="isGoal">ノードがゴールか否か</param>
        /// <returns></returns>
        public static Option<Traceable<T>> Search<T>(T start, Func<T, IEnumerable<T>> nexts, Predicate<T> isGoal) 
            where T : IEquatable<T>
        {
            return Search(start, nexts, isGoal, _ => { });
        }

        /// <summary>
        /// 探索
        /// </summary>
        /// <typeparam name="T">ノードの型</typeparam>
        /// <param name="start">最初のノード</param>
        /// <param name="nexts">ノードの展開関数</param>
        /// <param name="isGoal">ノードがゴールか否か</param>
        /// <param name="log">ログ出力</param>
        /// <returns></returns>
        public static Option<Traceable<T>> Search<T>(T start, Func<T, IEnumerable<T>> nexts, Predicate<T> isGoal, Action<Traceable<T>> log)
            where T : IEquatable<T>
        {
            return Search(start, nexts, isGoal, Comparer<Traceable<T>>.Create((x, y) => Comparer<T>.Default.Compare(x.Value, y.Value)), log);
        }

        /// <summary>
        /// 探索
        /// </summary>
        /// <typeparam name="T">ノードの型</typeparam>
        /// <param name="start">最初のノード</param>
        /// <param name="nexts">ノードの展開関数</param>
        /// <param name="isGoal">ノードがゴールか否か</param>
        /// <param name="comparer">経路の良さ(良い経路ほど大きい)</param>
        /// <returns></returns>
        public static Option<Traceable<T>> Search<T>(T start, Func<T, IEnumerable<T>> nexts, Predicate<T> isGoal,
            IComparer<Traceable<T>> comparer)
        {
            return Search(start, nexts, isGoal, comparer, _ => { });
        }

        /// <summary>
        /// 探索
        /// </summary>
        /// <typeparam name="T">ノードの型</typeparam>
        /// <param name="start">最初のノード</param>
        /// <param name="nexts">ノードの展開関数</param>
        /// <param name="isGoal">ノードがゴールか否か</param>
        /// <param name="comparer">経路の良さ(良い経路ほど大きい)</param>
        /// <param name="log">ログ出力</param>
        /// <returns></returns>
        public static Option<Traceable<T>> Search<T>(T start, Func<T, IEnumerable<T>> nexts, Predicate<T> isGoal,
            IComparer<Traceable<T>> comparer, Action<Traceable<T>> log)
        {
            return Search(start, nexts, isGoal, comparer, EqualityComparer<T>.Default, log);
        }

        /// <summary>
        /// 探索
        /// </summary>
        /// <typeparam name="T">ノードの型</typeparam>
        /// <param name="start">最初のノード</param>
        /// <param name="nexts">ノードの展開関数</param>
        /// <param name="isGoal">ノードがゴールか否か</param>
        /// <param name="comparer">経路の良さ(良い経路ほど大きい)</param>
        /// <param name="equal">ノードの等価比較</param>
        /// <returns></returns>
        public static Option<Traceable<T>> Search<T>(T start, Func<T, IEnumerable<T>> nexts, Predicate<T> isGoal,
            IComparer<Traceable<T>> comparer, IEqualityComparer<T> equal)
        {
            return Search(start, nexts, isGoal, comparer, equal, _ => { });
        }

        /// <summary>
        /// 探索
        /// </summary>
        /// <typeparam name="T">ノードの型</typeparam>
        /// <param name="start">最初のノード</param>
        /// <param name="nexts">ノードの展開関数</param>
        /// <param name="isGoal">ノードがゴールか否か</param>
        /// <param name="comparer">経路の良さ(良い経路ほど大きい)</param>
        /// <param name="equal">ノードの等価比較</param>
        /// <param name="log">ログ出力</param>
        /// <returns></returns>
        public static Option<Traceable<T>>  Search<T>(T start, Func<T, IEnumerable<T>> nexts, Predicate<T> isGoal,
            IComparer<Traceable<T>> comparer, IEqualityComparer<T> equal, Action<Traceable<T>> log)
        {
            //優先度付きキュー
            var queue = new PriorityQueue<Traceable<T>>(comparer);
            //探索済みノード
            var visited = new Dictionary<T,Traceable<T>>(equal);

            var startPath = Traceable.Create(start, null);
            visited[start] = startPath;
            queue.Enqueue(startPath);

            while (queue.Count != 0)
            {
                var current = queue.Dequeue();
                log(current);

                if (isGoal(current.Value))
                    return Option.Some(current);

                nexts(current.Value)
                    //探索済みで、その経路が今の経路よりも良いときを除く
                    .Where(next => !(visited.ContainsKey(next) && (comparer.Compare(visited[next], Traceable.Create(next, current)) >= 0)))
                    .Select(next => Traceable.Create(next, current)) //パスを作る
                    .ForEach(path => {
                        visited[path.Value] = path;
                        queue.Enqueue(path);
                    });
            }

            return Option.None<Traceable<T>>();
        }
    }
}
