using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

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

		#region Header
		[Header("Header")]
		[SerializeField] private Button battleButton;
		public event UnityAction onBattleButtonClick;
		[SerializeField] private TMP_Text moneyLabel;
		[SerializeField] private TMP_Text enemyLeftLabel;

		#endregion

		[Header("Center")]
		[SerializeField] private TMP_Text defeatText;

		#endregion

		private void Awake()
		{
			GameManager.Instance.OnMoneyChanged += (money) =>
			{
				// use string format to display the money with a comma every 3 digits
				moneyLabel.text = string.Format("{0:#,0}", money);
			};
			GameManager.Instance.OnEnemyDeath += (enemy) =>
			{
				SetEnemyLeftLabel();
			};
			GameManager.Instance.OnGameStateChange += (state) =>
			{
				switch (state)
				{
					case GameState.Ended:
						defeatText.gameObject.SetActive(true);
						break;
					case GameState.Playing:
						SetEnemyLeftLabel();
						break;
				}
			};
		}

		private void Start()
		{
			battleButton.onClick.AddListener(OnBattleButtonClick);
			GameManager.Instance.OnGameStateChange += OnGameStateChange;
		}

		private void SetEnemyLeftLabel()
		{
			enemyLeftLabel.text = string.Format("{0:#,0}", GameManager.Instance.enemyAlive);
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
