using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovements : EnemyMovements {
    public GameObject projectile;
    public Transform projectileOrigin;

	// Update is called once per frame
	void FixedUpdate () {
        LookForAgro();
        if (_target != null)
            Attack();
	}

    protected override void Track()
    {
        float dist = Mathf.Abs(_target.position.x - transform.position.x);
        dir = (int)Mathf.Sign(_target.position.x - transform.position.x);
        if (dir < 0)
        {
            Vector3 tmpScale = transform.localScale;
            tmpScale.x = -baseScale;
            transform.localScale = tmpScale;
        }
        else
        {
            Vector3 tmpScale = transform.localScale;
            tmpScale.x = baseScale;
            transform.localScale = tmpScale;
        }

        if (dist > 5f)
        {
            _rigidbody.velocity = new Vector2(dir * speed * 1.3f, _rigidbody.velocity.y);
        }
        else
        {
            if (canFire)
                _rigidbody.velocity = new Vector2(-(dir * speed * 2f), _rigidbody.velocity.y);
        }
    }

    protected new void Attack()
    {
        float dist = Mathf.Abs(_target.position.x - transform.position.x);
        if(dist < 7f && canFire && dist > 5f)
        {
            _animator.SetBool("Fire", true);
            StartCoroutine(TimerHelper.SetTimeout(1f, DoAttack));
            StartCoroutine(TimerHelper.SetTimeout(0.5f, () => { attackAdvertiser.gameObject.SetActive(true); }));
            canFire = false;
        }
    }

    protected void DoAttack()
    {
        GameObject tmpProjectile =  GameObject.Instantiate(projectile, projectileOrigin.position , new Quaternion());
        tmpProjectile.GetComponent<Projectiles>().dir = dir;
        _animator.SetBool("Fire", false);
        canFire = true;
        attackAdvertiser.gameObject.SetActive(false);
    }
}
