using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helps 
{
	public static SpriteRenderer[] FindAllSpriteInChildren(this Transform transform)
	{
		SpriteRenderer[] sprites = transform.GetComponentsInChildren<SpriteRenderer>();

		return sprites;
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
