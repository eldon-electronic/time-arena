using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{

    public string MenuName;
    public bool MenuOpen;

    public void Open() {
        MenuOpen = true;
        gameObject.SetActive(true);
    }

    public void Close() {
        MenuOpen = false;
        gameObject.SetActive(false);
    }
}
