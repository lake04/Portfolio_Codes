using Island;
using Lop.Survivor;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public static TreeManager Instance;

    [Header("[# About Tree Setting]")]
    [SerializeField] private GameObject[] treePrefabs;
    [SerializeField] private float treeSpawnProbability;
    [SerializeField] private float checkTreeSpawnRange;
    [SerializeField] private LayerMask treeLayer;
    [SerializeField] private Transform treeSpawnParents;
    [SerializeField] private int respawnTreeCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        TickManager.Instance.OnDayInitialize += RespawnTree;
    }

    public void SpawnTree()
    {
        for (int x = 0; x < MapSettingManager.Instance.ChunkCountX * ChunkConfig.ChunkLengthValue; x++)
        {
            for (int z = 0; z < MapSettingManager.Instance.ChunkCountY * ChunkConfig.ChunkWidthValue; z++)
            {
                if (UnityEngine.Random.Range(0, 100) >= treeSpawnProbability)
                    continue;

                int blockHeight = MapSettingManager.Instance.Map.CalculateBlockHeight(new Vector3(x, 0, z));

                Vector3Int florPos = new Vector3Int(x, blockHeight - 1, z);
                Vector3Int pos1 = new Vector3Int(x, blockHeight, z);
                Vector3Int pos2 = new Vector3Int(x, blockHeight + 1, z);

                if (MapSettingManager.Instance.Map.GetBlockInChunk(florPos, ChunkType.Ground).id != BlockConstants.Ground)
                {
                    continue;
                }
                if (!CanSpawnTree(new Vector3(x, blockHeight, z), checkTreeSpawnRange))
                {
                    continue;
                }

                string newBlockId = BlockConstants.Trees;

                MapSettingManager.Instance.Map.GetBlockInChunk(new Vector3(x, blockHeight - 1, z), ChunkType.Ground).isDestroy = false;

                if (LOPNetworkManager.Instance.isConnected)
                {
                    if (LOPNetworkManager.Instance.IsWorldSpawner == false)
                    {
                        Debug.Log("[TreeManager] 월드 스포너 권한이 없으므로 월드 오브젝트 생성을 건너뜁니다.");
                        return;
                    }
                    else
                    {
                        LOPNetworkManager.Instance.NetworkInstantiate(
                            RandomTree(),
                            new Vector3(x, blockHeight + 0.18f, z),
                            Quaternion.identity
                        );
                        #if UNITY_EDITOR
                            Debug.Log($"나무 소환 위치 {new Vector3(x, blockHeight, z)}");
                        #endif
                    }
                }
                else
                {
                   GameObject tree = Instantiate(
                        RandomTree(),
                        new Vector3(x, blockHeight + 0.18f, z),
                        Quaternion.identity
                    );
                    tree.transform.SetParent(treeSpawnParents);
                }

                MapSettingManager.Instance.Map.GetBlockInChunk(pos1, ChunkType.Ground).id = newBlockId;
                MapSettingManager.Instance.Map.GetBlockInChunk(pos2, ChunkType.Ground).id = newBlockId;

                string blockID = MapSettingManager.Instance.Map?.GetBlockInChunk(pos2, ChunkType.Ground).id;

                MapSettingManager.Instance.Map.UpdateChunk(pos1);
                MapSettingManager.Instance.Map.UpdateChunk(pos2);

                if (LOPNetworkManager.Instance != null)
                {
                    if (LOPNetworkManager.Instance.isConnected)
                    {
                        LOPNetworkManager.Instance.SendBlockUpdate(pos1, newBlockId, 0);
                        LOPNetworkManager.Instance.SendBlockUpdate(pos2, newBlockId, 0);
                    }
                }
            }
        }
    }

    public void RespawnTree()
    {
        int curTreeCount = 0;

        for (int x = 0; x < MapSettingManager.Instance.ChunkCountX * ChunkConfig.ChunkLengthValue; x++)
        {
            for (int z = 0; z < MapSettingManager.Instance.ChunkCountY * ChunkConfig.ChunkWidthValue; z++)
            {
                if (curTreeCount > respawnTreeCount)
                {
                    Debug.Log("리스폰 트리 개수 초과");
                    return;
                }
                if (UnityEngine.Random.Range(0, 100) >= treeSpawnProbability)
                    continue;

                int blockHeight = MapSettingManager.Instance.Map.CalculateBlockHeight(new Vector3(x, 0, z));

                Vector3Int florPos = new Vector3Int(x, blockHeight - 1, z);
                Vector3Int pos1 = new Vector3Int(x, blockHeight, z);
                Vector3Int pos2 = new Vector3Int(x, blockHeight + 1, z);

                if (MapSettingManager.Instance.Map.GetBlockInChunk(florPos, ChunkType.Ground).id != BlockConstants.Ground)
                {
                    continue;
                }
                if (!CanSpawnTree(new Vector3(x, blockHeight, z), checkTreeSpawnRange))
                {
                    continue;
                }

                string newBlockId = BlockConstants.Trees;
                MapSettingManager.Instance.Map.GetBlockInChunk(new Vector3(x, blockHeight - 1, z), ChunkType.Ground).isDestroy = false;
                if (LOPNetworkManager.Instance.isConnected)
                {
                    if (LOPNetworkManager.Instance.IsWorldSpawner == false)
                    {
                        Debug.Log("[TreeManager] 월드 스포너 권한이 없으므로 월드 오브젝트 생성을 건너뜁니다.");
                        return;
                    }
                    else
                    {
                        LOPNetworkManager.Instance.NetworkInstantiate(
                            RandomTree(),
                            new Vector3(x, blockHeight + 0.18f, z),
                            Quaternion.identity
                        );
                        #if UNITY_EDITOR
                            Debug.Log($"나무 소환 위치 {new Vector3(x, blockHeight, z)}");
                        #endif
                        curTreeCount++;
                    }
                }
                else
                {
                    GameObject tree = Instantiate(
                         RandomTree(),
                         new Vector3(x, blockHeight + 0.18f, z),
                         Quaternion.identity
                     );
                    tree.transform.SetParent(treeSpawnParents);
                    curTreeCount++;
                }
                MapSettingManager.Instance.Map.GetBlockInChunk(pos1, ChunkType.Ground).id = newBlockId;
                MapSettingManager.Instance.Map.GetBlockInChunk(pos2, ChunkType.Ground).id = newBlockId;
                string blockID = MapSettingManager.Instance.Map?.GetBlockInChunk(pos2, ChunkType.Ground).id;
                MapSettingManager.Instance.Map.UpdateChunk(pos1);
                MapSettingManager.Instance.Map.UpdateChunk(pos2);
                if (LOPNetworkManager.Instance != null)
                {
                    if (LOPNetworkManager.Instance.isConnected)
                    {
                        LOPNetworkManager.Instance.SendBlockUpdate(pos1, newBlockId, 0);
                        LOPNetworkManager.Instance.SendBlockUpdate(pos2, newBlockId, 0);
                    }
                }
            }
        }
    }

    private bool CanSpawnTree(Vector3 pos, float range)
    {
        return !Physics.CheckBox(pos, Vector3.one * (range * 0.5f), Quaternion.identity, treeLayer);
    }

    private GameObject RandomTree()
    {
        int random = UnityEngine.Random.Range(0, 10);

        if (random < 2)
        {
            return treePrefabs[1];
        }
        else
        {
            return treePrefabs[0];
        }
    }
}
