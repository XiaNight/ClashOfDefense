using UnityEngine;
namespace ClashOfDefense.Game.PathFinding
{
	public class PathFindNode
	{
		public Vector2Int position;
		public int hCost;
		public int gCost;
		public int fCost;
		public PathFindNode parent;

		public PathFindNode(Vector2Int position, int h, int g)
		{
			this.position = position;
			hCost = h;
			gCost = g;
			fCost = h + g;
		}
	}
}