using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidePanel : MonoBehaviour {
    [SerializeField]
    MoveSelectionScreen maneuverScreen;

    GameObject selfGO;

    void Start() {
        selfGO = gameObject;
    }

    public void OnManueverClick() {
        ShowScreen(false);
        maneuverScreen.ShowScreen(true);
    }

    public void OnHelpClick() {

    }

    public void OnOptionsClick() {

    }

    public void ShowScreen(bool value) {
        selfGO.SetActive(value);
    }
}
