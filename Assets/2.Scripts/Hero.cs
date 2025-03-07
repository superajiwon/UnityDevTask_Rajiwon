using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Hero : Character
{
	[SerializeField] private GameObject weapon;

	[SerializeField] private GameObject hpPanel;
	[SerializeField] private Slider hpSlider;


	private void Awake()
	{
		collider = GetComponent<Collider2D>();
		rigid = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		hp = maxHP;
	}

	private void Update()
	{
		HPSetting();
	}

	private void HPSetting()
	{
		if (hp <= 0) gameObject.SetActive(false);

		if (hp < maxHP) hpPanel.SetActive(true);

		hpSlider.value = hp / 100;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.name == "headPivot")
		{
			hp -= 5;
		}
	}
}
