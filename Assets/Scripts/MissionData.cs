using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionData : ScriptableObject {
    public RocketData rocketData;
    public PayloadData payloadData;
    public DestinationData destinationData;
    
    public float durationSeconds() {
        float powerFactor = Mathf.Min(3, Mathf.Max(1, (rocketData.power - payloadData.weight) / (float) destinationData.requiredPower));
        return (destinationData.baseMissionDuration / powerFactor);
    }

    public bool isValid() {
        return rocketData.power - payloadData.weight >= destinationData.requiredPower;
    }
}
