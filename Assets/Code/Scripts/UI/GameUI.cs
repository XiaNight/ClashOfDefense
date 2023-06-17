using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ClashOfDefense.Game.UI
{
	using Base;
	public class GameUI : GameUIBase
	{
		[SerializeField] private Button battleButton;

		private void Start()
		{
			battleButton.onClick.AddListener(OnBattleButtonClick);
			GameManager.Instance.OnGameStateChange += OnGameStateChange;
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
