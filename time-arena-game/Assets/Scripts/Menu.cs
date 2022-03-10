using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{

    public string menuName; // when naming menus in Unity, use camelCase of the GameObject name
    public bool open;       // only for access accross scripts

    public void Open() {
        open = true;
        gameObject.SetActive(true);
    }

    public void Close() {
        open = false;
        gameObject.SetActive(false);
    }
}
