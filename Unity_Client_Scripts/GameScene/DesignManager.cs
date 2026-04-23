using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

/// <summary>
/// A vizuális témák és játékos kinézet kezelését végző osztály.
/// Kezeli a shop itemekből betöltött skin-eket és egyéb vizuális testreszabásokat.
/// </summary>
public class DesignManager : MonoBehaviour
{
    #region Konstansok

    private const string ShopItemsSaveFileName = "shopitems_save.json";
    private const string ShopItemsResourcePath = "shopitems";

    #endregion

    #region Unity Inspector Mezők

    [Header("Játékosok")]
    public SpriteRenderer PlayerA;
    public SpriteRenderer PlayerB;
    public SpriteRenderer AccessoryPlayerA;
    public SpriteRenderer AccessoryPlayerB;
    public Transform AccessoryParentA;
    public Transform AccessoryParentB;
    public RuleTile WallTile;
    public RuleTile FloorTile;

    [Header("Játékos Skinek")]
    public List<Sprite> PlayerSkins;

    [Header("Accessories")]
    public List<Sprite> Accessories;

    [Header("Tilemap Wall Sprites")]
    public List<Sprite> TilemapWalls;

    [Header("Tilemap Floor Sprites")]
    public List<Sprite> TilemapFloors;

    #endregion

    #region Privát Mezők
    private List<ShopItemData> ShopItems = new List<ShopItemData>();
   
    #endregion
    #region Unity Életciklus Metódusok

    /// <summary>
    /// Inicializálja a design managert.
    /// </summary>
    void Awake()
    {
        LoadShopItems();
        ApplyTilemap();
       

    }
    void Start()
    {
        
        ApplySelectedSkins();
        ApplyAccessories();
    }

    #endregion

    #region Shop Itemek Betöltése

    /// <summary>
    /// Betölti a shop itemeket helyi mentésből vagy Resources-ból.
    /// </summary>
    private void LoadShopItems()
    {
         string localPath = GetShopItemsSavePath();
        
        if (HasLocalSaveFile(localPath))
        {
            LoadShopItemsFromLocalSave(localPath);
        }
        else
        {
            LoadShopItemsFromResources();
        } 
    }

    /// <summary>
    /// Visszaadja a shop itemek mentési útvonalát.
    /// </summary>
    private string GetShopItemsSavePath()
    {
        return Path.Combine(Application.persistentDataPath, ShopItemsSaveFileName);
    }

    /// <summary>
    /// Ellenőrzi, hogy van-e helyi mentés.
    /// </summary>
    private bool HasLocalSaveFile(string path)
    {
        return File.Exists(path);
    }

    /// <summary>
    /// Betölti a shop itemeket a helyi mentésből.
    /// </summary>
    private void LoadShopItemsFromLocalSave(string path)
    {
        string json = File.ReadAllText(path);
        DeserializeShopItems(json);
        LogLocalLoadSuccess();
    }

    /// <summary>
    /// Deserializálja a shop itemeket JSON-ből.
    /// </summary>
    private void DeserializeShopItems(string json)
    {
        ShopItemListWrapper wrapper = JsonUtility.FromJson<ShopItemListWrapper>(json);
        if (wrapper != null)
        {
            ShopItems = wrapper.list;
        }
    }

    /// <summary>
    /// Logolja a helyi betöltés sikerességét.
    /// </summary>
    private void LogLocalLoadSuccess()
    {
        Debug.Log("📂 Shop Items betöltve a helyi mentésből.");
    }

    /// <summary>
    /// Betölti a shop itemeket a Resources mappából.
    /// </summary>
    private void LoadShopItemsFromResources()
    {
        TextAsset jsonFile = LoadShopItemsResource();
        
        if (jsonFile != null)
        {
            ProcessResourceFile(jsonFile);
        }
        else
        {
            LogResourceLoadError();
        }
    }

    /// <summary>
    /// Betölti a shop items resource fájlt.
    /// </summary>
    private TextAsset LoadShopItemsResource()
    {
        return Resources.Load<TextAsset>(ShopItemsResourcePath);
    }

    /// <summary>
    /// Feldolgozza a resource fájlt.
    /// </summary>
    private void ProcessResourceFile(TextAsset jsonFile)
    {
        DeserializeShopItems(jsonFile.text);
        LogResourceLoadSuccess();
    }

    /// <summary>
    /// Logolja a resource betöltés sikerességét.
    /// </summary>
    private void LogResourceLoadSuccess()
    {
        Debug.Log("🆕 Alap shop itemek betöltve.");
    }

    /// <summary>
    /// Logolja a resource betöltési hibát.
    /// </summary>
    private void LogResourceLoadError()
    {
        Debug.LogError("❌ Nem található az shopitems.json a Resources mappában!");
    }

    #endregion
    #region Skin Alkalmazás 

    /// <summary>
    /// Alkalmazza a kiválasztott skin-eket a játékosokra.
    /// </summary>
    private void ApplySelectedSkins()
    {
        ShopItemData appliedCostume = GetAppliedCostume();
        
        if (appliedCostume != null)
        {
            ApplyCostumeToPlayers(appliedCostume);
        }
        else
        {
            ApplyDefaultSkin();
        }
    }

    /// <summary>
    /// Visszaadja az alkalmazott costume itemet.
    /// </summary>
    private ShopItemData GetAppliedCostume()
    {
        return ShopItems.FirstOrDefault(item => 
            item.category == ShopCategory.PlayerCostume && 
            item.state == ShopItemState.Applied);
    }

