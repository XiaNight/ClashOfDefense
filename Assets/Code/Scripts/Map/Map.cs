using UnityEngine;
using UnityEngine.Events;

namespace ClashOfDefense.Game.Environment
{
	using System.Collections;
	using UnityEngine;

	public class Map : MonoBehaviour
	{
		[SerializeField] private float generationFrequency = 0.1f;
		[SerializeField] private MapNode mapNodePrefab;
		[SerializeField] private Transform mapNodeParent;
		[SerializeField] private NodeTypeData[] nodeTypeData;
		[SerializeField] private NodeTypeData fallbackNodeTypeData;
		private Vector2Int mapSize;
		private float tileSize;
		private MapNode[,] mapNodes;
		private int startupSeed = 0;
		private Coroutine generationCoroutine;
		private Coroutine querryNodeCostCoroutine;

		public void Setup(Vector2Int mapSize, int seed, float tileSize)
		{
			this.mapSize = mapSize;
			startupSeed = seed;
			this.tileSize = tileSize;
		}

		public void Clear()
		{
			if (mapNodes != null)
			{
				for (int x = 0; x < mapSize.x; x++)
				{
					for (int y = 0; y < mapSize.y; y++)
					{
						Destroy(mapNodes[x, y].gameObject);
					}
				}
			}
		}

		public void QuerryNodeCost(UnityAction<Vector2Int, Costs> nodeCostSetter, UnityAction callback)
		{
			if (querryNodeCostCoroutine != null)
			{
				StopCoroutine(querryNodeCostCoroutine);
			}
			StartCoroutine(QuerryNodeCostEnumerator(nodeCostSetter, callback));
		}

		private IEnumerator QuerryNodeCostEnumerator(UnityAction<Vector2Int, Costs> nodeCostSetter, UnityAction callback)
		{
			int maxCounter = 1;
			int counter = 1;

			for (int x = 0; x < mapSize.x; x++)
			{
				for (int y = 0; y < mapSize.y; y++)
				{
					Vector2Int position = new Vector2Int(x, y);
					nodeCostSetter(position, GetCost(position));

					if (--counter <= 0)
					{
						if (Time.deltaTime < 0.05f)
						{
							maxCounter++;
						}
						counter = maxCounter;
						yield return null;
					}
				}
			}
			callback?.Invoke();
		}

		public Costs GetCost(Vector2Int position)
		{
			return mapNodes[position.x, position.y].Costs;
		}

		public void GenerateMap(UnityAction callback)
		{
			if (generationCoroutine != null)
			{
				StopCoroutine(generationCoroutine);
			}
			generationCoroutine = StartCoroutine(GenerateMapCoroutine(callback));
		}

		private IEnumerator GenerateMapCoroutine(UnityAction callback)
		{
			mapNodes = new MapNode[mapSize.x, mapSize.y];
			int maxCounter = 1;
			int counter = 1;
			for (int x = 0; x < mapSize.x; x++)
			{
				for (int y = 0; y < mapSize.y; y++)
				{
					float treePercentage = PerlinNoiseWithOctave(x, y, 4, 0.5f, 1);
					treePercentage -= 0.5f; treePercentage *= 2; treePercentage = Mathf.Max(0.01f, treePercentage);

					float rockPercentage = PerlinNoiseWithOctave(x, y, 4, 0.5f, 0);
					rockPercentage -= 0.5f; rockPercentage *= 2; rockPercentage = Mathf.Max(0.01f, rockPercentage);

					float waterPercentage = PerlinNoiseWithOctave(x, y, 4, 0.5f, 2);
					waterPercentage -= 0.5f; waterPercentage *= 2; waterPercentage = Mathf.Max(0.01f, waterPercentage);

					NodeTypeData nodeTypeData = EvaluateNodeType(treePercentage, rockPercentage, waterPercentage);

					MapNode mapNode = Instantiate(nodeTypeData.nodePrefab, mapNodeParent);
					mapNodes[x, y] = mapNode;

					mapNode.SetProperties(new Vector2(x, y), tileSize, nodeTypeData.costs);
					if (--counter <= 0)
					{
						if (Time.deltaTime < 0.05f)
						{
							maxCounter++;
						}
						counter = maxCounter;
						yield return null;
					}
				}
			}
			callback?.Invoke();
		}

		private NodeTypeData EvaluateNodeType(float treePercentage, float rockPercentage, float waterPercentage)
		{
			foreach (NodeTypeData data in nodeTypeData)
			{
				if (data.Evaluate(treePercentage, rockPercentage, waterPercentage))
				{
					return data;
				}
			}
			return fallbackNodeTypeData;
		}

		private float PerlinNoiseWithOctave(float x, float y, int octaves, float persistence, int seed)
		{
			float total = 0;
			float frequency = generationFrequency;
			float amplitude = 0.5f;
			for (int i = 0; i < octaves; i++)
			{
				total += Mathf.PerlinNoise(x * frequency + seed * 1000 + startupSeed, y * frequency + startupSeed) * amplitude;
				amplitude *= persistence;
				frequency *= 2;
			}
			return total;
		}

		public Vector2Int WorldToMapPosition(Vector3 position)
		{
			return new Vector2Int(Mathf.FloorToInt(position.x / tileSize), Mathf.FloorToInt(position.z / tileSize));
		}

		public bool IsInMap(Vector2Int position)
		{
			return position.x >= 0 && position.x < mapSize.x && position.y >= 0 && position.y < mapSize.y;
		}
	}

	[System.Serializable]
	public enum MapNodeType
	{
		Plains,
		Grass,
		Rock,
		Tree,
		Water,
	}
}