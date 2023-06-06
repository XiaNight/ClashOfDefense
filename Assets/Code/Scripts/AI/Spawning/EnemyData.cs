using UnityEngine;
using UnityEngine.Events;

namespace ClashOfDefense.Game.Entity
{
	[CreateAssetMenu(fileName = "EnemyData", menuName = "ClashOfDefense/EnemyData")]
	public class EnemyData : ScriptableObject
	{
		public EntityAI entityPrefab;

		[Header("Movement")]
		public float speed;
		public float health;
		public Costs.Layer pathFindLayer;
	}
}