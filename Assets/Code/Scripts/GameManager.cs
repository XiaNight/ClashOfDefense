using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClashOfDefense.Game
{
	using Environment;
	using PathFinding;
	using Control;
	using Entity;
	using Structure;

	public class GameManager : MonoBehaviour
	{
		[SerializeField] private Vector2Int mapSize;
		[SerializeField] private float tileSize;
		[SerializeField] private Map map;
		[SerializeField] private PathFindMap pathFindMap;
		[SerializeField] private CameraControl cameraControl;

		[Header("Round Settings")]
		[SerializeField] private EnemyLevelData enemyLevelData;
		[SerializeField] private int currentRoundIndex;
		[Header("Building Data")]
		[SerializeField] private Building baseBuildingPrefab;
		[SerializeField] private List<Building> spawnedBuildings;

		public int baseHealth = 100;
		public int baseGold = 100;

		private List<EntityAI> spawnedEnemies;
		private List<Coroutine> spawnCoroutines;

		private Vector2Int mapCenter;
		private void Awake()
		{
			// use system time as seed
			Random.InitState(System.DateTime.Now.Millisecond);
			cameraControl.onMouseClicked += OnMouseClicked;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				GenerateNewMap();
			}
		}

		private void OnMouseClicked(int button, Vector3 position)
		{
			Vector2Int mapPosition = map.WorldToMapPosition(position);
			if (map.IsInMap(mapPosition))
			{
				if (button == 0)
				{

				}
				if (button == 1)
				{

				}
			}
		}

		private Vector3 MouseRayPosition()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000))
			{
				return hit.point;
			}
			return -Vector3.one;
		}

		private void GenerateNewMap()
		{
			ClearAll();

			pathFindMap.SetMapSize(mapSize);
			pathFindMap.Setup();
			mapCenter = mapSize / 2;

			// set map size
			map.Setup(mapSize, Random.Range(0, 1000000), tileSize);
			map.GenerateMap(MapGenerated);

			cameraControl.SetBounds(new Vector3(mapSize.x, 100, mapSize.y) * tileSize);
		}

		private void ClearAll()
		{
			currentRoundIndex = 0;
			pathFindMap.Clear();
			map.Clear();
			StopAllSpawnCoroutines();
			KillAllEnemies();
		}

		private void MapGenerated()
		{
			TransferNodeCost();
			SpawnStructure(baseBuildingPrefab, mapCenter);
		}

		private void SpawnStructure(Building buildingPrefab, Vector2Int position)
		{
			Vector3 worldPosition = Helper.MapPositionTransformer.MapToWorldPosition(position, tileSize);
			Building building = Instantiate(buildingPrefab, worldPosition, Quaternion.identity);
			spawnedBuildings.Add(building);
		}

		private void TransferNodeCost()
		{
			map.QuerryNodeCost((pos, costs) => { pathFindMap.SetNodeCost(pos, costs); }, SpawnRound);
		}

		private void SpawnRound()
		{
			StopAllSpawnCoroutines();
			if (spawnCoroutines == null)
			{
				spawnCoroutines = new List<Coroutine>();
			}

			if (spawnedEnemies == null)
			{
				spawnedEnemies = new List<EntityAI>();
			}

			RoundData currentRound = enemyLevelData.rounds[currentRoundIndex];
			foreach (BatchData batchData in currentRound.batches)
			{
				spawnCoroutines.Add(StartCoroutine(SpawnBatchEnumerator(batchData)));
			}
		}

		private void StopAllSpawnCoroutines()
		{
			if (spawnCoroutines != null)
			{
				foreach (Coroutine coroutine in spawnCoroutines)
				{
					StopCoroutine(coroutine);
				}
				spawnCoroutines.Clear();
			}
		}

		private void KillAllEnemies()
		{
			if (spawnedEnemies != null)
			{
				foreach (EntityAI enemy in spawnedEnemies)
				{
					Destroy(enemy.gameObject);
				}
				spawnedEnemies.Clear();
			}
		}

		private IEnumerator SpawnBatchEnumerator(BatchData batchData)
		{
			// spawn enemy
			foreach (BatchData.SpawnData spawnData in batchData.enemies)
			{
				for (int i = 0; i < spawnData.count; i++)
				{
					Vector2Int mapSpawnPosition = DeterminSpawnPosition(batchData.spawnPattern);
					Vector3 worldSpawnPosition = Helper.MapPositionTransformer.MapToWorldPosition(mapSpawnPosition, tileSize);
					EntityAI enemy = Instantiate(spawnData.enemyData.entityPrefab, worldSpawnPosition, Quaternion.identity);
					enemy.OnTreaversedTile += (pos) => { enemy.ProcessStructureData(spawnedBuildings); };
					spawnedEnemies.Add(enemy);

					pathFindMap.FindPathAsync(mapSpawnPosition, mapCenter, spawnData.enemyData.pathFindLayer, (path) => { enemy.Setup(path, tileSize); });
					yield return new WaitForSeconds(batchData.spawnInterval);
				}
			}
		}

		private Vector2Int DeterminSpawnPosition(BatchData.SpawnPattern spawnPattern)
		{
			if (spawnPattern == BatchData.SpawnPattern.ArroundTheMap)
			{
				spawnPattern = (BatchData.SpawnPattern)Random.Range(0, 4);
			}
			switch (spawnPattern)
			{
				case BatchData.SpawnPattern.FromTop:
					return new Vector2Int(Random.Range(0, mapSize.x), mapSize.y - 1);
				case BatchData.SpawnPattern.FromBottom:
					return new Vector2Int(Random.Range(0, mapSize.x), 0);
				case BatchData.SpawnPattern.FromLeft:
					return new Vector2Int(0, Random.Range(0, mapSize.y));
				case BatchData.SpawnPattern.FromRight:
					return new Vector2Int(mapSize.x - 1, Random.Range(0, mapSize.y));
				default:
					return Vector2Int.zero;
			}
		}

		private void OnDrawGizmosSelected()
		{

		}
	}
}
