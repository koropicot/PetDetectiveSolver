using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetDetectiveSolver;

namespace PetDetectiveSolverTest
{
    [TestClass]
    public class PriorityQueueTest
    {
        [TestMethod]
        public void エンキューした値は優先度順でデキューされる()
        {
            var priorityQueue = new PriorityQueue<int>(new[] { 3, 5, 6 });

            priorityQueue.Enqueue(2);
            priorityQueue.Enqueue(8);

            Assert.AreEqual(8, priorityQueue.Dequeue());
            Assert.AreEqual(6, priorityQueue.Dequeue());
            Assert.AreEqual(5, priorityQueue.Dequeue());
            Assert.AreEqual(3, priorityQueue.Dequeue());
            Assert.AreEqual(2, priorityQueue.Dequeue());
        }

        [TestMethod]
        public void 与えたComparerが優先度の比較に用いられる()
        {
            var priorityQueue = new PriorityQueue<Tuple<int, int>>(
                Comparer<Tuple<int, int>>.Create((t1, t2) =>
                    t1.Item1 == t2.Item1
                        ? t1.Item2 - t2.Item2
                        : t1.Item1 - t2.Item1));

            priorityQueue.Enqueue(Tuple.Create(1, 1));
            priorityQueue.Enqueue(Tuple.Create(2, 1));
            priorityQueue.Enqueue(Tuple.Create(1, 3));
            priorityQueue.Enqueue(Tuple.Create(2, 2));
            priorityQueue.Enqueue(Tuple.Create(3, 2));


            Assert.AreEqual(Tuple.Create(3, 2), priorityQueue.Dequeue());
            Assert.AreEqual(Tuple.Create(2, 2), priorityQueue.Dequeue());
            Assert.AreEqual(Tuple.Create(2, 1), priorityQueue.Dequeue());
            Assert.AreEqual(Tuple.Create(1, 3), priorityQueue.Dequeue());
            Assert.AreEqual(Tuple.Create(1, 1), priorityQueue.Dequeue());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void エンキューした数より多くデキューすると例外が発生する()
        {
            var priorityQueue = new PriorityQueue<int>();
            priorityQueue.Enqueue(1);
            priorityQueue.Enqueue(2);

            priorityQueue.Dequeue();
            priorityQueue.Dequeue();

            //ここで例外が発生する
            priorityQueue.Dequeue();
        }
    }
}
