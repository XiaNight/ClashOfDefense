using UnityEngine;

namespace ClashOfDefense.PathFinding
{
	public class PathMapNode
	{
		// The position of the node
		private Vector2Int position;
		// The cost of the node, support 8 layers of cost
		private byte[] cost;

		public PathMapNode(Vector2Int position)
		{
			// Set the position
			this.position = position;
			// Create the cost array
			cost = new byte[8];
			// Set the cost to 1
			for (int i = 0; i < cost.Length; i++)
			{
				cost[i] = 1;
			}
		}

		public void SetCost(byte[] costs)
		{
			// Set the cost array
			cost = costs;
		}

		public void SetCost(byte layer, byte newCost)
		{
			// Set the cost of the layer
			cost[layer] = newCost;
		}

		public byte GetCost(byte layer)
		{
			// Return the cost of the layer
			return cost[layer];
		}
	}
}