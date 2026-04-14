namespace Island
{
    using System.Collections;
	// # System
	using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Unity.VisualScripting;
    using System.Diagnostics;

    // # Unity
    using UnityEngine;
	using UnityEngine.UIElements;
    using Debug = UnityEngine.Debug;

    // # Etc
    using static BlockConstants;

	public class Map
	{
		private MapSettingManager mapSettingManager;
		private List<Chunk> outerWaterChunks = new();

        private int mapWidthInBlocks;
        private int mapLengthInBlocks;
        private int mapHeightInBlocks;

        private List<DepthLayer> depthLayers = new List<DepthLayer>
		{
			new DepthLayer
			{
				minY = 1, maxY = 3, mineralRatio = 0.39f,
				minerals = new()
				{
					new ResourceProbability { blockId = GoldOre,   probability = 20 },
					new ResourceProbability { blockId = SilverOre, probability = 30 },
				},
				commons = new()
				{
					new ResourceProbability { blockId = Stone, probability = 50 },
				}
			},

			new DepthLayer
			{
				minY = 4, maxY = 7, mineralRatio = 0.15f,
				minerals = new()
				{
					new ResourceProbability { blockId = GoldOre,   probability = 14 },
					new ResourceProbability { blockId = SilverOre, probability = 25 },
				},
				commons = new()
				{
					new ResourceProbability { blockId = Stone, probability = 61 },
				}
			}
		};

        private Dictionary<Vector2Int, Chunk> groundChunks = new Dictionary<Vector2Int, Chunk>();
        private Dictionary<Vector2Int, Chunk> waterChunks = new Dictionary<Vector2Int, Chunk>();

        private int activationRange = 5;

        Stopwatch watch = new Stopwatch();

        public Map(MapSettingManager mapSettingManager)
		{
			this.mapSettingManager = mapSettingManager;
		}

        public void Initialize()
        {
			mapWidthInBlocks =  mapSettingManager.ChunkCountX * ChunkConfig.ChunkWidthValue;
            mapLengthInBlocks = mapSettingManager.ChunkCountY * ChunkConfig.ChunkLengthValue;
            mapHeightInBlocks = ChunkConfig.ChunkHeightValue;

            GenerateMap();
            ApplyTextureRules();

			for (int x = 0; x < mapSettingManager.ChunkCountX; x++)
            {
                for (int z = 0; z < mapSettingManager.ChunkCountY; z++)
                {
                    DrawMap(x, z);
                }
            }
		}


        /// <summary> 주어진 청크 좌표에서 해당하는 청크를 업데이트합니다. </summary>
        public void DrawMap(int x, int z)
		{
           GetChunkFromChunkCoord(x, z, ChunkType.Ground).UpdateChunk();
           GetChunkFromChunkCoord(x, z, ChunkType.Water).UpdateChunk();
		}

		/// <summary> 맵을 생성하고, 청크를 생성하여 표시합니다. </summary>
		public void GenerateMap()
		{
			for (int x = 0; x < mapSettingManager.ChunkCountX; x++)
			{
				for (int z = 0; z < mapSettingManager.ChunkCountY; z++)
				{
					GenerateChunk(x, z);
				}
			}

			//GenerateOuterWaterChunks();
			//DrawOuterWaterChunks();
        }

        /// <summary> 주어진 좌표에서 청크를 생성합니다. </summary>
        public void GenerateChunk(int x, int z)
		{
            Vector2Int coord = new Vector2Int(x, z);

            Chunk groundTempChunk =  new Chunk(coord, this, mapSettingManager, ChunkType.Ground, false);
            Chunk waterTempChunk  =  new Chunk(coord, this, mapSettingManager, ChunkType.Water, false);

            groundChunks.Add(coord, groundTempChunk);
            waterChunks.Add(coord, waterTempChunk);

            // 광물 클러스터 생성
            GenerateResourceClusters(groundTempChunk);

        }

		private void DrawOuterWaterChunks()
		{
			foreach (var chunk in outerWaterChunks)
			{
				chunk.UpdateChunk();
			}
		}

		private void GenerateOuterWaterChunks()
		{
			int countX = mapSettingManager.ChunkCountX;
			int countY = mapSettingManager.ChunkCountY;

			for (int x = -1; x <= countX; x++)
			{
				for (int z = -1; z <= countY; z++)
				{
					bool isOuterEdge =
						x == -1 || x == countX ||
						z == -1 || z == countY;

					if (isOuterEdge)
					{
						var chunk = new Chunk(new Vector2Int(x, z), this, mapSettingManager, ChunkType.Water, true);
						outerWaterChunks.Add(chunk);
					}
				}
			}
		}

		/// <summary> 주어진 위치의 청크를 업데이트합니다. </summary>
		public void UpdateChunk(Vector3 pos)
		{
			if (!IsVoxelInMap(pos))
			{
				Debug.Log("청크가 존재하지 않아 UpdateChunk() 실행 불가");
				return;
			}

			GetChunkFromPosition(pos, ChunkType.Ground).UpdateChunk();
		}

		public IEnumerator AroundUpdateChunk(Vector2 pos)
		{
            watch.Reset();
            watch.Start();

            int chunkX = Mathf.FloorToInt(pos.x);
            int chunkZ = Mathf.FloorToInt(pos.y);

            HashSet<Vector2Int> activeCoords = new HashSet<Vector2Int>();
            for (int x = chunkX - activationRange; x <= chunkX + activationRange; x++)
			{
				for (int z = chunkZ - activationRange; z <= chunkZ + activationRange; z++)
				{
                    Vector2Int coord = new Vector2Int(x, z);

                    activeCoords.Add(coord);
                    if (IsChunkInMap(x,z))
					{
                        SetChunkActive(coord, true);
                    }
					else
					{
                        GenerateChunk(x, z);
                        DrawMap(x, z);

                        yield return null;
                    }
                    Chunk chunk = GetChunkFromChunkCoord(x, z, ChunkType.Ground);
                    if(chunk.isDayTreeSpawn == true)
                    {
                        TreeManager.Instance.SpawnTree(coord);
                        chunk.isDayTreeSpawn = false;
                    }
                }
            }
            watch.Stop();
            RemoveChunks(activeCoords);
        }

        /// <summary> 스폰 위치를 초기화합니다. </summary>
        public void InitializeSpawnPosition()
		{
			Vector3 spawnPosition = new Vector3(
					ChunkConfig.ChunkWidthValue * mapSettingManager.ChunkCountX * 0.5f,
					ChunkConfig.ChunkHeightValue,
					ChunkConfig.ChunkLengthValue * mapSettingManager.ChunkCountY * 0.5f
			);
		}

		#region 높이 데이터, 블럭 데이터 설정 
		/// <summary> 주어진 위치에서 블록 높이를 계산하여 반환합니다. </summary>
		public int CalculateBlockHeight(Vector3 pos)
		{
			float baseHeight = PerlinNoise.GetHeightFromNoise(
				new Vector2(pos.x, pos.z),
				mapSettingManager.HeightNoiseScale,
				mapSettingManager.MaxStoneHeight + 1,
				mapSettingManager.Seed
			);

            Vector2 mapCenter = new Vector2(ChunkConfig.ChunkWidthValue / 2f, ChunkConfig.ChunkLengthValue/ 2f);

            float distFromCenter = Vector2.Distance(new Vector2(pos.x, pos.z), mapCenter);

            float t = 1f - Mathf.Clamp01(distFromCenter / mapSettingManager.FalloffRadius);
            float falloffFactor = Mathf.SmoothStep(0.2f, 1f, t);

            float adjustedHeight = baseHeight - (1f - falloffFactor) * mapSettingManager.MaxFalloffHeight;

            adjustedHeight = Mathf.Max(adjustedHeight, mapSettingManager.MinHeightLimit);
            return Mathf.FloorToInt(adjustedHeight);
		}

		/// <summary> 주어진 위치와 블록 높이에 따라 해당 블록의 데이터를 계산하여 반환합니다. </summary>
		public BlockData CalculateBlockData(Vector3 pos, int blockHeight)
		{
			if (pos.y < 1)
				return FindBlockType(Bedrock);

			else if (pos.y < blockHeight && blockHeight <= mapSettingManager.MaxWaterHeight)
				return FindBlockType(Water);

			else if (pos.y < blockHeight && pos.y < mapSettingManager.MaxStoneHeight)
				return FindBlockType(Stone);

			else if (pos.y < blockHeight)
				return GetBlockTypeWithNoise(new Vector2(pos.x, pos.z));

			else return FindBlockType(Air);
        }
		#endregion

		#region 광물 블럭 설정 
		private void PlaceResourceCluster(Chunk chunk, Vector3Int origin, int size)
		{
			for (int i = 0; i < size; i++)
			{
				Vector3Int offset = new Vector3Int(
					Random.Range(-1, 2),
					Random.Range(-1, 2),
					Random.Range(-1, 2)
				);

				Vector3Int target = origin + offset;

				if (!IsInChunkBounds(target)) continue;

				BlockData currentBlock = chunk.chunkData.chunkBlocks[target.x, target.y, target.z];
				if (currentBlock.id == Stone)
				{
					string blockId = GetResourceBlockIdByHeight(target.y);
					chunk.chunkData.chunkBlocks[target.x, target.y, target.z] = mapSettingManager.Map.FindBlockType(blockId);
				}
			}
		}

		private string GetResourceBlockIdByHeight(int y)
		{
			DepthLayer layer = depthLayers.FirstOrDefault(dl => y >= dl.minY && y <= dl.maxY);
			if (layer == null) return Stone;

			float roll = Random.value;
			List<ResourceProbability> table = roll < layer.mineralRatio ? layer.minerals : layer.commons;

			float totalWeight = table.Sum(r => r.probability);
			float pick = Random.value * totalWeight;

			foreach (var res in table)
			{
				pick -= res.probability;
				if (pick <= 0f)
					return res.blockId;
			}

			return table.Last().blockId;
		}

		private void GenerateResourceClusters(Chunk chunk)
		{
			// 청크당 클러스터 수 (매우 많게)
			int clusterCount = Random.Range(12, 24);

			for (int i = 0; i < clusterCount; i++)
			{
				Vector3Int origin = new Vector3Int(
					Random.Range(0, ChunkConfig.ChunkWidthValue),
					Random.Range(1, 18),
					Random.Range(0, ChunkConfig.ChunkLengthValue)
				);

				// 클러스터 크기도 훨씬 크게
				PlaceResourceCluster(chunk, origin, Random.Range(24, 40));
			}
		}
		#endregion

		#region 청크 조회 및 월드 범위 검사 
		/// <summary> 주어진 위치와 청크 타입에 맞는 청크를 반환합니다. </summary>
		public Chunk GetChunkFromPosition(Vector3 pos, ChunkType chunkType)
		{
			int x = Mathf.FloorToInt(pos.x / ChunkConfig.ChunkWidthValue);
			int z = Mathf.FloorToInt(pos.z / ChunkConfig.ChunkLengthValue);

			if (IsChunkInMap(x, z))
			{
                if (chunkType == ChunkType.Ground && groundChunks.TryGetValue(new Vector2Int(x,z), out Chunk groundChunk))
                {
                    return groundChunk;
                }
                if (chunkType == ChunkType.Water && waterChunks.TryGetValue(new Vector2Int(x, z), out Chunk waterChunk))
                {
                    return waterChunk;
                }
            }

			if (chunkType == ChunkType.Water)
			{
				foreach (var chunk in outerWaterChunks)
				{
					if (chunk.Position.x == x * ChunkConfig.ChunkWidthValue &&
						chunk.Position.z == z * ChunkConfig.ChunkLengthValue)
					{
						return chunk;
					}
				}
			}
			return null;
		}

		/// <summary> 주어진 좌표와 청크 타입에 맞는 청크를 반환합니다. </summary>
		public Chunk GetChunkFromPosition(float x, float z, ChunkType chunkType)
		{
			int coordX = Mathf.FloorToInt(x / ChunkConfig.ChunkWidthValue);
			int coordZ = Mathf.FloorToInt(z / ChunkConfig.ChunkLengthValue);

            if (chunkType == ChunkType.Ground && groundChunks.TryGetValue(new Vector2Int(coordX,coordZ), out Chunk groundChunk))
            {
                return groundChunk;
            }
            if (chunkType == ChunkType.Water && waterChunks.TryGetValue(new Vector2Int(coordX, coordZ), out Chunk waterChunk))
            {
                return waterChunk;
            }

            return null;
		}

		/// <summary> 주어진 청크 좌표와 청크 타입에 맞는 청크를 반환합니다. </summary>	
		public Chunk GetChunkFromChunkCoord(int x, int z, ChunkType chunkType)
		{
            Vector2Int coord = new Vector2Int(x, z);
            if (chunkType == ChunkType.Ground && groundChunks.TryGetValue(coord, out Chunk groundChunk))
			{
                return groundChunk;
            }
            if (chunkType == ChunkType.Water && waterChunks.TryGetValue(coord, out Chunk waterChunk))
			{
                return waterChunk;
            }
#if UNITY_EDITOR
            Debug.Log($"{x} : {z} 좌표에 있는 청크가 없습니다");
#endif
            return null;
		}

		/// <summary> 주어진 위치에 맞는 블럭 데이터를 반환합니다. </summary>
		public BlockData GetBlockInChunk(Vector3 pos, ChunkType chunkType)
		{
            if (!IsVoxelInMap(pos)) return null;

			int x = Mathf.FloorToInt(pos.x / ChunkConfig.ChunkWidthValue);
			int z = Mathf.FloorToInt(pos.z / ChunkConfig.ChunkLengthValue);

			return GetChunkFromChunkCoord(x, z, chunkType).GetBlockData(pos);
		}

		/// <summary> 주어진 위치에서 청크 좌표를 계산하여 반환합니다. </summary>
		public Vector2 GetChunkCoordFromPosition(Vector3 pos)
		{
			int x = Mathf.FloorToInt(pos.x / ChunkConfig.ChunkWidthValue);
			int z = Mathf.FloorToInt(pos.z / ChunkConfig.ChunkLengthValue);

			return new Vector2(x, z);
		}

		/// <summary> 주어진 복셀 위치가 맵 내에 있는지 확인하여 반환합니다. </summary>
		public bool IsVoxelInMap(Vector3 pos)
		{
			int y = Mathf.FloorToInt(pos.y);

			if (y < 0 || y >= ChunkConfig.ChunkHeightValue)
				return false;
			else
				return true;
        }

        /// <summary> 주어진 청크가 맵 내에 있는지 확인하여 반환합니다. </summary>
        public bool IsChunkInMap(int x, int z)
        {
            return (groundChunks.ContainsKey(new Vector2Int(x, z)));
        }

        private bool IsInChunkBounds(Vector3Int pos)
		{
			return pos.x >= 0 && pos.x < ChunkConfig.ChunkWidthValue &&
				   pos.y >= 0 && pos.y < ChunkConfig.ChunkHeightValue &&
				   pos.z >= 0 && pos.z < ChunkConfig.ChunkLengthValue;
		}

        private void SetChunkActive(Vector2Int coord, bool isActive)
        {
            if (groundChunks.TryGetValue(coord, out Chunk groundChunk))
            {
                groundChunk.IsActive = isActive;
            }
            if (waterChunks.TryGetValue(coord, out Chunk waterChunk))
            {
                waterChunk.IsActive = isActive;
            }
        }

        private void RemoveChunks(HashSet<Vector2Int> activeCoords)
        {
            watch.Reset();
            watch.Start();

            List<Vector2Int> chunksToRemove = new List<Vector2Int>();

            foreach (var coord in groundChunks.Keys)
            {
                if (!activeCoords.Contains(coord)) chunksToRemove.Add(coord);
            }
            foreach (var coord in chunksToRemove)
            {
                SetChunkActive(coord, false);

                groundChunks.Remove(coord);
                waterChunks.Remove(coord);
            }
            watch.Stop();
        }

        #endregion

        #region 텍스쳐

        public BlockData GetBlockClamped(int x, int y, int z)
        {
            // Y 경계 확인
            if (y < 0 || y >= mapHeightInBlocks)
                return FindBlockType(BlockConstants.Air);

            int chunkX = Mathf.FloorToInt((float)x / ChunkConfig.ChunkWidthValue);
            int chunkZ = Mathf.FloorToInt((float)z / ChunkConfig.ChunkLengthValue);

            int localX = x - (chunkX * ChunkConfig.ChunkWidthValue);
            int localZ = z - (chunkZ * ChunkConfig.ChunkLengthValue);

            if (groundChunks.TryGetValue(new Vector2Int(chunkX, chunkZ), out Chunk groundChunk))
            {
                return groundChunk.chunkData.chunkBlocks[localX, y, localZ];
            }

            return FindBlockType(BlockConstants.Air);
        }


        public void UpdateBlockTopTexture(int x, int y, int z, string newTextureID, int rotation = 0)
        {
            int chunkX = Mathf.FloorToInt((float)x / ChunkConfig.ChunkWidthValue);
            int chunkZ = Mathf.FloorToInt((float)z / ChunkConfig.ChunkLengthValue);

            if (groundChunks.TryGetValue(new Vector2Int(chunkX, chunkZ), out Chunk chunk))
            {
                int localX = x - (chunkX * ChunkConfig.ChunkWidthValue);
                int localZ = z - (chunkZ * ChunkConfig.ChunkLengthValue);

                BlockData blockToUpdate = chunk.chunkData.chunkBlocks[localX, y, localZ];
                blockToUpdate.SetBlockTextureID(BlockSurfaceType.Top, newTextureID);
                blockToUpdate.rotation = rotation;

                chunk.UpdateChunk();
            }
        }

        /// <summary>
        /// 맵 전체를 순회하며 텍스처 규칙을 적용합니다. (사용자의 UpdateIceBlockTextureByRule)
        /// </summary>
        public void ApplyTextureRules()
        {
            for (int x = 0; x < mapWidthInBlocks; x++)
            {
                for (int z = 0; z < mapLengthInBlocks; z++)
                {
                    for (int y = 0; y < mapHeightInBlocks; y++)
                    {
                        UpdateGroundTexture(x, y, z);
                        UpdateIceTexture(x, y, z);
                    }
                }
            }
        }

        /// <summary>
        /// Ice 블록의 텍스처를 규칙에 맞게 업데이트합니다. (사용자의 UpdateIceTexture)
        /// </summary>
        public BlockData UpdateIceTexture(int x, int y, int z)
        {
            if (GetBlockClamped(x, y, z).id != "Ice") return null;

            int[] block = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

            if (GetBlockClamped(x - 1, y, z).id == "Ground") { block[0] = 1; } else if (GetBlockClamped(x - 1, y, z).id == "Snow") { block[0] = 2; }
            if (GetBlockClamped(x, y, z + 1).id == "Ground") { block[1] = 1; } else if (GetBlockClamped(x, y, z + 1).id == "Snow") { block[1] = 2; }
            if (GetBlockClamped(x + 1, y, z).id == "Ground") { block[2] = 1; } else if (GetBlockClamped(x + 1, y, z).id == "Snow") { block[2] = 2; }
            if (GetBlockClamped(x, y, z - 1).id == "Ground") { block[3] = 1; } else if (GetBlockClamped(x, y, z - 1).id == "Snow") { block[3] = 2; }
            if (GetBlockClamped(x - 1, y, z + 1).id == "Ground") { block[4] = 1; } else if (GetBlockClamped(x - 1, y, z + 1).id == "Snow") { block[4] = 2; }
            if (GetBlockClamped(x + 1, y, z + 1).id == "Ground") { block[5] = 1; } else if (GetBlockClamped(x + 1, y, z + 1).id == "Snow") { block[5] = 2; }
            if (GetBlockClamped(x + 1, y, z - 1).id == "Ground") { block[6] = 1; } else if (GetBlockClamped(x + 1, y, z - 1).id == "Snow") { block[6] = 2; }
            if (GetBlockClamped(x - 1, y, z - 1).id == "Ground") { block[7] = 1; } else if (GetBlockClamped(x - 1, y, z - 1).id == "Snow") { block[7] = 2; }

            if (block[0] == 1 && block[1] == 0 && block[2] == 0 && block[3] == 0) { UpdateBlockTopTexture(x, y, z, "IceDirtML"); return GetBlockClamped(x, y, z); }
            if (block[0] == 0 && block[1] == 1 && block[2] == 0 && block[3] == 0) { UpdateBlockTopTexture(x, y, z, "IceDirtUM"); return GetBlockClamped(x, y, z); }
            if (block[0] == 0 && block[1] == 0 && block[2] == 1 && block[3] == 0) { UpdateBlockTopTexture(x, y, z, "IceDirtMR"); return GetBlockClamped(x, y, z); }
            if (block[0] == 0 && block[1] == 0 && block[2] == 0 && block[3] == 1) { UpdateBlockTopTexture(x, y, z, "IceDirtBM"); return GetBlockClamped(x, y, z); }
            if (block[0] == 1 && block[1] == 1 && block[2] == 0 && block[3] == 0) { UpdateBlockTopTexture(x, y, z, "IceDirtUL"); return GetBlockClamped(x, y, z); }
            if (block[0] == 0 && block[1] == 1 && block[2] == 1 && block[3] == 0) { UpdateBlockTopTexture(x, y, z, "IceDirtUR"); return GetBlockClamped(x, y, z); }
            if (block[0] == 0 && block[1] == 0 && block[2] == 1 && block[3] == 1) { UpdateBlockTopTexture(x, y, z, "IceDirtBR"); return GetBlockClamped(x, y, z); }
            if (block[0] == 1 && block[1] == 0 && block[2] == 0 && block[3] == 1) { UpdateBlockTopTexture(x, y, z, "IceDirtBL"); return GetBlockClamped(x, y, z); }

            if (block[0] == 2 && block[1] == 0 && block[2] == 0 && block[3] == 0) { UpdateBlockTopTexture(x, y, z, "IceSnowML"); return GetBlockClamped(x, y, z); }
            if (block[0] == 0 && block[1] == 2 && block[2] == 0 && block[3] == 0) { UpdateBlockTopTexture(x, y, z, "IceSnowUM"); return GetBlockClamped(x, y, z); }
            if (block[0] == 0 && block[1] == 0 && block[2] == 2 && block[3] == 0) { UpdateBlockTopTexture(x, y, z, "IceSnowMR"); return GetBlockClamped(x, y, z); }
            if (block[0] == 0 && block[1] == 0 && block[2] == 0 && block[3] == 2) { UpdateBlockTopTexture(x, y, z, "IceSnowBM"); return GetBlockClamped(x, y, z); }
            if (block[0] == 2 && block[1] == 2 && block[2] == 0 && block[3] == 0) { UpdateBlockTopTexture(x, y, z, "IceSnowUL"); return GetBlockClamped(x, y, z); }
            if (block[0] == 0 && block[1] == 2 && block[2] == 2 && block[3] == 0) { UpdateBlockTopTexture(x, y, z, "IceSnowUR"); return GetBlockClamped(x, y, z); }
            if (block[0] == 0 && block[1] == 0 && block[2] == 2 && block[3] == 2) { UpdateBlockTopTexture(x, y, z, "IceSnowBR"); return GetBlockClamped(x, y, z); }
            if (block[0] == 2 && block[1] == 0 && block[2] == 0 && block[3] == 2) { UpdateBlockTopTexture(x, y, z, "IceSnowBL"); return GetBlockClamped(x, y, z); }

            if (block[4] == 1 && block[5] == 0 && block[6] == 0 && block[7] == 0) { UpdateBlockTopTexture(x, y, z, "IceDirtCorner", 2); return GetBlockClamped(x, y, z); }
            if (block[4] == 0 && block[5] == 1 && block[6] == 0 && block[7] == 0) { UpdateBlockTopTexture(x, y, z, "IceDirtCorner", 1); return GetBlockClamped(x, y, z); }
            if (block[4] == 0 && block[5] == 0 && block[6] == 1 && block[7] == 0) { UpdateBlockTopTexture(x, y, z, "IceDirtCorner", 0); return GetBlockClamped(x, y, z); }
            if (block[4] == 0 && block[5] == 0 && block[6] == 0 && block[7] == 1) { UpdateBlockTopTexture(x, y, z, "IceDirtCorner", 3); return GetBlockClamped(x, y, z); }

            if (block[4] == 2 && block[5] == 0 && block[6] == 0 && block[7] == 0) { UpdateBlockTopTexture(x, y, z, "IceSnowCorner", 2); return GetBlockClamped(x, y, z); }
            if (block[4] == 0 && block[5] == 2 && block[6] == 0 && block[7] == 0) { UpdateBlockTopTexture(x, y, z, "IceSnowCorner", 1); return GetBlockClamped(x, y, z); }
            if (block[4] == 0 && block[5] == 0 && block[6] == 2 && block[7] == 0) { UpdateBlockTopTexture(x, y, z, "IceSnowCorner", 0); return GetBlockClamped(x, y, z); }
            if (block[4] == 0 && block[5] == 0 && block[6] == 0 && block[7] == 2) { UpdateBlockTopTexture(x, y, z, "IceSnowCorner", 3); return GetBlockClamped(x, y, z); }

            if (block[0] == 1 && block[1] == 1 && block[2] == 0 && block[3] == 1) { UpdateBlockTopTexture(x, y, z, "IceDirt3", 0); return GetBlockClamped(x, y, z); }
            if (block[0] == 1 && block[1] == 1 && block[2] == 1 && block[3] == 0) { UpdateBlockTopTexture(x, y, z, "IceDirt3", 1); return GetBlockClamped(x, y, z); }
            if (block[0] == 0 && block[1] == 1 && block[2] == 1 && block[3] == 1) { UpdateBlockTopTexture(x, y, z, "IceDirt3", 2); return GetBlockClamped(x, y, z); }
            if (block[0] == 1 && block[1] == 0 && block[2] == 1 && block[3] == 1) { UpdateBlockTopTexture(x, y, z, "IceDirt3", 3); return GetBlockClamped(x, y, z); }

            if (block[0] == 2 && block[1] == 2 && block[2] == 0 && block[3] == 2) { UpdateBlockTopTexture(x, y, z, "IceSnow3", 0); return GetBlockClamped(x, y, z); }
            if (block[0] == 2 && block[1] == 2 && block[2] == 2 && block[3] == 0) { UpdateBlockTopTexture(x, y, z, "IceSnow3", 3); return GetBlockClamped(x, y, z); }
            if (block[0] == 0 && block[1] == 2 && block[2] == 2 && block[3] == 2) { UpdateBlockTopTexture(x, y, z, "IceSnow3", 2); return GetBlockClamped(x, y, z); }
            if (block[0] == 2 && block[1] == 0 && block[2] == 2 && block[3] == 2) { UpdateBlockTopTexture(x, y, z, "IceSnow3", 1); return GetBlockClamped(x, y, z); }

            UpdateBlockTopTexture(x, y, z, "Ice");

            return null;
        }

        /// <summary>
        /// Ground 블록의 텍스처를 규칙에 맞게 업데이트합니다. (사용자의 UpdateGroundTexture)
        /// </summary>
        public BlockData UpdateGroundTexture(int x, int y, int z)
        {
            if (GetBlockClamped(x, y, z).id != "Ground") return null;
            bool[] block = new bool[8]; 

            if (GetBlockClamped(x - 1, y, z).id == "Snow") { block[0] = true; }
            if (GetBlockClamped(x, y, z + 1).id == "Snow") { block[1] = true; }
            if (GetBlockClamped(x + 1, y, z).id == "Snow") { block[2] = true; }
            if (GetBlockClamped(x, y, z - 1).id == "Snow") { block[3] = true; }
            if (GetBlockClamped(x - 1, y, z + 1).id == "Snow") { block[4] = true; }
            if (GetBlockClamped(x + 1, y, z + 1).id == "Snow") { block[5] = true; }
            if (GetBlockClamped(x + 1, y, z - 1).id == "Snow") { block[6] = true; }
            if (GetBlockClamped(x - 1, y, z - 1).id == "Snow") { block[7] = true; }

			if (block[0] && !block[1] && !block[2] && !block[3]) { UpdateBlockTopTexture(x, y, z, "GroundSnowML"); return GetBlockClamped(x, y, z); }
			if (!block[0] && block[1] && !block[2] && !block[3]) { UpdateBlockTopTexture(x, y, z, "GroundSnowUM"); return GetBlockClamped(x, y, z); }
			if (!block[0] && !block[1] && block[2] && !block[3]) { UpdateBlockTopTexture(x, y, z, "GroundSnowMR"); return GetBlockClamped(x, y, z); }
			if (!block[0] && !block[1] && !block[2] && block[3]) { UpdateBlockTopTexture(x, y, z, "GroundSnowBM"); return GetBlockClamped(x, y, z); }
			if (block[0] && block[1] && !block[2] && !block[3]) { UpdateBlockTopTexture(x, y, z, "GroundSnowUL"); return GetBlockClamped(x, y, z); }
			if (!block[0] && block[1] && block[2] && !block[3]) { UpdateBlockTopTexture(x, y, z, "GroundSnowUR"); return GetBlockClamped(x, y, z); }
			if (!block[0] && !block[1] && block[2] && block[3]) { UpdateBlockTopTexture(x, y, z, "GroundSnowBR"); return GetBlockClamped(x, y, z); }
			if (block[0] && !block[1] && !block[2] && block[3]) { UpdateBlockTopTexture(x, y, z, "GroundSnowBL"); return GetBlockClamped(x, y, z); }

			if (block[4] && !block[5] && !block[6] && !block[7]) { UpdateBlockTopTexture(x, y, z, "GroundSnowCorner", 2); return GetBlockClamped(x, y, z); }
			if (!block[4] && block[5] && !block[6] && !block[7]) { UpdateBlockTopTexture(x, y, z, "GroundSnowCorner", 1); return GetBlockClamped(x, y, z); }
			if (!block[4] && !block[5] && block[6] && !block[7]) { UpdateBlockTopTexture(x, y, z, "GroundSnowCorner", 0); return GetBlockClamped(x, y, z); }
			if (!block[4] && !block[5] && !block[6] && block[7]) { UpdateBlockTopTexture(x, y, z, "GroundSnowCorner", 3); return GetBlockClamped(x, y, z); }

			if (block[0] && block[1] && !block[2] && block[3]) { UpdateBlockTopTexture(x, y, z, "GroundSnow3", 0); return GetBlockClamped(x, y, z); }
			if (block[0] && block[1] && block[2] && !block[3]) { UpdateBlockTopTexture(x, y, z, "GroundSnow3", 3); return GetBlockClamped(x, y, z); }
			if (!block[0] && block[1] && block[2] && block[3]) { UpdateBlockTopTexture(x, y, z, "GroundSnow3", 2); return GetBlockClamped(x, y, z); }
			if (block[0] && !block[1] && block[2] && block[3]) { UpdateBlockTopTexture(x, y, z, "GroundSnow3", 1); return GetBlockClamped(x, y, z); }


			return null;
        }

		public void UpdateNeighborTextures(int centerX, int centerY, int centerZ)
		{
			for(int x = centerX - 1;x <= centerX +1;x++)
			{
				for(int z = centerZ - 1; z <= centerZ + 1;z++)
				{
					if(!IsVoxelInMap(new Vector3(x, centerY, z))) continue;

					UpdateGroundTexture(x, centerY, z);
                    UpdateIceTexture(x, centerY, z);
                }
			}
		}
        #endregion

        #region 투명벽

        /// <summary>
        /// 투명벽 생성
        /// </summary>
        public void BuildBoundaryColliders()
		{
           int width = mapSettingManager.ChunkCountX * ChunkConfig.ChunkWidthValue;
           int length = mapSettingManager.ChunkCountY * ChunkConfig.ChunkLengthValue;
           float height = ChunkConfig.ChunkHeightValue;

           float blockThick = 1f;

           // 왼쪽 벽
           CreateWall(new Vector3(-0.5f, height / 2f, length / 2f), new Vector3(blockThick, height, length));
           // 오른쪽 벽
           CreateWall(new Vector3(width - 0.5f, height / 2f, length / 2f), new Vector3(blockThick, height, length));
           // 아래쪽 벽
           CreateWall(new Vector3(width / 2f, height / 2f, -0.5f), new Vector3(width, height, blockThick));
           // 위쪽 벽
           CreateWall(new Vector3(width / 2f, height / 2f, length - 0.5f), new Vector3(width, height, blockThick));
		
		}

		/// <summary>
		/// 벽 생성
		/// </summary>
		/// <param name="center"></param>
		/// <param name="size"></param>
		private void CreateWall(Vector3 center, Vector3 size)
		{
			GameObject wall = new GameObject("Wall");
			wall.transform.SetParent(mapSettingManager.WallBorderChunkParent, false);
			wall.transform.localPosition = center;

			var col = wall.AddComponent<BoxCollider>();
			col.size = size;
		}

        #endregion


        /// <summary>
        /// 주어진 좌표에서 노이즈 값을 기반으로 블럭 데이터를 반환합니다.
        /// </summary>
        public BlockData GetBlockTypeWithNoise(Vector2 coord)
		{
			int x = Mathf.FloorToInt(coord.x);
			int z = Mathf.FloorToInt(coord.y);

			float maxAmplitude = mapSettingManager.BlockWeightConfig.Max(config => config.threshold) + 1;
			float noiseValue = PerlinNoise.GetBlockFromNoise(new Vector2(x, z), maxAmplitude, mapSettingManager.BlockTypeNoiseScale, mapSettingManager.Seed);

			foreach (var config in mapSettingManager.BlockWeightConfig)
			{
				if (noiseValue < config.threshold)
					return FindBlockType(config.id);
			}

			return FindBlockType(Snow);
		}

		/// <summary>
		/// 블럭 ID에 해당하는 블럭 데이터를 검색해 반환합니다.
		/// </summary>
		public BlockData FindBlockType(string blockID)
		{
			foreach (var blockDataInList in mapSettingManager.BlockDataList)
			{
				if (blockDataInList.id == blockID)
				{
					BlockData selecetedBlockData = new BlockData(GetBlockTextureWeightRandom(blockDataInList.blockDatas));
					return selecetedBlockData;
				}
			}
			return new BlockData(FindBlockType(Air));
		}

		/// <summary>
		/// 주어진 블럭 데이터 배열에서 가중치를 기반으로 랜덤한 블럭을 반환합니다.
		/// </summary>
		private BlockData GetBlockTextureWeightRandom(BlockData[] blockDatas)
		{
			if (blockDatas.Length == 0)
				return null;

			// 모든 가중치를 더하여 총합 구하기
			float totalWeight = 0.0f;
			foreach (BlockData blockData in blockDatas)
				totalWeight += blockData.weight;

			if (totalWeight == 0.0f)
				return blockDatas[0]; // 첫 번째 데이터를 반환

			// 0부터 총합 사이의 랜덤 값을 생성
			float randomValue = UnityEngine.Random.value * totalWeight;

			// 랜덤 값이 어느 범위에 속하는지 확인하여 데이터 선택
			foreach (var weightedBlockData in blockDatas)
			{
				randomValue -= weightedBlockData.weight;

				if (randomValue < 0.0f)
					return weightedBlockData;
			}

			// 해당되지 않은 경우 마지막 데이터 반환
			return blockDatas[blockDatas.Length - 1];
		}

		/// <summary>
		/// 주어진 블록 ID에 해당하는 텍스처를 찾고, 주어진 인덱스에 해당하는 텍스처를 반환합니다.
		/// </summary>
		public int FindTexture(string blockID, int index = 0)
		{
			foreach (var textureinlist in mapSettingManager.BlockTextureDataList)
			{
				if (textureinlist.id == blockID)
				{
					return textureinlist.blockTextures[Mathf.Min(index, textureinlist.blockTextures.Length - 1)];
				}
			}
			return 0;
		}


		public class ResourceProbability
		{
			public string blockId;
			public float probability;
		}

		public class DepthLayer
		{
			public int minY;
			public int maxY;
			public List<ResourceProbability> minerals = new();
			public List<ResourceProbability> commons = new();
			public float mineralRatio; // 0~1 사이 (예: 0.39)
		}

        public void Soil(Vector3 pos)
        {
            BlockData blockData = GetBlockInChunk(pos, ChunkType.Ground);
            if (!blockData.isDestroy) return;
            blockData.id = BlockConstants.TilledSoil;
            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);
            int z = Mathf.FloorToInt(pos.z);
            UpdateBlockTopTexture(x, y, z, "Soil");
            UpdateTiledSoil(x, y, z);
            Vector3Int globalPos = new Vector3Int(x, y, z);

            string blockID = GetBlockInChunk(pos, ChunkType.Ground).id;
            if (LOPNetworkManager.Instance.isConnected)
            {
                LOPNetworkManager.Instance.SendBlockUpdate(globalPos, blockID);
            }
        }

        public void WetSoil(Vector3 pos, int moisture)
        {
            BlockData blockData = GetBlockInChunk(pos, ChunkType.Ground);
            if (!blockData.isDestroy) return;
            if (blockData != null)
            {
				if (blockData.id == BlockConstants.TilledSoil || blockData.id == BlockConstants.Crops)
				{
                    InventoryManager.Instance.RemoveItem(new InventoryItem(QuickslotNumberBtn.Instance.selectedItem.item, 1));
                    if (blockData.moistureLevel < 5)
                    {
                        blockData.moistureLevel += moisture;
                        Debug.Log(blockData.moistureLevel);
                    }

                    if (blockData.moistureLevel > 0 && blockData.id != BlockConstants.WetTilledSoil)
                    {
                        blockData.id = BlockConstants.WetTilledSoil;

                        int x = Mathf.FloorToInt(pos.x);
                        int y = Mathf.FloorToInt(pos.y);
                        int z = Mathf.FloorToInt(pos.z);

                        Vector3Int globalPos = new Vector3Int(x, y, z);

                        UpdateBlockTopTexture(x, y, z, "WetSoil");
                        UpdateTiledSoil(x, y, z);
                        DrySoil(pos);

                        string blockID = GetBlockInChunk(pos, ChunkType.Ground).id;
                        if (LOPNetworkManager.Instance.isConnected)
                        {
                            LOPNetworkManager.Instance.SendBlockUpdate(globalPos, blockID);
                        }
                    }
                }
                
            }
        }

        public void UpdateTiledSoil(int x, int y, int z)
        {
            // # 경작된 흙 수 
            // - 기본적으로 한개를 깔고 이 함수를 실행하므로 개수는 1부터 시작
            int tiledSoilCount = 1;

            int firstTiledSoilX = x;
            int lastTiledSoilX = x;

            // 왼쪽 탐지
            CheckLeftTiledSoil(x,y, z, ref tiledSoilCount, ref firstTiledSoilX);

            // 오른쪽 탐지
            CheckRightTiledSoil(x,y, z, ref tiledSoilCount, ref lastTiledSoilX);

            UpdateTiledSoilData(tiledSoilCount, firstTiledSoilX, lastTiledSoilX, y, z);
        }

        private void UpdateTiledSoilData(int tiledSoilCount, int firstTiledSoilX, int lastTiledSoilX, int y, int z)
        {
            if (tiledSoilCount >= 2)
            {
                int direction = firstTiledSoilX - lastTiledSoilX > 0 ? -1 : 1;

                BlockData blockData = GetBlockInChunk(new Vector3(firstTiledSoilX,y,z), ChunkType.Ground);

                for (int currentX = firstTiledSoilX + direction; currentX != lastTiledSoilX + direction; currentX += direction)
                {
                    BlockData currentBlock = GetBlockClamped(currentX, y, z); 

                    if (currentBlock != null && currentBlock.id == BlockConstants.WetTilledSoil)
                    {
                        UpdateBlockTopTexture(currentX, y, z, "WetSoilMiddle");
                    }
                    else if (currentBlock != null)
                    {
                        UpdateBlockTopTexture(currentX, y, z, "SoilMiddle");
                    }
                }


                // 수정된 데이터들이 있는 청크 업데이트하기
                GetChunkFromPosition(new Vector3(firstTiledSoilX, y, z), ChunkType.Ground)?.UpdateChunk();
                GetChunkFromPosition(new Vector3(lastTiledSoilX, y, z), ChunkType.Ground)?.UpdateChunk();
            }
        }

        public void CheckLeftTiledSoil(int globalX, int globalY, int globalZ, ref int tiledSoilCount, ref int firstTiledSoilX)
        {
            for (int x = globalX - 1; x >= 0; x--) 
            {
                BlockData neighbor = GetBlockInChunk(new Vector3(x, globalY, globalZ), ChunkType.Ground);
                if (neighbor.id == BlockConstants.TilledSoil || neighbor.id == BlockConstants.WetTilledSoil)
                {
                    if (x < firstTiledSoilX)
                    {
                        firstTiledSoilX = x;
                    }
                    tiledSoilCount++;
                }
                else
                {
                    break;
                }
            }
        }

        public void CheckLeftTiledSoil(int globalX, int globalY, int globalZ, ref int tiledSoilCount, ref int firstTiledSoilX, ref int lastTiledSoilX)
        {

            for (int x = globalX - 1; x >= 0; x--) 
            {
                BlockData neighbor = GetBlockInChunk(new Vector3(x, globalY, globalZ), ChunkType.Ground);
                if (neighbor.id == BlockConstants.TilledSoil || neighbor.id == BlockConstants.WetTilledSoil)
                {
                    tiledSoilCount++;
                }
                else
                {
                    if (tiledSoilCount >= 2)
                    {
                        lastTiledSoilX = x + 1; 
                    }
                    break;
                }
            }
        }

        public void CheckRightTiledSoil(int globalX, int globalY, int globalZ, ref int tiledSoilCount, ref int lastTiledSoilX)
        {
            int mapWidth = mapSettingManager.ChunkCountX * ChunkConfig.ChunkWidthValue;

            for (int x = globalX + 1; x < mapWidth; x++) 
            {
                BlockData neighbor = GetBlockInChunk(new Vector3(x, globalY, globalZ), ChunkType.Ground);
                if (neighbor.id == BlockConstants.TilledSoil || neighbor.id == BlockConstants.WetTilledSoil)
                {
                    if (x > lastTiledSoilX)
                    {
                        lastTiledSoilX = x;
                    }
                    tiledSoilCount++;
                }
                else
                {
                    break;
                }
            }
        }

        public void DrySoil(Vector3 pos)
        {
            BlockData blockData = GetBlockInChunk(pos, ChunkType.Ground);
            if (blockData.id == BlockConstants.WetTilledSoil)
            {
                mapSettingManager.DrySoil(pos);
            }
        }

    }
}
