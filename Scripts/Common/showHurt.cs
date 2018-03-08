using UnityEngine;
using System.Collections;

public class showHurt : MonoBehaviour {
    HUDText hudText;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	}
    [RPC]
    public void showHurtNum(int hurtNum)
    {
        hudText = this.GetComponent<HUDText>();
        hudText.Add(-hurtNum, Color.red, 1);
    }
}
