using UnityEngine;

namespace ClashOfDefense.Game.Structure
{
	[CreateAssetMenu(fileName = "BuildingDataCollection", menuName = "ClashOfDefense/BuildingDataCollection", order = 0)]
	public class BuildingDataCollection : ScriptableObject
	{
		[field: SerializeField] public BuildingData[] buildingDatas { get; private set; }
	}
}