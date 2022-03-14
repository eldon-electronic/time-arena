using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{

    public string MenuName;
    public bool IsOpen;

    public void Open() {
        IsOpen = true;
        gameObject.SetActive(true);
    }

    public void Close() {
        IsOpen = false;
        gameObject.SetActive(false);
    }
}
