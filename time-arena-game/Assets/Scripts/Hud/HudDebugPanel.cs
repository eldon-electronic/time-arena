using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public interface Debuggable
{
    public Hashtable GetDebugValues();
}

public class HudDebugPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup _debugCanvasGroup;
    [SerializeField] private Text _text;
    private List<Debuggable> _subsribers;
    private Hashtable _debugItems;

    void Awake()
    {
        _debugCanvasGroup.alpha = 0.0f;
        _subsribers = new List<Debuggable>();
        _debugItems = new Hashtable();
    }

    void Update()
    {
        _debugItems = new Hashtable();
        foreach (var subscriber in _subsribers)
        {
            if (subscriber == null) _subsribers.Remove(subscriber);
            else
            {
                Hashtable values = subscriber.GetDebugValues();
                Utilities.Union(ref _debugItems, values);
            }
        }
    }

    void LateUpdate()
    {
        // Set visibility.
        if (Input.GetKeyDown(KeyCode.P))
        {
            _debugCanvasGroup.alpha = 1.0f - _debugCanvasGroup.alpha;
        }

        // Set text to display.
        System.String debugText = "";
        foreach (DictionaryEntry de in _debugItems)
        {
            debugText += $"{de.Key}: {de.Value}\n";
        }
        _text.text = debugText;
    }

    public void Register(Debuggable subscriber)
    {
        _subsribers.Add(subscriber);
    }
}
