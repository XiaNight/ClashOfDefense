using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ClashOfDefense.Game.UI
{
	using Base;
	public class GameUI : MonoBehaviour
	{
		#region Singleton
		private static GameUI instance;
		public static GameUI Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<GameUI>();
				}
				return instance;
			}
		}
		#endregion
		#region Parameters

		[SerializeField] private Button battleButton;
		public event UnityAction onBattleButtonClick;

		#endregion

		private void Start()
		{
			battleButton.onClick.AddListener(OnBattleButtonClick);
			GameManager.Instance.OnGameStateChange += OnGameStateChange;
		}

		protected void OnBattleButtonClick()
		{
			onBattleButtonClick?.Invoke();
		}

		// determin if the mouse is over the UI
		public bool IsMouseOverUI()
		{
			return EventSystem.current.IsPointerOverGameObject();
		}

		private void OnGameStateChange(GameState gameState)
		{
			switch (gameState)
			{
				case GameState.Building:
					battleButton.interactable = true;
					break;
				case GameState.Playing:
					battleButton.interactable = false;
					break;
				case GameState.Ended:
					battleButton.interactable = false;
					break;
				default:
					break;
			}
		}
	}
}
