using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClashOfDefense.Game
{
	using Base;
	using Environment;
	using PathFinding;
	using Control;
	using Entity;
	using Structure;
	using UI;
	using UnityEngine.Events;

	public class GameManager : MonoBehaviour
	{
		// Singleton
		private static GameManager instance;
		public static GameManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<GameManager>();
				}
				return instance;
			}
		}

		[Header("Map Settings")]
		[SerializeField] private Vector2Int mapSize;
		[field: SerializeField] public float tileSize { get; private set; }
		[SerializeField] private Map map;
		[SerializeField] private PathFindMap pathFindMap;

		[Header("Round Settings")]
		[SerializeField] private EnemyLevelData enemyLevelData;
		[SerializeField] private int currentRoundIndex;

		[Header("Building Data")]
		[SerializeField] private BuildingData baseBuildingData;
		[SerializeField] private List<Building> spawnedBuildings;

		[Header("Status")]
		[SerializeField] private GameState gameState;
		[SerializeField] private PlayerControlState playerControlState;
		public List<EntityAI> spawnedEnemies { get; private set; }
		private List<Coroutine> spawnCoroutines;

		[Header("Others")]
		[SerializeField] private Transform spawnedContainer;
		public event UnityAction<EntityAI, Vector2Int> OnEnemyTreaversedTile;
		public event UnityAction<EntityAI> OnEnemyDeath;
		public event UnityAction<GameState> OnGameStateChange;

		private Vector2Int mapCenter;
		private void Awake()
		{
			// use system time as seed
			Random.InitState(System.DateTime.Now.Millisecond);
			CameraControl.Instance.onMouseClicked += OnMouseClicked;

			GameUIBase.Instance.onBattleButtonClick += OnBattleButtonClick;
			BuildingSelectionManager.Instance.OnBuildingSelected += OnBuildingSelected;
		}

		private void Start()
		{
			GenerateNewMap();
		}

		private void Update()
		{

		}

		private void OnBuildingSelected(BuildingData buildingData)
		{

		}

		private void OnBattleButtonClick()
		{
			if (gameState == GameState.Building)
			{
				SetGameState(GameState.Playing);
				SpawnRound();
			}
		}

		private void OnMouseClicked(int button, Vector3 position)
		{
			if (!GameUI.Instance.IsMouseOverUI())
			{
				Vector2Int mapPosition = map.WorldToMapPosition(position);
				if (map.IsInMap(mapPosition))
				{
					if (button == 0)
					{
						switch (gameState)
						{
							case GameState.Building:
								if (playerControlState == PlayerControlState.Building)
								{
									SpawnStructure(BuildingSelectionManager.Instance.selectedBuildingData, mapPosition);
								}
								break;
							case GameState.Playing:
								break;
						}
					}
					if (button == 1)
					{

					}
				}
			}
		}

		private void ResetAllBuildings()
		{
			for (int i = 0; i < spawnedBuildings.Count; i++)
			{
				spawnedBuildings[i].Reset();
			}
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

			CameraControl.Instance.SetBounds(new Vector3(mapSize.x, 100, mapSize.y) * tileSize);
		}

		private void ClearAll()
		{
			currentRoundIndex = 0;
			ClearAllBuildings();
			KillAllEnemies();
			pathFindMap.Clear();
			map.Clear();
			StopAllSpawnCoroutines();
			SetGameState(GameState.Instantiating);
		}

		private void MapGenerated()
		{
			TransferNodeCost();
			SpawnStructure(baseBuildingData, mapCenter);
		}

		/// <summary>
		/// Spawn a building from building data at position
		/// </summary>
		/// <param name="buildingData">BuildingData</param>
		/// <param name="position">Position</param>
		/// <param name="buildingLevelIndex">Index of the building's level</param>
		private void SpawnStructure(BuildingData buildingData, Vector2Int position, int buildingLevelIndex = 0)
		{
			if (buildingData == null)
			{
				return;
			}
			Building buildingPrefab = buildingData.levels[buildingLevelIndex].prefab;
			Vector3 worldPosition = Helper.MapPositionTransformer.MapToWorldPosition(position, tileSize);
			Building building = Instantiate(buildingPrefab, worldPosition, Quaternion.identity, spawnedContainer);
			building.Setup(Instantiate(buildingData), position);
			spawnedBuildings.Add(building);
		}

		private void TransferNodeCost()
		{
			map.QuerryNodeCost((pos, costs) => { pathFindMap.SetNodeCost(pos, costs); }, MapPrepared);
		}

		private void MapPrepared()
		{
			SetGameState(GameState.Building);
		}

		/// <summary>
		/// Start spawning a round of enemies
		/// </summary>
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

		/// <summary>
		/// Stops the batch spawning coroutines
		/// </summary>
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

		/// <summary>
		/// Destroy all buildings and clears spawnedBuildings list
		/// </summary>
		private void ClearAllBuildings()
		{
			if (spawnedBuildings != null)
			{
				foreach (Building building in spawnedBuildings)
				{
					if (building != null)
					{
						Destroy(building.gameObject);
					}
				}
				spawnedBuildings.Clear();
			}
		}

		/// <summary>
		/// Kill all living enemies
		/// </summary>
		private void KillAllEnemies()
		{
			if (spawnedEnemies != null)
			{
				foreach (EntityAI enemy in spawnedEnemies)
				{
					if (enemy != null)
					{
						Destroy(enemy.gameObject);
					}
				}
				spawnedEnemies.Clear();
			}
		}

		/// <summary>
		/// Spawn a batch of enemies
		/// </summary>
		/// <param name="batchData">BatchData</param>
		/// <returns></returns>
		private IEnumerator SpawnBatchEnumerator(BatchData batchData)
		{
			// spawn enemy
			foreach (BatchData.SpawnData spawnData in batchData.Enemies)
			{
				for (int i = 0; i < spawnData.count; i++)
				{
					Vector2Int mapSpawnPosition = DeterminSpawnPosition(batchData.SpawningPattern);
					Vector3 worldSpawnPosition = Helper.MapPositionTransformer.MapToWorldPosition(mapSpawnPosition, tileSize);
					EntityAI enemy = Instantiate(spawnData.enemyData.entityPrefab, worldSpawnPosition, Quaternion.identity, spawnedContainer);
					enemy.OnTreaversedTile += (pos) =>
					{
						enemy.ProcessStructureData(spawnedBuildings);
						OnEnemyTreaversedTile?.Invoke(enemy, pos);
					};
					enemy.OnDeath += ProcessEnemyDeath;
					spawnedEnemies.Add(enemy);

					pathFindMap.FindPathAsync(mapSpawnPosition, mapCenter, spawnData.enemyData.pathFindLayer, (path) => { enemy.Setup(spawnData.enemyData, path); });
					yield return new WaitForSeconds(batchData.SpawnInterval);
				}
			}
		}

		private void ProcessEnemyDeath(EntityAI enemy)
		{
			spawnedEnemies.Remove(enemy);
			OnEnemyDeath?.Invoke(enemy);
			if (spawnedEnemies.Count == 0)
			{
				currentRoundIndex++;
				SetGameState(GameState.Building);
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

		/// <summary>
		/// Advance the game state, between Playing and Building
		/// </summary>
		private void SetGameState(GameState state)
		{
			gameState = state;
			OnGameStateChange?.Invoke(gameState);
		}

		/// <summary>
		/// When gameState in Building state, what is the player doing.
		/// </summary>
		public enum PlayerControlState
		{
			None,
			Building,
		}
	}
}
