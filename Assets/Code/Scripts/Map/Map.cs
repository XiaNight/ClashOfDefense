using UnityEngine;
using UnityEngine.Events;

namespace ClashOfDefense.Game.Environment
{
	using UnityEngine;

	public class Map : MonoBehaviour
	{
		[SerializeField] private float generationFrequency = 0.1f;
		[SerializeField] private MapNode mapNodePrefab;
		[SerializeField] private Transform mapNodeParent;
		[SerializeField] private NodeTypeRestrictions[] nodeTypeRestrictions;
		private Vector2Int mapSize;
		private float tileSize;
		private MapNode[,] mapNodes;
		private int startupSeed = 0;

		public void Setup(Vector2Int mapSize, int seed, float tileSize)
		{
			this.mapSize = mapSize;
			startupSeed = seed;
			this.tileSize = tileSize;
		}

		public void QuerryNodeCost(UnityAction<Vector2Int, byte[]> callback)
		{
			for (int x = 0; x < mapSize.x; x++)
			{
				for (int y = 0; y < mapSize.y; y++)
				{
					Vector2Int position = new Vector2Int(x, y);
					callback(position, GetCost(position));
				}
			}
		}

		public byte[] GetCost(Vector2Int position)
		{
			return mapNodes[position.x, position.y].Costs;
		}

		public void GenerateMap()
		{
			mapNodes = new MapNode[mapSize.x, mapSize.y];
			for (int x = 0; x < mapSize.x; x++)
			{
				for (int y = 0; y < mapSize.y; y++)
				{
					MapNode mapNode = Instantiate(mapNodePrefab, mapNodeParent);
					mapNode.SetProperties(new Vector2Int(x, y), tileSize, Random.value, Random.value, Random.value);
					mapNodes[x, y] = mapNode;

					float rockPercentage = PerlinNoiseWithOctave(x, y, 4, 0.5f, 0);
					rockPercentage -= 0.5f; rockPercentage *= 2; rockPercentage = Mathf.Max(0.01f, rockPercentage);

					float treePercentage = PerlinNoiseWithOctave(x, y, 4, 0.5f, 1);
					treePercentage -= 0.5f; treePercentage *= 2; treePercentage = Mathf.Max(0.01f, treePercentage);

					float waterPercentage = PerlinNoiseWithOctave(x, y, 4, 0.5f, 2);
					waterPercentage -= 0.5f; waterPercentage *= 2; waterPercentage = Mathf.Max(0.01f, waterPercentage);

					mapNode.SetProperties(new Vector2(x, y), tileSize, rockPercentage, treePercentage, waterPercentage);
				}
			}
		}

		private NodeTypeRestrictions EvaluateNodeType(float treePercentage, float rockPercentage, float waterPercentage)
		{
			NodeTypeRestrictions restrictions = null;
			foreach (NodeTypeRestrictions restriction in nodeTypeRestrictions)
			{
				if (restriction.Evaluate(treePercentage, rockPercentage, waterPercentage))
				{
					restrictions = restriction;
					break;
				}
			}
			return restrictions;
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

		[System.Serializable]
		[CreateAssetMenu(fileName = "NodeTypeRestrictions", menuName = "ClashOfDefense/Map/NodeTypeRestrictions")]
		private class NodeTypeRestrictions : ScriptableObject
		{
			public MapNodeType type;
			public Limitation treeLimits;
			public Limitation rockLimits;
			public Limitation waterLimits;
			public MapNode nodePrefab;
			public SpawnableObjects[] spawnableObjects;

			[System.Serializable]
			public struct SpawnableObjects
			{
				public GameObject prefab;
				public float spawnChance;
			}

			public bool Evaluate(float treePercentage, float rockPercentage, float waterPercentage)
			{
				return treeLimits.Evaluate(treePercentage) &&
					rockLimits.Evaluate(rockPercentage) &&
					waterLimits.Evaluate(waterPercentage);
			}

			[System.Serializable]
			public struct Limitation
			{
				public float min;
				public float max;
				public bool Evaluate(float value)
				{
					return value >= min && value <= max;
				}
			}
		}

		[System.Serializable]
		private enum MapNodeType
		{
			Plains,
			Grass,
			Rock,
			Tree,
			Water,
		}
	}
}