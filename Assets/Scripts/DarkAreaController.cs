using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkAreaController
    : MonoBehaviour
{
    private Collider2D coll;
    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        SwitchWorldManager.Instance.RegisterDarkArea(coll);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(SwitchWorldManager.Instance.IsDarkAreaOn())
            SwitchWorldManager.InsideDarkArea = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        other.attachedRigidbody.AddForce(2 * Physics.gravity, ForceMode2D.Force);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        SwitchWorldManager.InsideDarkArea = false;
        if (SwitchWorldManager.ScheduledTurnOffDarkArea)
        {
            coll.isTrigger = false;
        }
    }


}
