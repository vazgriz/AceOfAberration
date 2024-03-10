using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManeuverIcon : MonoBehaviour {
    [SerializeField]
    Image iconImage;

    public void SetImage(Sprite sprite) {
        if (iconImage == null) return;
        iconImage.sprite = sprite;
    }
}
