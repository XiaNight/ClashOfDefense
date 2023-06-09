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
		#region Singleton
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
		#endregion
		#region Parameters

		[Header("Map Settings")]
		[SerializeField] private Vector2Int mapSize;
		[field: SerializeField] public float tileSize { get; private set; }
		[SerializeField] private Map map;
		[SerializeField] private PathFindMap pathFindMap;

		[Header("Round Settings")]
		[SerializeField] private EnemyLevelData enemyLevelData;
		[SerializeField] private int currentRoundIndex;

		[Header("Building")]
		[SerializeField] private BuildingData baseBuildingData;
		[SerializeField] private List<Building> spawnedBuildings;


		[Header("Status")]
		[SerializeField] private int money;
		public int Money { get { return money; } }
		[SerializeField] private GameState gameState;
		[SerializeField] private PlayerControlState playerControlState;
		public List<EntityAI> spawnedEnemies { get; private set; }
		public int enemyAlive { get; private set; }
		private List<Coroutine> spawnCoroutines;

		[Header("Others")]
		[SerializeField] private Transform spawnedContainer;
		public event UnityAction<EntityAI, Vector2Int> OnEnemyTreaversedTile;
		public event UnityAction<EntityAI> OnEnemyDeath;
		public event UnityAction<GameState> OnGameStateChange;
		public event UnityAction<int> OnMoneyChanged;

		private Vector2Int mapCenter;
		private Building previewingBuilding;

		#endregion Parameters
		#region Unity Functions

		private void Awake()
		{
			// use system time as seed
			Random.InitState(System.DateTime.Now.Millisecond);
			CameraControl.Instance.onMouseClicked += OnMouseClicked;

			GameUI.Instance.onBattleButtonClick += OnBattleButtonClick;
			BuildingSelectionManager.Instance.OnBuildingSelected += OnBuildingSelected;
			BuildingPreview.Instance.OnAccept += OnBuildingPreviewAccept;
			BuildingPreview.Instance.OnCancel += OnBuildingPreviewCancel;

			AddMoney(2000);
		}

		private void Start()
		{
			GenerateNewMap();
		}

		private void Update()
		{

		}
		#endregion
		#region User Control

		private void OnBuildingSelected(BuildingData buildingData)
		{

		}

		private void OnBattleButtonClick()
		{
			if (gameState == GameState.Building)
			{
				SpawnRound();
				SetGameState(GameState.Playing);
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
						MainClickOnMap(mapPosition);
					}
					if (button == 1)
					{

					}
				}
			}
		}

		private void MainClickOnMap(Vector2Int mapPosition)
		{
			switch (gameState)
			{
				case GameState.Building:
					if (playerControlState == PlayerControlState.Building)
					{
						BuildingData selectedBuilding = BuildingSelectionManager.Instance.selectedBuildingData;
						if (selectedBuilding != null)
						{
							if (previewingBuilding != null)
							{
								// Previewing
								SetBuildingPosition(previewingBuilding, mapPosition);
								BuildingPreview.Instance.UpdateData();
								BuildingPreview.Instance.SetAcceptable(!IsPositionOccupied(mapPosition));
							}
							else
							{
								// Putting a new preview building
								previewingBuilding = InstantiateBuildingData(selectedBuilding);
								SetBuildingPosition(previewingBuilding, mapPosition);
								BuildingPreview.Instance.Setup(previewingBuilding);
								BuildingPreview.Instance.SetAcceptable(!IsPositionOccupied(mapPosition));
							}
						}
					}
					break;
				case GameState.Playing:
					break;
			}
		}

		private void OnBuildingPreviewAccept()
		{
			if (previewingBuilding == null)
			{
				return;
			}
			int cost = previewingBuilding.Data.levels[0].cost;
			if (cost > money)
			{
				Debug.Log("Not enough money");
				return;
			}
			AddBuildingToList(previewingBuilding);
			SpendMoney(cost);
			previewingBuilding = null;
			BuildingPreview.Instance.SetState(false);
		}

		private void OnBuildingPreviewCancel()
		{
			if (previewingBuilding != null)
			{
				previewingBuilding.Delete();
				previewingBuilding = null;
			}
			BuildingPreview.Instance.SetState(false);
		}

		#endregion
		#region Building

		/// <summary>
		/// Spawn a building from building data at position
		/// </summary>
		/// <param name="buildingData">BuildingData</param>
		/// <param name="position">Position</param>
		/// <param name="buildingLevelIndex">Index of the building's level</param>
		/// <returns>Spawned building</returns>
		private Building SpawnBuilding(BuildingData buildingData, Vector2Int position, int buildingLevelIndex = 0)
		{
			if (buildingData == null)
			{
				return null;
			}
			if (IsPositionOccupied(position))
			{
				return null;
			}
			Building building = InstantiateBuildingData(buildingData, buildingLevelIndex);
			SetBuildingPosition(building, position);
			AddBuildingToList(building);
			return building;
		}

		private bool IsPositionOccupied(Vector2Int tilePosition)
		{
			for (int i = 0; i < spawnedBuildings.Count; i++)
			{
				if (spawnedBuildings[i].tilePosition == tilePosition)
				{
					return true;
				}
			}
			return false;
		}

		private void SetBuildingPosition(Building building, Vector2Int position)
		{
			Vector3 worldPosition = Helper.MapPositionTransformer.MapToWorldPosition(position, tileSize);
			building.transform.position = worldPosition;
			building.SetPosition(position);
		}

		private Building InstantiateBuildingData(BuildingData buildingData, int buildingLevelIndex = 0)
		{
			if (buildingData == null)
			{
				return null;
			}
			Building buildingPrefab = buildingData.levels[buildingLevelIndex].prefab;
			Building building = Instantiate(buildingPrefab, spawnedContainer);
			building.Setup(Instantiate(buildingData));
			return building;
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

		private void AddBuildingToList(Building building)
		{
			if (building != null)
			{
				spawnedBuildings.Add(building);
				building.OnDeleted += () =>
				{
					RemoveBuildingFromList(building);
				};
			}
		}

		private void RemoveBuildingFromList(Building building)
		{
			if (building != null)
			{
				spawnedBuildings.Remove(building);
			}
		}

		#endregion
		#region Map

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

		private void MapGenerated()
		{
			TransferNodeCost();
			SpawnBuilding(baseBuildingData, mapCenter).OnDead += BaseDestroyed;
		}

		private void BaseDestroyed()
		{
			SetGameState(GameState.Ended);
		}

		private void TransferNodeCost()
		{
			map.QuerryNodeCost((pos, costs) => { pathFindMap.SetNodeCost(pos, costs); }, MapPrepared);
		}

		private void MapPrepared()
		{
			SetGameState(GameState.Building);
		}

		#endregion
		#region Enemy
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

			enemyAlive = 0;
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
			// Calculate enemy count
			foreach (BatchData.SpawnData spawnData in batchData.Enemies)
			{
				enemyAlive += spawnData.enemyCount;
			}

			// spawn enemy
			foreach (BatchData.SpawnData spawnData in batchData.Enemies)
			{
				for (int i = 0; i < spawnData.enemyCount; i++)
				{
					SpawnEnemy(spawnData, batchData.SpawningPattern);
					yield return new WaitForSeconds(batchData.SpawnInterval);
				}
			}
		}

		private void SpawnEnemy(BatchData.SpawnData spawnData, BatchData.SpawnPattern pattern)
		{
			Debug.Log("Spawn enemy");
			Vector2Int mapSpawnPosition = DeterminSpawnPosition(pattern);
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
		}

		private void ProcessEnemyDeath(EntityAI enemy)
		{
			enemyAlive--;
			spawnedEnemies.Remove(enemy);
			OnEnemyDeath?.Invoke(enemy);
			AddMoney(enemy.Data.killReward);
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

		#endregion
		#region Others

		private void AddMoney(int amount)
		{
			if (amount > 0)
			{
				money += amount;
				OnMoneyChanged?.Invoke(money);
			}
		}

		private bool SpendMoney(int amount)
		{
			if (amount > 0 && money >= amount)
			{
				money -= amount;
				OnMoneyChanged?.Invoke(money);
				return true;
			}
			return false;
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

		/// <summary>
		/// Advance the game state, between Playing and Building
		/// </summary>
		private void SetGameState(GameState state)
		{
			gameState = state;
			OnGameStateChange?.Invoke(gameState);
		}

		private void OnDrawGizmosSelected()
		{

		}

		#endregion

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
