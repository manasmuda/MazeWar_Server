using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class TempScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string hg1 = "CoinCollectBot";
        string hg2 = "CoinCollectBot";
        string lg1 = "ShootingBot";
        string lg2 = "ShootingBot";

        Gadget lgadget1 = null;
        Gadget hgadget1 = null;
        Gadget lgadget2 = null;
        Gadget hgadget2 = null;

        try
        {
            // Debug.Log(Assembly.GetExecutingAssembly().GetName().Name);
            //Debug.Log(typeof(ShootingBotGadget).FullName);
            //Type type = Type.GetType(lg1 + "Gadget");
            //lgadget1 = (Gadget)Activator.CreateInstance(type);
            //Debug.Log(lgadget1.reloadTime);
            GameObject gameObject = Resources.Load<GameObject>("GadgetControllers/ShootingBotGadgetController");
            gameObject = Instantiate(gameObject, Vector3.zero, Quaternion.identity);
            lgadget1 = gameObject.GetComponent<Gadget>();
            Debug.Log(lgadget1.reloadTime);
            //hgadget1 = (Gadget)Activator.CreateInstance(Type.GetType("Assets.Scripts.GadgetControllers." + hg1 + "Gadget,Assembly-CSharp"));
            //lgadget2 = (Gadget)Activator.CreateInstance(Type.GetType("Assets.Scripts.GadgetControllers." + lg1 + "Gadget,Assembly-CSharp"));
            //hgadget2 = (Gadget)Activator.CreateInstance(Type.GetType("Assets.Scripts.GadgetControllers." + hg1 + "Gadget,Assembly-CSharp"));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogError(e.Message);
            Debug.LogError(e.InnerException);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
