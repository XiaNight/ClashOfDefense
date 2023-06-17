using UnityEngine;
using UnityEngine.Events;

namespace ClashOfDefense.Game.Entity
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "BatchData", menuName = "ClashOfDefense/BatchData")]
	public class BatchData : ScriptableObject
	{
		[SerializeField] private SpawnData[] enemies;
		public SpawnData[] Enemies { get { return enemies; } }
		[SerializeField] private float spawnInterval;
		public float SpawnInterval { get { return spawnInterval; } }
		[SerializeField] private EnemyType enemyType;
		public EnemyType Type { get { return enemyType; } }
		[SerializeField] private SpawnPattern spawnPattern;
		public SpawnPattern SpawningPattern { get { return spawnPattern; } }

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