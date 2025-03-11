using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Hero : Character
{
	private Camera mainCam;

	private GameObject weapon;			// ���� ������Ʈ
	private GameObject muzzle;          // �ѱ� �������� ���� ������Ʈ
	private GameObject truck;			// Ʈ��

	[Header("Weapon Option")]
	[SerializeField] private GameObject bulletPrefab;   // �Ѿ� ������
	[SerializeField] private GameObject bulletPoolObj;	// Ǯ�� �� �Ѿ� ��
	private List<GameObject> bulletPool;				// �Ѿ� Ǯ ����Ʈ
	private int bulletPoolSize;							// �Ѿ� Ǯ�� �� ����

	[Space]
	[SerializeField] private float shootingTime;		// �ڵ� �߻� �ӵ� (����)
	[SerializeField] private float spreadAngle;			// ��ź�� �߻� ���� (����)

	[Space]
	[SerializeField] private float bulletSpeed;			// �Ѿ� �߻� �ӵ�
	[SerializeField] private float bulletDisableTime;	// ���� Disable �ð�
	[SerializeField] private int bulletCount;			// �ѹ��� �߻� �� �Ѿ� ����

	private bool isShooting;			// �߻� ����
	private Vector2 aimDirection;		// ���� ����
	private LayerMask zombieLayerMask;	// ���� ���̾� ����ũ

	private Vector3 target;		// ������

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

		// �Ѿ� ������Ʈ Ǯ��
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

		// ���콺�� ��ġ�� ���� ����
		if (Input.GetMouseButton(0))
		{
			Vector2 mousePos = (Vector2)mainCam.ScreenToWorldPoint(Input.mousePosition);
			aimDirection = mousePos - (Vector2)weapon.transform.position;
			weapon.transform.right = aimDirection.normalized;

			target = mousePos;

			return;
		}

		// ���� ����� ���� ����
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
			// ������
			target = targetZombie.transform.position;
			aimDirection = targetZombie.transform.position - weapon.transform.position;
			weapon.transform.right = aimDirection.normalized;
		}
    }

	/// <summary>
	/// ���� �ڵ� �߻縦 ���� �ڷ�ƾ
	/// </summary>
	private IEnumerator Shoot()
	{
		while (true)
		{
			yield return new WaitForSeconds(shootingTime);

			if (isShooting == true) continue;

			isShooting = true;

			// ���� �Ѿ� ��ŭ �����ְ� �߻�
			for (int i = 0; i < bulletCount; i++)
			{
				GameObject bullet = GetBulletInPool();
				
				if (bullet == null) continue;

				bullet.transform.position = muzzle.transform.position;

				// �������� ������ ���� ��� 
				float angleOffset = Random.Range(-spreadAngle / 2, spreadAngle / 2);
				Vector2 spreadDirection = Quaternion.Euler(0, 0, angleOffset) * aimDirection;

				// �Ѿ��� ���ư��� ������ �ٶ󺸵���
				bullet.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(spreadDirection.y, spreadDirection.x) * Mathf.Rad2Deg);

				bullet.SetActive(true);
				bullet.GetComponent<Bullet>().SetDirection(spreadDirection.normalized, bulletSpeed, attackPower, 5);
			}

			isShooting = false;
		}
	}

	/// <summary>
	/// ��Ȱ��ȭ �Ǿ��ִ� �Ѿ� return
	/// </summary>
	private GameObject GetBulletInPool()
	{
		foreach (GameObject bullet in bulletPool)
		{
			if (bullet.activeSelf == false) 
				return bullet;
		}

		// �̸� ������ �� Ǯ���� ���ڸ� ��� ���� ����
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
				//GUILayout.Label($"Ÿ�� ���� : {targetZomD.name}", guIStyle);
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
