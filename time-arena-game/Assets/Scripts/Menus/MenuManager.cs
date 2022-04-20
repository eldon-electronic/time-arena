using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public static MenuManager Instance;
    [SerializeField] Menu[] menus;

    void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string name) { 
        for (int i = 0; i < menus.Length; i++) {
            if (menus[i].MenuName == name) {
                menus[i].Open();
            } else if (menus[i].MenuOpen) {
                CloseMenu(menus[i]);
            }
        }
    }

    public void OpenMenu(Menu menu) {
        for (int i = 0; i < menus.Length; i++) {
            if (menus[i].MenuOpen) {
                CloseMenu(menus[i]);
            }
        }
        menu.Open();
    }

    public void CloseMenu(Menu menu) { 
        menu.Close();
    }

    public bool IsOpen(string name) {
        for (int i = 0; i < menus.Length; i++) {
            if ((menus[i].MenuName == name) && menus[i].MenuOpen) return true;
        } return false;
    }
}
