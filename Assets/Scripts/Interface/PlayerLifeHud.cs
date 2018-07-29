using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeHud : MonoBehaviour {
    public Transform sprite;
    public Text text;

    private static PlayerLifeHud instance;

	// Use this for initialization
	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void SetLife(int max, int act)
    {
        float ratio = act / (float)max;
        Vector3 scale = instance.sprite.localScale;
        scale.x = ratio;
        instance.sprite.localScale = scale;
        instance.text.text = act + "/" + max;
    }
}
