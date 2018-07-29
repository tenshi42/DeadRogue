using UnityEngine;

public class BaseDagger : Weapons
{
    public int smallActivationDelay;
    public int smallActivationDuration;
    public int bigActivationDelay;
    public int bigActivationDuration;
    private Animator playerAnimator;

    protected override bool SmallFireAction(Animator animator)
    {
        smallCooldown = smallBaseCooldown;
        playerAnimator = animator;
        return true;
    }

    protected override bool BigFireAction(Animator animator)
    {
        bigCooldown = bigBaseCooldown;
        playerAnimator = animator;
        return true;
    }

    protected new void FixedUpdate()
    {
        base.FixedUpdate();

        if (smallCooldown <= 0 && smallFired)
        {
            smallFired = false;
            if(playerAnimator != null)
                playerAnimator.SetBool("Fire1", false);
        }
        else if (smallBaseCooldown - smallCooldown >= smallActivationDelay + smallActivationDuration && smallFired)
        {
            _collider.enabled = false;
            //Debug.Log("activate small");
            //Debug.Log(smallFired);
        }
        else if (smallBaseCooldown - smallCooldown >= smallActivationDelay && smallFired)
        {
            _collider.enabled = true;
           // Debug.Log("disactivate small");
        }

        if (bigCooldown <= 0 && bigFired)
        {
            bigFired = false;
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("Fire2", false);
                //Debug.Log("set false to fire2");
            }
        }
        else if (bigBaseCooldown - bigCooldown >= bigActivationDelay + bigActivationDuration && bigFired)
        {
            _collider.enabled = false;
            //Debug.Log("activate big");
        }
        else if (bigBaseCooldown - bigCooldown >= bigActivationDelay && bigFired)
        {
            _collider.enabled = true;
            //Debug.Log("disactivate big");
        }
    }
}
