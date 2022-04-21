using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartButtonController : MonoBehaviour
{

    [SerializeField] private TMP_Text _helpText;
    public bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        _helpText.gameObject.SetActive(false);
        isActive = false;
    }

    public void OnMouseOver() {
        if (!isActive) _helpText.gameObject.SetActive(true);
    }

    public void OnMouseExit() {
        if (!isActive) _helpText.gameObject.SetActive(false);
    }
}
