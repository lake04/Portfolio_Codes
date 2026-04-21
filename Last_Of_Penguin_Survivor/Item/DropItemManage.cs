using UnityEngine;
using GlobalAudio;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Island;

public class DropItemManage : MonoBehaviour
{
    public SpriteRenderer plane;
    public bool isEnable = true; // ��� ������ Ȱ��ȭ ����
    public InventoryItem item; // ��� ������ ����
    [SerializeField] private AudioClip itempPickUpClip;

    [Header("Buoyancy Settings")]
    [SerializeField] private float buoyancyForce = 25f;
    [SerializeField] private float waterDrag = 0.05f;
    [SerializeField] private float airDrag = 0.05f;

    [SerializeField] private Rigidbody rb;

    public void DropItemUIInit()
    {
        plane.sprite = item.item.icon;
    }

    private void OnEnable()
    {
        StartCoroutine(AutoDesrtoy());
    }

    private void FixedUpdate()
    {
        HandleBuoyancy();
    }

    private void HandleBuoyancy()
    {
        if (rb == null) return;

        var currentBlock = MapSettingManager.Instance.Map.GetBlockInChunk(transform.position, ChunkType.Ground);

        if (currentBlock.id == BlockConstants.Water)
        {
            rb.AddForce(Vector3.up * buoyancyForce, ForceMode.Acceleration);

            rb.linearDamping = waterDrag;
            rb.angularDamping = 1f;
        }
        else
        {
            rb.linearDamping = airDrag;
            rb.angularDamping = 0.05f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isEnable && other.gameObject.CompareTag("Player"))
        {
            isEnable = false; // ������ ��� �� Ȱ��ȭ ��Ȱ��ȭ
            AudioPlayer.PlaySound(this.gameObject, itempPickUpClip);
            CharacterController controller = other.GetComponent<CharacterController>();

            if (controller.inv.IsFullInventory() || controller == null)
                return;

            if (LOPNetworkManager.Instance.isConnected)
            {
                NetworkIdentity identity = controller.GetComponent<NetworkIdentity>();

                if (identity.IsOwner) 
                {
                    controller.inv.AddItem(item);
                }
                LOPNetworkManager.Instance.NetworkDestroy(gameObject);
            }
            else if (!LOPNetworkManager.Instance.isConnected)
            {
                controller.inv.AddItem(item);
                Destroy(gameObject);
            }
        }
    }

    public IEnumerator EnablePickUp()
    {
        yield return new WaitForSeconds(1.3f);
        isEnable = true;
    }

    public IEnumerator AutoDesrtoy()
    {
        yield return new WaitForSeconds(120f);

        if (LOPNetworkManager.Instance.isConnected)
        {
            LOPNetworkManager.Instance.NetworkDestroy(gameObject);
        }
        else if(LOPNetworkManager.Instance.isConnected == false)
        {
            Destroy(gameObject);
        }
    }
}
