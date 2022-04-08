using System.Collections;
using System.Collections.Generic;


public static class Utilities
{
    // Finds the union of ht1 and ht2, storing the result in ht1.
    public static void Union(ref Hashtable ht1, Hashtable ht2)
    {
        foreach (DictionaryEntry entry in ht2)
        {
            if(!ht1.ContainsKey(entry.Key))
            {
                ht1.Add(entry.Key, entry.Value);
            }
        }
    }
}
