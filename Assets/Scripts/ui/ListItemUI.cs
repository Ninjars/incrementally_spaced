using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListItemUI : MonoBehaviour {
    public Text nameView;
    public Text detail1;
    public Text detail2;
    public Text detail3;
    public Image image;

    public void setData(string name, Sprite icon, string one, string two, string three) {
        nameView.text = name;
        detail1.text = one;
        detail2.text = two;
        detail3.text = three;
        image.sprite = icon;
    }
}
