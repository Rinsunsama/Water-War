using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

    public GameObject acher1Prefab;
    public GameObject acher2Prefab;
    public GameObject sword1Prefab;
    public GameObject sword2Prefab;
    public GameObject demonCar1Prefab;
    public GameObject demonCar2Prefab;
    public GameObject base1Prefab;
    public GameObject base2Prefab;
    public GameObject maincCamera;
	// Use this for initialization
	void Start () {
        int player =int.Parse(Network.player+"");
        
        if(player ==0)
        {
            int group1 = player;
            GameObject Acher1 = Network.Instantiate(acher1Prefab, acher1Prefab.transform.position, acher1Prefab.transform.rotation, group1) as GameObject;
            GameObject base1 = Network.Instantiate(base1Prefab, base1Prefab.transform.position, base1Prefab.transform.rotation, group1) as GameObject;
            GameObject sword1 = Network.Instantiate(sword1Prefab, sword1Prefab.transform.position, sword1Prefab.transform.rotation, group1) as GameObject;
            GameObject demonCar1 = Network.Instantiate(demonCar1Prefab, demonCar1Prefab.transform.position, demonCar1Prefab.transform.rotation, group1) as GameObject;
        }
        else if(player==1)
        {
            maincCamera.transform.position = new Vector3(4, 10, 21);
            maincCamera.transform.localEulerAngles = new Vector3(maincCamera.transform.localEulerAngles.x, 180, maincCamera.transform.localEulerAngles.z);
            int group2 = player;
            GameObject Acher2 = Network.Instantiate(acher2Prefab, acher2Prefab.transform.position, acher2Prefab.transform.rotation, group2) as GameObject;
            GameObject base2 = Network.Instantiate(base2Prefab, base2Prefab.transform.position, base2Prefab.transform.rotation, group2) as GameObject;
            GameObject sword2 = Network.Instantiate(sword2Prefab, sword2Prefab.transform.position, sword2Prefab.transform.rotation, group2) as GameObject;
            GameObject demonCar2 = Network.Instantiate(demonCar2Prefab, demonCar2Prefab.transform.position, demonCar2Prefab.transform.rotation, group2) as GameObject;
        }
        //GetComponent<NetworkView>().RPC("findObstacleAndCalculateWaterPower", RPCMode.All);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
