using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using ApiForGame;

/// <summary>
/// A bolt (shop) kezelését végző osztály.
/// Kezeli az itemek betöltését, UI generálását, vásárlásokat és kategória navigációt.
/// </summary>
public class ShopManager : MonoBehaviour
{
    #region Konstansok

    private const string ShopItemsSaveFileName = "shopitems_save.json";
    private const string ShopItemsResourcePath = "shopitems";
    private const string ShopIconsResourcePath = "ShopIcons/item_";
    private const string CoinDisplayFormat = "{0} $";
    private const string DefaultCoinDisplay = "0";
    private const string ColorTagFormat = "<color={0}>{1}</color>";
    private const string ColorBlack = "black";
    private const string StatusUsed = "USED";
    private const string StatusOwned = "OWNED";
    private const string CategoryNameTilemapDesign = "Tilemap Themes";
    private const string CategoryNameCostumes = "Costumes";
    private const string CategoryNameAccessories = "Accessories";
    private const string UIElementNameStatusIcon = "StatusIcon";
    private const string UIElementNameDescription = "Description";
    private const string UIElementNameAmount = "Amount";
    private const float CategoryScrollOffsetY = 50f;
    private const float ScrollAnimationDuration = 0.3f;
    private const float AppliedItemColorR = 0.8f;
    private const float AppliedItemColorG = 1f;
    private const float AppliedItemColorB = 0.8f;
    private const float PurchasedItemColorR = 1f;
    private const float PurchasedItemColorG = 1f;
    private const float PurchasedItemColorB = 0.8f;
    private const float UnpurchasedItemColorR = 0.7f;
    private const float UnpurchasedItemColorG = 0.7f;
    private const float UnpurchasedItemColorB = 0.7f;
    private const float UnpurchasedItemColorA = 0.5f;

    #endregion

    #region Unity Inspector Mezők

    [Header("UI Elemek")]
    public TextMeshProUGUI coinText;
    public RectTransform contentContainer;
    public ScrollRect scrollRect;

    [Header("Prefabok")]
    public GameObject itemPrefab;
    public GameObject headerPrefab;

    [Header("Adatok")]
    public List<ShopItemData> ShopItems = new List<ShopItemData>();

    [Header("Könyvjelző Gombok")]
    public List<RectTransform> categoryButtons;

    [Header("Animáció")]
    public float activeYPosition = -100f;
    public float inactiveYPosition = 0f;
    public float animationSpeed = 10f;

    #endregion

    #region Privát Mezők

    private Dictionary<ShopCategory, RectTransform> categoryAnchors = new Dictionary<ShopCategory, RectTransform>();

    #endregion

    #region Unity Életciklus Metódusok

    /// <summary>
    /// Inicializálja a boltot: coin UI, itemek betöltése és lista generálása.
    /// </summary>
    void Start()
    {
        InitializeShop();
    }

    /// <summary>
    /// Frissíti az aktív kategória vizualizációját.
    /// </summary>
    void Update()
    {
        UpdateActiveCategoryHighlight();
    }

    #endregion

    #region Inicializálás

    /// <summary>
    /// Inicializálja a bolt összes elemét.
    /// </summary>
    private void InitializeShop()
    {
        UpdateCoinUI();
        LoadItems();
        GenerateList();
    }

    #endregion

    #region Coin UI Kezelés

    /// <summary>
    /// Frissíti a coin UI megjelenítését.
    /// </summary>
    public void UpdateCoinUI()
    {
        if (IsGameDataValid())
        {
            DisplayCurrentCoins();
        }
        else
        {
            DisplayDefaultCoins();
        }
    }

    /// <summary>
    /// Ellenőrzi, hogy a GameDataManager és profil érvényes-e.
    /// </summary>
    private bool IsGameDataValid()
    {
        return GameDataManager.Instance != null &&
               GameDataManager.Instance.currentProfile != null;
    }

    /// <summary>
    /// Megjeleníti az aktuális coin mennyiséget.
    /// </summary>
    private void DisplayCurrentCoins()
    {
        int currentCoins = GetCurrentCoins();
        SetCoinText(string.Format(CoinDisplayFormat, currentCoins));
    }

