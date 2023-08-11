using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using TMPro;

public class AIGameOverMenu : GameOverMenu {
    protected override void SetHeightText() {
        var logId = "SetHeightText";
        var currentHeight = gameManager.TowerHeightManager.CurrentHeight;
        logd(logId, "Setting HeightAmount to "+currentHeight);
        heightAmountText.text = currentHeight.ToString();
    }
}