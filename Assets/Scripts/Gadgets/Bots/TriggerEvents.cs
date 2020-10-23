using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class TriggerEvents : MonoBehaviour
{
    public string checkTag;
    public List<GameObject> playerList;
    public GameObject curPlayer=null;
    private EnemyChasingBot enemyChasingBotScript;

    public CurrentState enterState;
    public CurrentState exitState;

    private void Start()
    {
        playerList = new List<GameObject>();
        enemyChasingBotScript= transform.parent.GetComponent<EnemyChasingBot>();
    }

    private void Update()
    {
        if (curPlayer == null)
        {
            if (playerList.Count > 0)
            {
                curPlayer = playerList[0];
                enemyChasingBotScript.playerPos = playerList[0].transform;
                enemyChasingBotScript.UpdateState(enterState);
            }
            else
            {

                    if (enemyChasingBotScript.CheckState(exitState))
                    {
                        if (exitState != CurrentState.chasing)
                        {
                            curPlayer = null;
                            enemyChasingBotScript.playerPos = null;
                        }
                        enemyChasingBotScript.UpdateState(exitState);
                    }
            }
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag(checkTag))
        {
            if (!playerList.Contains(other.gameObject))
            {
                playerList.Add(other.gameObject);
                if (playerList.Count == 1)
                {
                    curPlayer = other.gameObject;
                    enemyChasingBotScript.playerPos = other.transform;
                    enemyChasingBotScript.UpdateState(enterState);
                }
            }
            else
            {
                if (curPlayer == null && playerList.Count>0)
                {
                    curPlayer = playerList[0];
                    enemyChasingBotScript.playerPos = playerList[0].transform;
                    enemyChasingBotScript.UpdateState(enterState);
                }
            }
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(checkTag))
        {
            if (playerList.Contains(other.gameObject))
            {
                playerList.Remove(other.gameObject);
                if (other.gameObject == curPlayer)
                {
                    if (playerList.Count > 0)
                    {
                        curPlayer = playerList[0];
                        enemyChasingBotScript.playerPos = playerList[0].transform;
                        enemyChasingBotScript.UpdateState(enterState);
                    }
                    else
                    {
                        enemyChasingBotScript.UpdateState(exitState);
                        if (exitState !=CurrentState.chasing)
                        {
                            curPlayer = null;
                            enemyChasingBotScript.playerPos = null;
                        }
                    }
                }
            }
        }
    }
}
