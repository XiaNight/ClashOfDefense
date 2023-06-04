using UnityEngine;

namespace ClashOfDefense.Game.PathFinding
{
	public class PathMapNode
	{
		// The position of the node
		private Vector2Int position;
		// The cost of the node, support 8 layers of cost
		private Costs cost;

		public PathMapNode(Vector2Int position)
		{
			// Set the position
			this.position = position;
		}

		public void SetCost(Costs costs)
		{
			// Set the cost array
			cost = costs;
		}

		public void SetCost(Costs.Layer layer, byte newCost)
		{
			// Set the cost of the layer
			cost[layer] = newCost;
		}

		public byte GetCost(Costs.Layer layer)
		{
			// Return the cost of the layer
			return cost[layer];
		}
	}
}