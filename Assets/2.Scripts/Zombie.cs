using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Zombie : Character
{
	/// <summary>
	/// ���� ���� ���񳢸� �ε����� ���� ���� ���� ���� �Ӹ� ���� �ö�
	/// ���� �ٳ��� �浹 ó���� ����
	/// Zfront(6), Zzero(7), Zback(8) -> layer�� ó��
	/// 
	/// �ڿ� ���� �ö󼭸� �� ����� �ڷ� �з�������
	/// </summary>

	private Animator animator;


	[Header("Layer")]
	public ZombieLayerMask zombieLayerMask;		// ���� �ٳ��� �浹 ó���� ���� ���̾� ó��
	private int layerMask;                      // ���� �� �� ���̾� ������ ���� ����

	[Space]
	[SerializeField] private Canvas canvas;		// HP UI ĵ����

	[Header("Move")]
	[SerializeField] private float moveSpeed;   // �����̴� �ӵ�
	[SerializeField] private float climbPower;	// �ö󰡴� ��
	[SerializeField] private float climbTime;	// �ö� �� �ְ� �ϴ� �ð�
	[SerializeField] private float pullingTime; // �и��� �ð�
	[SerializeField] private float raysize;		// ������ Ray ����

	private bool isClimbing = false;			// �ö󰡴� ������ Ȯ��
	private bool isPulling = false;				// �и����� Ȯ��

	private GameObject frontZombie;				// ������ �� ����
	private GameObject upZombie;                // ������ �� ����

	private Collider2D attackCollider;			// ���� �ݶ��̴� (�Ӹ�)

	protected override void Awake()
	{
		base.Awake();

		animator = GetComponent<Animator>();

		// ���� �ݶ��̴�
		attackCollider = GetComponentsInChildren<Collider2D>()[1];
		attackCollider.enabled = false;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		gameObject.layer = (int)zombieLayerMask;
		layerMask = LayerMask.GetMask(zombieLayerMask.ToString());

		// z��ġ�� �´� sprite ���̾� ����
		canvas.sortingLayerName = zombieLayerMask.ToString();
		SpriteRenderer[] sprites = transform.FindAllComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in sprites)
			sprite.sortingLayerName = zombieLayerMask.ToString();

		isAttacking = false; 
		isClimbing = false;
		isPulling = false;
	}

	protected override void Update()
	{
		if (gameObject.activeSelf == false) return;

		HPSetting();

		Moving();

		Attack();
	}

	protected override void HPSetting()
	{
		if (hp <= 0)
		{
			isDead = true;
			animator.SetBool("IsDead", isDead);

			StopAllCoroutines();
		}

		base.HPSetting();
	}

	private void Moving()
	{
		Vector3 center = collider.bounds.center;
		//float otherZombieTop = transform.position.y;

		/// ������ �̵�
		transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

		/// �տ� ���� �ִ��� Ȯ���ϱ�
		RaycastHit2D[] hits = Physics2D.RaycastAll(center, Vector2.left, raysize, layerMask);
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.collider == collider) continue;
			if (hit.collider.gameObject.CompareTag("Zombie"))
			{
				frontZombie = hit.collider.gameObject;
				//otherZombieTop = hit.collider.bounds.max.y;

				break;
			}
		}

		/// �ö󰡱�
		if (frontZombie != null)
		{
			if (gameObject.activeSelf == false) return;
			StartCoroutine(Climbing());
		}

		/// ������ ���� �ö󼭸� �ڷ� �и�����
		RaycastHit2D[] uphits = Physics2D.RaycastAll(center, Vector2.up, raysize * 2, layerMask);
		foreach (RaycastHit2D hit in uphits)
		{
			if (hit.collider == collider) continue;
			if (hit.collider.gameObject.CompareTag("Zombie"))
			{
				upZombie = hit.collider.gameObject;

				if (gameObject.activeSelf == false) return;
				StartCoroutine(Pulling());

				break;
			}
		}

	}

	/// <summary>
	/// �� ���� ���� �ö󰡱� ���� �ڷ�ƾ
	/// </summary>
	private IEnumerator Climbing()
	{
		if (isClimbing == true) yield break;

		rigid.velocity = new Vector2(rigid.velocity.x, climbPower);
		isClimbing = true;

		yield return new WaitForSeconds(climbTime);

		frontZombie = null;
		isClimbing = false;
	}

	/// <summary>
	/// �ڷ� �и��� ���� �ڷ�ƾ
	/// </summary>
	private IEnumerator Pulling()
	{
        if (isPulling == true) yield break;

		isPulling = true;
		moveSpeed = -1;

		yield return new WaitForSeconds(pullingTime);

		isPulling = false;
		moveSpeed = 1;
		
		upZombie = null;
	}

	/// <summary>
	/// ���� ������ Hero�� �浹�Ǿ��� �� ����
	/// </summary>
	private void Attack()
	{
		if (isAttacking == true) return;

		// �Ӹ��ʿ� �浹 ���� ��ġ
		Vector2 pos = transform.position + new Vector3(-0.5f, 0.7f, 0);
		Collider2D[] hits = Physics2D.OverlapCircleAll(pos, attackRange);
		foreach (Collider2D hit in hits)
		{
			if (hit.gameObject.CompareTag("Hero"))
			{
				isAttacking = true;
				animator.SetBool("IsAttacking", isAttacking);
			}
		}
	}

	/// <summary> 
	/// ���� �� ���� �ݶ��̴� Ȱ��ȭ (�ִϸ��̼� �̺�Ʈ)
	/// </summary>
	public void OnAttack()
	{
		if (isAttacking == false) return;

		attackCollider.enabled = true;
	}

	/// <summary>
	/// ���� ������ ���� �ݶ��̴� ��Ȱ��ȭ (�ִϸ��̼� �̺�Ʈ)
	/// </summary>
	public void EndAttack()
	{
		if (isAttacking == false) return;

		attackCollider.enabled = false;

		isAttacking = false;
		animator.SetBool("IsAttacking", isAttacking);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// �Ѿ� �ǰ� ��
		if (collision.transform.CompareTag("Bullet"))
		{
			Bullet bullet = collision.gameObject.GetComponent<Bullet>();
			Hitted(bullet.AttackPower);
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
			GUILayout.Label("\n\n");
			GUILayout.Label($"{gameObject.name} : {rigid.velocity}", guIStyle);
			GUILayout.Label($"isClimbing : {isClimbing}", guIStyle);
			if (frontZombie != null) 
				GUILayout.Label($"frontZombie : {frontZombie.name}", guIStyle);
			if (upZombie != null)
				GUILayout.Label($"upZombie : {upZombie.name}", guIStyle);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Vector2 pos = transform.position + new Vector3(-0.5f, 0.7f, 0);
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(pos, 0.1f);
	}
#endif
}