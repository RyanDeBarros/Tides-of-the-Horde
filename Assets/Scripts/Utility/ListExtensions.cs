using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class ListExtensions
{
    public static List<T> GetRandomDistinctElements<T>(this List<T> list, int count)
    {
        // Fisher-Yates shuffle

        List<T> result = new(list);

        for (int i = result.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (result[i], result[j]) = (result[j], result[i]);
        }

        return result.GetRange(0, Mathf.Min(count, result.Count));
    }

    public static T GetUniqueElement<T>(this T[] array)
    {
        Assert.IsTrue(array.Length == 1);
        return array[0];
    }
}
