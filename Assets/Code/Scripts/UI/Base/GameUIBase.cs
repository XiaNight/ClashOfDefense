using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ClashOfDefense.Game.UI
{
	public class GameUIBase : MonoBehaviour
	{
		// Singleton
		private static GameUIBase instance;
		public static GameUIBase Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<GameUIBase>();
				}
				return instance;
			}
		}

		public event UnityAction onBattleButtonClick;

		protected void OnBattleButtonClick()
		{
			onBattleButtonClick?.Invoke();
		}

		// determin if the mouse is over the UI
		public bool IsMouseOverUI()
		{
			return EventSystem.current.IsPointerOverGameObject();
		}
	}
}