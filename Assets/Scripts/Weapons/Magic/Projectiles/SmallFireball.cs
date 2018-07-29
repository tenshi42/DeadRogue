using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MakeDamage))]
public class SmallFireball : Projectiles {
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update () {
        _rigidbody.velocity = new Vector2(dir * speed, _rigidbody.velocity.y);
        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.02f, LayerMaskHelper.Wall);
        if(col != null)
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
