using UnityEngine;
namespace ClashOfDefense.Game.Util
{
	public class ObjectPool : MonoBehaviour
	{
		[SerializeField] private GameObject prefab;
		[SerializeField] private int poolSize;
		private GameObject[] pool;
		private int index = 0;

		private void Awake()
		{
			pool = new GameObject[poolSize];
			for (int i = 0; i < poolSize; ++i)
			{
				pool[i] = Instantiate(prefab, transform);
				pool[i].SetActive(false);
			}
		}

		public GameObject GetObject()
		{
			GameObject obj = pool[index];
			obj.SetActive(true);
			index = (index + 1) % poolSize;
			return obj;
		}

		public void ReturnObject(GameObject obj)
		{
			obj.SetActive(false);
		}
	}
}