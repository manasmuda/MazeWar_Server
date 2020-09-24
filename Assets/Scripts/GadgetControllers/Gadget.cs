using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget : MonoBehaviour
{
    public bool lethal;
    public int damage;
    public int health;
    public bool directAction;
    public bool uiChange;
    public bool mapChange;
    public int reloadTime;
    public int useLimit;
    public GameObject gadetPrefab;
    public GameObject character;

    public bool timerMode = false;
    public float timer = 0f;

    public bool enable = true;

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if (timerMode)
        {
            timer = timer - Time.deltaTime;
            if (timer <= 0)
            {
                this.EndAction();
            }
        }
    }

    public virtual void CallMapChange()
    {

    }

    public virtual void CallUiChange()
    {

    }

    public virtual void CallAction()
    {
        if (useLimit == 0)
            return;
        useLimit--;
        enable = false;
    }

    public virtual void EndAction()
    {
        if (useLimit > 0)
        {
            timerMode = false;
            enable = true;
        }
    }

}
