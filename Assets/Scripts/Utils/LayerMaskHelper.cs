using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerMaskHelper : MonoBehaviour {
    public LayerMask player;
    public LayerMask wall;
    public LayerMask allButPlayer;
    public LayerMask all;

    public static LayerMask Player;
    public static LayerMask Wall;
    public static LayerMask AllButPlayer;
    public static LayerMask All;

    // Use this for initialization
    void Start () {
        Player = player;
        Wall = wall;
        AllButPlayer = allButPlayer;
        All = all;
	}
}
