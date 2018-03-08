using UnityEngine;
using System.Collections;

public class selectEffectController : MonoBehaviour {

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (GameController.selected_hero != null)
        {
            Vector3 temp = GameController.selected_hero.transform.position;
            this.gameObject.transform.position = new Vector3(temp.x, 0.6f, temp.z);
        }
        else
        {
            this.gameObject.transform.position = new Vector3(0, -1.0f, 0);
        }
    }
}
