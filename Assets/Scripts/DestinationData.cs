using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationData : ScriptableObject {
    public string displayName;
    [Tooltip("Minimum required power to reach this destination")]
    public int requiredPower;
    [Tooltip("Duration in seconds of mission to this destination with minimum power requirement")]
    public float baseMissionDuration;
    public Sprite icon;
    public int progressionValue;
}
