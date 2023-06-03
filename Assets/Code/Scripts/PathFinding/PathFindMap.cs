using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClashOfDefense.PathFinding
{
	public class PathFindMap : MonoBehaviour
	{
		// The size of the map
		public Vector2Int mapSize;
		// The grid of nodes
		public PathMapNode[,] grid;

		public void SetMapSize(Vector2Int size)
		{
			// Set the map size
			mapSize = size;
		}

		public void Setup()
		{
			// Create the grid
			grid = new PathMapNode[mapSize.x, mapSize.y];
			// Fill the grid with nodes
			for (int x = 0; x < mapSize.x; x++)
			{
				for (int y = 0; y < mapSize.y; y++)
				{
					// Create a new node
					PathMapNode newNode = new PathMapNode(new Vector2Int(x, y));
					// Add the node to the grid
					grid[x, y] = newNode;
				}
			}
		}

		#region A Star Path Finding

		// using the A star algorithm to find the path
		public Vector2Int[] PathFind(Vector2Int start, Vector2Int end, byte layer)
		{
			List<PathFindNode> openList = new List<PathFindNode>();
			List<PathFindNode> closedList = new List<PathFindNode>();

			// Create the start node
			CalculateHGCost(start, end, 0, layer, out (int, int) startCosts);
			PathFindNode startNode = new PathFindNode(start, startCosts.Item1, startCosts.Item2);

			// Add the start node to the open list
			openList.Add(startNode);

			// Loop until the open list is empty
			while (openList.Count > 0)
			{
				// Loop through the open list to find the node with the lowest f cost
				PathFindNode currentNode = openList[0];
				for (int i = 1; i < openList.Count; i++)
				{
					// Check if the current node has a lower f cost
					if (openList[i].fCost < currentNode.fCost)
					{
						// Set the current node to the node with the lowest f cost
						currentNode = openList[i];
					}
				}

				// Remove the current node from the open list
				openList.Remove(currentNode);
				// Add the current node to the closed list
				closedList.Add(currentNode);

				// Check if the current node is the end node
				if (currentNode.position == end)
				{
					// Create a list of nodes
					List<Vector2Int> path = new List<Vector2Int>();
					// Loop through the parents of the nodes
					while (currentNode.parent != null)
					{
						// Add the node to the path
						path.Add(currentNode.position);
						// Set the current node to the parent
						currentNode = currentNode.parent;
					}
					// Reverse the path
					path.Reverse();
					// Return the path
					return path.ToArray();
				}

				// Loop through the neighbours of the current node
				foreach (Vector2Int neighbourPosition in GetNeighbourPositions(currentNode.position))
				{
					// Check if the neighbour is in the closed list
					if (closedList.Find(node => node.position == neighbourPosition) != null)
					{
						// Skip the neighbour
						continue;
					}

					// Calculate the h and g cost
					bool passable = CalculateHGCost(neighbourPosition, end, currentNode.gCost, layer, out (int, int) neighbourCosts);

					// Create the neighbour node
					PathFindNode neighbourNode = new PathFindNode(neighbourPosition, neighbourCosts.Item1, neighbourCosts.Item2);

					if (passable)
					{
						// Set the parent of the neighbour node
						neighbourNode.parent = currentNode;
						// Check if the neighbour is in the open list
						if (openList.Find(node => node.position == neighbourPosition) != null)
						{
							// Skip the neighbour
							continue;
						}
						// Add the neighbour to the open list
						openList.Add(neighbourNode);
					}
					else
					{
						closedList.Add(neighbourNode);
					}
				}
			}

			Debug.LogWarning("No path found");

			// Return null if no path was found
			return null;
		}

		private Vector2Int[] GetNeighbourPositions(Vector2Int position)
		{
			// Create a list of neighbour positions
			List<Vector2Int> neighbourPositions = new List<Vector2Int>();
			// Loop through the neighbour positions
			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					// Check if the neighbour position is valid
					if (x == 0 && y == 0)
					{
						// Skip the self position
						continue;
					}

					// Calculate the neighbour position
					Vector2Int neighbourPosition = new Vector2Int(position.x + x, position.y + y);
					// Check if the neighbour position is in the grid
					if (neighbourPosition.x >= 0 && neighbourPosition.x < mapSize.x && neighbourPosition.y >= 0 && neighbourPosition.y < mapSize.y)
					{
						// Add the neighbour position to the list
						neighbourPositions.Add(neighbourPosition);
					}
				}
			}
			// Return the neighbour positions
			return neighbourPositions.ToArray();
		}

		private bool CalculateHGCost(Vector2Int start, Vector2Int end, int parentGCost, byte layer, out (int, int) costs)
		{
			// Calculate the h cost
			int hCost = CalculateHCost(start, end);
			// Calculate the g cost
			bool passable = CalculateGCost(start, end, parentGCost, layer, out int gCost);
			costs = (hCost, gCost);
			// Return the h and g cost
			return passable;
		}

		private int CalculateHCost(Vector2Int start, Vector2Int end)
		{
			// Calculate the h cost
			int hCost = Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);
			// Return the h cost
			return hCost;
		}

		private bool CalculateGCost(Vector2Int start, Vector2Int end, int parentGCost, byte layer, out int gCost)
		{
			// Calculate the g cost
			byte nodeCost = GetNodeCost(start, layer);
			gCost = parentGCost + nodeCost;
			// Return the g cost
			return nodeCost < Constants.IMPASSABLE_COST;
		}

		#endregion

		private byte GetNodeCost(Vector2Int position, byte layer)
		{
			// Return the cost of the node
			return grid[position.x, position.y].GetCost(layer);
		}

		public void SetNodeCost(Vector2Int position, byte[] cost)
		{
			// Set the cost of the node
			grid[position.x, position.y].SetCost(cost);
		}

		public void SetNodeCost(Vector2Int position, byte layer, byte cost)
		{
			// Set the cost of the node
			grid[position.x, position.y].SetCost(layer, cost);
		}

		private void OnDrawGizmosSelected()
		{
			// Check if the grid is not null
			if (grid != null)
			{
				// Loop through the nodes in the grid
				for (int x = 0; x < mapSize.x; x++)
				{
					for (int y = 0; y < mapSize.y; y++)
					{
						// Check if the node is walkable
						float cost = grid[x, y].GetCost(0) / 255f;
						Gizmos.color = new Color(cost, 1 - cost, 0);

						// Draw a cube at the position of the node
						Gizmos.DrawCube(new Vector3(x, 0, y), Vector3.one * 0.1f);
					}
				}
			}
		}
	}
}