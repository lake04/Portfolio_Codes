using Island;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    public static WaterManager Instance { get; private set; }
    [SerializeField] private MapSettingManager mapSettingManager;

    private Queue<Vector3Int> ActiveWaterQueue = new Queue<Vector3Int>();
    private HashSet<Vector3Int> WaterQueueHashSet = new HashSet<Vector3Int>();

    [SerializeField] private float tickRate = 0.2f;
    private WaitForSeconds waterTickWait;
    private const int SourceWaterLevel = 3;

    private static readonly Vector3Int[] FlowDirections = new Vector3Int[]
    {
         new Vector3Int( 0, 0, -1 ), // Back
		 new Vector3Int( 0, 0, 1 ), // Front
		 new Vector3Int(-1, 0, 0 ), // Left
         new Vector3Int( 1, 0, 0 ), // Right    
    };

    private static readonly Vector3Int[] WakeUpDirections = new Vector3Int[]
   {
        new Vector3Int( 0, -1, -1), // Back
        new Vector3Int( 0, -1,  1), // Front
        new Vector3Int(-1, -1,  0), // Left
        new Vector3Int( 1, -1,  0), // Right
        new Vector3Int( 0, -1,  0), // Bottom
        new Vector3Int( 0,  0,  0), // One
        new Vector3Int( 0,  1,  0), // Up
   };

    private HashSet<Chunk> chunksToUpdate = new HashSet<Chunk>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        waterTickWait = new WaitForSeconds(tickRate);
    }

    private void Start()
    {
        StartCoroutine(WaterTickRoutine());
    }

    private IEnumerator WaterTickRoutine()
    {
        while (true)
        {
            yield return waterTickWait;

            int count = ActiveWaterQueue.Count;
            if (count == 0) continue;

            chunksToUpdate.Clear();

            for (int i = 0; i < count; i++)
            {
                if (i > 0 && i % 500 == 0) yield return null;

                Vector3Int currentPos = ActiveWaterQueue.Dequeue();
                WaterQueueHashSet.Remove(currentPos);

                BlockData currentWater = mapSettingManager.Map.GetBlockInChunk(currentPos, ChunkType.Water);
                if (currentWater == null || currentWater.id != BlockConstants.Water)
                {
                    continue;
                }

                int currentLevel = currentWater.level;
                Vector3Int downWaterPos = currentPos + Vector3Int.down;
                int downLevel = currentLevel - 1;

                if (CanFlowIntoWaterCell(downWaterPos, downLevel))
                {
                    if (SetWaterBlock(downWaterPos, downLevel, chunksToUpdate))
                    {
                        WakeUpWaterAt(downWaterPos);
                        continue;
                    }
                }

                if (currentLevel <= 1)
                {
                    continue;
                }

                foreach (var dir in FlowDirections)
                {
                    Vector3Int neighborWaterPos = currentPos + dir;

                    if (CanFlowIntoWaterCell(neighborWaterPos, currentLevel - 1))
                    {
                        int nextLevel = GetHorizontalFlowLevel(currentPos, neighborWaterPos, currentLevel);
                        if (SetWaterBlock(neighborWaterPos, nextLevel, chunksToUpdate))
                        {
                            WakeUpWaterAt(neighborWaterPos);
                        }
                    }
                }
            }

            foreach (var chunk in chunksToUpdate)
            {
                chunk.UpdateChunk();
            }
        }
    }

    private bool SetWaterBlock(Vector3Int pos, int level, HashSet<Chunk> chunksToUpdate)
    {
        if (level <= 0) return false;

        var chunk = mapSettingManager.Map.GetChunkFromPosition(pos, ChunkType.Water);
        if (chunk == null) return false;

        int globalX = Mathf.FloorToInt(pos.x);
        int globalY = Mathf.FloorToInt(pos.y);
        int globalZ = Mathf.FloorToInt(pos.z);

        int localX = globalX - Mathf.FloorToInt(chunk.Position.x);
        int localZ = globalZ - Mathf.FloorToInt(chunk.Position.z);

        BlockData currentBlock = chunk.chunkData.chunkBlocks[localX, globalY, localZ];
        if (currentBlock != null && currentBlock.id == BlockConstants.Water && currentBlock.level == level)
        {
            return false;
        }

        BlockData waterBlock = mapSettingManager.Map.FindBlockType(BlockConstants.Water);
        waterBlock.level = level;
        chunk.chunkData.chunkBlocks[localX, globalY, localZ] = waterBlock;
        chunksToUpdate.Add(chunk);

        if (LOPNetworkManager.Instance != null && LOPNetworkManager.Instance.isConnected)
        {
            LOPNetworkManager.Instance.SendBlockUpdate(pos, BlockConstants.Water, level);
        }

        return true;
    }

    private bool CanFlowIntoWaterCell(Vector3Int targetWaterPos, int sourceLevel)
    {
        if (!mapSettingManager.Map.IsVoxelInMap(targetWaterPos))
        {
            return false;
        }

        Vector3Int targetGroundPos = targetWaterPos + Vector3Int.up;
        if (!mapSettingManager.Map.IsVoxelInMap(targetGroundPos))
        {
            return false;
        }

        BlockData ground = mapSettingManager.Map.GetBlockInChunk(targetGroundPos, ChunkType.Ground);
        if (ground == null || ground.id != BlockConstants.Air)
        {
            return false;
        }

        BlockData water = mapSettingManager.Map.GetBlockInChunk(targetWaterPos, ChunkType.Water);

        if (water == null || water.id != BlockConstants.Water)
        {
            return true;
        }

        bool canReplace = water.level < sourceLevel;
        return canReplace;
    }

    private int GetHorizontalFlowLevel(Vector3Int sourceWaterPos, Vector3Int targetWaterPos, int currentLevel)
    {
        if (IsSourceWater(sourceWaterPos) ||
            CountAdjacentSources(sourceWaterPos) >= 1 ||
            CountAdjacentSources(targetWaterPos) >= 1)
        {
            return SourceWaterLevel;
        }

        return currentLevel - 1;
    }

    private bool IsSourceWater(Vector3Int pos)
    {
        if (!mapSettingManager.Map.IsVoxelInMap(pos)) return false;

        BlockData water = mapSettingManager.Map.GetBlockInChunk(pos, ChunkType.Water);
        return water != null && water.id == BlockConstants.Water && water.level >= SourceWaterLevel;
    }

    public void WakeUpWaterAt(Vector3Int pos)
    {
        if (!mapSettingManager.Map.IsVoxelInMap(pos)) return;

        if (!WaterQueueHashSet.Contains(pos))
        {
            ActiveWaterQueue.Enqueue(pos);
            WaterQueueHashSet.Add(pos);
        }
    }

    private int CountAdjacentSources(Vector3Int pos)
    {
        int count = 0;

        foreach (var dir in FlowDirections)
        {
            Vector3Int neighborPos = pos + dir;

            if (!mapSettingManager.Map.IsVoxelInMap(neighborPos)) continue;

            BlockData neighbor = mapSettingManager.Map.GetBlockInChunk(neighborPos, ChunkType.Water);

            if (neighbor != null && neighbor.id == BlockConstants.Water && neighbor.level == 3)
            {
                count++;
            }
        }

        return count;
    }

    public void WakeUpAdjacentWater(Vector3Int emptyPos)
    {

        foreach (Vector3Int dir in WakeUpDirections)
        {
            Vector3Int neighborPos = emptyPos + dir;

            if (!mapSettingManager.Map.IsVoxelInMap(neighborPos))
            {
                continue;
            }

            BlockData water = mapSettingManager.Map.GetBlockInChunk(neighborPos, ChunkType.Water);
            if (water != null && water.id == BlockConstants.Water)
            {
                WakeUpWaterAt(neighborPos);
            }
        }
    }
}
