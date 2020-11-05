using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoinCollectBotGadget : Gadget
{
    // Start is called before the first frame update
    public Dictionary<string,GameObject> activeObjects = new Dictionary<string,GameObject>();
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
        timerGadget = false;
        useTime = 0;
        gadetPrefab = Resources.Load<GameObject>("Gadgets/Bots/CoinCollectorBot");
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(UpdatePos());
        // character = transform.parent.gameObject; Load Character object
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void CallAction(SimpleMessage message=null)
    {
        base.CallAction();
        if (team == "blue")
            character = GameData.blueTeamData.clientsData[playerId].character;
        else
            character = GameData.redTeamData.clientsData[playerId].character;
        string id = "ccb" + RandomStringGenerator.GetRandomString(5);
        message.stringData = id;
        GameObject gadgetObject = Instantiate(gadetPrefab, character.transform.position, Quaternion.identity);
        BotInfo bi = gadgetObject.GetComponent<BotInfo>();
        bi.team = team;
        bi.botId = id;
        bi.botHealth = 80;
        bi.playerID = playerId;
        activeObjects.Add(id,gadgetObject);
        AutoGadgetState autoGadgetState = new AutoGadgetState();
        autoGadgetState.health = 80;
        autoGadgetState.id = id;
        autoGadgetState.name = gname;
        autoGadgetState.team = team;
        autoGadgetState.position = new float[3] { gadgetObject.transform.position.x, gadgetObject.transform.position.y, gadgetObject.transform.position.z };
        autoGadgetState.angle = new float[3] { gadgetObject.transform.rotation.eulerAngles.x, gadgetObject.transform.rotation.eulerAngles.y, gadgetObject.transform.rotation.eulerAngles.z };
        GameHistory.AddAutoGadgetState(autoGadgetState, team, playerId);
    }

    public override void EndAction()
    {
        base.EndAction();
    }

    IEnumerator UpdatePos()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            ClientState clientState = null;
            if (team == "blue") 
                clientState=GameHistory.recentState.blueTeamState[playerId];
            else
                clientState = GameHistory.recentState.redTeamState[playerId];
            for (int i = 0; i < activeObjects.Count; i++)
            {
                string tempkey = activeObjects.ElementAt(i).Key;
                GameObject tempObject = activeObjects.ElementAt(i).Value;
                BotInfo tempbi = tempObject.GetComponent<BotInfo>();
                AutoGadgetState tempState = clientState.autoGadgetStates[tempkey];
                tempState.angle = new float[3] { tempObject.transform.rotation.eulerAngles.x, tempObject.transform.rotation.eulerAngles.y, tempObject.transform.rotation.eulerAngles.z };
                tempState.position = new float[3] { tempObject.transform.position.x, tempObject.transform.position.y, tempObject.transform.position.z };
                tempState.health = tempbi.botHealth;
                tempState.id = tempbi.botId;
                tempState.name = gname;
                tempState.team = team;
                tempState.tick = GameHistory.recentState.tick;
                clientState.autoGadgetStates[tempkey] = tempState;
            }
        }
    }
}
