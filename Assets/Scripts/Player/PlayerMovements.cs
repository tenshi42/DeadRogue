using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Stats))]
[RequireComponent(typeof(PlayerEquipement))]
[RequireComponent(typeof(TakeDamage))]
public class PlayerMovements : MonoBehaviour {
    public Transform groundCheck;
    public float speed = 1f;
    private Rigidbody2D _rigidbody;
    private bool isLanded = true;
    private bool doubleJump = false;
    private int jumpDelay = 1;
    private Animator _animator;
    private bool facingRight = true;

    public float JumpForce = 2700f;

    public bool canFire1 = true;
    public bool canFire2 = true;

    private PlayerEquipement playerEquipement;

    public LayerMask whatIsGround;
    public LayerMask traversableLayerMask;


    // Use this for initialization
    void Start () {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        playerEquipement = GetComponent<PlayerEquipement>();

        SetWeapon(WeaponsManager._instance.weapons["Daguer"] , playerEquipement.rightWeapon);
        SetWeapon(WeaponsManager._instance.weapons["Daguer"] , playerEquipement.leftWeapon);
        playerEquipement.weaponType = WeaponType.Dagger;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Move();
        Actions();
	}

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");

        isLanded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, whatIsGround);
        if (isLanded)
            doubleJump = false;

        _rigidbody.velocity = new Vector2(h * speed, _rigidbody.velocity.y);
        _animator.SetFloat("Speed", Mathf.Abs(h));

        if (h > 0 && !facingRight)
            Flip();
        else if (h < 0 && facingRight)
            Flip();
    }

    private void Update()
    {
        bool jump = Input.GetButtonDown("Jump");
        float v = Input.GetAxis("Vertical");

        if (v < 0 && jump && isLanded)
        {
            if (MoveDownTraversable())
            {
                _rigidbody.AddForce(new Vector2(0, -JumpForce * 1f));
                return;
            }
        }

        if ((isLanded || !doubleJump) && jump)
        {
            _rigidbody.AddForce(new Vector2(0, JumpForce));

            if (!isLanded && !doubleJump)
                doubleJump = true;
        }
    }

    private bool MoveDownTraversable()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f, traversableLayerMask);
        if (cols.Length > 0)
        {
            foreach (Collider2D col in cols)
            {
                col.GetComponent<PlatformEffector2D>().colliderMask = LayerMaskHelper.AllButPlayer;
                StartCoroutine(TimerHelper.SetTimeout(0.5f, () => { col.GetComponent<PlatformEffector2D>().colliderMask = LayerMaskHelper.All; }));
            }
            return true;
        }
        return false;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 baseScale = transform.localScale;
        baseScale.x *= -1;
        transform.localScale = baseScale;
    }

    private void Actions()
    {
        bool fire1 = Input.GetButton("Fire1");
        bool fire2 = Input.GetButton("Fire2");

        if (playerEquipement.weaponType == WeaponType.Dagger)
        {
            DaggerActions(fire1, fire2);
        }
    }

    private void DaggerActions(bool fire1, bool fire2)
    {
        if (fire1 && canFire1)
        {
            bool tmpCanFire = playerEquipement.GetRightWeapon().SmallFire(_animator);
            if (tmpCanFire)
                fire1_action();
        }
        else if (fire2 && canFire2)
        {
            bool tmpCanSmallFire = playerEquipement.GetRightWeapon().BigFire(_animator);
            bool tmpCanBigFire = playerEquipement.GetLeftWeapon().BigFire(_animator);
            if (tmpCanSmallFire)
                fire2_action();
        }
    }

    private void fire1_action()
    {
        _animator.SetBool("Fire1", true);
    }

    private void fire2_action()
    {
        _animator.SetBool("Fire2", true);
    }

    public void SetWeapon(GameObject weapon, Transform hand)
    {
        GameObject tmpWeapon = GameObject.Instantiate(weapon);
        tmpWeapon.transform.SetParent(hand);
        tmpWeapon.transform.localPosition = new Vector3();
        tmpWeapon.transform.localRotation = new Quaternion();
        tmpWeapon.transform.localScale = Vector3.one;
    }
}
