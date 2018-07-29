using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour {
    public int Strength;
    public int Agility;
    public int Defence;
    public int Magic;

    public int MaxLife;
    public int Life { get; set; }
    public int MaxMana;
    public int Mana { get; set; }

    // Use this for initialization
    void Start () {
        Life = MaxLife;
        Mana = MaxMana;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
