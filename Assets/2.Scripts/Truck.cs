using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Truck : MonoBehaviour
{
	private Rigidbody2D rigid;
	private new Collider2D collider;

	[SerializeField] private float speed;
	[SerializeField] private float wspeed;

	private GameObject backWheel;
	private GameObject frontWheel;

	private void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		collider = GetComponent<Collider2D>();

		backWheel = transform.FindChildByName("BackWheel").gameObject;
		frontWheel = transform.FindChildByName("FrontWheel").gameObject;
	}

	private void Update()
	{
		rigid.velocity = new Vector2(speed, rigid.velocity.y);

		float wheelspeed = (wspeed * -rigid.velocity.magnitude);

		backWheel.transform.Rotate(0, 0, wheelspeed);
		frontWheel.transform.Rotate(0, 0, wheelspeed);
	}

	public void Failed()
	{
		collider.isTrigger = true;
		speed = 0;
		wspeed = 0;
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

		//GUILayout.Label($"Truck : {(wspeed * -rigid.velocity.magnitude *Time.deltaTime)}", guIStyle);
		//GUILayout.Label($"{backWheel.name} : {backWheel.transform.rotation}", guIStyle);
		//GUILayout.Label($"{backWheel.name} : {frontWheel.transform.rotation}", guIStyle);
	}
#endif
}
