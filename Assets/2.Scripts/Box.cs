using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Box : Character
{
	private Transform Center;

	protected override void Start()
	{
		base.Start();

		Center = transform.root.Find("Center");
	}

	protected override void Update()
	{
		base.Update();

		transform.position = new Vector3(Center.position.x, transform.position.y, 0);
	}

	protected override void HPSetting()
	{
		if (hp <= 0)
		{
			isDead = true;
			gameObject.SetActive(false);

			return;
		}

		base.HPSetting();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.name == "headPivot")
		{
			Zombie zombie = collision.transform.root.GetComponent<Zombie>();
			Hitted(zombie.AttackPower);
		}
	}
}
