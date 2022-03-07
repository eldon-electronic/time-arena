using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public static MenuManager Instance;
    [SerializeField] Menu[] menus;

    void Awake() {
        Instance = this; // References this script, and not an object
    }

    // Allows reference to menus inside scripts simply using strings, rather than storing them
    public void OpenMenu(string name) { 
        for (int i = 0; i < menus.Length; i++) {
            if (menus[i].menuName == name) {
                OpenMenu(menus[i]);
            } else if (menus[i].open) {
                CloseMenu(menus[i]);
            }
        }
    }

    // Allows us to attach menu GameObjects from the Inspector
    public void OpenMenu(Menu menu) { 
        menu.Open();
    }

    public void CloseMenu(Menu menu) { 
        menu.Close();
    }
}
