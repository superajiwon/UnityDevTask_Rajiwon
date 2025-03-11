using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
	protected new Collider2D collider;
	protected Rigidbody2D rigid;

	protected GameObject hpPanel;
	protected Slider hpSlider;

	[SerializeField] protected float maxHP = 100;
	[SerializeField] protected float hp;

	[SerializeField] protected float attackRange;
	[SerializeField] protected float attackPower;
	public float AttackPower { get { return attackPower; } }

	protected bool isAttacking = false;
	protected bool isDead = false;

	protected virtual void Awake()
	{
		collider = GetComponent<Collider2D>();
		rigid = GetComponent<Rigidbody2D>();

		hpPanel = transform.Find("HPPanel").gameObject;
		hpSlider = transform.FindChildByName("HpSlider").GetComponent<Slider>();
	}

	protected virtual void OnEnable()
	{
		hp = maxHP;
	}

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
		HPSetting();
	}

	protected virtual void HPSetting()
	{
		if (hp <= 0)
		{
			gameObject.SetActive(false);

			return;
		}

		if (hp < maxHP) 
			hpPanel.SetActive(true);
		else
			hpPanel.SetActive(false);

		hpSlider.value = hp / 100;
	}

	protected void Hitted(float power)
	{
		hp -= power;
	}
}
