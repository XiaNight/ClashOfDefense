using UnityEngine;

namespace ClashOfDefense.Game.Structure
{
	[CreateAssetMenu(fileName = "BuildingData", menuName = "ClashOfDefense/BuildingData", order = 0)]

	public class BuildingData : ScriptableObject
	{
		[field: SerializeField] public string structureName { get; private set; }
		[field: SerializeField] public Sprite icon { get; private set; }
		[field: SerializeField] public Level[] levels { get; private set; }

		public bool regenerateHealth;

		[System.Serializable]
		public struct Level
		{
			public int maxHealth;
			public Building prefab;
			public int upgradeCostToNextLevel;
		}

		protected virtual void Reset()
		{
			regenerateHealth = true;
		}
	}
}