using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListItemUI : MonoBehaviour {
    public TMP_Text nameView;
    public TMP_Text detail1;
    public TMP_Text detail2;
    public TMP_Text detail3;
    public Image image;

    public void setData(string name, string one, string two, string three) {
        nameView.text = name;
        detail1.text = one;
        detail2.text = two;
        detail3.text = three;
    }
}
