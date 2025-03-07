using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

/// ������ �� z��ġ �ٸ���
/// ���� �ٳ��� �浹 ó���� ����
/// Zfront(6), Zzero(7), Zback(8) -> layer�� ó��

#region ENUMS
public enum ZombieLayerMask { Zfront = 6, Zzero, Zback, }

#endregion

public class GameManager : MonoBehaviour
{
	private static GameManager instance = null;
	public static GameManager Instance { get { return instance; } }

	/* ���� ������ ���� ���� */
	[SerializeField] private GameObject zombiePrefab;           // ���� ������
	[SerializeField] private GameObject zombieSpawnPos;         // ���� ��ȯ Position

	[SerializeField] private float zombieSpawnDelay = 3.0f;     // ���� ��ȯ ������ �ð�
	[SerializeField] private int zombieSpawnMax = 10;           // �ִ� ���� ��

	private List<GameObject> zombies;   // ���� Ǯ
	private int zombieSpawnCount = 0;   // Ȱ��ȭ �� ���� �� (....)
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
		// ���� Ǯ ����
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
	/// �̸� ������ �� ���� �ϳ��� ����
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
