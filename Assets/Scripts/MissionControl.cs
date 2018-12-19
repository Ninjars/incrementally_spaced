using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MissionControl : MonoBehaviour {
    public Registry registry;
    public GameStateProvider gameStateProvider;
    public MissionPlanner missionPlanner;
    public Button startPlanningButton;
    public Text fundsReadout;
    public Text missionsReadout;
    public RectTransform creditsScreen;
    public CanvasGroup explodedRocketMessage;
    public Button creditsButton;

    private GameState gameState;
    private System.Random random;

    void Start() {
        startPlanningButton.onClick.AddListener(() => startPlanningMission());
        gameState = gameStateProvider.getGameState();
        creditsButton.onClick.AddListener(() => toggleCredits());
        random = new System.Random();
    }

    void Update() {
        fundsReadout.text = "Available funds: " + gameState.funds;
        missionsReadout.text = buildActiveMissionsString();
        if (explodedRocketMessage.alpha > 0) {
            explodedRocketMessage.alpha -= (Time.deltaTime / 5);
        }
    }

    private string buildActiveMissionsString() {
        StringBuilder stringBuilder = new StringBuilder("Active Missions:");
        foreach (var mission in gameState.GetActiveMissions()) {
            var data = mission.getMissionData();
            stringBuilder.Append("\n" + data.rocketData.rocketName + " : " + data.payloadData.payloadName + " : " + data.destinationData.displayName);
            stringBuilder.Append("\n>> T-" + mission.remainingMissionDuration().ToString("0.0"));
        }
        return stringBuilder.ToString();
    }

    private void setUiModePlanning() {
        missionPlanner.gameObject.SetActive(true);
        startPlanningButton.gameObject.SetActive(false);
        fundsReadout.gameObject.SetActive(false);
        missionsReadout.gameObject.SetActive(false);
        creditsButton.gameObject.SetActive(false);
    }

    private void setUiModeMissionControl() {
        missionPlanner.gameObject.SetActive(false);
        startPlanningButton.gameObject.SetActive(true);
        fundsReadout.gameObject.SetActive(true);
        missionsReadout.gameObject.SetActive(true);
        creditsButton.gameObject.SetActive(true);
    }

    public void startPlanningMission() {
        setUiModePlanning();
    }

    public void cancelPlanningMission() {
        setUiModeMissionControl();
    }

    public void launchMission(MissionData mission) {
        Debug.Log("launchMission: " + mission);
        setUiModeMissionControl();

        FlightPlan flightPlan = registry.flightPlans.Where(plan => plan.destination == mission.destinationData).First();
        Rocket rocket = GameObject.Instantiate(mission.rocketData.rocketObject);

        var activeMission = new ActiveMission(this, rocket, mission, Time.time, flightPlan);
        gameState.registerActiveMission(activeMission);
        gameState.funds -= mission.getCost();
        gameState.registerProgress(mission.destinationData.progressionValue);
        activeMission.launch();

        if (missionWillFail(mission)) {
            float explodeAfter = mission.getDurationSeconds() * 0.1f * Convert.ToSingle(random.NextDouble());
            StartCoroutine(failMission(activeMission, explodeAfter));
        }
    }

    private bool missionWillFail(MissionData mission) {
        float launchSuccessChance = mission.rocketData.getLaunchChance(gameState.getTotalLaunchCount());
        if (launchSuccessChance < 1) {
            var randomVal = random.NextDouble();
            return launchSuccessChance < randomVal;
        } else {
            return false;
        }
    }

    private IEnumerator failMission(ActiveMission activeMission, float delay) {
        yield return new WaitForSeconds(delay);
        gameState.registerMissionFailure(activeMission);
        activeMission.explode();
        explodedRocketMessage.alpha = 1;
    }

    internal void onMissionComplete(ActiveMission activeMission) {
        activeMission.getRocket().onMissionComplete();
        var data = activeMission.getMissionData();
        Debug.Log("onMissionComplete: " + data);
        var completionBonus = data.payloadData.successBonus;
        gameState.registerMissionCompletion(activeMission);
        gameState.funds += completionBonus;
    }

    public MissionData buildMission(RocketData rocket, PayloadData payload, DestinationData destination) {
        var mission = new MissionData();
        mission.payloadData = payload;
        mission.rocketData = rocket;
        mission.destinationData = destination;
        return mission;
    }

    public void debugAdvanceGameStage() {
        gameState.registerProgress(gameState.getCurrentProgressValue() + 10);
    }

    public void toggleCredits() {
        if (creditsScreen.gameObject.activeSelf) {
            creditsScreen.gameObject.SetActive(false);
            startPlanningButton.gameObject.SetActive(true);
        } else {
            creditsScreen.gameObject.SetActive(true);
            startPlanningButton.gameObject.SetActive(false);
        }
    }
}
