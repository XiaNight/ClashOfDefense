using UnityEngine;
using UnityEngine.Events;

namespace ClashOfDefense.Game.Entity
{
	[CreateAssetMenu(fileName = "RoundData", menuName = "ClashOfDefense/RoundData")]
	public class RoundData : ScriptableObject
	{
		public BatchData[] batches;
	}
}