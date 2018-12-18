using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveMission {
    private Rocket rocket;
    private MissionData missionData;
    private float startTime;
    private FlightPlan flightPlan;

    public ActiveMission(Rocket rocket, MissionData mission, float realtimeSinceStartup, FlightPlan flightPlan) {
        this.rocket = rocket;
        missionData = mission;
        startTime = realtimeSinceStartup;
        this.flightPlan = flightPlan;
        rocket.transform.position = flightPlan.flightPath.nodes[0];
    }

    public void launch() {
        iTween.MoveTo(rocket.gameObject, iTween.Hash(
                    "path", flightPlan.flightPath.nodes.ToArray(), 
                    "time", missionData.getDurationSeconds(),
                    "easeType", iTween.EaseType.easeInOutCubic,
                    "onComplete", "onMissionComplete"
                )
            );
    }

    public void onMissionComplete() {
        rocket.missionComplete();
    }
}
