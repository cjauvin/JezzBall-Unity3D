using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainScript : MonoBehaviour {

    public GameObject ball;
    public GameObject gate;
    public GameObject back;
    public Dictionary<Vector3, bool> gateExists;
    public int N;

    void Start() {
        back = GameObject.Find("Back");
        N = (int)back.transform.localScale.x / 2;
        for(int i = 0; i < 10; i++) {
            Vector2 p = new Vector2(Random.Range(-(N-5), N-5), 
                                    Random.Range(-(N-5), N-5));
            GameObject b = (GameObject)Instantiate(ball, p, Quaternion.identity);
            Physics2D.IgnoreCollision(back.gameObject.collider2D, 
                                      b.gameObject.collider2D);
        }
        gateExists = new Dictionary<Vector3, bool>();
    }
    
    void Update() {
        int btn = 0;
        if (Input.GetMouseButtonDown(0)) {
            btn = 1;
        } else if (Input.GetMouseButtonDown(1)) {
            btn = 2;
        }
        if (btn > 0) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), 
                                                 Vector2.zero);    
            if (hit.collider != null) {
                if (btn == 1) {
                    StartCoroutine(LaunchGate(hit.centroid, Vector2.right, Color.blue));
                    StartCoroutine(LaunchGate(hit.centroid, -Vector2.right, Color.red));
                } else {
                    StartCoroutine(LaunchGate(hit.centroid, Vector2.up, Color.blue));
                    StartCoroutine(LaunchGate(hit.centroid, -Vector2.up, Color.red));
                }
            }
        }
    }

    IEnumerator LaunchGate(Vector3 pos, Vector2 dir, Color color) {
        Vector2 p = new Vector2(Mathf.Round(pos.x),
                                Mathf.Round(pos.y));
        if (dir == Vector2.right) {
            p -= Vector2.right;
        } else if (dir == Vector2.up) {
            p -= Vector2.up;
        }
        List<GameObject> gateLinks = new List<GameObject>();
        while (true) {
            p += dir;
            Collider2D[] colls = Physics2D.OverlapPointAll(p);
            if (colls.Length == 0) {
                // outside the back plane
                break;
            } else if (colls.Length > 1) {
                // back plane + wall or ball 
                foreach (Collider2D coll in colls) {
                    if (coll.gameObject.name == "Ball(Clone)") {
                        foreach (GameObject gl in gateLinks) {
                            Destroy(gl);
                        }
                    }    
                }
                foreach (GameObject gl in gateLinks) {
                    if (gl) {
                        gl.GetComponent<GateScript>().locked = true;
                    }
                }
                break;
            }
            GameObject g = (GameObject)Instantiate(gate, p, Quaternion.identity);
            g.renderer.material.color = color;
            gateLinks.Add(g);
            g.GetComponent<GateScript>().firstLink = gateLinks[0];
            if (g != gateLinks[0]) {
                if (gateLinks[gateLinks.Count-2] != null) {
                    gateLinks[gateLinks.Count-2].GetComponent<GateScript>().nextLink = g;
                } else {
                    Destroy(g);
                    break;
                }
            }
            yield return new WaitForSeconds(0.001f);
        }
        gateLinks.Clear();
    }

}
