using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	protected new Collider2D collider;
	protected Rigidbody2D rigid;

	protected float maxHP = 100;
	protected float hp;
	protected float attackPow;

	protected bool isAttacking;
	protected bool isDead;

    void Start()
    {
        
    }
}
