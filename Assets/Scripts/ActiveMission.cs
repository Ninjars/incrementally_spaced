using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveMission : ScriptableObject {
    public MissionData missionData;
    public float startTime;

    internal static ActiveMission create(MissionData mission, float realtimeSinceStartup) {
        var obj = ScriptableObject.CreateInstance<ActiveMission>();
        obj.missionData = mission;
        obj.startTime = realtimeSinceStartup;
        return obj;
    }
}
