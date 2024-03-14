using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauges : MonoBehaviour {
    [SerializeField]
    float slowAngle;
    [SerializeField]
    float cruiseAngle;
    [SerializeField]
    float fastAngle;

    [SerializeField]
    new RectTransform transform;

    public void SetSpeed(Plane.PlaneSpeed speed) {
        float angle = 0;
        switch (speed) {
            case Plane.PlaneSpeed.Slow:
                angle = slowAngle;
                break;
            case Plane.PlaneSpeed.Cruise:
                angle = cruiseAngle;
                break;
            case Plane.PlaneSpeed.Fast:
                angle = fastAngle;
                break;
        }

        transform.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void Show(bool value) {
        gameObject.SetActive(value);
    }
}