    /// <summary>
    /// Alkalmazza a costume-ot a játékosokra.
    /// </summary>
    private void ApplyCostumeToPlayers(ShopItemData costume)
    {
        int costumeIndex = costume.id-10;
        if (costumeIndex >= 0 && costumeIndex < PlayerSkins.Count)
        {
            Sprite selectedSkin = PlayerSkins[costumeIndex];
            SetRendererSprite(PlayerA, selectedSkin);
            SetRendererSprite(PlayerB, selectedSkin);

        }
    }

    

    /// <summary>
    /// Alkalmazza az alapértelmezett skin-t.
    /// </summary>
    private void ApplyDefaultSkin()
    {
        if (PlayerSkins.Count > 0)
        {
            Sprite defaultSkin = PlayerSkins[0];
            SetRendererSprite(PlayerA, defaultSkin);
            SetRendererSprite(PlayerB, defaultSkin);
        }
    }

    #endregion
    #region Accessories Betöltése és Alkalmazása
    /// <summary>
    /// Alkalmazza a kiválasztott accessory-kat a játékosokra.
    /// </summary>
    private void ApplyAccessories()
    {
        
        ShopItemData appliedAccessory = GetAppliedAccessory();
        
        if (appliedAccessory != null)
        {
            
            ApplyAccessoryToPlayers(appliedAccessory);
            ScaleAndShiftAccessory(appliedAccessory);
        }
        else
        {
            ApplyDefaultAccessory();
        }
    }
    /// <summary>
    /// Visszaadja az alkalmazott accessory itemet.
    /// </summary>
    private ShopItemData GetAppliedAccessory()
    {
        
        return ShopItems.FirstOrDefault(item => 
            item.category == ShopCategory.PlayerAccessory && 
            item.state == ShopItemState.Applied);
    }
    /// <summary>
    /// Alkalmazza az accessory-t a játékosokra.
    /// </summary>
    private void ApplyAccessoryToPlayers(ShopItemData accessory)
    {
        
        int accessoryIndex = accessory.id - 20;
        if (accessoryIndex >= 0 && accessoryIndex < Accessories.Count)
        {
            
            Sprite selectedAccessory = Accessories[accessoryIndex];
            SetRendererSprite(AccessoryPlayerA, selectedAccessory);
            SetRendererSprite(AccessoryPlayerB, selectedAccessory);
        }
    }
    /// <summary>
    /// Alkalmazza az alapértelmezett accessory-t.
    /// </summary>
    private void ApplyDefaultAccessory()
    {
        
        if (PlayerSkins.Count > 0)
        {
            Sprite defaultAccessory = null;
            SetRendererSprite(AccessoryPlayerA, defaultAccessory);
            SetRendererSprite(AccessoryPlayerB, defaultAccessory);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    private void ScaleAndShiftAccessory(ShopItemData appliedAccessoryData)
    {
        float scale = appliedAccessoryData.scales;
        float posX = appliedAccessoryData.positionX;
        float posY = appliedAccessoryData.positionY;
        AccessoryParentA.localScale = new Vector3(scale, scale, 1f);
        AccessoryParentB.localScale = new Vector3(scale, scale, 1f);
        //Debug.Log($"Accessory pos input: x={posX}, y={posY}");
        AccessoryParentA.localPosition = new Vector3(posX, posY, AccessoryParentA.localPosition.z);
        AccessoryParentB.localPosition = new Vector3(posX, posY, AccessoryParentB.localPosition.z);
    }
    
    #endregion
    #region Közös Metódusok
    /// <summary>
    /// Beállítja a renderer sprite-ját.
    /// </summary>
    private void SetRendererSprite(SpriteRenderer renderer, Sprite sprite)
    {
        if (renderer != null && sprite != null)
        {
            renderer.sprite = sprite;
            
        }
    }
    
    #endregion
    #region Tilemap Design Alkalmazás 

    /// <summary>
    /// Alkalmazza a kiválasztott tilemap design témát.
    /// </summary>
    private void ApplyTilemap()
    {
        ShopItemData appliedTile = GetAppliedTile();
        if(appliedTile != null )
        {
            ApplySpriteToTiles(appliedTile);
        }
        else
        {
            ApplyDefaultTiles();
        }
    }
    ///<summary>
    ///Visszaadja az alkalmazott tilemap itemet.
    ///</summary>
    private ShopItemData GetAppliedTile()
    {
        return ShopItems.FirstOrDefault(item=>
            item.category ==ShopCategory.TilemapDesign &&
            item.state == ShopItemState.Applied);
    }
    /// <summary>
    /// Alkalmazza a tilemap design-t a falakra és padlókra.
    /// </summary>
    private void ApplySpriteToTiles(ShopItemData tilemapDesign)
    {
        
        int designIndex = tilemapDesign.id;
        if (designIndex >= 0 && designIndex < TilemapWalls.Count && designIndex < TilemapFloors.Count)
        {
            Debug.Log($"Applying tilemap design with index: {designIndex}");
            Sprite wallSprite = TilemapWalls[designIndex];
            Sprite floorSprite = TilemapFloors[designIndex];
            WallTile.m_DefaultSprite = wallSprite;
            FloorTile.m_DefaultSprite = floorSprite;
        }
    }
    /// <summary>
    /// Alkalmazza az alapértelmezett tilemap design-t.
    /// </summary>
    private void ApplyDefaultTiles()
    {
        if (TilemapWalls.Count > 0&& TilemapFloors.Count>0)
        {
            Sprite defaultWall = TilemapWalls[0];
            Sprite defaultFloor = TilemapFloors[0];
            WallTile.m_DefaultSprite = defaultWall;
            FloorTile.m_DefaultSprite = defaultFloor;
        }
    }
    #endregion
}
