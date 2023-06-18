using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ClashOfDefense.Game.UI
{
	using Structure;
	public class BuildingSelectionManager : MonoBehaviour
	{
		// Singleton
		private static BuildingSelectionManager instance;
		public static BuildingSelectionManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<BuildingSelectionManager>();
				}
				return instance;
			}
		}

		[SerializeField] private Transform content;
		[SerializeField] private BuildingDataCollection buildingDataCollection;
		[SerializeField] private BuildingSelectionContentEntry contentEntryPrefab;
		[field: SerializeField] public BuildingData selectedBuildingData { get; private set; }
		public event UnityAction<BuildingData> OnBuildingSelected;

		private List<BuildingSelectionContentEntry> spawnedEntries = new List<BuildingSelectionContentEntry>();

		private void Start()
		{
			// clear content
			foreach (Transform child in content)
			{
				Destroy(child.gameObject);
			}

			// populate content
			foreach (BuildingData buildingData in buildingDataCollection.buildingDatas)
			{
				BuildingSelectionContentEntry contentEntry = Instantiate(contentEntryPrefab, content);
				contentEntry.Setup(buildingData.icon, buildingData.structureName);
				contentEntry.OnClick += () =>
				{
					selectedBuildingData = buildingData;
					OnBuildingSelected?.Invoke(buildingData);
					spawnedEntries.ForEach((entry) => entry.Disable());
				};
				spawnedEntries.Add(contentEntry);
			}
		}
	}
}