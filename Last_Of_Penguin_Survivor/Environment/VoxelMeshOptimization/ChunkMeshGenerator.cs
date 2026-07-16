namespace Island
{
	// # System
	using System;

	// # Unity
	using UnityEngine;

	public static class ChunkMeshGenerator
	{
		public static MeshData Generate(Chunk chunk)
		{
			MeshData meshData = new MeshData();

			for (int y = 0; y < ChunkConfig.ChunkHeightValue; y++)
			{
				for (int x = 0; x < ChunkConfig.ChunkWidthValue; x++)
				{
					for (int z = 0; z < ChunkConfig.ChunkLengthValue; z++)
					{
						if (ShouldRenderFace(chunk.chunkData.chunkBlocks[x, y, z], chunk.chunkData.type))
						{
							AddVoxelData(meshData, chunk, new Vector3(x, y, z));
						}
					}
				}
			}

			return meshData;
		}

		private static void AddVoxelData(MeshData meshData, Chunk chunk, Vector3 pos)
		{
			for (int face = 0; face < VoxelData.FaceCount; face++)
			{
				if (chunk.IsBlockSolid(pos + VoxelData.FaceChecks[face]))
					continue;

				for (int i = 0; i < VoxelData.VerticesCount; i++)
				{
					meshData.Vertices.Add(pos + VoxelData.VoxelVerts[VoxelData.VoxelTris[face, i]]);
				}

				BlockData block = chunk.chunkData.chunkBlocks[(int)pos.x, (int)pos.y, (int)pos.z];
				meshData.AddMeshData(block, face);
			}
		}

		private static Func<BlockData, ChunkType, bool> ShouldRenderFace = (blockData, chunkType)
			=> chunkType == ChunkType.Ground
			 ? blockData.isSolid
			 : blockData.id == BlockConstants.Water;

	}
}