    /// <summary>
    /// Visszaadja az aktuális coin mennyiséget.
    /// </summary>
    private int GetCurrentCoins()
    {
        return GameDataManager.Instance.currentProfile.coins;
    }

    /// <summary>
    /// Beállítja a coin szöveg értékét.
    /// </summary>
    private void SetCoinText(string text)
    {
        if (coinText != null)
        {
            coinText.text = text;
        }
    }

    /// <summary>
    /// Megjeleníti az alapértelmezett coin értéket.
    /// </summary>
    private void DisplayDefaultCoins()
    {
        SetCoinText(DefaultCoinDisplay);
    }

    #endregion

    #region Item Betöltés és Mentés

    /// <summary>
    /// Betölti a shop itemeket helyi mentésből vagy Resources-ból.
    /// </summary>
    void LoadItems()
    {
        string localPath = GetShopItemsSavePath();

        if (HasLocalSaveFile(localPath))
        {
            LoadItemsFromLocalSave(localPath);
        }
        else
        {
            LoadItemsFromResources();
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
    /// Betölti az itemeket a helyi mentésből.
    /// </summary>
    private void LoadItemsFromLocalSave(string path)
    {
        string json = File.ReadAllText(path);
        DeserializeShopItems(json);
        LogLocalLoadSuccess(path);
    }

    /// <summary>
    /// Deserializálja a shop itemeket JSON-ből.
    /// </summary>
    private void DeserializeShopItems(string json)
    {
        ShopItemListWrapper wrapper = JsonUtility.FromJson<ShopItemListWrapper>(json);
        ShopItems = wrapper.list;
    }

    /// <summary>
    /// Logolja a helyi betöltés sikerességét.
    /// </summary>
    private void LogLocalLoadSuccess(string path)
    {
        Debug.Log($"📂 Shop Items betöltve a helyi mentésből: {path}");
    }

    /// <summary>
    /// Betölti az itemeket a Resources mappából.
    /// </summary>
    private void LoadItemsFromResources()
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
        SaveShopItemsLocal();
        LogResourceLoadSuccess();
    }

    /// <summary>
    /// Logolja a resource betöltés sikerességét.
    /// </summary>
    private void LogResourceLoadSuccess()
    {
        Debug.Log("🆕 Alap shop itemek betöltve és mentve.");
    }

    /// <summary>
    /// Logolja a resource betöltési hibát.
    /// </summary>
    private void LogResourceLoadError()
    {
        Debug.LogError("❌ Nem található az shopitems.json a Resources mappában!");
    }

    /// <summary>
    /// Menti a shop itemek állapotát helyi fájlba.
    /// </summary>
    public void SaveShopItemsLocal()
    {
        string json = SerializeShopItems();
        string path = GetShopItemsSavePath();
        WriteShopItemsToFile(path, json);
    }

    /// <summary>
    /// Szerializálja a shop itemeket JSON formátumba.
    /// </summary>
    private string SerializeShopItems()
    {
        ShopItemListWrapper wrapper = CreateShopItemsWrapper();
        return JsonUtility.ToJson(wrapper, true);
    }

    /// <summary>
    /// Létrehoz egy wrapper objektumot a shop itemekhez.
    /// </summary>
    private ShopItemListWrapper CreateShopItemsWrapper()
    {
        ShopItemListWrapper wrapper = new ShopItemListWrapper();
        wrapper.list = ShopItems;
        return wrapper;
    }

    /// <summary>
    /// Kiírja a shop items JSON-t fájlba.
    /// </summary>
    private void WriteShopItemsToFile(string path, string json)
    {
        File.WriteAllText(path, json);
    }

    #endregion

    #region Lista Generálás

    /// <summary>
    /// Generálja a teljes shop UI listát kategóriákkal.
    /// </summary>
    void GenerateList()
    {
        ClearExistingList();
        GenerateCategoriesAndItems();
    }

    /// <summary>
    /// Törli a meglévő lista tartalmát.
    /// </summary>
    private void ClearExistingList()
    {
        DestroyAllChildren();
        ClearCategoryAnchors();
    }

    /// <summary>
    /// Törli az összes child objektumot a content containerből.
    /// </summary>
    private void DestroyAllChildren()
    {
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Törli a kategória anchor pozíciókat.
    /// </summary>
    private void ClearCategoryAnchors()
    {
        categoryAnchors.Clear();
    }

    /// <summary>
    /// Generálja a kategóriákat és hozzájuk tartozó itemeket.
    /// </summary>
    private void GenerateCategoriesAndItems()
    {
        foreach (ShopCategory category in System.Enum.GetValues(typeof(ShopCategory)))
        {
            CreateCategoryHeader(category);
            CreateCategoryItems(category);
        }
    }

    /// <summary>
    /// Létrehozza egy kategória header-ét.
    /// </summary>
    private void CreateCategoryHeader(ShopCategory category)
    {
        GameObject header = InstantiateCategoryHeader();
        SetCategoryHeaderText(header, category);
        RegisterCategoryAnchor(category, header);
    }

    /// <summary>
    /// Példányosítja a kategória header prefab-ot.
    /// </summary>
    private GameObject InstantiateCategoryHeader()
    {
        return Instantiate(headerPrefab, contentContainer);
    }

    /// <summary>
    /// Beállítja a kategória header szövegét.
    /// </summary>
    private void SetCategoryHeaderText(GameObject header, ShopCategory category)
    {
        TMP_Text headerText = header.GetComponentInChildren<TMP_Text>();
        if (headerText != null)
        {
            headerText.text = FormatCategoryName(category);
        }
    }

    /// <summary>
    /// Regisztrálja a kategória anchor pozícióját.
    /// </summary>
    private void RegisterCategoryAnchor(ShopCategory category, GameObject header)
    {
        categoryAnchors.Add(category, header.GetComponent<RectTransform>());
    }

    /// <summary>
    /// Létrehozza egy kategória összes item-ét.
    /// </summary>
    private void CreateCategoryItems(ShopCategory category)
    {
        List<ShopItemData> itemsInCategory = GetItemsInCategory(category);

        foreach (ShopItemData item in itemsInCategory)
        {
            CreateItemGameObject(item);
        }
    }

    /// <summary>
    /// Visszaadja egy kategóriához tartozó itemeket.
    /// </summary>
    private List<ShopItemData> GetItemsInCategory(ShopCategory category)
    {
        return ShopItems.Where(x => x.category == category).ToList();
    }

    /// <summary>
    /// Formázza a kategória nevét megjelenítéshez.
    /// </summary>
    private string FormatCategoryName(ShopCategory category)
    {
        switch (category)
        {
            case ShopCategory.TilemapDesign: return CategoryNameTilemapDesign;
            case ShopCategory.PlayerCostume: return CategoryNameCostumes;
            case ShopCategory.PlayerAccessory: return CategoryNameAccessories;
            default: return category.ToString();
        }
    }

    #endregion

    #region Item GameObject Létrehozás

    /// <summary>
    /// Létrehoz egy shop item UI elemet.
    /// </summary>
    void CreateItemGameObject(ShopItemData item)
    {
        GameObject newItem = InstantiateItemPrefab();
        SetupItemIcon(newItem, item);
        SetupItemDescription(newItem, item);
        SetupItemAmount(newItem, item);
        SetupItemButton(newItem, item);
    }

    /// <summary>
    /// Példányosítja az item prefab-ot.
    /// </summary>
    private GameObject InstantiateItemPrefab()
    {
        return Instantiate(itemPrefab, contentContainer);
    }

    /// <summary>
    /// Beállítja az item ikon képét és színét.
    /// </summary>
    private void SetupItemIcon(GameObject itemObject, ShopItemData item)
    {
        Transform iconObj = itemObject.transform.Find(UIElementNameStatusIcon);
        if (iconObj != null)
        {
            Image iconImg = iconObj.GetComponent<Image>();
            if (iconImg != null)
            {
                LoadAndSetItemSprite(iconImg, item.id);
                SetItemIconProperties(iconImg, item.state);
            }
        }
    }

    /// <summary>
    /// Betölti és beállítja az item sprite-ját.
    /// </summary>
    private void LoadAndSetItemSprite(Image iconImg, int itemId)
    {
        Sprite loadedSprite = LoadItemSprite(itemId);

        if (loadedSprite != null)
        {
            iconImg.sprite = loadedSprite;
        }
    }

    /// <summary>
    /// Betölti az item sprite-ját a Resources-ból.
    /// </summary>
    private Sprite LoadItemSprite(int itemId)
    {
        return Resources.Load<Sprite>($"{ShopIconsResourcePath}{itemId}");
    }

    /// <summary>
    /// Beállítja az ikon tulajdonságait állapot szerint.
    /// </summary>
    private void SetItemIconProperties(Image iconImg, ShopItemState state)
    {
        iconImg.preserveAspect = true;
        iconImg.color = GetIconColorByState(state);
    }

    /// <summary>
    /// Visszaadja az ikon színét az item állapota szerint.
    /// </summary>
    private Color GetIconColorByState(ShopItemState state)
    {
        switch (state)
        {
            case ShopItemState.Applied:
                return new Color(AppliedItemColorR, AppliedItemColorG, AppliedItemColorB);
            default:
                return Color.white;
        }
    }

    /// <summary>
    /// Beállítja az item leírás szövegét.
    /// </summary>
    private void SetupItemDescription(GameObject itemObject, ShopItemData item)
    {
        Transform descObj = itemObject.transform.Find(UIElementNameDescription);
        if (descObj != null)
        {
            descObj.GetComponent<TMP_Text>().text = item.name;
        }
    }

    /// <summary>
    /// Beállítja az item ár/státusz szövegét.
    /// </summary>
    private void SetupItemAmount(GameObject itemObject, ShopItemData item)
    {
        Transform amountObj = itemObject.transform.Find(UIElementNameAmount);
        if (amountObj != null)
        {
            string displayText = GetItemAmountText(item);
            string formattedText = FormatTextWithColor(displayText, ColorBlack);
            amountObj.GetComponent<TMP_Text>().text = formattedText;
        }
    }

    /// <summary>
    /// Visszaadja az item ár/státusz szövegét.
    /// </summary>
    private string GetItemAmountText(ShopItemData item)
    {
        if (item.state == ShopItemState.Unpurchased)
        {
            return $"{item.price} $";
        }
        else if (item.state == ShopItemState.Applied)
        {
            return StatusUsed;
        }
        else
        {
            return StatusOwned;
        }
    }

    /// <summary>
    /// Formázza a szöveget színnel.
    /// </summary>
    private string FormatTextWithColor(string text, string color)
    {
        return string.Format(ColorTagFormat, color, text);
    }

    /// <summary>
    /// Beállítja az item gomb funkcionalitását és megjelenését.
    /// </summary>
    private void SetupItemButton(GameObject itemObject, ShopItemData item)
    {
        Button btn = itemObject.GetComponent<Button>();
        Image bg = itemObject.GetComponent<Image>();

        if (btn != null)
        {
            AddItemClickListener(btn, item, itemObject);
            SetItemBackgroundColor(bg, item.state);
        }
    }

    /// <summary>
    /// Hozzáadja a klikk eseménykezelőt az item gombhoz.
    /// </summary>
    private void AddItemClickListener(Button btn, ShopItemData item, GameObject itemObject)
    {
        btn.onClick.AddListener(() => OnShopItemClicked(item, itemObject));
    }

    /// <summary>
    /// Beállítja az item háttérszínét az állapot szerint.
    /// </summary>
    private void SetItemBackgroundColor(Image bg, ShopItemState state)
    {
        if (bg != null)
        {
            bg.color = GetBackgroundColorByState(state);
        }
    }

    /// <summary>
    /// Visszaadja a háttérszínt az item állapota szerint.
    /// </summary>
    private Color GetBackgroundColorByState(ShopItemState state)
    {
        switch (state)
        {
            case ShopItemState.Applied:
                return new Color(AppliedItemColorR, AppliedItemColorG, AppliedItemColorB);
            case ShopItemState.Purchased:
                return new Color(PurchasedItemColorR, PurchasedItemColorG, PurchasedItemColorB);
            default:
                return new Color(UnpurchasedItemColorR, UnpurchasedItemColorG, UnpurchasedItemColorB, UnpurchasedItemColorA);
        }
    }

    #endregion

    #region Vásárlás Kezelés

    /// <summary>
    /// Kezeli a shop item kattintást.
    /// </summary>
    void OnShopItemClicked(ShopItemData item, GameObject itemObject)
    {
        if (IsItemUnpurchased(item))
        {
            HandlePurchaseAttempt(item);
        }
        else if (IsItemPurchased(item))
        {
            HandleEquipItem(item);
        }
        else if (IsItemApplied(item))
        {
            HandleUnequipItem(item);
        }
    }

    /// <summary>
    /// Ellenőrzi, hogy az item még nincs megvéve.
    /// </summary>
    private bool IsItemUnpurchased(ShopItemData item)
    {
        return item.state == ShopItemState.Unpurchased;
    }

    /// <summary>
    /// Ellenőrzi, hogy az item megvéve van, de nincs felszerelve.
    /// </summary>
    private bool IsItemPurchased(ShopItemData item)
    {
        return item.state == ShopItemState.Purchased;
    }

    /// <summary>
    /// Ellenőrzi, hogy az item fel van szerelve.
    /// </summary>
    private bool IsItemApplied(ShopItemData item)
    {
        return item.state == ShopItemState.Applied;
    }

    /// <summary>
    /// Kezeli a vásárlási kísérletet.
    /// </summary>
    private void HandlePurchaseAttempt(ShopItemData item)
    {
        if (CanAffordItem(item))
        {
            ProcessPurchase(item);
        }
        else
        {
            LogInsufficientFunds();
        }
    }

    /// <summary>
    /// Ellenőrzi, hogy a játékosnak van-e elég pénze.
    /// </summary>
    private bool CanAffordItem(ShopItemData item)
    {
        return IsGameDataValid() &&
               GameDataManager.Instance.currentProfile.coins >= item.price;
    }

    /// <summary>
    /// Feldolgozza a vásárlást.
    /// </summary>
    private void ProcessPurchase(ShopItemData item)
    {
        DeductItemPrice(item.price);
        
        SetItemAsPurchased(item);
        RefreshShopUI();
        LogPurchaseSuccess(item.name);
    }

    /// <summary>
    /// Levonja az item árát.
    /// </summary>
    private void DeductItemPrice(int price)
    {
        GameDataManager.Instance.AddCoins(-price);
    }

    /// <summary>
    /// Beállítja az item állapotát megvéve-re.
    /// </summary>
    private void SetItemAsPurchased(ShopItemData item)
    {
        item.state = ShopItemState.Purchased;
    }

    /// <summary>
    /// Frissíti a shop UI-t.
    /// </summary>
    private void RefreshShopUI()
    {
        SaveShopItemsLocal();
        UpdateCoinUI();
        GenerateList();
    }

    /// <summary>
    /// Logolja a sikeres vásárlást.
    /// </summary>
    private void LogPurchaseSuccess(string itemName)
    {
        Debug.Log($"Vásárlás sikeres: {itemName}");
    }

    /// <summary>
    /// Logolja az elégtelen pénzösszeget.
    /// </summary>
    private void LogInsufficientFunds()
    {
        Debug.Log("Nincs elég pénzed!");
    }

    /// <summary>
    /// Kezeli az item felszerelését.
    /// </summary>
    private void HandleEquipItem(ShopItemData item)
    {
        UnequipAllInCategory(item.category);
        SetItemAsApplied(item);
        RefreshShopUIAfterEquip();
        LogEquipSuccess(item.name);
    }

    /// <summary>
    /// Beállítja az item állapotát felszerelve-re.
    /// </summary>
    private void SetItemAsApplied(ShopItemData item)
    {
        item.state = ShopItemState.Applied;
    }

    /// <summary>
    /// Frissíti a shop UI-t felszerelés után.
    /// </summary>
    private void RefreshShopUIAfterEquip()
    {
        SaveShopItemsLocal();
        GenerateList();
    }

    /// <summary>
    /// Logolja a sikeres felszerelést.
    /// </summary>
    private void LogEquipSuccess(string itemName)
    {
        Debug.Log($"Felszerelve: {itemName}");
    }

    /// <summary>
    /// Kezeli az item levételét.
    /// </summary>
    private void HandleUnequipItem(ShopItemData item)
    {
        if (CanUnequipItem(item))
        {
            ProcessUnequip(item);
        }
    }

    /// <summary>
    /// Ellenőrzi, hogy az item levehető-e.
    /// </summary>
    private bool CanUnequipItem(ShopItemData item)
    {
        return item.category == ShopCategory.PlayerAccessory;
    }

    /// <summary>
    /// Feldolgozza az item levételét.
    /// </summary>
    private void ProcessUnequip(ShopItemData item)
    {
        SetItemAsPurchased(item);
        RefreshShopUIAfterUnequip();
        LogUnequipSuccess(item.name);
    }

    /// <summary>
    /// Frissíti a shop UI-t levétel után.
    /// </summary>
    private void RefreshShopUIAfterUnequip()
    {
        SaveShopItemsLocal();
        GenerateList();
    }

    /// <summary>
    /// Logolja a sikeres levételt.
    /// </summary>
    private void LogUnequipSuccess(string itemName)
    {
        Debug.Log($"Kiegészítő levéve: {itemName}");
    }

    /// <summary>
    /// Leveszi az összes felszerelt itemet egy kategóriában.
    /// </summary>
    void UnequipAllInCategory(ShopCategory category)
    {
        foreach (var item in ShopItems)
        {
            if (ShouldUnequipItem(item, category))
            {
                SetItemAsPurchased(item);
            }
        }
    }

    /// <summary>
    /// Ellenőrzi, hogy az itemet le kell-e venni.
    /// </summary>
    private bool ShouldUnequipItem(ShopItemData item, ShopCategory category)
    {
        return item.category == category && item.state == ShopItemState.Applied;
    }

    #endregion

    #region Kategória Navigáció

    /// <summary>
    /// Görget egy megadott kategóriához.
    /// </summary>
    public void ScrollToCategory(int categoryIndex)
    {
        ShopCategory category = (ShopCategory)categoryIndex;

        if (IsCategoryValid(category))
        {
            StartScrollAnimation(category);
        }
    }

    /// <summary>
    /// Ellenőrzi, hogy a kategória érvényes-e.
    /// </summary>
    private bool IsCategoryValid(ShopCategory category)
    {
        return categoryAnchors.ContainsKey(category) && scrollRect != null;
    }

    /// <summary>
    /// Elindítja a görgetési animációt.
    /// </summary>
    private void StartScrollAnimation(ShopCategory category)
    {
        StartCoroutine(SnapTo(categoryAnchors[category]));
    }

    /// <summary>
    /// Animáltan görget egy célponthoz.
    /// </summary>
    IEnumerator SnapTo(RectTransform target)
    {
        ForceCanvasUpdate();

        Vector2 targetPosition = CalculateTargetScrollPosition(target);
        yield return AnimateScrollToPosition(targetPosition);

        FinalizeScrollPosition(targetPosition);
    }

    /// <summary>
    /// Kényszeríti a canvas frissítését.
    /// </summary>
    private void ForceCanvasUpdate()
    {
        Canvas.ForceUpdateCanvases();
    }

    /// <summary>
    /// Kiszámolja a cél görgetési pozíciót.
    /// </summary>
    private Vector2 CalculateTargetScrollPosition(RectTransform target)
    {
        Vector2 finalPos = (Vector2)scrollRect.transform.InverseTransformPoint(contentContainer.position)
            - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);

        finalPos.x = 0;
        return new Vector2(0, finalPos.y);
    }

    /// <summary>
    /// Animálja a görgetést a cél pozícióhoz.
    /// </summary>
    private IEnumerator AnimateScrollToPosition(Vector2 targetPosition)
    {
        float elapsedTime = 0;
        Vector2 startPos = contentContainer.anchoredPosition;

        while (elapsedTime < ScrollAnimationDuration)
        {
            contentContainer.anchoredPosition = Vector2.Lerp(startPos, targetPosition, elapsedTime / ScrollAnimationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Véglegesíti a görgetési pozíciót.
    /// </summary>
    private void FinalizeScrollPosition(Vector2 targetPosition)
    {
        contentContainer.anchoredPosition = targetPosition;
    }

    #endregion

    #region Aktív Kategória Kijelzés

    /// <summary>
    /// Frissíti az aktív kategória kijelzését.
    /// </summary>
    void UpdateActiveCategoryHighlight()
    {
        if (!AreCategoryAnchorsValid())
            return;

        ShopCategory activeCategory = DetermineActiveCategory();
        UpdateButtonVisuals(activeCategory);
    }

    /// <summary>
    /// Ellenőrzi, hogy a kategória anchorok érvényesek-e.
    /// </summary>
    private bool AreCategoryAnchorsValid()
    {
        return categoryAnchors.Count > 0 && contentContainer != null;
    }

    /// <summary>
    /// Meghatározza az aktuálisan aktív kategóriát.
    /// </summary>
    private ShopCategory DetermineActiveCategory()
    {
        float currentScrollY = GetCurrentScrollPosition();
        ShopCategory activeCategory = ShopCategory.TilemapDesign;

        foreach (var entry in categoryAnchors)
        {
            if (IsCategoryActive(currentScrollY, entry.Value))
            {
                activeCategory = entry.Key;
            }
        }

        return activeCategory;
    }

    /// <summary>
    /// Visszaadja az aktuális görgetési pozíciót.
    /// </summary>
    private float GetCurrentScrollPosition()
    {
        return contentContainer.anchoredPosition.y;
    }

    /// <summary>
    /// Ellenőrzi, hogy a kategória aktív-e a jelenlegi görgetési pozíción.
    /// </summary>
    private bool IsCategoryActive(float currentScrollY, RectTransform headerRect)
    {
        float headerY = CalculateCategoryThreshold(headerRect);
        return currentScrollY >= headerY;
    }

    /// <summary>
    /// Kiszámolja a kategória küszöbértékét.
    /// </summary>
    private float CalculateCategoryThreshold(RectTransform headerRect)
    {
        return Mathf.Abs(headerRect.anchoredPosition.y) - CategoryScrollOffsetY;
    }

    /// <summary>
    /// Frissíti a kategória gombok vizuális megjelenését.
    /// </summary>
    void UpdateButtonVisuals(ShopCategory activeCategory)
    {
        for (int i = 0; i < categoryButtons.Count; i++)
        {
            UpdateSingleButtonVisual(categoryButtons[i], i, activeCategory);
        }
    }

    /// <summary>
    /// Frissíti egy gomb vizuális megjelenését.
    /// </summary>
    private void UpdateSingleButtonVisual(RectTransform btnRect, int buttonIndex, ShopCategory activeCategory)
    {
        float targetY = GetButtonTargetY(buttonIndex, activeCategory);
        AnimateButtonToPosition(btnRect, targetY);
    }

    /// <summary>
    /// Visszaadja a gomb cél Y pozícióját.
    /// </summary>
    private float GetButtonTargetY(int buttonIndex, ShopCategory activeCategory)
    {
        if ((int)activeCategory == buttonIndex)
        {
            return activeYPosition;
        }
        else
        {
            return inactiveYPosition;
        }
    }

    /// <summary>
    /// Animálja a gombot a cél pozícióhoz.
    /// </summary>
    private void AnimateButtonToPosition(RectTransform btnRect, float targetY)
    {
        Vector2 currentPos = btnRect.anchoredPosition;
        float newY = Mathf.Lerp(currentPos.y, targetY, Time.deltaTime * animationSpeed);
        btnRect.anchoredPosition = new Vector2(currentPos.x, newY);
    }

    #endregion
}
