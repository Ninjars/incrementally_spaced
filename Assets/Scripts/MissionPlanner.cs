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
    private GameState gameState;

    void Awake() {
		rocketToggleGroup = rocketList.GetComponent<ToggleGroup>();
		payloadToggleGroup = payloadList.GetComponent<ToggleGroup>();
		destinationToggleGroup = destinationList.GetComponent<ToggleGroup>();
	}

    void OnEnable() {
		gameState = gameStateProvider.getGameState();
		plannedMission = new WipMission();
		updateRockets();
		updatePayloads();
		updatePlan();
	}

	void updateRockets() {
		foreach (var rocket in registry.rockets.OrderBy(arg => arg.power)) {
			float launchSuccessChance = rocket.getLaunchChance(gameState.getTotalLaunchCount());
			int successValue = Convert.ToInt32(Math.Floor(launchSuccessChance * 100));
			addItem(rocketList, rocketToggleGroup, rocket.icon, rocket.rocketName, "Power: " + rocket.power, "Cost: " + rocket.launchCost, "Success %: " + successValue, (isToggled) => setSelectedRocket(rocket, isToggled));
		}
	}

	void Start() {
		plannedMission = new WipMission();
		updatePlan();
	}

	private void addItem(RectTransform parent, ToggleGroup group, Sprite icon, string name, string one, string two, string three, UnityAction<Boolean> call, bool toggled = false) {
		ListItemUI item = Instantiate(listItem);
		item.transform.SetParent(parent, false);
		item.setData(name, icon, one, two, three);
		var toggle = item.GetComponent<Toggle>();
		toggle.group = group;
		toggle.isOn = toggled;
		toggle.onValueChanged.AddListener(call);
	}

	private void setSelectedRocket(RocketData data, bool isToggled) {
		if (isToggled) {
			plannedMission.rocket = data;
		} else {
			if (plannedMission.rocket == data) plannedMission.rocket = null;
		}
		updatePayloads();
		updateDestinations();
		updatePlan();
	}

    private void updateDestinations() {
        foreach (Transform child in destinationList.transform) {
			GameObject.Destroy(child.gameObject);
		}
		var destinations = getAvailableDestinations(plannedMission.rocket, plannedMission.payload);
		if (destinations.Count() == 1) {
			plannedMission.destination = destinations.First();
		}
		if (plannedMission.destination != null && !destinations.Contains(plannedMission.destination)) {
			plannedMission.destination = null;
		}
		foreach (var destination in destinations) {
			var toggled = plannedMission.destination != null && destination == plannedMission.destination;
			addItem(destinationList, destinationToggleGroup, destination.icon, destination.displayName, "Duration: " + destination.baseMissionDuration, null, null, (isToggled) => setSelectedDestination(destination, isToggled), toggled);
		}
    }

    private IEnumerable<DestinationData> getAvailableDestinations(RocketData rocket, PayloadData payload) {
		if (rocket == null || payload == null) return Enumerable.Empty<DestinationData>();
        int excessRocketPower = rocket.power - payload.weight;
        return registry.destinations
				.Where(destination => payload.validDestinations.Contains(destination))
				.OrderBy(arg => arg.requiredPower);
    }

	private void updatePayloads() {
        foreach (Transform child in payloadList.transform) {
			GameObject.Destroy(child.gameObject);
		}
		if (plannedMission.rocket == null) return;

		var availablePayloads = getAvailablePayloads(plannedMission.rocket).Where(arg => arg.meetsConditions(gameState)).OrderBy(arg => arg.weight);
		if (availablePayloads.Count() == 1) {
			plannedMission.payload = availablePayloads.First();
		}
		if (plannedMission.payload != null && !availablePayloads.Contains(plannedMission.payload)) {
			plannedMission.payload = null;
		}
		foreach (var payload in availablePayloads) {
			var toggled = plannedMission.payload != null && payload == plannedMission.payload;
			addItem(payloadList, payloadToggleGroup, payload.icon, payload.payloadName, "Weight: " + payload.weight, "Value: " + payload.launchValue, "Bonus: " + payload.successBonus, (isToggled) => setSelectedPayload(payload, isToggled), toggled);
		}
	}

    private IEnumerable<PayloadData> getAvailablePayloads(RocketData rocket) {
		if (rocket == null) return Enumerable.Empty<PayloadData>();
        return registry.payloads
				.Where(arg => rocket.power - arg.weight - minPayloadDestinationPower(arg) >= 0);
    }

	private int minPayloadDestinationPower(PayloadData payload) {
		return payload.validDestinations.Min(arg => arg.requiredPower);
	}

    private void setSelectedPayload(PayloadData data, bool isToggled) {
		if (isToggled) {
			plannedMission.payload = data;
		} else {
			if (plannedMission.payload == data) plannedMission.payload = null;
		}
		updatePayloads();
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

	void Update() {
		updatePlanDetailsUI(plannedMission);
	}

	private void updatePlanDetailsUI(WipMission plan) {
        foreach (Transform child in detailsList.transform) {
			GameObject.Destroy(child.gameObject);
		}
		var plannedCost = (plan.rocket == null ? 0 : plan.rocket.launchCost)
						 - (plan.payload == null ? 0 : plan.payload.launchValue);
		var missionBonus = plan.payload == null ? 0 : plan.payload.successBonus;
		addDetail(detailsList, "Available Funds:", gameState.funds.ToString(), Color.white);
		addDetail(detailsList, "Funds from Launch:", (-plannedCost).ToString(), plannedCost <= gameState.funds ? Color.green : Color.white);
		if (missionBonus > 0) {
			addDetail(detailsList, "Success Bonus Funds:", missionBonus.ToString(), Color.white);
		}
		var value = missionBonus - plannedCost;
		addDetail(detailsList, "Total Mission Funds:", value.ToString(), 
				value >= 0 ? Color.green 
				: value < 0 ? Color.red
				: Color.white);
		var power = (plan.rocket == null ? 0 : plan.rocket.power) 
					- (plan.payload == null ? 0 : plan.payload.weight) 
					- (plan.destination == null ? 0 : plan.destination.requiredPower);
		addDetail(detailsList, "Available power:", power.ToString(), power >= 0 ? Color.green : Color.red);
    }

	private void addDetail(RectTransform parent, string label, string value, Color color) {
		PlanDetailsItemUI item = Instantiate(detailsItem);
		item.transform.SetParent(parent, false);
		item.setData(label, value, color);
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
        return mission.isValid() && gameState.funds >= mission.getCost();
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
