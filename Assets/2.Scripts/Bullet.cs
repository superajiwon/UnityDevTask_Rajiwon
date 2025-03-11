using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	private TrailRenderer trailRenderer;

	private bool isMoving = false;                  // 움직이고 있는지 확인
	private Vector2 targetDirection;                // 목표 방향

	[SerializeField] private float bulletSpeed;     // 발사 속도
	[SerializeField] private float disableTime;     // 강제로 사라지게 할 시간
	[SerializeField] private float attackPower;     // 공격력
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
	/// 발사 될 방향, 속도, 공격력, 지속시간 입력
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
	/// 총알이 충돌하지 않았을 경우 강제로 사라지게 하는 코루틴
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