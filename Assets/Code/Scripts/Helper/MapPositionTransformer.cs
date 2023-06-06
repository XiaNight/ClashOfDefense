using UnityEngine;
namespace ClashOfDefense.Game.Helper
{
	public static class MapPositionTransformer
	{
		public static Vector2Int WorldToMapPosition(Vector3 position, float tileSize)
		{
			return new Vector2Int(Mathf.FloorToInt(position.x / tileSize), Mathf.FloorToInt(position.z / tileSize));
		}

		public static Vector3 MapToWorldPosition(Vector2Int position, float tileSize)
		{
			return new Vector3(position.x * tileSize, 0, position.y * tileSize);
		}
	}

}