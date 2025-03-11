using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Zombie : Character
{
	/// <summary>
	/// 같은 줄의 좀비끼리 부딪히면 뒤의 좀비가 앞의 좀비 머리 위로 올라섬
	/// 같은 줄끼리 충돌 처리를 위해
	/// Zfront(6), Zzero(7), Zback(8) -> layer로 처리
	/// 
	/// 뒤에 좀비가 올라서면 밑 좀비는 뒤로 밀려나도록
	/// </summary>

	private Animator animator;


	[Header("Layer")]
	public ZombieLayerMask zombieLayerMask;		// 같은 줄끼리 충돌 처리를 위한 레이어 처리
	private int layerMask;                      // 생성 될 때 레이어 적용을 위한 변수

	[Space]
	[SerializeField] private Canvas canvas;		// HP UI 캔버스

	[Header("Move")]
	[SerializeField] private float moveSpeed;   // 움직이는 속도
	[SerializeField] private float climbPower;	// 올라가는 힘
	[SerializeField] private float climbTime;	// 올라갈 수 있게 하는 시간
	[SerializeField] private float pullingTime; // 밀리는 시간
	[SerializeField] private float raysize;		// 감지할 Ray 길이

	private bool isClimbing = false;			// 올라가는 중인지 확인
	private bool isPulling = false;				// 밀리는지 확인

	private GameObject frontZombie;				// 감지된 앞 좀비
	private GameObject upZombie;                // 감지된 위 좀비

	private Collider2D attackCollider;			// 공격 콜라이더 (머리)

	protected override void Awake()
	{
		base.Awake();

		animator = GetComponent<Animator>();

		// 공격 콜라이더
		attackCollider = GetComponentsInChildren<Collider2D>()[1];
		attackCollider.enabled = false;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		gameObject.layer = (int)zombieLayerMask;
		layerMask = LayerMask.GetMask(zombieLayerMask.ToString());

		// z위치에 맞는 sprite 레이어 셋팅
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

		/// 앞으로 이동
		transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

		/// 앞에 좀비가 있는지 확인하기
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

		/// 올라가기
		if (frontZombie != null)
		{
			if (gameObject.activeSelf == false) return;
			StartCoroutine(Climbing());
		}

		/// 누군가 위에 올라서면 뒤로 밀리도록
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
	/// 앞 좀비 위로 올라가기 위한 코루틴
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
	/// 뒤로 밀리기 위한 코루틴
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
	/// 공격 범위에 Hero가 충돌되었을 때 공격
	/// </summary>
	private void Attack()
	{
		if (isAttacking == true) return;

		// 머리쪽에 충돌 범위 위치
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
	/// 공격 시 공격 콜라이더 활성화 (애니메이션 이벤트)
	/// </summary>
	public void OnAttack()
	{
		if (isAttacking == false) return;

		attackCollider.enabled = true;
	}

	/// <summary>
	/// 공격 끝나면 공격 콜라이더 비활성화 (애니메이션 이벤트)
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
		// 총알 피격 시
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