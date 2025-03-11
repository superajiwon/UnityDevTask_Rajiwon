using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Hero : Character
{
	private Camera mainCam;

	private GameObject weapon;			// 무기 오브젝트
	private GameObject muzzle;          // 총구 포지션을 위한 오브젝트
	private GameObject truck;			// 트럭

	[Header("Weapon Option")]
	[SerializeField] private GameObject bulletPrefab;   // 총알 프리팹
	[SerializeField] private GameObject bulletPoolObj;	// 풀링 된 총알 집
	private List<GameObject> bulletPool;				// 총알 풀 리스트
	private int bulletPoolSize;							// 총알 풀링 총 개수

	[Space]
	[SerializeField] private float shootingTime;		// 자동 발사 속도 (간격)
	[SerializeField] private float spreadAngle;			// 산탄총 발사 각도 (범위)

	[Space]
	[SerializeField] private float bulletSpeed;			// 총알 발사 속도
	[SerializeField] private float bulletDisableTime;	// 강제 Disable 시간
	[SerializeField] private int bulletCount;			// 한번에 발사 할 총알 개수

	private bool isShooting;			// 발사 유무
	private Vector2 aimDirection;		// 에임 방향
	private LayerMask zombieLayerMask;	// 좀비 레이어 마스크

	private Vector3 target;		// 디버깅용

	protected override void Awake()
	{
		base.Awake();

		mainCam = Camera.main;
	}

	protected override void Start()
	{
		base.Start();

		isShooting = false;

		bulletPool = new List<GameObject>();
		bulletPoolSize = bulletCount * 4;
		
		weapon = transform.Find("Weapon").gameObject;
		muzzle = weapon.transform.Find("Muzzle").gameObject;
		truck = transform.root.gameObject;

		// 총알 오브젝트 풀링
		for (int i = 0; i < bulletPoolSize; i++)
		{
			GameObject bullet = Instantiate(bulletPrefab, bulletPoolObj.transform);
			bullet.SetActive(false);

			bulletPool.Add(bullet);
		}

		zombieLayerMask = LayerMask.GetMask("Zfront", "Zzero", "Zback");

		StartCoroutine(Shoot());
	}

	protected override void Update()
	{
		base.Update();

		transform.position = new Vector3(truck.transform.position.x, transform.position.y, 0);

		Aiming();
	}

	private void Aiming()
	{
		if (weapon == null) return;

		// 마우스나 터치로 직접 에임
		if (Input.GetMouseButton(0))
		{
			Vector2 mousePos = (Vector2)mainCam.ScreenToWorldPoint(Input.mousePosition);
			aimDirection = mousePos - (Vector2)weapon.transform.position;
			weapon.transform.right = aimDirection.normalized;

			target = mousePos;

			return;
		}

		// 가장 가까운 좀비 에임
		Vector3 pos = transform.position - new Vector3(-3, 3, 0);
		Collider2D[] zombies = Physics2D.OverlapCircleAll(pos, attackRange, zombieLayerMask);

		GameObject targetZombie = null;
		float targetDistance = Mathf.Infinity;

		foreach (Collider2D zombie in zombies)
		{
			if (zombie.transform.CompareTag("Zombie") == false) continue;

			float distance = Vector2.Distance(transform.position, zombie.transform.position);
			if (distance < targetDistance)
			{
				targetDistance = distance;
				targetZombie = zombie.gameObject;
			}
		}

		if (targetZombie != null)
		{
			// 디버깅용
			target = targetZombie.transform.position;
			aimDirection = targetZombie.transform.position - weapon.transform.position;
			weapon.transform.right = aimDirection.normalized;
		}
    }

	/// <summary>
	/// 샷건 자동 발사를 위한 코루틴
	/// </summary>
	private IEnumerator Shoot()
	{
		while (true)
		{
			yield return new WaitForSeconds(shootingTime);

			if (isShooting == true) continue;

			isShooting = true;

			// 샷건 총알 만큼 꺼내주고 발사
			for (int i = 0; i < bulletCount; i++)
			{
				GameObject bullet = GetBulletInPool();
				
				if (bullet == null) continue;

				bullet.transform.position = muzzle.transform.position;

				// 랜덤으로 퍼지는 각도 계산 
				float angleOffset = Random.Range(-spreadAngle / 2, spreadAngle / 2);
				Vector2 spreadDirection = Quaternion.Euler(0, 0, angleOffset) * aimDirection;

				// 총알이 날아가는 방향을 바라보도록
				bullet.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(spreadDirection.y, spreadDirection.x) * Mathf.Rad2Deg);

				bullet.SetActive(true);
				bullet.GetComponent<Bullet>().SetDirection(spreadDirection.normalized, bulletSpeed, attackPower, 5);
			}

			isShooting = false;
		}
	}

	/// <summary>
	/// 비활성화 되어있는 총알 return
	/// </summary>
	private GameObject GetBulletInPool()
	{
		foreach (GameObject bullet in bulletPool)
		{
			if (bullet.activeSelf == false) 
				return bullet;
		}

		// 미리 생성해 둔 풀보다 모자를 경우 새로 생성
		GameObject b = Instantiate(bulletPrefab, bulletPoolObj.transform);
		bulletPool.Add(b);
		b.SetActive(false);

		return b;
	}

	protected override void HPSetting()
	{
		if (hp <= 0) truck.GetComponent<Truck>().Failed();

		base.HPSetting();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.name == "headPivot")
		{
			Zombie zombie = collision.transform.parent.parent.GetComponent<Zombie>();
			Hitted(zombie.AttackPower);
		}
	}

#if UNITY_EDITOR
	private GUIStyle guIStyle;
	private void GuiSetting()
	{
		guIStyle = new GUIStyle();
		guIStyle.fontSize = 50;
		guIStyle.fontStyle = FontStyle.Bold;
	}

	private void OnGUI()
	{
		GuiSetting();

		if (Selection.activeGameObject == gameObject)
		{
			GUILayout.Label($"{weapon.name} : {weapon.transform.right}", guIStyle);
			//if (targetZomD != null)
				//GUILayout.Label($"타겟 좀비 : {targetZomD.name}", guIStyle);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 pos = transform.position - new Vector3(-3, 3, 0);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(pos, attackRange);

		//if (targetZomD != null)
		//{
			Gizmos.color = Color.red;
			//Gizmos.DrawWireSphere(targetZomD.transform.position, 2);
			Gizmos.DrawWireSphere(target, 1);
		//}
	}
#endif
}
