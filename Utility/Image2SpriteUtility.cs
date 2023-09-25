using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Image2SpriteUtility : MonoBehaviour
{
    
    public static Image2SpriteUtility Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    public Sprite GetLongTablet()
    {
        return Resources.Load<Sprite>("tabletHorizontal");
    }
    public Sprite GetTallTablet()
    {
        return Resources.Load<Sprite>("tabletVertical");
    }

    public Sprite GetStatIconSprite(RankEnum stat)
    {
        Sprite sprite = null;
        switch (stat)
        {
            case RankEnum.Brawn:
                sprite = Resources.Load<Sprite>("redRock");
                break;
            case RankEnum.Art:
                sprite = Resources.Load<Sprite>("yellowRock");
                break;
            case RankEnum.Science:
                sprite = Resources.Load<Sprite>("greenRock");
                break;
            case RankEnum.Philosophy:
                sprite = Resources.Load<Sprite>("blueRock");
                break;
           
        }
        return sprite;
    }
    public Sprite GetSkillEffectIconSprite(EffectTypeEnum effectType)
    {
        Sprite sprite = null;
        switch (effectType)
        {
            case EffectTypeEnum.Strange:
                sprite = Resources.Load<Sprite>("strange");
                break;
                //case EffectTypeEnum.BonusEnergy:
                //    sprite = Resources.Load<Sprite>("inspiredStatus");
                //    break;
                //case EffectTypeEnum.GainExperience:
                //    sprite = Resources.Load<Sprite>("wisdomStatus");
                //    break;
                //case EffectTypeEnum.Pacify:
                //    sprite = Resources.Load<Sprite>("peaceIcon");
                //    break;
        }
        return sprite;
    }
    public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
    {

        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

        
        Texture2D SpriteTexture = LoadTexture(FilePath);
        Sprite NewSprite = Sprite.Create(
            SpriteTexture, 
            new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), 
            new Vector2(0, 0), 
            PixelsPerUnit);

        return NewSprite;
    }

    private Texture2D LoadTexture(string FilePath)
    {

        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails

        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }


}
