using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Stats))]
public class Stuff : MonoBehaviour {
    private Stats stats;

	// Use this for initialization
	void Start () {
        stats = GetComponent<Stats>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
