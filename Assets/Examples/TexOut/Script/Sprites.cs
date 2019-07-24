using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//public enum ItemType
//{
//    Undefine = -1,
//    Beauty = 0,
//    CommonFilter,
//    Makeup,
//    Animoji,
//    ItemSticker,
//    ARMask,
//    ChangeFace,
//    ExpressionRecognition,
//    MusicFilter,
//    BackgroundSegmentation,
//    GestureRecognition,
//    MagicMirror,
//    PortraitLightEffect,
//    PortraitDrive,
//    //...更多类型道具请咨询技术支持
//}

//Texout场景所需的UI合集，配合ItemConfig使用，这个组件只是为了方便，不具备任何实际编程价值，请谨慎参考

public class Sprites : MonoBehaviour {

    public Sprite[] Beauty;
    public Sprite[] CommonFilter;
    public Sprite[] Makeup;
    public Sprite[] Animoji;
    public Sprite[] ItemSticker;
    public Sprite[] ARMask;
    public Sprite[] ChangeFace;
    public Sprite[] ExpressionRecognition;
    public Sprite[] MusicFilter;
    public Sprite[] BackgroundSegmentation;
    public Sprite[] GestureRecognition;
    public Sprite[] MagicMirror;
    public Sprite[] PortraitLightEffect;
    public Sprite[] PortraitDrive;

    Dictionary<ItemType,Sprite[]> spritelist = new Dictionary<ItemType,Sprite[]>();

    public Texture2D[] MakeupBlush;
    public Texture2D[] MakeupEyebrow;
    public Texture2D[] MakeupEyeshadow;
    Dictionary<MakeupType, Texture2D[]> texlist = new Dictionary<MakeupType, Texture2D[]>();

    private void Awake()
    {
        spritelist.Clear();

        spritelist.Add(ItemType.Beauty, Beauty);
        spritelist.Add(ItemType.CommonFilter, CommonFilter);
        spritelist.Add(ItemType.Makeup, Makeup);
        spritelist.Add(ItemType.Animoji, Animoji);
        spritelist.Add(ItemType.ItemSticker, ItemSticker);
        spritelist.Add(ItemType.ARMask, ARMask);
        spritelist.Add(ItemType.ChangeFace, ChangeFace);
        spritelist.Add(ItemType.ExpressionRecognition, ExpressionRecognition);
        spritelist.Add(ItemType.MusicFilter, MusicFilter);
        spritelist.Add(ItemType.BackgroundSegmentation, BackgroundSegmentation);
        spritelist.Add(ItemType.GestureRecognition, GestureRecognition);
        spritelist.Add(ItemType.MagicMirror, MagicMirror);
        spritelist.Add(ItemType.PortraitLightEffect, PortraitLightEffect);
        spritelist.Add(ItemType.PortraitDrive, PortraitDrive);

        texlist.Add(MakeupType.Blush, MakeupBlush);
        texlist.Add(MakeupType.Eyebrow, MakeupEyebrow);
        texlist.Add(MakeupType.Eyeshadow, MakeupEyeshadow);
    }


    /**\brief 获取Sprite\param type 道具类型\param id 道具ID\return Sprite    */
    public Sprite GetSprite(ItemType type,int id)
    {
        if (spritelist.ContainsKey(type))
        {
            var sprites = spritelist[type];
            if (id >= 0 && id < sprites.Length)
                return sprites[id];
            else
                return null;
        }
        else
            return null;
    }

    /**\brief 获取Texture2D，主要是给美妆用\param type 美妆类型\param id 美妆ID\return Texture2D*/
    public Texture2D GetTexture(MakeupType type, int id)
    {
        if (texlist.ContainsKey(type))
        {
            var tex = texlist[type];
            if (id >= 0 && id < tex.Length)
                return tex[id];
            else
                return null;
        }
        else
            return null;
    }

}
