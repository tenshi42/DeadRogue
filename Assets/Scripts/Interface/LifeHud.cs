using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeHud : MonoBehaviour {
    private Transform life;

	// Use this for initialization
	void Start () {
        life = transform.Find("Life");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetLife(float percent)
    {
        Vector3 scale = life.localScale;
        scale.x = 0.9f * percent;
        life.localScale = scale;
    }
}
