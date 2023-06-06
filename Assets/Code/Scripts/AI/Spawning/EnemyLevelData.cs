using UnityEngine;

namespace ClashOfDefense.Game.Entity
{
	[CreateAssetMenu(fileName = "EnemyLevelData", menuName = "ClashOfDefense/EnemyLevelData", order = 0)]
	public class EnemyLevelData : ScriptableObject
	{
		public RoundData[] rounds;
	}
}