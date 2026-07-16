namespace Island
{
	using UnityEngine;

	public class ChunkData
	{
		public Vector2Int coord { get; private set; }
		public ChunkType type { get; private set; }
		public BlockData[,,] chunkBlocks { get; private set; }
		public int[,] blockHeights { get; private set; }

		public ChunkData(Vector2Int coord, ChunkType type)
		{
			this.coord = coord;
			this.type = type;

			chunkBlocks = new BlockData[ChunkConfig.ChunkWidthValue, ChunkConfig.ChunkHeightValue, ChunkConfig.ChunkLengthValue];
			blockHeights = new int[ChunkConfig.ChunkWidthValue, ChunkConfig.ChunkLengthValue];
		}
	}
}