using System.Collections;
using System.Collections.Generic;
using System.Security;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;

public class Zombie : Character
{
	/// <summary>
	/// ���� ���� ���񳢸� �ε����� ���� ���� ���� ���� �Ӹ� ���� �ö�
	/// ���� �ٳ��� �浹 ó���� ����
	/// Zfront(6), Zzero(7), Zback(8) -> layer�� ó��
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

		// ���� �ݶ��̴�
		attackCollider = GetComponentsInChildren<Collider2D>()[1];
		attackCollider.enabled = false;

		// z��ġ�� �´� sprite ���̾� ����
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

	/* �浹 ó�� �Լ� */
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Zombie"))
		{
			float otherZombieLeft = collision.collider.bounds.min.x;
			// �տ� ���� �� 
			if (otherZombieLeft >= collider.bounds.min.x) return;
			//if (collision.transform.position.x >= transform.position.x) return;

			float otherZombieTop = collision.collider.bounds.max.y; // �� ������ �Ӹ� ��ġ 

			// �� ���񺸴� ���� ���� ��ġ���� �浹���� ���� ����
			if (transform.position.y < otherZombieTop - 0.1f)
			{
				Debug.Log($"{name} : �ö󰥷�");

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

	/* �ִϸ��̼� ó�� �Լ� */
	/// <summary>
	/// ���� �� ���� �ݶ��̴� Ȱ��ȭ
	/// </summary>
	public void OnAttack()
	{
		if (isDead == true || isAttacking == false) return;

		attackCollider.enabled = true;
	}

	/// <summary>
	/// ���� ������ ���� �ݶ��̴� ��Ȱ��ȭ 
	/// </summary>
	public void EndAttack()
	{
		if (isDead == true || isAttacking == false) return;

		attackCollider.enabled = false;

		isAttacking = false;
		animator.SetBool("IsAttacking", isAttacking);
	}
}