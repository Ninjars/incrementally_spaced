using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanDetailsItemUI : MonoBehaviour {
    public TMP_Text labelView;
    public TMP_Text valueView;

    public void setData(string label, string value, bool valid) {
        labelView.text = label;
        valueView.text = value;
        valueView.color = valid ? Color.white : Color.red;
    }
}
