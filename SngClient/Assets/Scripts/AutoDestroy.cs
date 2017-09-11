using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 1); // aouto delete after 1 sec.
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
