using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ClashOfDefense.Game.Base
{
	public enum GameState
	{
		Instantiating,
		Building,
		Playing,
		Paused,
		Ended,
	}

	public interface IHaveHealth
	{
		int Health { get; }
		int MaxHealth { get; }
		event UnityAction<int> OnHealthChanged;
	}
}