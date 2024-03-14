using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaneSelector : MonoBehaviour {
    [Serializable]
    public class PlaneData {
        public string name;
        public Texture preview;
        public GameObject prefab;
    }

    [SerializeField]
    List<PlaneData> planes;
    [SerializeField]
    RawImage image;
    [SerializeField]
    TextMeshProUGUI label;

    int index;

    public PlaneData GetSelection() {
        return planes[index];
    }

    void Start() {
        ShowData(planes[index]);
    }

    void ShowData(PlaneData data) {
        image.texture = data.preview;
        label.text = data.name;
    }

    public void MoveNext() {
        index = (index + 1) % planes.Count;
        ShowData(planes[index]);
    }

    public void MovePrevious() {
        index = (index + planes.Count + 1) % planes.Count;
        ShowData(planes[index]);
    }
}
