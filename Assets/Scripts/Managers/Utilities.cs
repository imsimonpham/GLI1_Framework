using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static void SelectThreeRandomIntegers(List<int> list, int min, int max)
    {
        if (max - min + 1 < 3)
        {
            Debug.LogError("Range is too small to generate 3 unique integers.");
            return;
        }

        int newIndex;
        int maxAttempts = 100; // Maximum attempts to prevent infinite loop
        int attempts = 0;

        while (list.Count < 3 && attempts < maxAttempts)
        {
            newIndex = Random.Range(min, max);
            if (!list.Contains(newIndex))
            {
                list.Add(newIndex);
            }
            attempts++;
        }

        if (attempts >= maxAttempts)
        {
            Debug.LogError("Maximum attempts reached without finding 3 unique integers.");
        }
    }

    public static List<int> GetRandomCoverList(List<List<int>> list)
    {
        int randomIndex = Random.Range(0, list.Count);

        return list[randomIndex];
    }
}
