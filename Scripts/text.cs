using UnityEngine;
using System.Collections;

public class text : MonoBehaviour {

    public GameObject obj1, obj2;
    int angle = 180;
    int direction = 0;
	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
    void Update(){
        float y =obj1.transform.eulerAngles.y;
        if (obj1.transform.eulerAngles.y > 360 - (angle/2)-50) y = -(360 - y);
        if (direction == 0)
        {
            obj1.transform.RotateAround(obj2.transform.position, new Vector3(0, 1, 0), 200f * Time.deltaTime);
            print(y);
        }
        else
        {
            obj1.transform.RotateAround(obj2.transform.position, new Vector3(0, 1, 0), -200f * Time.deltaTime);
            print(y);
        }
        if (y >= angle / 2)
        {
            direction = 1;
        }
        if (y <=-angle/2) 
        {
            direction = 0;
        }

    }
    public void round(GameObject obj1, GameObject obj2)
    {
        obj1.transform.RotateAround(obj2.transform.position, new Vector3(1, 0, 1), 10f * Time.deltaTime);
    }
}
