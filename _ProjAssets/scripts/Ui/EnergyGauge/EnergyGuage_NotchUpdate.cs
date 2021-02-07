using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(EnergyGaugeUI))]
///auto set up Zelda style hearts based on how many hp chip damage per notch containers
public class EnergyGuage_NotchUpdate : MonoBehaviour
{
    EnergyGaugeUI energyGaugeUI;
    public int perHpNotch = 4;

    [Header("output")]
    public int requiredNotches;
    private void Start()
    {
        energyGaugeUI = GetComponent<EnergyGaugeUI>();
    }

    void Update()
    {
        if (perHpNotch <= 0) perHpNotch = 1;

        requiredNotches = Mathf.FloorToInt(  (float)energyGaugeUI.max/ (float)perHpNotch);

        if (requiredNotches > energyGaugeUI.barSets.Count) energyGaugeUI.AddNotch();
        else if (requiredNotches < energyGaugeUI.barSets.Count) energyGaugeUI.RemoveNotch();
    }
}
