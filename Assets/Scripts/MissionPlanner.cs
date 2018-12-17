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

	public Registry registry;
	public ListItemUI listItem;
	public RectTransform rocketList;
	public RectTransform payloadList;
	public RectTransform destinationList;
	public Button launchButton;
	private WipMission plannedMission;

	void OnEnable() {
		plannedMission = new WipMission();
		updatePlan();
	}

	void Start() {
		var rocketToggleGroup = rocketList.GetComponent<ToggleGroup>();
		foreach (var rocket in registry.rockets) {
			addItem(rocketList, rocketToggleGroup, rocket.name, "Power: " + rocket.power, "Cost: " + rocket.launchCost, null, (isToggled) => setSelectedRocket(rocket, isToggled));
		}
		var payloadToggleGroup = payloadList.GetComponent<ToggleGroup>();
		foreach (var payload in registry.payloads) {
			addItem(payloadList, payloadToggleGroup, payload.name, "Weight: " + payload.weight, "Value: " + payload.launchValue, "Bonus: " + payload.successBonus, (isToggled) => setSelectedPayload(payload, isToggled));
		}
		plannedMission = new WipMission();
		updatePlan();
	}

	private void addItem(RectTransform parent, ToggleGroup group, string name, string one, string two, string three, UnityAction<Boolean> call) {
		ListItemUI item = Instantiate(listItem);
		item.transform.SetParent(parent, false);
		item.setData(name, one, two, three);
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
		
		var destinationToggleGroup = destinationList.GetComponent<ToggleGroup>();
		foreach (var destination in destinations) {
			addItem(destinationList, destinationToggleGroup, destination.name, "Power: " + destination.requiredPower, "Duration: " + destination.baseMissionDuration, null, (isToggled) => setSelectedDestination(destination, isToggled));
		}
    }

    private IEnumerable<DestinationData> getAvailableDestinations(RocketData rocket, PayloadData payload) {
		if (rocket == null || payload == null) return Enumerable.Empty<DestinationData>();
        int excessRocketPower = rocket.power - payload.weight;
        return registry.destinations.Where(destination => destination.requiredPower <= excessRocketPower);
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
	}

	private void setLaunchStateForMissionData(MissionData missionData) {
		if (!missionData.isValid()) {
			launchButton.interactable = false;
		} else {
			launchButton.interactable = true;
		}
	}

	private void setLaunchStateInvalid() {
		launchButton.interactable = false;
	}
}
