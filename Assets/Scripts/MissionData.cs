using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionData : ScriptableObject {
    public RocketData rocketData;
    public PayloadData payloadData;
    public long startTime;
    public long duration;
    public Point startPoint;
    public Point endPoint;
}
