using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : MonoBehaviour
{
    [Header("Turning Speed of the turret")]
    public float turnSpeed;
    [Header("Collider Radius")]
    public float radius;

    [Header("Target to attack")]
     public List<GameObject> targets = new List<GameObject>();
     public  GameObject currentTarget;

    [Header("Firing")]
    public bool canShoot = true;
    public float fireRate = 0.5f;
    public float bulletForce = 100f;
    public Transform  bulletSpawnPosition;
    public GameObject bulletPrefab;
  
    [SerializeField]
    RaycastHit hit;
    private Vector3 direction;

  
    // Start is called before the first frame update
    void Start()
    {
        canShoot = true;
       this.transform.GetComponent<SphereCollider>().radius = radius;
    }

    // Update is called once per frame
    void Update()
    {
        targets.RemoveAll(item => item == null);
        currentTarget = null;
            for (int i = 0; i < targets.Count; i++)
            {
                direction = (new Vector3(targets[i].transform.position.x, this.transform.position.y, targets[i].transform.position.z) - this.transform.position).normalized;
                Debug.DrawRay(this.transform.position, direction *11.4f, Color.white);
                if (Physics.Raycast(this.transform.position, direction, out hit, 11.4f)) 
                {
                    if (hit.collider != null)
                    {
                        Debug.Log(hit.collider.name + "  collider");
                        if (hit.collider.gameObject.tag=="Player")
                        {
                            currentTarget = targets[i];
                            break;
                        }
                    }
                }

            }
            if (currentTarget!= null)
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookRotation, Time.deltaTime * turnSpeed);

                if (canShoot)
                {
                    if (canShoot && Vector3.Angle(direction, this.transform.forward) < 5f)
                    {
                        Fire();
                        canShoot = false;
                        Invoke("ChangeFire", fireRate);
                    }
                }
            }

    }
   

    void ChangeFire()
    {
        canShoot = true;
    }

    void Fire()
    {
       //GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition.position, Quaternion.identity);
        //bullet.AddComponent<Rigidbody>();
        //bullet.GetComponent<Rigidbody>().AddForce(bulletSpawnPosition.forward * bulletForce, ForceMode.Impulse);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!targets.Contains(other.gameObject))
            {
                targets.Add(other.gameObject);
            }
        }
    }

    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag=="Player")
        {
            if (targets.Contains(other.gameObject))
            {
                targets.Remove(other.gameObject);
                if (currentTarget == other.gameObject)
                {
                    currentTarget = null;
                }
            }
        }
        
    }

  
}
