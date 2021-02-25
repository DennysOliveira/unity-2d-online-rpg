using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Glow name space
public class LinearBlur {
    static  private float _rSum = 0;
    static  private float _gSum = 0;
    static private float _bSum = 0;
    static private float _aSum = 0;

    static private Texture2D _sourceImage;
    static private int _sourceWidth;
    static private int _sourceHeight;
    static  private int _windowSize;

    static public Texture2D GetTextureFromSprite(Sprite sprite) {
         // Backup the currently set RenderTexture
        RenderTexture previous = RenderTexture.active;

         // Create a temporary RenderTexture of the same size as the texture
        RenderTexture tmp = RenderTexture.GetTemporary(sprite.texture.width, sprite.texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        
        // Blit the pixels on texture to the RenderTexture
        Graphics.Blit(sprite.texture, tmp);

        // Set the current RenderTexture to the temporary one we created
        RenderTexture.active = tmp;

        // Create a new readable Texture2D to copy the pixels to it
        // Texture2D myTexture2D = new Texture2D(sprite.texture.width, sprite.texture.height);
        Rect spriteRect = sprite.rect;
        Texture2D myTexture2D = new Texture2D((int)spriteRect.width, (int)spriteRect.height);

        // Copy the pixels from the RenderTexture to the new Texture
        Rect tempRect = spriteRect;
        tempRect.y = sprite.texture.height - tempRect.y - spriteRect.height;

        myTexture2D.ReadPixels(tempRect, 0, 0);
        myTexture2D.Apply();

        // Release the temporary RenderTexture
        RenderTexture.ReleaseTemporary(tmp);

       // RenderTexture.active = null;
        RenderTexture.active = previous;

        return(myTexture2D);
    }

   static public Sprite Blur(Sprite image, int radius, int iterations, Color color){
        color.a = 1;
        Debug.Log(radius + " " + iterations);
        Texture2D texture = new Texture2D((int)image.rect.width * 2, (int)image.rect.height * 2);

        Texture2D myTexture = GetTextureFromSprite(image);

       for(int x = 0; x < texture.width; x++) {
            for(int y = 0; y < texture.height; y++) {
                texture.SetPixel(x, y, new Color(0, 0, 0, 0));
            }
        }

        int offsetX = texture.width / 4;
        int offsetY = texture.height / 4;
        for(int x = 0; x < (int)image.rect.width; x++) {
            for(int y = 0; y < (int)image.rect.height; y++) {
                Color ccolor = myTexture.GetPixel(x + (int)image.rect.x, y + (int)image.rect.y);
                if (ccolor.a > 0.01f) {
                     texture.SetPixel(x + offsetX, y + offsetY, color);
                }
                //texture.SetPixel(x + texture.width / 4, y + texture.height/ 4, image.GetPixel(x, y));
            }
        }
        texture.Apply();

        var tex = texture;
        _windowSize = radius * 2 + 1;
        _sourceWidth = texture.width;
        _sourceHeight = texture.height;

        for (var i = 0; i < iterations; i++) {
            //tex = OneDimensialBlur(tex, radius, true);
            //tex = OneDimensialBlur(tex, radius, false);
        }

        Vector2 pivot = new Vector2(image.pivot.x / image.rect.width, image.pivot.y / image.rect.height);
        
        float sizeX = pivot.x - 0.5f;
        float sizeY = pivot.y - 0.5f;

        float rot = Mathf.Atan2(sizeY, sizeX);
        float dist = Mathf.Sqrt(sizeX * sizeX + sizeY * sizeY);
        dist *= 0.5f;

        pivot.x = 0.5f + Mathf.Cos(rot) * dist;
        pivot.y = 0.5f + Mathf.Sin(rot) * dist;

        return(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivot, image.pixelsPerUnit));
    }

    static private Texture2D OneDimensialBlur(Texture2D image, int radius, bool horizontal) {
        _sourceImage = image;

        var blurred = new Texture2D(image.width, image.height, image.format, false);

        if (horizontal) {
            for (int imgY = 0; imgY < _sourceHeight; ++imgY) {
                ResetSum();

                for (int imgX = 0; imgX < _sourceWidth; imgX++) {
                    if (imgX == 0) {
                        for (int x = radius * -1; x <= radius; ++x) {
                            AddPixel(GetPixelWithXCheck(x, imgY));
                        }
                    } else {
                        var toExclude = GetPixelWithXCheck(imgX - radius - 1, imgY);
                        var toInclude = GetPixelWithXCheck(imgX + radius, imgY);

                        SubstPixel(toExclude);
                        AddPixel(toInclude);
                    }

                    blurred.SetPixel(imgX, imgY, CalcPixelFromSum());
                }
            }
        } else {
            for (int imgX = 0; imgX < _sourceWidth; imgX++) {
                ResetSum();

                for (int imgY = 0; imgY < _sourceHeight; ++imgY) {
                    if (imgY == 0) {
                        for (int y = radius * -1; y <= radius; ++y) {
                            AddPixel(GetPixelWithYCheck(imgX, y));
                        }
                    } else {
                        var toExclude = GetPixelWithYCheck(imgX, imgY - radius - 1);
                        var toInclude = GetPixelWithYCheck(imgX, imgY + radius);

                        SubstPixel(toExclude);
                        AddPixel(toInclude);
                    }

                    blurred.SetPixel(imgX, imgY, CalcPixelFromSum());
                }
            }
        }

        blurred.Apply();
        return blurred;
    }

   static  private Color GetPixelWithXCheck(int x, int y) {
        if (x <= 0) return _sourceImage.GetPixel(0, y);
        if (x >= _sourceWidth) return _sourceImage.GetPixel(_sourceWidth - 1, y);
        return _sourceImage.GetPixel(x, y);
    }

   static  private Color GetPixelWithYCheck(int x, int y) {
        if (y <= 0) return _sourceImage.GetPixel(x, 0);
        if (y >= _sourceHeight) return _sourceImage.GetPixel(x, _sourceHeight - 1);
        return _sourceImage.GetPixel(x, y);
    }

    static private void AddPixel(Color pixel) {
        _rSum += pixel.r;
        _gSum += pixel.g;
        _bSum += pixel.b;
        _aSum += pixel.a;
    }

   static private void SubstPixel(Color pixel) {
        _rSum -= pixel.r;
        _gSum -= pixel.g;
        _bSum -= pixel.b;
        _aSum -= pixel.a;
    }

   static  private void ResetSum() {
        _rSum = 0.0f;
        _gSum = 0.0f;
        _bSum = 0.0f;
        _aSum = 0.0f;
    }

    static Color CalcPixelFromSum() {
        return new Color(_rSum / _windowSize, _gSum / _windowSize, _bSum / _windowSize, _aSum / _windowSize);
    }
}