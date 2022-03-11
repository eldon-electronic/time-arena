using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{

    public string MenuName; // when naming menus in Unity, use camelCase of the GameObject name
    public bool IsOpen;       // only for access accross scripts

    public void Open() {
        IsOpen = true;
        gameObject.SetActive(true);
    }

    public void Close() {
        IsOpen = false;
        gameObject.SetActive(false);
    }
}
