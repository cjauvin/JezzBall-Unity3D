using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {
	
	void Start () {
		rigidbody2D.AddForce(Random.insideUnitCircle * Random.Range (100, 500));
	}

	void Update () {
	}

}
