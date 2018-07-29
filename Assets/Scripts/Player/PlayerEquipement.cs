using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipement : MonoBehaviour {
    public Transform leftWeapon;
    public Transform rightWeapon;

    public Stuff torso { get; set; }
    public Stuff boots { get; set; }
    public Stuff head { get; set; }

    public WeaponType weaponType { get; set; }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject GetLeftWeaponGameObject()
    {
        if(leftWeapon.childCount > 0)
            return leftWeapon.GetChild(0).gameObject;
        return null;
    }

    public GameObject GetRightWeaponGameObject()
    {
        if(rightWeapon.childCount > 0)
            return rightWeapon.GetChild(0).gameObject;
        return null;
    }

    public Weapons GetLeftWeapon()
    {
        if (leftWeapon.childCount > 0)
            return leftWeapon.GetChild(0).GetComponent<Weapons>();
        return null;
    }

    public Weapons GetRightWeapon()
    {
        if(rightWeapon.childCount > 0)
            return rightWeapon.GetChild(0).GetComponent<Weapons>();
        return null;
    }
}
