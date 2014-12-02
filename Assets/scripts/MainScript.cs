using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainScript : MonoBehaviour {

    public GameObject ball;
    public GameObject gate;
    public GameObject back;
    //public GameObject pointer;
    public Dictionary<Vector3, bool> gateExists;
    public int N;

    void Start() {
        back = GameObject.Find("Back");
        N = (int)back.transform.localScale.x / 2;
        //pointer = GameObject.Find("Pointer");
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
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);    
            if (hit.collider != null) {
                Vector3 dir = (btn == 1) ? new Vector3(1, 0, 0) : new Vector3(0, 1, 0);
                /*
                GameObject g1 = (GameObject)Instantiate(gate, hit.centroid, Quaternion.identity);
                g1.renderer.material.color = Color.blue;
                StartCoroutine(LaunchGate(g1, dir, -1)); */
                GameObject g2 = (GameObject)Instantiate(gate, hit.centroid, Quaternion.identity);
                g2.renderer.material.color = Color.red;
                StartCoroutine(LaunchGate(g2, dir, 1));
            }
        }
        /*
        // is on either left/right vertical walls
        Vector3 pos = pointer.transform.position;
        bool isVertical = (Mathf.Abs(pos.x) == N);
        // is on either top/bottom horizontal walls
        bool isHorizontal = (Mathf.Abs(pos.y) == N);
        if (Input.GetKey(KeyCode.LeftArrow) && pos.x > -N && isHorizontal) {
            pointer.transform.Translate(-1, 0, 0);
        } else if (Input.GetKey(KeyCode.RightArrow) && pos.x < N && isHorizontal) {
               pointer.transform.Translate(1, 0, 0);
        } else if (Input.GetKey(KeyCode.UpArrow) && pos.y < N && isVertical) {
            pointer.transform.Translate(0, 1, 0);
        } else if (Input.GetKey(KeyCode.DownArrow) && pos.y > -N && isVertical) {
            pointer.transform.Translate(0, -1, 0);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            bool allowed = (((isVertical && Mathf.Abs(pos.y) != N) || 
                              (isHorizontal && Mathf.Abs(pos.x) != N)) &&
                            !gateExists.ContainsKey(pos));
            if (allowed) {
                GameObject g = (GameObject)Instantiate(gate, pos, Quaternion.identity);
                Vector3 dir = isVertical ? new Vector3(1, 0, 0) : new Vector3(0, 1, 0);
                int invert = ((isVertical && pos.x == N) || (isHorizontal && pos.y == N)) ? -1 : 1;
                gateExists.Add(pos, true);
                g.GetComponent<GateScript>().pos = pos;
                StartCoroutine(LaunchGate(g, dir, invert));
            }
        }*/
    }
    
    IEnumerator LaunchGate(GameObject g, Vector3 dir, int invert) {
        bool is_vertical = (dir.x == 1);
        float p0 = is_vertical ? g.transform.position.x : g.transform.position.y;
        Vector3 p = g.transform.position;
        p0 *= invert;
        //while (g && p0 + (is_vertical ? g.transform.localScale.x : g.transform.localScale.y) < N) {
        while (g && g.GetComponent<GateScript>().growing) {
            Vector3 v = dir * invert;
            RaycastHit2D hit = Physics2D.Raycast(p, Vector2.right);
            if (hit && hit.collider.gameObject.name == "Wall") {
                Debug.Log (hit.point.x);
            }
            g.transform.localScale += dir;
            g.transform.Translate((dir * invert) / 2);
            yield return new WaitForSeconds(.01f);
        }
    }
    
}
