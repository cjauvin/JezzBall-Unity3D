using UnityEngine;
using System.Collections;

public class GateScript : MonoBehaviour {

    public bool growing = true;
    public GameObject firstLink = null;
    public GameObject nextLink = null;
    public bool locked = false;
    
    void Start() {
    }
    
    void Update() {
    }

    void OnCollisionEnter2D(Collision2D coll) {
        GameObject curr = gameObject.GetComponent<GateScript>().firstLink;
        while (curr != null && !curr.GetComponent<GateScript>().locked) {
            Destroy(curr);
            curr = curr.GetComponent<GateScript>().nextLink;
        }
    }

}
