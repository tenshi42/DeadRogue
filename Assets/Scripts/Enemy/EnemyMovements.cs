using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TakeDamage))]
[RequireComponent(typeof(Stats))]
public class EnemyMovements : MonoBehaviour {
    public Transform groundCheck;
    public Transform wall_col;
    public Transform attackAdvertiser;

    public float agroDistance = 1.2f;
    public float speed = 6f;
    public bool isSpawned = false;

    protected TakeDamage takeDamage;
    protected Stats stats;
    protected int hitCount = 0;

    protected Animator _animator;
    protected Rigidbody2D _rigidbody;

    protected int dir = 0;
    protected int movementContinuity = 0;

    protected bool isAgro = false;

    protected Transform _target;
    protected float baseScale;

    protected bool canFire = true;

	// Use this for initialization
	void Start () {
        takeDamage = GetComponent<TakeDamage>();
        stats = GetComponent<Stats>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        takeDamage.Hit += OnHit;
        takeDamage.Die += OnDie;
        dir = (int)Mathf.Sign(UnityEngine.Random.Range(0, 100) - 50);
        baseScale = transform.localScale.x;
        if(dir < 0)
        {
            Vector3 tmpScale = transform.localScale;
            tmpScale.x *= -1;
            transform.localScale = tmpScale;
        }
        attackAdvertiser.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        //_animator.GetCurrentAnimatorStateInfo(0).
        if (!isSpawned && _animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
        {
            isSpawned = true;
        }
    }

    private void FixedUpdate()
    {
        
    }

    protected void LookForAgro()
    {
        if (isSpawned)
        {
            //Debug.Log("ok");
            isAgro = false;
            _target = null;
            Collider2D col = Physics2D.OverlapCircle(groundCheck.position, agroDistance, LayerMaskHelper.Player);
            if (col != null && Mathf.Abs(col.transform.position.y - groundCheck.position.y) < 1f)
            {
                Debug.DrawLine(groundCheck.position , col.transform.position);
                _target = col.transform;
                isAgro = true;
            }
            else
            {
                
            }
            if (!isAgro)
            {
                Move();
            }
            else
            {
                Track();
            }
        }
    }

    protected void Move()
    {
        Collider2D t = null;
        if (t = Physics2D.OverlapCircle(wall_col.position, 0.02f, LayerMaskHelper.Wall))
        {
            dir = -dir;
            Vector3 tmpScale = transform.localScale;
            tmpScale.x *= -1;
            transform.localScale = tmpScale;
        }

        _animator.SetFloat("Speed", 1f);

        _rigidbody.velocity = new Vector2(dir * speed, _rigidbody.velocity.y);
    }

    protected virtual void Track()
    {
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

        _rigidbody.velocity = new Vector2(dir * speed * 1.3f, _rigidbody.velocity.y);
    }

    protected void Attack()
    {

    }

    protected void OnHit(Transform other)
    {
        // TO DO : Implement damage cooldown
        isAgro = true;
        _target = other;
        _animator.SetTrigger("Hit");
        hitCount++;
        //Debug.Log(takeDamage.currentHealth);
        //Debug.Log("Get hit : " + hitCount.ToString());
    }

    protected void OnDie()
    {
        _animator.SetBool("Dying", true);
        Debug.Log("I'm fucking diying !!!");
    }

    public void EndDie()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GameObject.Destroy(this.gameObject);
    }
}
