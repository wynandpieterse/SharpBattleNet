namespace SharpBattleNet.Runtime.Utilities.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class EnumerableExtensions
    {
        public static void ForEach<Type>(this IEnumerable<Type> enumerable, Action<Type> callback)
        {
            foreach(var item in enumerable)
            {
                callback(item);
            }

            return;
        }

        public static void ForEachAsync<Type>(this IEnumerable<Type> enumerable, Action<Type> callback)
        {
            List<Task> tasks = new List<Task>();
            Task task = null;

            foreach(var item in enumerable)
            {
                task = Task.Factory.StartNew(() => callback(item));
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            return;
        }
    }
}
