using UnityEngine;
using System.Collections;

public class StartController : MonoBehaviour {

    public GameObject acher1Prefab;
    public GameObject acher2Prefab;
    public GameObject sword1Prefab;
    public GameObject sword2Prefab;
    public GameObject demonCar1Prefab;
    public GameObject demonCar2Prefab;
    public GameObject base1Prefab;
    public GameObject base2Prefab;
    // Use this for initialization
    void Start()
    {

        GameObject Acher1 = GameObject.Instantiate(acher1Prefab, acher1Prefab.transform.position, acher1Prefab.transform.rotation) as GameObject;
        GameObject base1 = GameObject.Instantiate(base1Prefab, base1Prefab.transform.position, base1Prefab.transform.rotation) as GameObject;
        GameObject sword1 = GameObject.Instantiate(sword1Prefab, sword1Prefab.transform.position, sword1Prefab.transform.rotation) as GameObject;
        GameObject demonCar1 = GameObject.Instantiate(demonCar1Prefab, demonCar1Prefab.transform.position, demonCar1Prefab.transform.rotation) as GameObject;

        GameObject Acher2 = GameObject.Instantiate(acher2Prefab, acher2Prefab.transform.position, acher2Prefab.transform.rotation) as GameObject;
        GameObject base2 = GameObject.Instantiate(base2Prefab, base2Prefab.transform.position, base2Prefab.transform.rotation) as GameObject;
        GameObject sword2 = GameObject.Instantiate(sword2Prefab, sword2Prefab.transform.position, sword2Prefab.transform.rotation) as GameObject;
        GameObject demonCar2 = GameObject.Instantiate(demonCar2Prefab, demonCar2Prefab.transform.position, demonCar2Prefab.transform.rotation) as GameObject;

        //GridManager.instance.findObstacleAndCalculateWaterPower();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
