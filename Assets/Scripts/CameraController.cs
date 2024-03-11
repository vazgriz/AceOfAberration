using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField]
    Vector3 offset;
    [SerializeField]
    float smoothing;

    new Transform transform;
    Transform attachedTransform;

    void Awake() {
        transform = GetComponent<Transform>();
    }

    public void SetAttachment(Transform transform) {
        if (transform == null) throw new ArgumentNullException(nameof(transform));

        attachedTransform = transform;
    }

    void LateUpdate() {
        if (attachedTransform == null) return;

        var attachmentRotation = attachedTransform.rotation;

        transform.position = attachedTransform.position + attachmentRotation * offset;
        transform.rotation = attachmentRotation;
    }
}
