using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
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

	[SerializeField] private GameObject zombiePrefab;		// ���� ������
	[SerializeField] private GameObject zombieSpawnPos;     // ���� ��ȯ Position
	[SerializeField] private GameObject truck;
															   
	[SerializeField] private float zombieSpawnDelay;		// ���� ��ȯ ������ �ð�
	[SerializeField] private int zombieSpawnMax;			// �ִ� ���� ��

	private List<GameObject> zombies;						// ���� Ǯ�� ����Ʈ


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
	/// �̸� ������ �� ���� �ϳ��� ����
	/// ���� �� ������ �������� front, zero, back ���̾� ����
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
