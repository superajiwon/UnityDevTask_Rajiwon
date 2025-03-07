using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

	/* 좀비 생성을 위한 변수 */
	[SerializeField] private GameObject zombiePrefab;           // 좀비 프리팹
	[SerializeField] private GameObject zombieSpawnPos;         // 좀비 소환 Position

	[SerializeField] private float zombieSpawnDelay = 3.0f;     // 좀비 소환 딜레이 시간
	[SerializeField] private int zombieSpawnMax = 10;           // 최대 좀비 수

	private List<GameObject> zombies;   // 좀비 풀
	private int zombieSpawnCount = 0;   // 활성화 된 좀비 수 (....)
	public void ZombieCount(int number) {  zombieSpawnCount += number; }

	private bool isSpawning = true; 

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
			GameObject zombie = Instantiate(zombiePrefab, zombieSpawnPos.transform.position, Quaternion.identity, zombieSpawnPos.transform);

			Zombie zom = zombie.GetComponent<Zombie>();
			zom.zombieLayerMask = (ZombieLayerMask)Random.Range(6, 9);

			zombie.name += zom.zombieLayerMask.ToString();
			zombies.Add(zombie);

			zombie.SetActive(false);
		}

		StartCoroutine(SpawnZombie());
	}

	private void Update()
	{
		isSpawning = zombieSpawnCount < zombieSpawnMax;
		//if (zombieSpawnCount >= zombieSpawnMax)
		//	isSpawning = false;
	}

	/// <summary>
	/// 미리 생성해 둔 좀비 하나씩 스폰
	/// </summary>
	private IEnumerator SpawnZombie()
	{
		while (isSpawning)
		{
			zombies[zombieSpawnCount].SetActive(true);
			zombieSpawnCount++;

			yield return new WaitForSeconds(zombieSpawnDelay);
		}
	}
}
