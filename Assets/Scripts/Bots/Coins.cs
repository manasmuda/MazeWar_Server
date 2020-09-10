
using UnityEngine;

public class Coins : MonoBehaviour
{

    public Transform coinParent;

    void Start()
    {
        transform.parent = coinParent;
        CoinCollectorBot.coinCollectorBot_instance.isChecking = true;
    }
}
