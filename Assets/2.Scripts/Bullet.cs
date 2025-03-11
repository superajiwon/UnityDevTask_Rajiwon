using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	private TrailRenderer trailRenderer;

	private bool isMoving = false;                  // �����̰� �ִ��� Ȯ��
	private Vector2 targetDirection;                // ��ǥ ����

	[SerializeField] private float bulletSpeed;     // �߻� �ӵ�
	[SerializeField] private float disableTime;     // ������ ������� �� �ð�
	[SerializeField] private float attackPower;     // ���ݷ�
	public float AttackPower { get { return attackPower; } }

	private void Awake()
	{
		trailRenderer = GetComponent<TrailRenderer>();
	}

	private void Update()
	{
		if (isMoving == false) return;

		transform.Translate(targetDirection * bulletSpeed * Time.deltaTime, Space.World);
	}

	/// <summary>
	/// �߻� �� ����, �ӵ�, ���ݷ�, ���ӽð� �Է�
	/// </summary>
	public void SetDirection(Vector2 shootDirection, float speed, float power, float time)
	{
		trailRenderer.Clear();

		bulletSpeed = speed;
		disableTime = time;
		attackPower = power;

		targetDirection = shootDirection;
		isMoving = true;

		StartCoroutine(DisableBullet());
	}

	/// <summary>
	/// �Ѿ��� �浹���� �ʾ��� ��� ������ ������� �ϴ� �ڷ�ƾ
	/// </summary>
	private IEnumerator DisableBullet()
	{
		yield return new WaitForSeconds(disableTime);

		gameObject.SetActive(false);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.transform.CompareTag("Zombie") || (LayerMask.LayerToName(collision.gameObject.layer) == "Zfront"))
		{
			gameObject.SetActive(false);
			isMoving = false;
		}
	}
}