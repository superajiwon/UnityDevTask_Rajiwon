using System.Collections;
using System.Collections.Generic;
using System.Security;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;

public class Zombie : Character
{
	/// <summary>
	/// 같은 줄의 좀비끼리 부딪히면 뒤의 좀비가 앞의 좀비 머리 위로 올라섬
	/// 같은 줄끼리 충돌 처리를 위해
	/// Zfront(6), Zzero(7), Zback(8) -> layer로 처리
	/// </summary>

	private Animator animator;
	private Collider2D attackCollider;

	public float speed = .1f;
	public float jumpPower = 5f;

	public ZombieLayerMask zombieLayerMask;


	void Start()
	{
		gameObject.layer = (int)zombieLayerMask;

		collider = GetComponentsInChildren<Collider2D>()[0];
		rigid = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();

		// 공격 콜라이더
		attackCollider = GetComponentsInChildren<Collider2D>()[1];
		attackCollider.enabled = false;

		// z위치에 맞는 sprite 레이어 셋팅
		SpriteRenderer[] sprites = Helps.FindAllSpriteInChildren(transform);
		foreach (SpriteRenderer sprite in sprites)
		{
			sprite.sortingLayerName = zombieLayerMask.ToString();
		}
	}

	void Update()
	{
		transform.Translate(-Vector3.right * speed * Time.deltaTime);
	}

	/* 충돌 처리 함수 */
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Zombie"))
		{
			float otherZombieLeft = collision.collider.bounds.min.x;
			// 앞에 있을 때 
			if (otherZombieLeft >= collider.bounds.min.x) return;
			//if (collision.transform.position.x >= transform.position.x) return;

			float otherZombieTop = collision.collider.bounds.max.y; // 앞 좀비의 머리 위치 

			// 앞 좀비보다 조금 낮은 위치에서 충돌했을 때만 점프
			if (transform.position.y < otherZombieTop - 0.1f)
			{
				Debug.Log($"{name} : 올라갈래");

				rigid.velocity = new Vector2(rigid.velocity.x, jumpPower);
			}
		}

	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Hero") && isAttacking == false)
		{
			isAttacking = true;
			animator.SetBool("IsAttacking", isAttacking);
		}
	}

	/* 애니메이션 처리 함수 */
	/// <summary>
	/// 공격 시 공격 콜라이더 활성화
	/// </summary>
	public void OnAttack()
	{
		if (isDead == true || isAttacking == false) return;

		attackCollider.enabled = true;
	}

	/// <summary>
	/// 공격 끝나면 공격 콜라이더 비활성화 
	/// </summary>
	public void EndAttack()
	{
		if (isDead == true || isAttacking == false) return;

		attackCollider.enabled = false;

		isAttacking = false;
		animator.SetBool("IsAttacking", isAttacking);
	}
}