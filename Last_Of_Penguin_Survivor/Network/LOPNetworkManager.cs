private void HandleMapBlockUpdate(GameMessage msg)
{
    var update = msg.MapBlockUpdate;

    if (update == null || MapSettingManager.Instance == null) return;

    Vector3 pos = new Vector3(update.X, update.Y, update.Z);

    var map = MapSettingManager.Instance.Map;

    var groundChunk = map.GetChunkFromPosition(pos, Island.ChunkType.Ground);
    var waterChunk = map.GetChunkFromPosition(pos, Island.ChunkType.Water);

    if (groundChunk == null || waterChunk == null) return;

    int localX = Mathf.FloorToInt(pos.x) - Mathf.FloorToInt(groundChunk.Position.x);
    int localY = Mathf.FloorToInt(pos.y);
    int localZ = Mathf.FloorToInt(pos.z) - Mathf.FloorToInt(groundChunk.Position.z);

    BlockData newBlock = map.FindBlockType(update.NewBlockId);

    if (newBlock == null) return;

    newBlock.id = update.NewBlockId;

    groundChunk.chunkData.chunkBlocks[localX, localY, localZ] =

    MapSettingManager.Instance.Map.FindBlockType(update.NewBlockId);

    BlockData blockDataInstance = new BlockData(newBlock);

    if (update.NewLevel != -1)
    {
        blockDataInstance.level = update.NewLevel;
        groundChunk.chunkData.chunkBlocks[localX, localY, localZ] = newBlock;
        groundChunk.chunkData.chunkBlocks[localX, localY, localZ].level = update.NewLevel;
    }



    if (update.NewBlockId == BlockConstants.Water)
    {
        waterChunk.chunkData.chunkBlocks[localX, localY, localZ] = blockDataInstance;
        groundChunk.chunkData.chunkBlocks[localX, localY, localZ] = map.FindBlockType(BlockConstants.Air);
    }

    else if (update.NewBlockId == BlockConstants.Air)
    {
        waterChunk.chunkData.chunkBlocks[localX, localY, localZ] = map.FindBlockType(BlockConstants.Air);
        groundChunk.chunkData.chunkBlocks[localX, localY, localZ] = map.FindBlockType(BlockConstants.Air);
    }

    else
    {
        groundChunk.chunkData.chunkBlocks[localX, localY, localZ] = blockDataInstance;
        waterChunk.chunkData.chunkBlocks[localX, localY, localZ] = map.FindBlockType(BlockConstants.Air);
    }

    Debug.Log($"[MapSync] Block at {pos} updated to {update.NewBlockId}. Local data synchronized.");

    groundChunk.UpdateChunk();

    if (waterChunk != groundChunk)
    {
       waterChunk.UpdateChunk();
    }

    map.UpdateChunk(pos);
}

 private void HandleBlockDestroyFlag(BlockDestroyFlag destroyFlag)
 {
     // 수신된 int32 좌표를 Vector3로 변환하여 맵 함수에 전달
     var pos = new Vector3(destroyFlag.X, destroyFlag.Y, destroyFlag.Z);

     if (MapSettingManager.Instance != null && MapSettingManager.Instance.Map != null)
     {
         var mapInstance = MapSettingManager.Instance.Map;

         try
         {
             // GetBlockInChunk는 Vector3를 받습니다.
             var blockData = mapInstance.GetBlockInChunk(pos, ChunkType.Ground);

             if (blockData != null)
             {
                 blockData.isDestroy = destroyFlag.IsDestroyed;
                 mapInstance.UpdateChunk(pos);
                 Debug.Log($"[Client] Block {(destroyFlag.IsDestroyed ? "DESTROYED" : "RESTORED")} at {pos}");
             }
         }
         catch (Exception ex)
         {
             Debug.LogError($"[Client Error] Failed to handle BlockDestroyFlag at {pos}: {ex.Message}");
         }
     }
 }

  public void SendBlockUpdate(Vector3Int position, string newBlockId, int level = -1)
 {
     var msg = new GameMessage
     {
         PlayerId = playerId,
         Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
         MapBlockUpdate = new MapBlockUpdate
         {
             X = position.x,
             Y = position.y,
             Z = position.z,
             NewBlockId = newBlockId,
             NewLevel = level
         }
     };
     SendRaw(msg);
 }