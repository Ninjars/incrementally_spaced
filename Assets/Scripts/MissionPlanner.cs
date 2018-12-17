using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MissionPlanner : MonoBehaviour {

	private class WipMission {
		public RocketData rocket;
		public PayloadData payload;
		public DestinationData destination;
	}

	public GameStateProvider gameStateProvider;
	public Registry registry;
	public MissionControl missionControl;
	public ListItemUI listItem;
	public RectTransform rocketList;
	public RectTransform payloadList;
	public RectTransform destinationList;
	public PlanDetailsItemUI detailsItem;
	public RectTransform detailsList;
	public Button launchButton;
	private WipMission plannedMission;
    private ToggleGroup rocketToggleGroup;
    private ToggleGroup payloadToggleGroup;
    private ToggleGroup destinationToggleGroup;

	void Awake() {
		rocketToggleGroup = rocketList.GetComponent<ToggleGroup>();
		payloadToggleGroup = payloadList.GetComponent<ToggleGroup>();
		destinationToggleGroup = destinationList.GetComponent<ToggleGroup>();
	}

    void OnEnable() {
		plannedMission = new WipMission();
		updatePlan();
	}

	void Start() {
		foreach (var rocket in registry.rockets.OrderBy(arg => arg.power)) {
			addItem(rocketList, rocketToggleGroup, rocket.icon, rocket.rocketName, "Power: " + rocket.power, "Cost: " + rocket.launchCost, null, (isToggled) => setSelectedRocket(rocket, isToggled));
		}
		foreach (var payload in registry.payloads.OrderBy(arg => arg.weight)) {
			addItem(payloadList, payloadToggleGroup, payload.icon, payload.payloadName, "Weight: " + payload.weight, "Value: " + payload.launchValue, "Bonus: " + payload.successBonus, (isToggled) => setSelectedPayload(payload, isToggled));
		}
		plannedMission = new WipMission();
		updatePlan();
	}

	private void addItem(RectTransform parent, ToggleGroup group, Sprite icon, string name, string one, string two, string three, UnityAction<Boolean> call) {
		ListItemUI item = Instantiate(listItem);
		item.transform.SetParent(parent, false);
		item.setData(name, icon, one, two, three);
		var toggle = item.GetComponent<Toggle>();
		toggle.onValueChanged.AddListener(call);
		toggle.group = group;
	}

	private void setSelectedRocket(RocketData data, bool isToggled) {
		if (isToggled) {
			plannedMission.rocket = data;
		} else {
			if (plannedMission.rocket == data) plannedMission.rocket = null;
		}
		updateDestinations();
		updatePlan();
	}

    private void updateDestinations() {
        foreach (Transform child in destinationList.transform) {
			GameObject.Destroy(child.gameObject);
		}
		var destinations = getAvailableDestinations(plannedMission.rocket, plannedMission.payload);
		
		foreach (var destination in destinations) {
			addItem(destinationList, destinationToggleGroup, destination.icon, destination.displayName, "Required Power: " + destination.requiredPower, "Duration: " + destination.baseMissionDuration, null, (isToggled) => setSelectedDestination(destination, isToggled));
		}
    }

    private IEnumerable<DestinationData> getAvailableDestinations(RocketData rocket, PayloadData payload) {
		if (rocket == null || payload == null) return Enumerable.Empty<DestinationData>();
        int excessRocketPower = rocket.power - payload.weight;
        return registry.destinations
				.Where(destination => payload.validDestinations.Contains(destination) && destination.requiredPower <= excessRocketPower)
				.OrderBy(arg => arg.requiredPower);
    }

    private void setSelectedPayload(PayloadData data, bool isToggled) {
		if (isToggled) {
			plannedMission.payload = data;
		} else {
			if (plannedMission.payload == data) plannedMission.payload = null;
		}
		updateDestinations();
		updatePlan();
	}

	private void setSelectedDestination(DestinationData data, bool isToggled) {
		if (isToggled) {
			plannedMission.destination = data;
		} else {
			if (plannedMission.destination == data) plannedMission.destination = null;
		}
		updatePlan();
	}

	private static MissionData createMissionDataFromPlan(WipMission plan) {
		if (plan.rocket == null || plan.payload == null || plan.destination == null) return null;
		return MissionData.create(plan.rocket, plan.payload, plan.destination);
	}

	private void updatePlan() {
		var missionData = createMissionDataFromPlan(plannedMission);
		if (missionData == null) {
			setLaunchStateInvalid();
		} else {
			setLaunchStateForMissionData(missionData);
		}
		updatePlanDetailsUI(plannedMission);
	}

	private void updatePlanDetailsUI(WipMission plan) {
        foreach (Transform child in detailsList.transform) {
			GameObject.Destroy(child.gameObject);
		}
		var plannedCost = (plan.rocket == null ? 0 : plan.rocket.launchCost)
						 - (plan.payload == null ? 0 : plan.payload.launchValue);
		var missionBonus = plan.payload == null ? 0 : plan.payload.successBonus;
		var gameState = gameStateProvider.getGameState();
		addDetail(detailsList, "Available Funds:", gameState.funds.ToString(), true);
		addDetail(detailsList, "Funds from Launch:", (-plannedCost).ToString(), plannedCost <= gameState.funds);
		if (missionBonus > 0) {
			addDetail(detailsList, "Success Bonus Funds:", missionBonus.ToString(), true);
		}
		var power = (plan.rocket == null ? 0 : plan.rocket.power) 
					- (plan.payload == null ? 0 : plan.payload.weight) 
					- (plan.destination == null ? 0 : plan.destination.requiredPower);
		addDetail(detailsList, "Available power:", power.ToString(), power >= 0);
    }

	private void addDetail(RectTransform parent, string label, string value, bool valid) {
		PlanDetailsItemUI item = Instantiate(detailsItem);
		item.transform.SetParent(parent, false);
		item.setData(label, value, valid);
	}

	private void setLaunchStateForMissionData(MissionData missionData) {
		launchButton.onClick.RemoveAllListeners();
		if (!canLaunchMission(missionData)) {
			launchButton.interactable = false;
		} else {
			launchButton.interactable = true;
			launchButton.onClick.AddListener(() => {
				turnOffToggles();
				missionControl.launchMission(missionData);
			});
		}
	}

    public bool canLaunchMission(MissionData mission) {
        return mission.isValid() && gameStateProvider.getGameState().funds >= mission.getCost();
    }

	private void setLaunchStateInvalid() {
		launchButton.interactable = false;
	}

	public void cancel() {
		turnOffToggles();
		missionControl.cancelPlanningMission();
	}

	private void turnOffToggles() {
		rocketToggleGroup.SetAllTogglesOff();
		payloadToggleGroup.SetAllTogglesOff();
		destinationToggleGroup.SetAllTogglesOff();
	}
}
