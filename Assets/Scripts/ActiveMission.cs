using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveMission {
    private Rocket rocket;
    private MissionControl missionControl;
    private MissionData missionData;
    private float startTime;
    private float duration;
    private FlightPlan flightPlan;

    public ActiveMission(MissionControl missionControl, Rocket rocket, MissionData mission, float realtimeSinceStartup, FlightPlan flightPlan) {
        this.rocket = rocket;
        this.missionControl = missionControl;
        missionData = mission;
        startTime = realtimeSinceStartup;
        this.flightPlan = flightPlan;
        rocket.transform.position = flightPlan.flightPath.nodes[0];
        duration = mission.getDurationSeconds();
    }

    public float remainingMissionDuration() {
        return duration - (Time.time - startTime);
    }

    public void launch() {
        Debug.Log("launch ActiveMission time " + missionData.getDurationSeconds());
        iTween.MoveTo(rocket.gameObject, iTween.Hash(
                    "path", flightPlan.flightPath.nodes.ToArray(), 
                    "time", missionData.getDurationSeconds(),
                    "easeType", iTween.EaseType.easeInSine,
                    "oncomplete", "onMissionComplete",
                    "oncompletetarget", missionControl.gameObject,
                    "oncompleteparams", this
                )
            );
    }

    public Rocket getRocket() {
        return rocket;
    }

    public MissionData getMissionData() {
        return missionData;
    }
}
