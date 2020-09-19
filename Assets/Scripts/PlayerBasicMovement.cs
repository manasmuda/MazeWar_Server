using UnityEngine;

public class PlayerBasicMovement : MonoBehaviour
{

    public float moveSpeed=5;
    public float rotateSpeed = 50;
    public CharacterSyncScript cs;

    public int tick = 0;
    public float ticktimer = 0f;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            Debug.Log("W");
            transform.Translate(new Vector3(0, 0, 1)*moveSpeed*Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, 0, -1) * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-1, 0, 0) * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(1, 0, 0) * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(new Vector3(0, 1, 0) * rotateSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(new Vector3(0, -1, 0) * rotateSpeed * Time.deltaTime);
        }
        ticktimer = ticktimer + Time.deltaTime;
        if (ticktimer >= 0.2f)
        {
            ticktimer = 0f;
            cs.AddNewMove(new Vector3(transform.position.x,9f,transform.position.z),transform.rotation.eulerAngles);
        }
    }
}
