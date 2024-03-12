using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidePanel : MonoBehaviour {
    [SerializeField]
    MoveSelectionScreen maneuverScreen;
    [SerializeField]
    AudioSource errorSound;

    GameObject selfGO;

    void Start() {
        selfGO = gameObject;
    }

    public void OnManueverClick() {
        bool success = maneuverScreen.ShowScreen(true);

        if (success) {
            ShowScreen(false);
        } else {
            errorSound.Play();
        }
    }

    public void OnHelpClick() {

    }

    public void OnOptionsClick() {

    }

    public void ShowScreen(bool value) {
        selfGO.SetActive(value);
    }
}
