using UnityEngine;
using System.Collections;

public class GateScript : MonoBehaviour {

    public bool growing = true;
    //public MainScript mainScript;
    //public Vector3 pos;
    
    void Start() {
        //mainScript = GameObject.Find("Main Camera").GetComponent<MainScript>();
    }
    
    void Update() {
    }

    /*
    void OnCollisionEnter2D(Collision2D coll) {
        //if (coll.gameObject.name
        Debug.Log (coll.gameObject.name);
        if (!locked) {
            //Destroy(gameObject);
            //mainScript.gateExists.Remove(pos);
        }
    }*/

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "Wall") {
            growing = false;
        } else { // ball hit
            Destroy(gameObject);
        }
    }
}
