namespace Lop.Survivor.Island.Buildingbase
{
    using global::Island;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class FruitTree : TreeChop
    {
        public List<InventoryItem> dropItems;

        [LopRPC]
        public void ChopingTree()
        {
            if (!isAtive)
            {
                return;
            }
            chopAmount--;
            anim.SetTrigger("Chop");

            if (chopAmount <= 0)
            {
                StartCoroutine(Die());
            }
        }

        public override IEnumerator Die()
        {
            isAtive = false;
            anim.SetTrigger("Die");
            Vector3Int treePosInt = Vector3Int.FloorToInt(transform.position);
            Vector3Int pos1 = new Vector3Int(treePosInt.x, treePosInt.y - 1, treePosInt.z);
            Vector3Int pos2 = treePosInt;
            Vector3Int pos3 = treePosInt + Vector3Int.up;

            MapSettingManager.Instance.Map.GetBlockInChunk(pos1, ChunkType.Ground).isDestroy = true;
            MapSettingManager.Instance.Map.GetBlockInChunk(pos1, ChunkType.Ground).id = BlockConstants.Ground;


            MapSettingManager.Instance.Map.GetBlockInChunk(pos2, ChunkType.Ground).id = BlockConstants.Air;
            MapSettingManager.Instance.Map.GetBlockInChunk(pos3, ChunkType.Ground).id = BlockConstants.Air;

            yield return new WaitForSeconds(1);

            InventoryItem fruitToDrop = SpawnFruit();

            if (LOPNetworkManager.Instance.isConnected)
            {
                DropItemSpawner.Instance.SpawnItem(dropItem, transform.position + Vector3.up);
                if(fruitToDrop != null)
                {
                    DropItemSpawner.Instance.SpawnItem(fruitToDrop, transform.position + Vector3.up);
                }

                LOPNetworkManager.Instance.SendBlockUpdate(pos1, BlockConstants.Ground);
                LOPNetworkManager.Instance.SendBlockUpdate(pos2, BlockConstants.Air);
                LOPNetworkManager.Instance.SendBlockUpdate(pos3 + Vector3Int.up, BlockConstants.Air);
                LOPNetworkManager.Instance.NetworkDestroy(gameObject);
            }
            else if (!LOPNetworkManager.Instance.isConnected)
            {
                DropItemSpawner.Instance.SpawnItem(dropItem, transform.position + Vector3.up);
                if (fruitToDrop != null)
                {
                    DropItemSpawner.Instance.SpawnItem(fruitToDrop, transform.position + Vector3.up);
                }

                MapSettingManager.Instance.Map.UpdateChunk(pos1);
                MapSettingManager.Instance.Map.UpdateChunk(pos2);
                MapSettingManager.Instance.Map.UpdateChunk(pos3);
                Destroy(gameObject, 1f);
            }
            QuestPanel.Instance.IncreaseProgress("MQ_Antarctica_1_1", 1);
        }

        public InventoryItem SpawnFruit()
        {
            //int random = UnityEngine.Random.Range(0,10);

            int fruitRanom = UnityEngine.Random.Range(0, dropItems.Count);
            return dropItems[fruitRanom];

            //if (random < 4)
            //{
                
            //}
            //else
            //{
            //    return null;
            //}
        }
    }
}

