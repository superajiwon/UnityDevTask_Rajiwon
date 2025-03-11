using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helps 
{
	//public static SpriteRenderer[] FindAllSpriteInChildren(this Transform transform)
	//{
	//	SpriteRenderer[] sprites = transform.GetComponentsInChildren<SpriteRenderer>();

	//	return sprites;
	//}
	

	public static T[] FindAllComponentsInChildren<T>(this Transform transform)
	{
		T[] coms = transform.GetComponentsInChildren<T>();

		return coms;
	}

	public static GameObject FindChildByTag(this Transform transform, string tag)
	{
		Transform[] transforms = transform.GetComponentsInChildren<Transform>();

		foreach (Transform t in transforms)
		{
			if (t.transform.CompareTag(tag))
				return t.gameObject;
		}

		return null;
	}

	public static GameObject FindChildByName(this Transform transform, string name)
	{
		Transform[] transforms = transform.GetComponentsInChildren<Transform>();

		foreach (Transform t in transforms)
		{
			if (t.gameObject.name.Equals(name))
				return t.gameObject;
		}

		return null;
	}
}
