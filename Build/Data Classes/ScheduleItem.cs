using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine;
using System;

[Serializable]
public class ScheduleItem
{
    [Title("$Combined")]
    [LabelWidth(45f)]
    [ValidateInput("HasName", "Give your Schedule Item a name befitting it's use.")]
    public string name = "Default";
    private string title = "Schedule Item";

    public string Combined { get { return this.title + " | " + this.name; } }

    [LabelText("Time: ")]
    [HorizontalGroup("Time", LabelWidth = 45f, Width = 20f)]
    [ValueDropdown("hNum")]
    public string hours = "09";
    [LabelText(":")]
    [LabelWidth(10)]
    [HorizontalGroup("Time", Width = 20f)]
    [ValueDropdown("mNum")]
    public string minutes = "30";
    [HideLabel]
    [HorizontalGroup("Time", Width = 20f)]
    [ValueDropdown("aNum")]
    [GUIColor("GetColor")]
    public string ampm = "AM";

    public UnityEvent OnTimeReached = new UnityEvent();

    [HorizontalGroup("Debugs", LabelWidth = 80f, Width = 200f)]
    [ReadOnly]
    public int internalGameTime;
    [HorizontalGroup("Debugs", LabelWidth = 150f, Width = 20f)]
    [ReadOnly]
    public bool scheduledTimeHasPassed; //TODO refactor out of ScriptableObject

    private bool HasName(string value)
    {
        if (value == "Default") { return false; }
        else { return true; }
    }
    private Color GetColor()
    {
        if (ampm == "PM") return new Color(0.8f, 0.5f, 1f);
        if (ampm == "AM") return new Color(1f, 0.8f, 0.5f);
        return Color.white;
    }

    private static string[] hNum = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
    private static string[] mNum = { "00", "05", "10", "15", "20", "25", "30", "35", "40", "45", "50", "55" };
    private static string[] aNum = { "AM", "PM" };
}
