using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Maneuver List")]
public class ManeuverList : ScriptableObject {
    public List<ManeuverData> maneuvers;
}
