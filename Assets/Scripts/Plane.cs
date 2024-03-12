using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Plane : MonoBehaviour {
    public enum PlaneSpeed {
        Slow,
        Cruise,
        Fast
    }

    [Serializable]
    struct AudioData {
        public AudioSource audioSource;
        public float volume;

        public void Play() {
            if (audioSource != null) {
                audioSource.Play();
            }
        }

        public void Stop() {
            if (audioSource != null) {
                audioSource.Stop();
            }
        }

        public void SetFadeVolume(float value) {
            if (audioSource != null) {
                audioSource.volume = volume * value;
            }
        }
    }

    [SerializeField]
    ManeuverList maneuverList;

    [SerializeField]
    float audioFadeTime;
    [SerializeField]
    AudioData propellerAudio;
    [SerializeField]
    AudioData windAudio;
    [SerializeField]
    List<ManeuverData> specialValidMoves;

    float audioFadeTarget;
    float audioFadeTimer;

    HexCoord positionHex;
    HexDirection planeDirection;
    new Transform transform;

    bool maneuvering;
    ManeuverData currentManeuver;
    Spline currentSpline;

    float manueverTimer;
    Vector3 startPosition;
    Vector3 targetPosition;
    Quaternion startRotation;
    Quaternion targetRotation;

    ManeuverData lastManeuver;

    public ManeuverList ManeuverList {
        get {
            return maneuverList;
        }
    }

    public HexCoord PositionHex {
        get {
            return positionHex;
        }
        set {
            SetPositionHex(value);
        }
    }

    public bool Maneuvering {
        get {
            return maneuvering;
        }
    }
    
    public float GridSize { get; set; }
    public float ManeuverTime { get; set; }

    public PlaneSpeed Speed { get; private set; }

    public void Init() {
        transform = GetComponent<Transform>();
    }

    void Update() {
        UpdateManeuver();
        UpdateAudio();
    }

    void SetPositionHex(HexCoord pos) {
        positionHex = pos;
        transform.position = HexGrid.GetCenter(pos) * GridSize;
    }

    public bool IsSpecialManeuverValid() {
        if (Speed == PlaneSpeed.Slow) return false;
        if (lastManeuver != null) {
            bool found = false;

            foreach (var move in specialValidMoves) {
                if (move == lastManeuver) {
                    found = true;
                    break;
                }
            }

            if (!found) return false;
        }

        return true;
    }

    public bool IsManeuverValid(ManeuverData data) {
        if (data.isSpecial) {
            return IsSpecialManeuverValid();
        }

        if ((int)Speed < (int)data.minSpeed) {
            return false;
        }

        if ((int)Speed > (int)data.maxSpeed) {
            return false;
        }

        return true;
    }

    public bool PlayManeuver(ManeuverData data) {
        if (maneuvering) return false;
        if (!IsManeuverValid(data)) return false;

        maneuvering = true;
        manueverTimer = 0;
        currentManeuver = data;
        currentSpline = data.spline.GetComponent<SplineContainer>().Spline;

        ManeuverState maneuverState = GameBoard.CalculateManeuver(data, planeDirection);

        Vector2 pos = HexGrid.GetCenter(positionHex) * GridSize;
        startPosition = transform.position;
        targetPosition = new Vector3(pos.x, 0, pos.y);

        float startAngle = HexGrid.GetAngle(planeDirection);
        float targetAngle = HexGrid.GetAngle(maneuverState.finalState.direction);
        startRotation = Quaternion.Euler(0, startAngle, 0);
        targetRotation = Quaternion.Euler(0, targetAngle, 0);

        positionHex += maneuverState.finalState.position;
        planeDirection = maneuverState.finalState.direction;

        PlayWindProp();
        Speed = data.finalSpeed;

        return true;
    }

    void UpdateManeuver() {
        if (!maneuvering) return;

        float t = Mathf.InverseLerp(0, ManeuverTime, manueverTimer);
        float smoothT;

        if (currentManeuver.speedOverride) {
            smoothT = currentManeuver.speedCurve.Evaluate(t);
        } else {
            smoothT = t;
        }

        currentSpline.Evaluate(smoothT, out var pos, out var forward, out var up);
        transform.position = startPosition + GridSize * (startRotation * pos);
        transform.rotation = startRotation * Quaternion.LookRotation(forward, up);

        if (t >= 1) {
            maneuvering = false;
            transform.position = targetPosition;
            transform.rotation = targetRotation;

            FadeOutWindProp();
        }

        manueverTimer += Time.deltaTime;
    }

    void PlayWindProp() {
        audioFadeTimer = 0;
        audioFadeTarget = 1;

        propellerAudio.Play();
        propellerAudio.SetFadeVolume(0);
        windAudio.Play();
        windAudio.SetFadeVolume(0);
    }

    void FadeOutWindProp() {
        audioFadeTimer = 0;
        audioFadeTarget = 0;
    }

    void StopWindProp() {
        propellerAudio.Stop();
        windAudio.Stop();
    }

    void SetWindPropFade(float t) {
        propellerAudio.SetFadeVolume(t);
        windAudio.SetFadeVolume(t);
    }

    void UpdateAudio() {
        float fade;

        if (audioFadeTimer > audioFadeTime || audioFadeTime == 0) {
            fade = audioFadeTarget;
        } else {
            fade = Mathf.InverseLerp(1 - audioFadeTarget, audioFadeTarget, audioFadeTimer / audioFadeTime);
        }

        SetWindPropFade(fade);

        if (audioFadeTarget == 0 && fade == 0) {
            StopWindProp();
        }

        audioFadeTimer += Time.deltaTime;
    }
}
