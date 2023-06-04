
namespace ClashOfDefense.Game
{
	[System.Serializable]
	public struct Costs
	{
		public byte[] costs;

		public byte Get(Layer type)
		{
			return costs[(int)type];
		}

		public void Set(Layer type, byte value)
		{
			costs[(int)type] = value;
		}

		public byte this[Layer type]
		{
			get => Get(type);
			set => Set(type, value);
		}

		public enum Layer
		{
			Ground = 0,
			Air = 1,
			Cover = 2,
			Rock = 3,
			Building = 4,
			Unit = 5,
			Projectile = 6,
			Trap = 7
		}
	}
}