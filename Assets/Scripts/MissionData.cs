using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionData : ScriptableObject {
    public RocketData rocketData;
    public PayloadData payloadData;
    public DestinationData destinationData;
    
    public float getDurationSeconds() {
        float powerFactor = Mathf.Min(3, Mathf.Max(1, (rocketData.power - payloadData.weight) / (float) destinationData.requiredPower));
        return (destinationData.baseMissionDuration / powerFactor);
    }

    public int getCost() {
        return rocketData.launchCost - payloadData.launchValue;
    }

    public bool isValid() {
        return rocketData.power - payloadData.weight >= destinationData.requiredPower;
    }

    internal static MissionData create(RocketData rocket, PayloadData payload, DestinationData destination) {
        var data = ScriptableObject.CreateInstance<MissionData>();
        data.rocketData = rocket;
        data.payloadData = payload;
        data.destinationData = destination;
        return data;
    }
}
