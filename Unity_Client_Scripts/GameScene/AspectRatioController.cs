using UnityEngine;

public class AspectRatioController : MonoBehaviour
{
    // Megmondjuk, hogy a kamera a felsõ vagy az alsó felét nézze
    public enum SplitPosition { Top, Bottom }
    public SplitPosition cameraPosition = SplitPosition.Top; // Alapértelmezés: Felsõ

    public float targetAspect = 0.5f; // A 18:9 arány (9/18 = 0.5)

    void Start()
    {
        float currentAspect = (float)Screen.width / (float)Screen.height;//0,45
        float aspectFactor = currentAspect / targetAspect;//   0,45/0,5=0,9

        Camera cam = GetComponent<Camera>();            

        // --- 1. Kiszámoljuk a fekete csíkok helyét (Pillarboxing) ---
        float rectX = 0f;
        float rectY = 0f;
        float scaledWidth = 1f;
        float scaledHeight = 1f;
        
        // Csak a szélesebb képernyõknél kell módosítani
        if (aspectFactor > 1.0f)
        {
            scaledWidth = 1.0f / aspectFactor;
            rectX = (1.0f - scaledWidth) / 2.0f;
        }else if (aspectFactor <1.0f)
        {
            scaledHeight = aspectFactor;
            rectY = (1.0f - scaledHeight) / 2.0f;
        }


        // --- 2. Beállítjuk a Viewport Rect-et a felosztással együtt ---
        float finalHeight = 0.5f*scaledHeight;

        if (cameraPosition == SplitPosition.Top)
        {
            // Felsõ kamera: A szélesség a pillarboxing-ból jön, a magasság a felosztásból.
            cam.rect = new Rect(rectX, 0.5f, scaledWidth, finalHeight);
        }
        else // Bottom
        {
            // Alsó kamera: A szélesség a pillarboxing-ból jön, a magasság a felosztásból.
            cam.rect = new Rect(rectX, 0.0f+rectY, scaledWidth, finalHeight);
        }
    }
}