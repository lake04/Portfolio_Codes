using Island;
using System.Collections;
using System.Drawing;
using Unity.Collections;
using UnityEngine;

public class TreeChop : MonoBehaviour
{
    public int chopAmount;
    public InventoryItem dropItem;
    public Animator anim;
    protected bool isAtive = true;

    [LopRPC]
    public void ChopingTree()
    {
        if(!isAtive)
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

    public virtual IEnumerator Die()
    {
        isAtive = false;
        anim.SetTrigger("Die");
        Vector3Int treePosInt = Vector3Int.FloorToInt(transform.position);

        Vector3Int floorPos = new Vector3Int(treePosInt.x, treePosInt.y - 1, treePosInt.z);
        Vector3Int pos = treePosInt; 
        Vector3Int topPos = treePosInt + Vector3Int.up; 

        MapSettingManager.Instance.Map.GetBlockInChunk(floorPos, ChunkType.Ground).id = BlockConstants.Ground;
        MapSettingManager.Instance.Map.GetBlockInChunk(floorPos, ChunkType.Ground).isDestroy = true;
        MapSettingManager.Instance.Map.GetBlockInChunk(pos, ChunkType.Ground).id = BlockConstants.Air;
        MapSettingManager.Instance.Map.GetBlockInChunk(topPos, ChunkType.Ground).id = BlockConstants.Air;
        yield return new WaitForSeconds(1);

        if (LOPNetworkManager.Instance.isConnected == true)
        {
            MapSettingManager.Instance.Map.UpdateChunk(floorPos);

            LOPNetworkManager.Instance.SendBlockUpdate(floorPos, BlockConstants.Ground);
            LOPNetworkManager.Instance.SendBlockUpdate(pos, BlockConstants.Air);
            LOPNetworkManager.Instance.SendBlockUpdate(topPos, BlockConstants.Air);
            DropItemSpawner.Instance.SpawnItem(dropItem, transform.position + Vector3.up);
            LOPNetworkManager.Instance.NetworkDestroy(gameObject);

        }
        else if (LOPNetworkManager.Instance.isConnected == false)
        {
            MapSettingManager.Instance.Map.UpdateChunk(floorPos);
            MapSettingManager.Instance.Map.UpdateChunk(pos);
            MapSettingManager.Instance.Map.UpdateChunk(topPos);
            DropItemSpawner.Instance.SpawnItem(dropItem, transform.position + Vector3.up);
            Destroy(gameObject, 1f);
        }
    }
   
}
