namespace Island
{
    // # System
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    // # Unity
    using UnityEngine;
    // # Static
    using static BlockConstants;

    public class MapSettingManager : MonoBehaviour
    {
        public static MapSettingManager Instance { get; private set; }

        private Map map;

        [Header("[# Chunk Parents ]")]
        [SerializeField]
        private Transform groundChunkParent;
        [SerializeField]
        private Transform waterChunkParent;
        [SerializeField]
        private Transform waterBorderChunkParent;
        [SerializeField]
        private Transform wallBorderChunkParent;
        [SerializeField]
        public float parentChunkYPos = -1f;

        [Header("[# Map Settings]")]
        [SerializeField]
        private GameObject chunkPrefab;
        [SerializeField]
        private Material mapGroundMaterial;
        [SerializeField]
        private Material mapWaterMaterial;
        [SerializeField]
        private int chunkCountX;
        [SerializeField]
        private int chunkCountY;
        [SerializeField]
        private int maxWaterHeight;
        [SerializeField]
        private int maxStoneHeight;
        [SerializeField]
        private int soilDryTime;

        [Header("[# Map Edge Falloff Settings]")]
        [SerializeField]
        private float falloffRadius;
        [SerializeField]
        private float maxFalloffHeight;
        [SerializeField]
        private int minHeightLimit;

        [Header("[# Noise Settings ]")]
        [SerializeField]
        private float heightNoiseScale;
        [SerializeField]
        private float blockTypeNoiseScale;
        [SerializeField, Range(0, 2000)]
        private int seed;

        [Header("[# About Data ]")]
        [SerializeField]
        private BlockDataConfig[] blockDataList;
        [SerializeField]
        private BlockTextureDataList[] blockTextureDataList;
        [SerializeField]
        private BlockWeightData[] blockWeightConfig;

        [Header("[# About Water Setting]")]
        [SerializeField]
        private float waterChunkObjectYScale;

       

        #region 프로퍼티 ( Get 기능 )
        // # 프로퍼티 ( Get 기능 )
        public Map Map => map;

        public Transform WaterChunkParent => waterChunkParent;
        public Transform WaterBorderChunkParent => waterBorderChunkParent;
        public Transform GroundChunkParent => groundChunkParent;
        public Transform WallBorderChunkParent => wallBorderChunkParent;

        public Material MapGroundMaterial => mapGroundMaterial;
        public Material MapWaterMaterial => mapWaterMaterial;

        public int ChunkCountX => chunkCountX;
        public int ChunkCountY => chunkCountY;
        public int MaxStoneHeight => maxStoneHeight;
        public int MaxWaterHeight => maxWaterHeight;
        public int Seed => seed;
        public int MinHeightLimit => minHeightLimit;

        public float FalloffRadius => falloffRadius;
        public float MaxFalloffHeight => maxFalloffHeight;
        public float HeightNoiseScale => heightNoiseScale;
        public float BlockTypeNoiseScale => blockTypeNoiseScale;

        public float WaterChunkObjectYScale => waterChunkObjectYScale;

        public BlockDataConfig[] BlockDataList => blockDataList;

        public BlockTextureDataList[] BlockTextureDataList => blockTextureDataList;

        public BlockWeightData[] BlockWeightConfig => blockWeightConfig;

        #endregion

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


            // 블럭 데이터 ID 설정 
            foreach (BlockDataConfig blockDataList in blockDataList)
            {
                for (int index = 0; index < blockDataList.blockDatas.Length; index++)
                {
                    blockDataList.blockDatas[index].id = blockDataList.id;
                }
            }
            map = new Map(this);

            map.Initialize();

        }

        private void Start()
        {
            InitializeChunk();
            map.InitializeSpawnPosition();
            if(LOPNetworkManager.Instance.isConnected == false)
            {
                TreeManager.Instance.SpawnTree();
            }
            TemperatureManager.Instance.SetDayCycleTemperature(TemperatureType.Normal, TemperatureType.Normal);//두번째 온도는 원래 TemperatureType.cold
        }



        /// <summary>
        /// 청크 프리팹을 생성해 반환합니다.
        /// </summary>
        public GameObject InstantiateChunk()
        {
            return Instantiate(chunkPrefab);
        }

        /// <summary>
        /// 땅 청크의 위치와 물 청크의 위치 및 스케일을 초기화합니다.
        /// </summary>
        private void InitializeChunk()
        {
            Vector3 mapPos = Vector3.up * parentChunkYPos;
            GroundChunkParent.transform.position = mapPos;

            Vector3 mapPos1 = Vector3.up * parentChunkYPos;
            WaterChunkParent.transform.position = mapPos1;
            WaterChunkParent.transform.localScale = new Vector3(1, WaterChunkObjectYScale, 1);

            waterBorderChunkParent.transform.position = mapPos1;
            waterBorderChunkParent.transform.localScale = new Vector3(1, WaterChunkObjectYScale, 1);

            wallBorderChunkParent.transform.position = mapPos;
            wallBorderChunkParent.transform.localScale = Vector3.one;
        }

        public void DrySoil(Vector3 pos)
        {
            StartCoroutine(DrySoilDelay(pos));
        }

        IEnumerator DrySoilDelay(Vector3 pos)
        {
            yield return new WaitForSeconds(soilDryTime);
            BlockData blockData = map.GetBlockInChunk(pos, ChunkType.Ground);
            if (blockData.id == BlockConstants.WetTilledSoil)
            {
                if (blockData.moistureLevel > 0)
                {
                    blockData.moistureLevel--;
                    Debug.Log("땅 마름");
                }
                if (blockData.moistureLevel <= 0)
                {
                    blockData.id = BlockConstants.TilledSoil;
                    int x = Mathf.FloorToInt(pos.x);
                    int y = Mathf.FloorToInt(pos.y);
                    int z = Mathf.FloorToInt(pos.z);

                    Vector3Int globalPos = new Vector3Int(x, y, z);

                    map.UpdateBlockTopTexture(x, y, z, "Soil");
                    map.UpdateGroundTexture(x, y, z);

                    string blockID = map.GetBlockInChunk(pos, ChunkType.Ground).id;
                    if (LOPNetworkManager.Instance.isConnected)
                    {
                        LOPNetworkManager.Instance.SendBlockUpdate(globalPos, blockID);
                    }
                }
                map.DrySoil(pos);
            }
            if (blockData.moistureLevel > 0)
            {
                StartCoroutine(DrySoilDelay(pos));
            }
        }
    }
}