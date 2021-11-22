using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier7Script : MonoBehaviour
{
    [SerializeField] private float MAX_HP;
    [SerializeField] private float DAMAGE_TAKEN;
    [SerializeField] private float barrier7Hp;
    [SerializeField] private List<GameObject> TouchingObjects;

    [SerializeField] private GameObject barrier, destroyedBarrier;
    //private GameObject spawnedBarrier1, spawnedBarrier2;
    private GameObject spawnedBarrier1;
    //private Vector3 barrierSpawnLoc1, barrierSpawnLoc2;
    private Vector3 barrierSpawnLoc1;
    [SerializeField] private bool underAttack;

    private const float tickTimerMax = 1.0f;
    private int tick;
    private float tickTimer;

    // Start is called before the first frame update
    void Start()
    {
        //tick = 0;
        barrier7Hp = MAX_HP;
        underAttack = false;
        TouchingObjects = new List<GameObject>();

        barrierSpawnLoc1 = new Vector3(-31.08026f, 7.797038f, -0.818f);
        //barrierSpawnLoc2 = new Vector3(18.32629f, 7.797038f, -4.44f);

        spawnedBarrier1 = Instantiate(barrier, barrierSpawnLoc1, Quaternion.Euler(0, 0, 0));
        //spawnedBarrier2 = Instantiate(barrier, barrierSpawnLoc2, Quaternion.Euler(0, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (TouchingObjects.Count > 0)
        {
            for (var i = TouchingObjects.Count - 1; i > -1; i--)
            {
                if (TouchingObjects[i] == null)
                    TouchingObjects.RemoveAt(i);
            }
        }

        if (TouchingObjects.Count == 0)
        {
            underAttack = false;
        }

        tickTimer += Time.deltaTime;
        
        if (tickTimer >= tickTimerMax)
        {
            tickTimer = 0;
            tick++;
            if (underAttack == true && barrier7Hp > -1)
                barrier7Hp -= DAMAGE_TAKEN;
        }

        if (barrier7Hp == 0)
        {
            Instantiate(destroyedBarrier, barrierSpawnLoc1, Quaternion.Euler(0, 0, 0));
            //Instantiate(destroyedBarrier, barrierSpawnLoc2, Quaternion.Euler(0, 0, 0));
            Destroy(spawnedBarrier1);
            //Destroy(spawnedBarrier2);
            barrier7Hp = -1;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if(!TouchingObjects.Contains(collider.gameObject) && collider.gameObject.tag == "Zombie")
        {
            Debug.Log("Enemy in range.");
            TouchingObjects.Add(collider.gameObject);
            underAttack = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (TouchingObjects.Contains(collider.gameObject) && collider.gameObject.tag == "Zombie")
        {
            Debug.Log("Enemy exited range.");
            TouchingObjects.Remove(collider.gameObject);
            underAttack = false;
        }
    }
}
