using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace ClashOfDefense.Game.UI
{
	public class GameUI : MonoBehaviour
	{
		[SerializeField] private Button battleButton;

		public event UnityAction onBattleButtonClick;

		private void Start()
		{
			battleButton.onClick.AddListener(() => onBattleButtonClick?.Invoke());
		}
	}
}
