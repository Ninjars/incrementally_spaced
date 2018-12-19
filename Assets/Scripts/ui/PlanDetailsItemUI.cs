using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanDetailsItemUI : MonoBehaviour {
    public Text labelView;
    public Text valueView;

    public void setData(string label, string value, Color color) {
        labelView.text = label;
        valueView.text = value;
        valueView.color = color;
    }
}
