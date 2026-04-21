using System.Collections;
using UnityEngine;

public class FlashSprite : MonoBehaviour
{
    [Header("¼³Á¤")]
    [SerializeField] private SpriteRenderer originalSprite;

    [SerializeField] private Material flashMaterial;

    [SerializeField] private float duration = 0.1f;
    [SerializeField] private float flashOnDuration = 0.05f;

    [SerializeField] private int reapeatFlashNum = 1;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Coroutine flashCoroutine;

    private void Start()
    {
        if (this.originalSprite != null)
        {
            this.spriteRenderer = this.originalSprite;
        }
        else
        {
            this.spriteRenderer = base.GetComponent<SpriteRenderer>();
        }

        if (this.spriteRenderer == null)
        {
            return;
        }

        this.originalMaterial = this.spriteRenderer.sharedMaterial;
    }

    private void OnDisable()
    {
        this.StopFlash();
    }

    public void Flash()
    {
        if (this.flashCoroutine != null)
        {
            base.StopCoroutine(this.flashCoroutine);
        }

        if (base.gameObject.activeSelf)
        {
            this.flashCoroutine = base.StartCoroutine(this.FlashRoutine());
        }
        Debug.Log("flash white");
    }

    public void StopFlash()
    {
        if (this.flashCoroutine != null)
        {
            base.StopCoroutine(this.flashCoroutine);

            this.spriteRenderer.material = this.originalMaterial;

            this.flashCoroutine = null;
        }
    }

    private IEnumerator FlashRoutine()
    {
        if (this.spriteRenderer != null)
        {
            Color orinColor = this.spriteRenderer.color;
            for (int i = 0; i < this.reapeatFlashNum; i++) 
            {
                this.spriteRenderer.material = this.flashMaterial;
                this.spriteRenderer.color = new Color(1f, 1f, 1f, 0.4f);
                yield return new WaitForSeconds(this.flashOnDuration); 

                this.spriteRenderer.material = this.originalMaterial;
                this.spriteRenderer.color = orinColor;
                yield return new WaitForSeconds(this.duration); 
            }
        }

        this.flashCoroutine = null;
        yield break;
    }
}