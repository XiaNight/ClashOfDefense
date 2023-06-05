using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ClashOfDefense.Game.Environment
{
	public class MapNode : MonoBehaviour
	{
		[SerializeField] private Renderer terrainRenderer;
		[SerializeField] private Vector2 worldPosition;

		[SerializeField] private Costs costs;
		public Costs Costs { get => costs; }
		private float nodeSize;

		public void SetProperties(Vector2 position, float nodeSize, Costs costs)
		{
			// Set Properties()
			this.nodeSize = nodeSize;
			this.worldPosition = position;
			this.costs = costs;

			// set position
			transform.position = new Vector3(position.x, 0, position.y) * nodeSize;
			transform.localScale = Vector3.one * nodeSize;
		}

		public void SpawnObject(GameObject gameObject)
		{
			// Spawn object on this node with some random position offset and random rotation and scale
			Vector3 position = transform.position + new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f) * nodeSize;
			Quaternion rotation = Quaternion.Euler(0, Random.value * 360, 0);
			Vector3 scale = Vector3.one * Random.Range(0.5f, 1.5f);
			Instantiate(gameObject, position, rotation, transform).transform.localScale = scale;
		}

		public void Destroy()
		{
			// Destroy this node
			Destroy(gameObject);
		}

#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			// // draw costs as gizmos using handle
			// Gizmos.color = Color.red;
			// Handles.Label(transform.position, costs.ToString());
		}
#endif
	}
}