using UnityEngine;
using System.Collections.Generic;

public enum ShopCategory
{
    TilemapDesign = 0,
    PlayerCostume = 1,
    PlayerAccessory = 2
}
[System.Serializable]
public class ShopItemData
{
    public int id;
    public string name;      
    public string description;
    public int price;         
    public ShopCategory category; 
    public ShopItemState state = ShopItemState.Unpurchased;
    public float scales;
    public float positionX;
    public float positionY;
}
[System.Serializable]
public class ShopItemListWrapper
{
    public List<ShopItemData> list;
}