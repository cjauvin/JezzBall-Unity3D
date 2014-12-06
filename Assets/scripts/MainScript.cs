using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainScript : MonoBehaviour {

    public GameObject ball;
    public GameObject gate;
    public GameObject back;
    public Dictionary<Vector3, bool> gateExists;
    public int N;
    public bool[,] grid;
    private List<GameObject> balls = new List<GameObject>();

    void Start() {
        back = GameObject.Find("Back");
        N = (int)back.transform.localScale.x;
        grid = new bool[N, N];
        for (int i = 0; i < 10; i++) {
            Vector2 p = new Vector2(Random.Range(1, N-1), 
                                    Random.Range(1, N-1));
            GameObject b = (GameObject)Instantiate(ball, p, Quaternion.identity);
            balls.Add(b);
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
                    /*
                    bool b = false;
                    for (int i = 0; i < N; i++) {
                        for (int j = 0; j < N; j++) {
                            Collider2D[] colls = Physics2D.OverlapPointAll(new Vector2(i, j));
                            foreach (Collider2D coll in colls) {
                                if (coll.gameObject.name == "Ball(Clone)") {
                                    b = true;
                                }
                            }
                        }
                    }
                    Debug.Log(b);
                    */
                                    } else {
                    StartCoroutine(LaunchGate(hit.centroid, Vector2.up, Color.blue));
                    StartCoroutine(LaunchGate(hit.centroid, -Vector2.up, Color.red));
                }
            }
        }
    }

    bool getGridAt(Vector3 v) {
        return grid[(int)v.x, (int)v.y];
    }

    void setGridAt(Vector3 v, bool b) {
        grid[(int)v.x, (int)v.y] = b;
    }

    bool isInBounds(Vector3 v) {
        return v.x >= 0 && v.x < N && v.y >= 0 && v.y < N;
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

            if (!isInBounds(p) || getGridAt(p)) {
                foreach (GameObject gl in gateLinks) {
                    if (gl) {
                        gl.GetComponent<GateScript>().locked = true;
                        setGridAt(gl.transform.position, true);
                    }
                }
                findBallFreePatch();
                break;
            }

            /*
            Collider2D[] colls = Physics2D.OverlapPointAll(p);
            bool ballDetected = false;
            foreach (Collider2D coll in colls) {
                if (coll.gameObject.name == "Ball(Clone)") {
                    foreach (GameObject gl in gateLinks) {
                        Destroy(gl);
                    }
                    ballDetected = true;
                }
            }
            if (ballDetected) {
                break;
            }*/

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

    void findBallFreePatch() {
        bool[,] visited = new bool[N, N];
        int nRegions = 0;
        for (int i = 0; i < N; i++) {
            for (int j = 0; j < N; j++) {
                if (!grid[i, j] && !visited[i, j]) {
                    Dictionary<Vector2, bool> region = new Dictionary<Vector2, bool>();
                    floodFill(i, j, visited, region);
                    nRegions += 1;
                    bool ballDetected = false;
                    foreach (GameObject b in balls) {
                        Vector3 p = b.transform.position;
                        Vector2 v = new Vector2(Mathf.Round(p.x),
                                                Mathf.Round(p.y));
                        if (region.ContainsKey(v)) {
                            ballDetected = true;
                            break;
                        }
                    }
                    if (!ballDetected) {
                        foreach (Vector2 v in region.Keys) {
                            GameObject g = (GameObject)Instantiate(gate, v, Quaternion.identity);
                            g.renderer.material.color = Color.green;
                            grid[(int)v.x, (int)v.y] = true;
                        }
                    }
                }
            }
        }
    }

    void floodFill(int i, int j, bool[,] visited, Dictionary<Vector2, bool> region) {
        visited[i, j] = true;      
        region.Add(new Vector2(i, j), true);
        if (i > 0 && !visited[i-1, j] && !grid[i-1, j]) {
            floodFill(i-1, j, visited, region);
        }
        if (i < (N-1) && !visited[i+1, j] && !grid[i+1, j]) {
            floodFill(i+1, j, visited, region);
        }
        if (j > 0 && !visited[i, j-1] && !grid[i, j-1]) {
            floodFill(i, j-1, visited, region);
        }
        if (j < (N-1) && !visited[i, j+1] && !grid[i, j+1]) {
            floodFill(i, j+1, visited, region);
        }
    }

    /*
    void floodFill(int i, int j, bool[,] visited, bool[] ballDetected, int[] dims) {
        visited[i, j] = true;
        if (!ballDetected[0]) {
            Collider2D[] colls = Physics2D.OverlapPointAll(new Vector2(i, j));
            foreach (Collider2D coll in colls) {
                if (coll.gameObject.name == "Ball(Clone)") {
                    ballDetected[0] = true;
                }
            }
        }
        dims[0] = Mathf.Min(i, dims[0]);
        dims[1] = Mathf.Min(j, dims[1]);
        dims[2] = Mathf.Max(i, dims[2]);
        dims[3] = Mathf.Max(j, dims[3]);
        if (i > 0 && !visited[i-1, j] && !grid[i-1, j]) {
            floodFill(i-1, j, visited, ballDetected, dims);
        }
        if (i < (N-1) && !visited[i+1, j] && !grid[i+1, j]) {
            floodFill(i+1, j, visited, ballDetected, dims);
        }
        if (j > 0 && !visited[i, j-1] && !grid[i, j-1]) {
            floodFill(i, j-1, visited, ballDetected, dims);
        }
        if (j < (N-1) && !visited[i, j+1] && !grid[i, j+1]) {
            floodFill(i, j+1, visited, ballDetected, dims);
        }
    } */

}
