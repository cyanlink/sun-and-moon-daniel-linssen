using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class SwitchWorldManager : MonoSingleton<SwitchWorldManager>
{
    private bool insideDarkArea = false;
    public static bool InsideDarkArea
    {
        get=>Instance.insideDarkArea;
        set
        {
            Debug.Log("Entered Dark Area");
            Instance.insideDarkArea = value;
        }
    }

    private bool scheduleTurnOffDarkArea = false;

    private Collider2D coll;
    public void RegisterDarkArea(Collider2D collider)
    {
        coll = collider;
    }

    public static bool ScheduledTurnOffDarkArea
    {
        get => Instance.scheduleTurnOffDarkArea;
        set => Instance.scheduleTurnOffDarkArea = value;
    }

    public bool IsDarkAreaOn()
    {
        return coll.isTrigger;
    }

    public void SwitchWorld()
    {
        Debug.Log("World Switching!");
        coll.isTrigger = true;
    }
}
