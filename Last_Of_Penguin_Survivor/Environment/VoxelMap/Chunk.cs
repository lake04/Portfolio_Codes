namespace Island
{
    using Lop.Survivor;
    // # System
    using System;
    using System.Collections.Generic;
    using Unity.VisualScripting;

    // # Unity
    using UnityEngine;
    using UnityEngine.SocialPlatforms;
    using static Unity.Collections.AllocatorManager;

    [System.Serializable]
    public class Chunk
    {
        public ChunkData chunkData { get; private set; }

        private bool isOuter = false;

        // # 메쉬 오브젝트 관련
        private GameObject chunkObject;
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;

        private MeshCollider waterCollider;

        // # 청크 크기 관련
        private int chunkMaxWidth = ChunkConfig.ChunkWidthValue - 1;
        private int chunkMaxLength = ChunkConfig.ChunkLengthValue - 1;
        private int chunkMaxHeight = ChunkConfig.ChunkHeightValue - 1;

        private Map map = null;

        public Vector3 Position
        {
            get { return chunkObject.transform.localPosition; }
        }

        public bool IsActive
        {
            get => chunkObject.activeSelf;
            set => chunkObject.SetActive(value);
        }

        public GameObject GameObject
        {
            get { return chunkObject; }
        }

        public bool isDayTreeSpawn = true;


        public Chunk(Vector2Int coord, Map map, MapSettingManager mapSettingManager, ChunkType chunkType, bool isOuter)
        {
            chunkData = new ChunkData(coord, chunkType);
            this.map = map;
            chunkObject = mapSettingManager.InstantiateChunk();

            if (chunkType == ChunkType.Water)
            {
                chunkObject.layer = LayerMask.NameToLayer("Water");
            }

            meshFilter = chunkObject.GetComponent<MeshFilter>();
            meshRenderer = chunkObject.GetComponent<MeshRenderer>();
            meshCollider = chunkObject.GetComponent<MeshCollider>();


            chunkObject.name = $"Chunk {coord.x}.{coord.y}";

            this.isOuter = isOuter;

            switch (chunkType)
            {
                case ChunkType.Ground:
                    chunkObject.transform.SetParent(mapSettingManager.GroundChunkParent);
                    chunkObject.transform.localPosition = new Vector3Int(coord.x * ChunkConfig.ChunkWidthValue, 0, coord.y * ChunkConfig.ChunkLengthValue);
                    meshRenderer.material = mapSettingManager.MapGroundMaterial;
                    break;

                case ChunkType.Water:
                    chunkObject.transform.SetParent(this.isOuter ? mapSettingManager.WaterBorderChunkParent : mapSettingManager.WaterChunkParent);
                    chunkObject.transform.localPosition = new Vector3Int(coord.x * ChunkConfig.ChunkWidthValue, 0, coord.y * ChunkConfig.ChunkLengthValue);
                    chunkObject.transform.localScale = Vector3.one;
                    meshRenderer.material = mapSettingManager.MapWaterMaterial;
                    break;
            }

            if (this.isOuter && chunkType == ChunkType.Water)
            {
                FillAllBlocksWithWater();
            }
            else
            {
                PopulateBlockHeight();
                PopulateChunkBlock();
            }

            bool isAssigned = false;
            int groundCount = 0;
            string targetID = "Ground";

            for (int y = 0; y < ChunkConfig.ChunkHeightValue; y++)
            {
                for (int x = 0; x < ChunkConfig.ChunkWidthValue; x++)
                {
                    for (int z = 0; z < ChunkConfig.ChunkLengthValue; z++)
                    {
                        if (chunkData.chunkBlocks[x, y, z].id == targetID)
                        {
                            groundCount++;
                        }
                        if (groundCount >= 10)
                        {
                            CollectibleObject collectible = chunkObject.AddComponent<CollectibleObject>();
                            collectible.objectId = targetID;

                            isAssigned = true;
                            break;
                        }
                    }
                    if (isAssigned) break;
                }
                if (isAssigned) break;
            }

            UpdateChunk();

            if (chunkType == ChunkType.Water)
            {
                if (this.isOuter)
                {
                    CollectibleObject collectible = chunkObject.AddComponent<CollectibleObject>();
                    collectible.objectId = "Water";
                }
                CreateWaterCollider();
            }

            TickManager.Instance.OnDayInitialize += RespawnTree;
        }


        private void CreateWaterCollider()
        {
            // 빈 자식 오브젝트 생성
            GameObject waterObj = new GameObject("WaterCollider");
            waterObj.transform.SetParent(chunkObject.transform, false);
            waterObj.transform.localPosition = Vector3.zero;

            // 레이어 설정
            waterObj.layer = LayerMask.NameToLayer("WaterCrash");

            // MeshCollider 추가
            waterCollider = waterObj.AddComponent<MeshCollider>();
            waterCollider.sharedMesh = meshFilter.mesh; // 부모 메시 복제
            waterCollider.convex = false;
            waterCollider.isTrigger = false;

            // Y축 확장
            waterObj.transform.localScale = new Vector3(1f, 3f, 1f); // 필요에 따라 높이 조정
        }

        ///<summary>현재 청크의 월드 공간 위치를 가져옵니다.</summary>


        private Vector3 ToWorldPos(in Vector3 pos) => Position + pos;
        private Vector3 ToWorldPos(int x, int y, int z) => Position + new Vector3(x, y, z);

        private void PopulateBlockHeight()
        {
            for (int x = 0; x < ChunkConfig.ChunkWidthValue; x++)
            {
                for (int z = 0; z < ChunkConfig.ChunkLengthValue; z++)
                {
                    chunkData.blockHeights[x, z] = map.CalculateBlockHeight(ToWorldPos(x, 0, z));
                }
            }
        }

        private void PopulateChunkBlock()
        {
            for (int y = 0; y < ChunkConfig.ChunkHeightValue; y++)
            {
                for (int x = 0; x < ChunkConfig.ChunkWidthValue; x++)
                {
                    for (int z = 0; z < ChunkConfig.ChunkLengthValue; z++)
                    {
                        chunkData.chunkBlocks[x, y, z] = map.CalculateBlockData(ToWorldPos(x, y, z), chunkData.blockHeights[x, z]);
                    }
                }
            }
        }

        private void FillAllBlocksWithWater()
        {
            var water = MapSettingManager.Instance.Map.FindBlockType(BlockConstants.Water);
            var air = MapSettingManager.Instance.Map.FindBlockType(BlockConstants.Air);
            int height = ChunkConfig.ChunkHeightValue;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < ChunkConfig.ChunkWidthValue; x++)
                {
                    for (int z = 0; z < ChunkConfig.ChunkLengthValue; z++)
                    {
                        chunkData.chunkBlocks[x, y, z] = (y < MapSettingManager.Instance.MaxWaterHeight) ? water : air;
                    }
                }
            }
        }

        ///<summary>청크 데이터를 갱신하고 메시를 생성합니다.</summary>
        public void UpdateChunk()
        {
            MeshData meshData = ChunkMeshGenerator.Generate(this);
            ApplyMesh(meshData);
        }

        ///<summary>현재 청크의 메시를 생성하고 충돌 메시를 설정합니다.</summary>
        public void ApplyMesh(MeshData meshData)
        {
            Mesh mesh = new Mesh();
            mesh.SetVertices(meshData.Vertices);
            mesh.SetTriangles(meshData.Triangles, 0);
            mesh.SetUVs(0, meshData.UVs);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshFilter.mesh = mesh;

            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;

            if (waterCollider != null)
                waterCollider.sharedMesh = mesh;
        }

        ///<summary>지정된 위치의 복셀 데이터를 새로운 데이터로 변경합니다.</summary>
        public void EditVoxel(Vector3 pos, BlockData newBlockData)
        {
            Debug.Log($"블럭파괴! : {pos}, {newBlockData}");

            int globalX = Mathf.FloorToInt(pos.x);
            int globalY = Mathf.FloorToInt(pos.y);
            int globalZ = Mathf.FloorToInt(pos.z);

            int localX = globalX - Mathf.FloorToInt(Position.x);
            int localZ = globalZ - Mathf.FloorToInt(Position.z);

            chunkData.chunkBlocks[localX, globalY, localZ] = newBlockData;

            Chunk waterChunk = map.GetChunkFromPosition(pos, ChunkType.Water);
            waterChunk.UpdateChunk();

            UpdateChunk();
            UpdateSurroundingVoxels(localX, globalY, localZ);
            map.UpdateNeighborTextures(globalX, globalY, globalZ);
        }

        ///<summary>지정된 위치의 복셀 데이터이 Level을 높입니다.</summary>
        public bool CrackVoxel(Vector3 pos)
        {
            int globalX = Mathf.FloorToInt(pos.x);
            int globalY = Mathf.FloorToInt(pos.y);
            int globalZ = Mathf.FloorToInt(pos.z);

            Vector3Int globalPos = new Vector3Int(globalX, globalY, globalZ);

            int localX = globalX - Mathf.FloorToInt(Position.x);
            int localZ = globalZ - Mathf.FloorToInt(Position.z);
            string blockID = map.GetBlockInChunk(pos, ChunkType.Ground).id;

            chunkData.chunkBlocks[localX, globalY, localZ].level += 1;
            int posLevel = chunkData.chunkBlocks[localX, globalY, localZ].level;

            if (LOPNetworkManager.Instance.isConnected)
            {
                LOPNetworkManager.Instance.SendBlockUpdate(globalPos, blockID, posLevel);
            }

            if (chunkData.chunkBlocks[localX, globalY, localZ].level >= 3)
            {
                DropItemSpawner.Instance.SpawnItem(new InventoryItem(ItemGenerator.Instance.GetItemDataFromBlock(blockID), 1), pos + new Vector3(0.5f, -0.5f, 0.5f));

                var waterChunk = map.GetChunkFromPosition(pos, ChunkType.Water);
                if (IsWaterAdjacent(pos))
                {
                    chunkData.chunkBlocks[localX, globalY, localZ] = MapSettingManager.Instance.Map.FindBlockType("Air");
                    chunkData.chunkBlocks[localX, globalY - 1, localZ] = MapSettingManager.Instance.Map.FindBlockType(BlockConstants.Water);

                    waterChunk.chunkData.chunkBlocks[localX, globalY, localZ] = MapSettingManager.Instance.Map.FindBlockType("Air");
                    waterChunk.chunkData.chunkBlocks[localX, globalY - 1, localZ] = MapSettingManager.Instance.Map.FindBlockType(BlockConstants.Water);

                    if (LOPNetworkManager.Instance.isConnected)
                    {
                        LOPNetworkManager.Instance.SendBlockUpdate(globalPos, "Air");

                        Vector3Int posBelow = new Vector3Int(globalX, globalY - 1, globalZ);
                        LOPNetworkManager.Instance.SendBlockUpdate(posBelow, BlockConstants.Water);
                    }

                    if (chunkData.chunkBlocks[localX, globalY - 1, localZ].id == BlockConstants.Water
            || waterChunk.chunkData.chunkBlocks[localX, globalY - 1, localZ].id == BlockConstants.Water)
                    {
                        Debug.Log("밑에 땅 있으니까, 바로 이 자리(globalY)에 물 채움");

                        chunkData.chunkBlocks[localX, globalY - 2, localZ] = MapSettingManager.Instance.Map.FindBlockType(BlockConstants.Water);
                        waterChunk.chunkData.chunkBlocks[localX, globalY - 2, localZ] = MapSettingManager.Instance.Map.FindBlockType(BlockConstants.Water);

                        if (LOPNetworkManager.Instance.isConnected)
                        {
                            Vector3Int posTwoBelow = new Vector3Int(globalX, globalY - 2, globalZ);
                            LOPNetworkManager.Instance.SendBlockUpdate(posTwoBelow, BlockConstants.Water);
                        }
                    }

                    waterChunk.UpdateChunk();
                    waterChunk.UpdateSurroundingVoxels(localX, globalY, localZ);
                }
                else
                {
                    chunkData.chunkBlocks[localX, globalY, localZ] = MapSettingManager.Instance.Map.FindBlockType("Air");
                    if (LOPNetworkManager.Instance.isConnected)
                    {
                        LOPNetworkManager.Instance.SendBlockUpdate(globalPos, "Air");

                    }
                }

                UpdateChunk();
                map.UpdateNeighborTextures(globalX, globalY, globalZ);
                UpdateSurroundingVoxels(localX, globalY, localZ);
                return true;
            }

            UpdateChunk();
            UpdateSurroundingVoxels(localX, globalY, localZ);
            return false;
        }

        /// <summary> 지정된 위치의 블럭 데이터를 가져옵니다. </summary>
        public BlockData GetBlockData(Vector3 pos)
        {
            int globalX = Mathf.FloorToInt(pos.x);
            int globalY = Mathf.FloorToInt(pos.y);
            int globalZ = Mathf.FloorToInt(pos.z);

            int localX = globalX - Mathf.FloorToInt(Position.x);
            int localZ = globalZ - Mathf.FloorToInt(Position.z);

            return chunkData.chunkBlocks[localX, globalY, localZ];
        }

        ///<summary>주변 청크의 메시를 갱신합니다.</summary>
        public void UpdateSurroundingVoxels(int x, int y, int z)
        {
            Vector3 thisVoxel = new Vector3(x, y, z);

            for (int p = 0; p < 6; p++)
            {
                Vector3 currentVoxel = thisVoxel + VoxelData.FaceChecks[p];

                if (!IsBlockInChunk((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z))
                {
                    Vector3 pos = currentVoxel + Position;

                    int globalX = Mathf.FloorToInt(pos.x);
                    int globalY = Mathf.FloorToInt(pos.y);
                    int globalZ = Mathf.FloorToInt(pos.z);

                    if (!map.IsVoxelInMap(pos)) continue;

                    Chunk tempChunk = map.GetChunkFromPosition(pos, ChunkType.Ground);
                    Chunk tempWaterChunk = map.GetChunkFromPosition(pos, ChunkType.Water);

                    if (tempChunk != null)
                        tempChunk.UpdateChunk();

                    if (tempWaterChunk != null)
                        tempWaterChunk.UpdateChunk();

                    string blockID = map.GetBlockInChunk(pos, ChunkType.Ground).id;
                    int level = map.GetBlockInChunk(pos, ChunkType.Ground).level;
                    if (LOPNetworkManager.Instance.isConnected)
                    {
                        LOPNetworkManager.Instance.SendBlockUpdate(new Vector3Int(globalX, globalY, globalZ), blockID, level);

                    }
                }
            }
        }



        /// <summary> 블럭이 청크 내에 있는지 확인합니다. </summary>
        public bool IsBlockInChunk(int blockX, int blockY, int blockZ)
        {
            if (blockX < 0 || blockX > chunkMaxWidth
             || blockY < 0 || blockY > chunkMaxHeight
             || blockZ < 0 || blockZ > chunkMaxLength)
                return false;
            else
                return true;
        }

        private bool IsRenderBlock(BlockData blockData)
        {
            return chunkData.type switch
            {
                ChunkType.Ground => blockData.isSolid,
                ChunkType.Water => !blockData.isSolid,
                _ => false
            };
        }

        /// <summary>
        /// 지정된 위치의 복셀이 청크 내에 있는지 확인하고, 해당 복셀이 솔리드한지 검사합니다.
        /// </summary>
        public bool IsBlockSolid(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);
            int z = Mathf.FloorToInt(pos.z);

            if (IsBlockInChunk(x, y, z))
            {
                BlockData block = chunkData.chunkBlocks[x, y, z];

                if (chunkData.type == ChunkType.Ground)
                    return block.isSolid;

                return block.id == BlockConstants.Water;
            }

            Vector3 globalPos = new Vector3(x, y, z) + Position;

            Chunk neighborChunk = map.GetChunkFromPosition(globalPos, chunkData.type);
            if (neighborChunk == null)
                return false;

            int localX = Utils.PositiveMod(Mathf.FloorToInt(globalPos.x), ChunkConfig.ChunkWidthValue);
            int localY = Mathf.FloorToInt(globalPos.y);
            int localZ = Utils.PositiveMod(Mathf.FloorToInt(globalPos.z), ChunkConfig.ChunkLengthValue);

            if (localY < 0 || localY >= ChunkConfig.ChunkHeightValue)
                return false;

            BlockData neighbor = neighborChunk.chunkData.chunkBlocks[localX, localY, localZ];

            if (chunkData.type == ChunkType.Ground)
                return neighbor.isSolid;

            return neighbor.id == BlockConstants.Water;
        }

        public bool IsWaterAdjacent(Vector3 pos)
        {
            int globalX = Mathf.FloorToInt(pos.x);
            int globalY = Mathf.FloorToInt(pos.y);
            int globalZ = Mathf.FloorToInt(pos.z);

            Vector3[] directions = new Vector3[5]
            {
                new Vector3( 0.0f,  -1.0f, -1.0f ), // Back
				new Vector3( 0.0f,  -1.0f,  1.0f ), // Front
				new Vector3(-1.0f,  -1.0f,  0.0f ), // Left
				new Vector3( 1.0f,  -1.0f,  0.0f ), // Right

                new Vector3( 0.0f,  0.0f,  -1.0f ) //bottom
            };

            foreach (Vector3 direction in directions)
            {
                int neighborX = globalX + (int)direction.x;
                int neighborY = globalY + (int)direction.y;
                int neighborZ = globalZ + (int)direction.z;

                Vector3 neighborWorldPosition = new Vector3(neighborX, neighborY, neighborZ);

                if (!map.IsVoxelInMap(neighborWorldPosition))
                    continue;

                BlockData neighborBlock = map.GetBlockInChunk(neighborWorldPosition, ChunkType.Ground);

                if (neighborBlock != null && neighborBlock.id == BlockConstants.Water)
                    return true;
            }

            return false;
        }

        private void RespawnTree()
        {
            isDayTreeSpawn = true;
        }
    }
}