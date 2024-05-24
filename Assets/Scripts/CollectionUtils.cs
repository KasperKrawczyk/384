using System;
using System.Collections.Generic;
using System.Linq;

public class CollectionUtils
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


    public static Dictionary<TKeys, TValues> Shuffle<TKeys, TValues>(IDictionary<TKeys, TValues> dictionary)
    {
        Random r = new Random();
        return dictionary.OrderBy(x => r.Next())
            .ToDictionary(item => item.Key, item => item.Value);
    }
}