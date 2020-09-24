using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollectBotGadget : Gadget
{
    // Start is called before the first frame update
    public List<GameObject> activeObjects = new List<GameObject>();
    // Start is called before the first frame update
    protected void Awake()
    {
        damage = 0;
        health = 100;
        lethal = false;
        directAction = true;
        uiChange = false;
        mapChange = true;
        reloadTime = 60;
        useLimit = 5;
        //load prefab
    }

    protected override void Start()
    {
        base.Start();
        // character = transform.parent.gameObject; Load Character object
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void CallAction()
    {
        base.CallAction();
        GameObject gadgetObject = Instantiate(gadetPrefab, character.transform.position, Quaternion.identity);
        activeObjects.Add(gadgetObject);

    }

    public override void EndAction()
    {
        base.EndAction();
    }
}
