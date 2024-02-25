using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScratchCardEffectUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public RawImage scratchImage;
    public float brushSize = 40f;
    private Texture2D writableTexture;
    public bool invoked = false;
    public UnityEvent OnScratch;
    public float WinValue = 0.9f;

    void Start()
    {
        // Get the original texture from the RawImage component
        Texture2D originalTexture = scratchImage.texture as Texture2D;

        if (originalTexture != null)
        {
            // Create a new Texture2D with the same dimensions and format as the original texture
            writableTexture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);

            // Copy the pixels from the original texture to the new writable texture
            Color[] pixels = originalTexture.GetPixels();
            writableTexture.SetPixels(pixels);
            writableTexture.Apply();

            // Assign the writable texture to the RawImage component
            scratchImage.texture = writableTexture;
        }
        else
        {
            Debug.LogError("The texture attached to the RawImage is not a Texture2D or is null.");
        }
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
           
            float progress = CalculateScratchProgress();
            Debug.Log($"Scratch Progress: {progress * 100}%");

            if (progress >= WinValue && !invoked)
            {
                OnScratch.Invoke();
                invoked = true;
            }
            
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Scratch(eventData.position);
    }

    private float CalculateScratchProgress()
    {
        Color[] pixels = writableTexture.GetPixels();
        int totalPixels = pixels.Length;
        int transparentPixels = 0;

        foreach (Color pixel in pixels)
        {
            if (pixel.a == 0) // Check if the pixel is fully transparent
            {
                transparentPixels++;
            }
        }

        return (float)transparentPixels / totalPixels; // Progress as a percentage
    }


    public void OnDrag(PointerEventData eventData)
    {
        Scratch(eventData.position);
    }

    void Scratch(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(scratchImage.rectTransform, screenPosition, null, out Vector2 localPoint);

        // Convert to texture coordinates
        Vector2 texturePos = new Vector2(
            (localPoint.x + scratchImage.rectTransform.rect.width / 2) / scratchImage.rectTransform.rect.width,
            (localPoint.y + scratchImage.rectTransform.rect.height / 2) / scratchImage.rectTransform.rect.height
        );

        int x = (int)(texturePos.x * writableTexture.width) - (int)(brushSize / 2);
        int y = (int)(texturePos.y * writableTexture.height) - (int)(brushSize / 2);

        // Clear pixels in the texture
        for (int i = x; i < x + brushSize; i++)
        {
            for (int j = y; j < y + brushSize; j++)
            {
                if (i >= 0 && i < writableTexture.width && j >= 0 && j < writableTexture.height)
                {
                    float distance = Vector2.Distance(new Vector2(i, j), new Vector2(x + brushSize / 2, y + brushSize / 2));
                    if (distance < brushSize / 2)
                    {
                        writableTexture.SetPixel(i, j, Color.clear);
                    }
                }
            }
        }
        writableTexture.Apply();
    }
}
