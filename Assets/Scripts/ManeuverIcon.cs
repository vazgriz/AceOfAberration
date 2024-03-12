using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManeuverIcon : MonoBehaviour {
    [SerializeField]
    Image iconImage;

    bool init;
    new RectTransform transform;
    ManeuverData maneuverData;

    public ManeuverData ManeuverData {
        get {
            return maneuverData;
        }
    }

    public event Action<ManeuverData> OnClicked = delegate { };
    public event Action<ManeuverData> OnHoverEnter = delegate { };
    public event Action<ManeuverData> OnHoverExit = delegate { };

    public void SetManeuverData(ManeuverData data) {
        if (data == null) throw new ArgumentNullException(nameof(data));
        maneuverData = data;

        transform = GetComponent<RectTransform>();

        SetImage(data.icon, data.invertIcon);
    }

    void SetImage(Sprite sprite, bool invert) {
        if (iconImage == null) return;
        iconImage.sprite = sprite;
        transform.localScale = new Vector3(invert ? -1 : 1, 1, 1);
    }

    public void ShowImage(bool value) {
        if (iconImage == null) return;
        iconImage.enabled = value;
    }

    public void OnUIClick() {
        OnClicked(maneuverData);
    }

    public void OnUIHoverEnter() {
        OnHoverEnter(maneuverData);
    }

    public void OnUIHoverExit() {
        OnHoverExit(maneuverData);
    }
}
