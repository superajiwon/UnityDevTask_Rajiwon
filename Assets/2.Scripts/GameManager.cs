using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// 스폰할 때 z위치 다르게
/// 같은 줄끼리 충돌 처리를 위해
/// Zfront(6), Zzero(7), Zback(8) -> layer로 처리

#region ENUMS
public enum ZombieLayerMask { Zfront = 6, Zzero, Zback, }

#endregion

public class GameManager : MonoBehaviour
{
	private static GameManager instance = null;
	public static GameManager Instance { get { return instance; } }

	[SerializeField] private GameObject zombiePrefab;		// 좀비 프리팹
	[SerializeField] private GameObject zombieSpawnPos;     // 좀비 소환 Position
	[SerializeField] private GameObject truck;
															   
	[SerializeField] private float zombieSpawnDelay;		// 좀비 소환 딜레이 시간
	[SerializeField] private int zombieSpawnMax;			// 최대 좀비 수

	private List<GameObject> zombies;						// 좀비 풀링 리스트


	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		// 좀비 풀 생성
		zombies = new List<GameObject>();
		for (int i = 0; i < zombieSpawnMax; i++)
		{
			GameObject zombie = Instantiate(zombiePrefab);

			zombies.Add(zombie);

			zombie.SetActive(false);
		}

		StartCoroutine(SpawnZombie());
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			Time.timeScale += 0.5f;
		}

		if (Input.GetKeyDown(KeyCode.X))
		{
			Time.timeScale = 1f;
		}

		if (Input.GetKeyDown(KeyCode.Z))
		{
			if (Time.timeScale > 1)
				Time.timeScale -= 0.5f;
		}
	}

	/// <summary>
	/// 미리 생성해 둔 좀비 하나씩 스폰
	/// 스폰 될 때마다 랜덤으로 front, zero, back 레이어 적용
	/// </summary>
	private IEnumerator SpawnZombie()
	{
		while (true)
		{
			foreach (GameObject zombie in zombies)
			{
				if (zombie.activeSelf == true) continue;

				Zombie zom = zombie.GetComponent<Zombie>();
				zom.zombieLayerMask = (ZombieLayerMask)Random.Range(6, 9);
				zombie.transform.position = zombieSpawnPos.transform.position;

				zombie.SetActive(true);
				break;
			}

			yield return new WaitForSeconds(zombieSpawnDelay);
		}
	}

}
