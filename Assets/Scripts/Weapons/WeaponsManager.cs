using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsManager : MonoBehaviour {
    public List<GameObject> Weapons;
    public Dictionary<string, GameObject> weapons { get; set; }

    public static WeaponsManager _instance;

	// Use this for initialization
	void Start () {
        _instance = this;

        weapons = new Dictionary<string, GameObject>();
		foreach(GameObject g in Weapons)
        {
            weapons.Add(g.name, g);
        }
	}
}
