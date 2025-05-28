using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UIElementEntry
{
    public string key;
    public GameObject element;
}

public class UIElementRegistry : MonoBehaviour
{
    private Dictionary<string, GameObject> uiElements = new Dictionary<string, GameObject>();

    public void RegisterElements(List<UIElementEntry> elements)
    {
        foreach (var entry in elements)
        {
            if (!uiElements.ContainsKey(entry.key))
            {
                uiElements.Add(entry.key, entry.element);
            }
        }
    }

    public GameObject GetElement(string key) => 
        uiElements.TryGetValue(key, out var element) ? element : null;
}