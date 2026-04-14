using Island;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CropManager : MonoBehaviour
{
    public static CropManager Instance;

    public CropList[] cropList;

    public CropData[] allCropData;
    public float stressCheckTime = 240f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }


    public void PlantCropOnMap(string seedId, Vector3 pos)
    {
        BlockData blockData = MapSettingManager.Instance.Map.GetBlockInChunk(pos, ChunkType.Ground);
        BlockData CropsBlockData = MapSettingManager.Instance.Map.GetBlockInChunk(pos + Vector3.up, ChunkType.Ground);
        Debug.Log(seedId);
        float localY = pos.y;

        if (seedId == null || !blockData.id.EndsWith("TilledSoil") || CropsBlockData.id == BlockConstants.Crops)
        {
            Debug.Log(pos);
            return;
        }
        QuestPanel.Instance.IncreaseProgress("MQ_Antarctica_4_3", 1);
        CharacterController.Instance.characterUIController.GaugeUpdate();
        CropData targetData = allCropData.FirstOrDefault(data => data != null && data.cropsId == seedId);
        CropList targetPrefab= cropList.FirstOrDefault(prefab => prefab.cropSeed == seedId);

        Vector3 spawnPosition = new Vector3 (pos.x + 0.5f, localY, pos.z + 0.5f);

        GameObject cropObject = null;

        if (LOPNetworkManager.Instance.isConnected)
        {
            LOPNetworkManager.Instance.NetworkInstantiateWithCallback(
                targetPrefab.cropPrefab,
                spawnPosition,
                Quaternion.identity,

                (GameObject spawnedCrop) =>
                {
                    InitializeCrop(spawnedCrop, targetData, pos);
                }
            );
        }
        else
        {
            cropObject = Instantiate(targetPrefab.cropPrefab, spawnPosition, Quaternion.identity);
            InitializeCrop(cropObject, targetData, pos);
        }


    }

    private void InitializeCrop(GameObject cropObject, CropData cropData, Vector3 blockPos)
    {
        if (cropObject == null)
        {
            return;
        }

        MapSettingManager.Instance.Map.GetBlockInChunk(blockPos + Vector3.up, ChunkType.Ground).id = BlockConstants.Crops;

        Crop cropComponent = cropObject.GetComponent<Crop>();
        if (cropComponent != null)
        {
            cropComponent.cropData = cropData;
            cropComponent.blockPos = blockPos;
            cropComponent.SetGrowthState(GrowthState.Seed);
            InventoryManager.Instance.RemoveItem(new InventoryItem(QuickslotNumberBtn.Instance.selectedItem.item, 1));
        }
        else
        {
            Destroy(cropObject);
            MapSettingManager.Instance.Map.GetBlockInChunk(blockPos + Vector3.up, ChunkType.Ground).id = BlockConstants.Air;
        }

    }
}