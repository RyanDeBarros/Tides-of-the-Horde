using System.Collections.Generic;
using System.Linq;
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

        return result.GetRange(0, System.Math.Min(count, result.Count));
    }

    public static List<T> GetWeightedRandomDistinctElements<T>(this List<T> list, int count, List<float> weights)
    {
        Assert.IsNotNull(list);
        Assert.IsNotNull(weights);
        if (list.Count != weights.Count)
            throw new System.ArgumentException("List and weights must have the same length.");

        List<T> result = new();
        List<T> tempList = new(list);
        List<float> tempWeights = new(weights);

        count = System.Math.Min(count, list.Count);

        for (int k = 0; k < count; k++)
        {
            int index = RandomSupport.GetWeightedIndex(tempWeights);
            result.Add(tempList[index]);
            tempList.RemoveAt(index);
            tempWeights.RemoveAt(index);
        }

        return result;
    }

    public static T GetUniqueElement<T>(this T[] array)
    {
        Assert.IsTrue(array.Length == 1);
        return array[0];
    }
}
