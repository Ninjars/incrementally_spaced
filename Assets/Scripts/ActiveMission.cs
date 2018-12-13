using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveMission : ScriptableObject {
    public MissionData missionData;
    public float startTime;

    public ActiveMission(MissionData mission, float realtimeSinceStartup) {
        this.missionData = mission;
        this.startTime = realtimeSinceStartup;
    }
}
