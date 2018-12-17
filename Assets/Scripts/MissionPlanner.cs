using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionPlanner : MonoBehaviour {

	public RectTransform rocketList;
	public RectTransform payloadList;
	public RectTransform destinationList;
	public Registry registry;
	public ListItemUI listItem;

	void Start() {
		foreach (var rocket in registry.rockets) {
			addItem(rocketList, rocket.name, "Power: " + rocket.power, "Cost: " + rocket.launchCost, null);
		}
		foreach (var payload in registry.payloads) {
			addItem(payloadList, payload.name, "Weight: " + payload.weight, "Value: " + payload.launchValue, "Bonus: " + payload.successBonus);
		}
		foreach (var destination in registry.destinations) {
			addItem(destinationList, destination.name, "Power: " + destination.requiredPower, "Duration: " + destination.baseMissionDuration, null);
		}
	}

	private void addItem(RectTransform parent, string name, string one, string two, string three) {
		ListItemUI item = Instantiate(listItem);
		item.transform.SetParent(parent, false);
		item.setData(name, one, two, three);
	}
}
