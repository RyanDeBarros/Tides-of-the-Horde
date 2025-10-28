using System;
using System.Collections.Generic;
using System.Linq;

public class RandomSupport
{
    private static readonly Random rng = new();

    public static int GetWeightedIndex(List<float> weights)
    {
        if (weights == null || weights.Count == 0)
            throw new ArgumentException("Weights list cannot be null or empty.");

        float totalWeight = weights.Sum();
        if (totalWeight <= 0f)
            throw new ArgumentException("The total weight must be positive.");

        float randomValue = (float)(rng.NextDouble() * totalWeight);
        float cumulative = 0f;
        for (int i = 0; i < weights.Count; i++)
        {
            cumulative += weights[i];
            if (randomValue < cumulative)
                return i;
        }
        return weights.Count - 1;
    }

    public static T RandomElement<T>(List<T> list)
    {
        return list[rng.Next(0, list.Count - 1)];
    }
}
