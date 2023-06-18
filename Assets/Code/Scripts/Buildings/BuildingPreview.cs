using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace ClashOfDefense.Game
{
	using Base;
	using Structure;
	public class BuildingPreview : MonoBehaviour
	{
		#region Singleton
		private static BuildingPreview instance;
		public static BuildingPreview Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<BuildingPreview>(true);
				}
				return instance;
			}
		}
		#endregion

		[SerializeField] private Button acceptButton;
		[SerializeField] private Button cancelButton;
		[SerializeField] private Building targetedBuilding;
		public event UnityAction OnAccept;
		public event UnityAction OnCancel;
		private void Awake()
		{
			GameManager.Instance.OnGameStateChange += (state) =>
			{
				if (state == GameState.Playing)
				{
					OnCancel?.Invoke();
				}
			};
			acceptButton.onClick.AddListener(() =>
			{
				OnAccept?.Invoke();
			});
			cancelButton.onClick.AddListener(() =>
			{
				OnCancel?.Invoke();
			});
			SetState(false);
		}

		public void Setup(Building building)
		{
			targetedBuilding = building;
			SetState(true);
			building.OnDestroyed += () =>
			{
				SetState(false);
			};
			UpdateData();
		}

		public void UpdateData()
		{
			transform.position = targetedBuilding.transform.position;
			SetAcceptable(GameManager.Instance.Money > targetedBuilding.Data.levels[0].cost);
		}

		public void SetAcceptable(bool state)
		{
			acceptButton.interactable = state;
		}

		public void SetState(bool state)
		{
			gameObject.SetActive(state);
			if (state)
			{

			}
			else
			{
				targetedBuilding = null;
			}
		}
	}
}