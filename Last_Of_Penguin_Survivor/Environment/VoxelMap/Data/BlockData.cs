namespace Island
{
	// # Unity
	using UnityEngine;

	[System.Serializable]
	public class BlockData
	{
		public BlockData(BlockData blockData)
		{
			id = blockData.id;
			weight = blockData.weight;
			isSolid = blockData.isSolid;
			isDestroy = blockData.isDestroy;

			backFaceTexture = blockData.backFaceTexture;
			frontFaceTexture = blockData.frontFaceTexture;
			topFaceTexture = blockData.topFaceTexture;
			bottomFaceTexture = blockData.bottomFaceTexture;
			leftFaceTexture = blockData.leftFaceTexture;
			rightFaceTexture = blockData.rightFaceTexture;
			rotation = blockData.rotation;
			level = blockData.level;
			temperature = blockData.temperature;
		}


		[HideInInspector]
		public string id;
		public float weight;

		[Header("Bool Setting")]
		public bool isSolid;
		public bool isDestroy;

		[Header("Texture Values")]
		public string backFaceTexture;
		public string frontFaceTexture;
		public string topFaceTexture;
		public string bottomFaceTexture;
		public string leftFaceTexture;
		public string rightFaceTexture;
		public int rotation;

		public int level;
		public int moistureLevel = 0;

        [Header("Temperature")]
		public TemperatureType temperature = TemperatureType.Normal;

		public int GetBlockTexutreID(int id)
		{

            switch (id)
			{
				
                case 0:

                    return MapSettingManager.Instance.Map.FindTexture(backFaceTexture, level);
				case 1:
					return MapSettingManager.Instance.Map.FindTexture(frontFaceTexture, level);
				case 2:
					return MapSettingManager.Instance.Map.FindTexture(topFaceTexture, level);
				case 3:
					return MapSettingManager.Instance.Map.FindTexture(bottomFaceTexture, level);
				case 4:
					return MapSettingManager.Instance.Map.FindTexture(leftFaceTexture, level);
				case 5:
					return MapSettingManager.Instance.Map.FindTexture(rightFaceTexture, level);
				default:
					return 0;
			}
		}

		public void SetBlockTextureID(BlockSurfaceType blockSurfaceType, string id)
		{
			switch (blockSurfaceType)
			{
				case BlockSurfaceType.Front:
					frontFaceTexture = id;
					break;
				case BlockSurfaceType.Back:
					backFaceTexture = id;
					break;
				case BlockSurfaceType.Top:
					topFaceTexture = id;
					break;
				case BlockSurfaceType.Bottom:
					bottomFaceTexture = id;
					break;
				case BlockSurfaceType.Left:
					leftFaceTexture = id;
					break;
				case BlockSurfaceType.Right:
					rightFaceTexture = id;
					break;
				default:
					Debug.LogError("SetBlockTextureID Function Error");
					break;
			}
		}
	}
}