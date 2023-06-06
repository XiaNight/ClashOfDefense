using UnityEngine;
using UnityEngine.Events;

namespace ClashOfDefense.Game.Entity
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "BatchData", menuName = "ClashOfDefense/BatchData")]
	public class BatchData : ScriptableObject
	{
		public SpawnData[] enemies;
		public float spawnInterval;
		public EnemyType enemyType;
		public SpawnPattern spawnPattern;

		[System.Serializable]
		public struct SpawnData
		{
			public EnemyData enemyData;
			public int count;
		}

		public enum EnemyType
		{
			Normal,
			Elite,
			Boss
		}
		public enum SpawnPattern
		{
			FromLeft,
			FromRight,
			FromTop,
			FromBottom,
			ArroundTheMap,
		}
	}
}