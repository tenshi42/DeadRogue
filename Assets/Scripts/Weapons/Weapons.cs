using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    None,
    Dagger,
    Sword,
    Bow
}

[RequireComponent(typeof(MakeDamage))]
public abstract class Weapons : MonoBehaviour {
    protected MakeDamage makeDamage;
    protected BoxCollider2D _collider;

    public int smallCooldown;
    protected int smallBaseCooldown;
    public int bigCooldown;
    protected int bigBaseCooldown;

    protected bool smallFired = false;
    protected bool bigFired = false;

    public int smallDamage;
    public int bigDamage;

    public WeaponType type;

    // Use this for initialization
    void Start () {
        makeDamage = GetComponent<MakeDamage>();
        _collider = GetComponent<BoxCollider2D>();
        _collider.enabled = false;
        smallBaseCooldown = smallCooldown;
        bigBaseCooldown = bigCooldown;
        smallCooldown = 0;
        bigCooldown = 0;
	}

    public bool SmallFire(Animator animator)
    {
        if (smallCooldown <= 0 && bigCooldown <= 0)
        {
            // DO THE FIRE
            smallFired = true;
            makeDamage.damage = smallDamage;
            return SmallFireAction(animator);
        }
        return false;
    }

    public bool BigFire(Animator animator)
    {
        if (bigCooldown <= 0 && smallCooldown <= 0)
        {
            // DO THE FIRE
            bigFired = true;
            makeDamage.damage = bigDamage;
            return BigFireAction(animator);
        }
        return false;
    }

    protected void FixedUpdate()
    {
        smallCooldown--;
        bigCooldown--;
    }

    protected abstract bool SmallFireAction(Animator animator);

    protected abstract bool BigFireAction(Animator animator);
}
