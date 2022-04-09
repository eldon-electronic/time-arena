using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudDebugPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup _debugCanvasGroup;
    [SerializeField] private Text _text;
    private Hashtable _debugItems;

    void Start()
    {
        _debugCanvasGroup.alpha = 0.0f;
        _debugItems = new Hashtable();
    }

    void LateUpdate()
    {
        System.String debugText = "";
        foreach (DictionaryEntry de in _debugItems)
        {
            debugText += $"{de.Key}: {de.Value}\n";
        }
        _text.text = debugText;
    }

    public void ToggleDebug()
    {
        _debugCanvasGroup.alpha = 1.0f - _debugCanvasGroup.alpha;
    }

    public void SetDebugValues(Hashtable items) { _debugItems = items; }
}
