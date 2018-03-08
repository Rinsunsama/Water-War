using UnityEngine;
using System.Collections;

public class JustATest : MonoBehaviour {
    GameObject sphere;
	// Use this for initialization
	void Start () {
        int x=2;
        sphere = GameObject.Find("Sphere" + x);
        print(sphere.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
