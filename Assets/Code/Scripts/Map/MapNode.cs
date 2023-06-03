using UnityEngine;

namespace ClashOfDefense.Game.Environment
{
	public class MapNode : MonoBehaviour
	{
		[SerializeField] private Renderer terrainRenderer;
		[SerializeField] private Vector2 worldPosition;
		[SerializeField] private float rockPercentage;
		[SerializeField] private float treePercentage;
		[SerializeField] private float waterPercentage;

		[SerializeField] private byte[] costs;
		public byte[] Costs { get => costs; }
		private float nodeSize;

		public void SetProperties(Vector2 position, float nodeSize, float rockPercentage, float treePercentage, float waterPercentage)
		{
			// Set Properties()
			this.nodeSize = nodeSize;
			this.worldPosition = position;
			this.rockPercentage = rockPercentage;
			this.treePercentage = treePercentage;
			this.waterPercentage = waterPercentage;

			CalculateCost();

			// set position
			transform.position = new Vector3(position.x, 0, position.y) * nodeSize;
			transform.localScale = Vector3.one * nodeSize;

			// Set renderer color using material property block
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			Vector3 color = new Vector3(rockPercentage, treePercentage, waterPercentage);
			materialPropertyBlock.SetColor("_BaseColor", new Color(color.x, color.y, color.z));
			terrainRenderer.SetPropertyBlock(materialPropertyBlock);
		}

		public void SpawnObject(GameObject gameObject)
		{
			// Spawn object on this node with some random position offset and random rotation and scale
			Vector3 position = transform.position + new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f) * nodeSize;
			Quaternion rotation = Quaternion.Euler(0, Random.value * 360, 0);
			Vector3 scale = Vector3.one * Random.Range(0.5f, 1.5f);
			Instantiate(gameObject, position, rotation, transform).transform.localScale = scale;
		}

		/// <summary>
		/// > 192 Immpassable
		/// </summary>
		private void CalculateCost()
		{
			costs = new byte[3];
			costs[0] = (byte)(rockPercentage * 255);
			costs[0] -= (byte)(costs[0] % 32);
			costs[1] = (byte)(treePercentage * 255);
			costs[1] -= (byte)(costs[1] % 32);
			costs[2] = (byte)(waterPercentage * 255);
			costs[2] -= (byte)(costs[2] % 32);
		}
	}
}