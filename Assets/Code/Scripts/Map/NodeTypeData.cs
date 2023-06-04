using UnityEngine;

namespace ClashOfDefense.Game.Environment
{
	[System.Serializable]
	[CreateAssetMenu(fileName = "NodeTypeData", menuName = "ClashOfDefense/NodeTypeRestrictions")]
	public class NodeTypeData : ScriptableObject
	{
		public MapNodeType type;
		public Limitation treeLimits;
		public Limitation rockLimits;
		public Limitation waterLimits;
		public MapNode nodePrefab;
		public SpawnableObjects[] spawnableObjects;
		public Costs costs;

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

		private void Reset()
		{
			treeLimits = new Limitation()
			{
				min = 0,
				max = 1
			};
			rockLimits = new Limitation()
			{
				min = 0,
				max = 1
			};
			waterLimits = new Limitation()
			{
				min = 0,
				max = 1
			};
			costs = new Costs()
			{
				costs = new byte[8] { 10, 10, 10, 10, 10, 10, 10, 10 }
			};
		}
	}
}