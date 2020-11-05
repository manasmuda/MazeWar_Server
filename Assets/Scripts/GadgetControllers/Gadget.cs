using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget : MonoBehaviour
{
    public string playerId;
    public string team;

    public string gname;
    public string gsname;
    public bool lethal;
    public int damage;
    public int health;
    public bool directAction;
    public bool uiChange;
    public bool mapChange;
    public int reloadTime;
    public int useLimit;
    public bool timerGadget;
    public int useTime;
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

    }

    public virtual void CallMapChange()
    {

    }

    public virtual void CallUiChange()
    {

    }

    public virtual bool CanCall()
    {
        return useLimit > 0 && enable;
    }

    public virtual void CallAction(SimpleMessage message=null)
    {
        if (useLimit == 0)
            return;
        useLimit--;
        enable = false;
        StartCoroutine(Reloading());
    }

    public virtual void EndAction()
    {
        if (useLimit > 0)
        {
            timerMode = false;
            enable = true;
        }
    }

    IEnumerator Reloading()
    {
        timer = reloadTime;
        while (timer > 0)
        {
            timer = timer - 0.2f;
            yield return new WaitForSeconds(0.2f);
        }
        this.EndAction();
    }

}
