using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClashOfDefense.Game
{
	using Environment;
	using PathFinding;
	using Game.Control;

	public class GameManager : MonoBehaviour
	{
		[SerializeField] private Vector2Int mapSize;
		[SerializeField] private float tileSize;
		[SerializeField] private Map map;
		[SerializeField] private PathFindMap pathFindMap;
		[SerializeField] private CameraControl cameraControl;
		[SerializeField] private Vector2Int[] pathFinded;

		private Vector2Int start;
		private Vector2Int end;

		private void Awake()
		{
			// use system time as seed
			Random.InitState(System.DateTime.Now.Millisecond);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				GenerateNewMap();
			}
			if (Input.GetMouseButtonDown(0))
			{
				Vector2Int position = map.WorldToMapPosition(MouseRayPosition());
				if (map.IsInMap(position))
				{
					start = position;
					PathFind();
				}
			}
			if (Input.GetMouseButtonDown(1))
			{
				Vector2Int position = map.WorldToMapPosition(MouseRayPosition());
				if (map.IsInMap(position))
				{
					end = position;
					PathFind();
				}
			}
		}

		private Vector3 MouseRayPosition()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000))
			{
				return hit.point;
			}
			return -Vector3.one;
		}

		private void GenerateNewMap()
		{
			pathFindMap.Clear();
			map.Clear();

			pathFindMap.SetMapSize(mapSize);
			pathFindMap.Setup();

			// set map size
			map.Setup(mapSize, Random.Range(0, 1000000), tileSize);
			map.GenerateMap(() =>
			{
				map.QuerryNodeCost((pos, costs) => { pathFindMap.SetNodeCost(pos, costs); }, () =>
				{
					// set start and end
					start = new Vector2Int(0, 0);
					end = mapSize - Vector2Int.one;
					PathFind();
				});
			});

			cameraControl.SetBounds(new Vector3(mapSize.x, 100, mapSize.y) * tileSize);
		}

		private void PathFind()
		{
			// find the path
			pathFinded = pathFindMap.PathFind(start, end, Costs.Layer.Ground);
		}

		private void OnDrawGizmosSelected()
		{
			if (pathFinded != null)
			{
				Gizmos.color = Color.red;
				for (int i = 0; i < pathFinded.Length - 1; i++)
				{
					Gizmos.DrawLine(new Vector3(pathFinded[i].x, 0, pathFinded[i].y) * tileSize, new Vector3(pathFinded[i + 1].x, 0, pathFinded[i + 1].y) * tileSize);
				}
			}
		}
	}
}
