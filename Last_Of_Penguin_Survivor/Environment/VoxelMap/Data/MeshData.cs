namespace Island
{
	// # System
	using System.Collections;
	using System.Collections.Generic;

	// # Unity
	using UnityEngine;

	public class MeshData
	{
		public List<Vector3> Vertices { get; private set; } = new List<Vector3>();
		public List<int> Triangles { get; private set; } = new List<int>();
		public List<Vector2> UVs { get; private set; } = new List<Vector2>();
		public int VertexIndex { get; private set; }

		///<summary>지정된 위치의 복셀 데이터를 메시에 추가합니다.</summary>
		public void AddMeshData(BlockData block, int faceIndex)
		{
			AddTexture(block.GetBlockTexutreID(faceIndex), block.rotation);

			// 복셀 면을 그리기 위한 삼각형 정보를 저장
			Triangles.Add(VertexIndex + 0);
			Triangles.Add(VertexIndex + 1);
			Triangles.Add(VertexIndex + 2);

			Triangles.Add(VertexIndex + 2);
			Triangles.Add(VertexIndex + 1);
			Triangles.Add(VertexIndex + 3);

			// 1면이 추가될 때마다 4개의 정점이 필요하기 때문에 4를 더함
			VertexIndex += 4;
		}

		///<summary>지정된 텍스처 ID와 회전을 기반으로 UV 좌표를 추가합니다.</summary>
		private void AddTexture(int textureID, int rotation = 0)
		{
			const float uvXBeginOffset = 0.003f;
			const float uvXEndOffset = 0.003f;
			const float uvYBeginOffset = 0.003f;
			const float uvYEndOffset = 0.003f;

			// textureID에 따라 텍스처의 위치를 계산합니다.
			float y = textureID / VoxelData.TextureAtlasSizeInBlocksX;
			float x = textureID % VoxelData.TextureAtlasSizeInBlocksX;

			x *= VoxelData.NormalizedBlockTextureSizeX;
			y *= VoxelData.NormalizedBlockTextureSizeY;

			y = 1f - y - VoxelData.NormalizedBlockTextureSizeY;

			// 텍스처의 4개의 꼭짓점에 대한 UV 좌표를 계산합니다.
			Vector2[] unprocessedUVs = new Vector2[]
			{
				new(x + uvXBeginOffset, y + uvYBeginOffset),
				new(x + uvXBeginOffset, y + VoxelData.NormalizedBlockTextureSizeY - uvYEndOffset),
				new(x + VoxelData.NormalizedBlockTextureSizeX - uvXEndOffset, y + uvYBeginOffset),
				new(x + VoxelData.NormalizedBlockTextureSizeX - uvXEndOffset, y + VoxelData.NormalizedBlockTextureSizeY - uvYEndOffset)
			};

			Vector2[] finalUVs = (Vector2[])unprocessedUVs.Clone();

			// 회전 값에 따라 UV 좌표를 회전시킵니다.
			switch (rotation)
			{
				case 1:
					finalUVs[0] = unprocessedUVs[1];
					finalUVs[1] = unprocessedUVs[3];
					finalUVs[2] = unprocessedUVs[0];
					finalUVs[3] = unprocessedUVs[2];
					break;
				case 2:
					finalUVs[0] = unprocessedUVs[3];
					finalUVs[1] = unprocessedUVs[2];
					finalUVs[2] = unprocessedUVs[1];
					finalUVs[3] = unprocessedUVs[0];
					break;
				case 3:
					finalUVs[0] = unprocessedUVs[2];
					finalUVs[1] = unprocessedUVs[0];
					finalUVs[2] = unprocessedUVs[3];
					finalUVs[3] = unprocessedUVs[1];
					break;
			}

			foreach (Vector2 uv in finalUVs)
			{
				UVs.Add(uv);
			}
		}
	}
}