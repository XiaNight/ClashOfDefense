using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClashOfDefense.Game
{
	using Environment;
	using PathFinding;
	public class GameManager : MonoBehaviour
	{
		[SerializeField] private Vector2Int mapSize;
		[SerializeField] private float tileSize;
		[SerializeField] private Map map;
		[SerializeField] private PathFindMap pathFindMap;
		[SerializeField] private Vector2Int[] pathFinded;
		private void Awake()
		{
			// use system time as seed
			Random.InitState(System.DateTime.Now.Millisecond);

			pathFindMap.SetMapSize(mapSize);
			pathFindMap.Setup();

			// set map size
			map.Setup(mapSize, Random.Range(0, 1000000), tileSize);
			map.GenerateMap();
			map.QuerryNodeCost((pos, costs) => { pathFindMap.SetNodeCost(pos, costs); });

			pathFinded = pathFindMap.PathFind(new Vector2Int(0, 0), mapSize - Vector2Int.one, 0);
		}

		private void OnDrawGizmosSelected()
		{
			if (pathFinded != null)
			{
				for (int i = 0; i < pathFinded.Length - 1; i++)
				{
					Gizmos.DrawLine(new Vector3(pathFinded[i].x, 0, pathFinded[i].y) * tileSize, new Vector3(pathFinded[i + 1].x, 0, pathFinded[i + 1].y) * tileSize);
				}
			}
		}
	}
}
