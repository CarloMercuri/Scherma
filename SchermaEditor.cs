using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

#if UNITY_EDITOR
public class SchermaEditor : EditorWindow
{



    private UnityEngine.Object spriteObj;
    private UnityEngine.Object armorSpriteObj;
    private UnityEngine.Object projectileSpriteObj;
    private UnityEngine.Object tileSpriteObj;
    private ItemType itemType;
    private UnityEngine.Object s1Obj;
    private UnityEngine.Object s2Obj;
    private UnityEngine.Object s3Obj;
    private UnityEngine.Object s4Obj;

    public Sprite armorSpriteTexture;
    public Sprite spriteTexture;
    public Sprite projectileSpriteTexture;

    private int currentSelectedItem = -1;
    private int currentSelectedTile = -1;
    private int currentSelectedTilePref = -1;
    private bool enumSortReverse = true; // start true because we're reversing when we click

    Vector2 scrollPos;

    public GUIContent DropButton;
    public string[] toolbarStrings = new string[] { "Item Editor", "Attributes", "Tiles", "Tile Prefs", "Auras", "Temp" };

    // 30 every 4 elements in toolbarStrings
    public int startY = 60; 

    public int toolbarInt = 0;
    public bool dataLoaded = false;

    private int itemTypesAmount = 18;
    private int curType;

    private Item[] tempList;
    private ItemType[] typesList;
    private int OrderSelection = 0;

    private static GUIStyle redTextStyle;
    private static GUIStyle objectFieldBig;
    private GUIStyle alignStyle;
    public static string[] methodsArray;

    // Databases
    ItemsDB itemsDatabase;
    TileProperties tileDatabase;





    [MenuItem("Scherma Editor/Main Editor")]
    public static void ShowWindow()
    {
        methodsArray = GetMethodsList(typeof(ItemScripts));
        CreateInput.SearchString = "";
        CreateInput.PopulateAll = false;
        EditorWindow ItemsListWindow = GetWindow<SchermaEditor>("Main Editor");
     

        redTextStyle = new GUIStyle();
        redTextStyle.normal.textColor = Color.red;

        objectFieldBig = new GUIStyle();
        objectFieldBig.fixedWidth = 80;
        objectFieldBig.fixedHeight = 80;

    }

    private void OnGUI()
    {

        if (dataLoaded == false)
        {
            if (GUILayout.Button("Load Data"))
            {
                ItemsDatabase.LoadFromDatabase();
                tileDatabase = Resources.Load<TileProperties>("Databases/TileDB");
                itemsDatabase = Resources.Load<ItemsDB>("Databases/ItemDB");
                EditorUtility.SetDirty(tileDatabase);
                EditorUtility.SetDirty(itemsDatabase);
                dataLoaded = true;
            }
        }


        if (dataLoaded == true)
        {

            // Create the upper selection bar (itemsdb, attributes, etc). Switch to GUILayout.SelectionGrid for multiple rows
            //toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);
            toolbarInt = GUILayout.SelectionGrid(toolbarInt, toolbarStrings, 4);


            switch (toolbarInt)
            {
                case 0:
                    ItemsListTab();
                    break;
                case 1:
                    CreateAttributesTab();
                    break;
                case 2:
                    TileListTab();
                    break;
                case 3:
                    TilePrefsTab();
                    break;

            }

        }

    }


    private void ItemsListTab()
    {

        if (currentSelectedItem != -1)
        {
            ItemCreationTab();
        }

        alignStyle = new GUIStyle(GUI.skin.button);
        alignStyle.alignment = TextAnchor.MiddleLeft;

        Rect search = EditorGUILayout.BeginHorizontal("Button", GUILayout.Width(360), GUILayout.Height(20));

        CreateInput.SearchString =
            EditorGUILayout.TextField("Search: ", CreateInput.SearchString);

        if (CreateInput.SearchString != "")
        {
            OrderSelection = 1;
        }

        EditorGUILayout.EndHorizontal();

        Rect h = (Rect)EditorGUILayout.BeginHorizontal("Button", GUILayout.Width(360), GUILayout.Height(20));

        GUI.backgroundColor = new Color(0, 0, 0, 0);

        if (GUILayout.Button("   #", alignStyle, GUILayout.Width(40), GUILayout.Height(20)))
        {

            OrderSelection = 0;
        }

        if (GUILayout.Button("Item Name", alignStyle, GUILayout.Width(130), GUILayout.Height(20)))
        {

            OrderSelection = 1;
        }

        if (GUILayout.Button("   Item Type", alignStyle, GUILayout.Width(100)))
        {
            OrderSelection = 2;
            enumSortReverse = !enumSortReverse;

        }



        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        Rect r = (Rect)EditorGUILayout.BeginVertical("Button", GUILayout.Width(200), GUILayout.Height(200));
        // EditorGUILayout.BeginVertical();




        scrollPos =
             EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(350), GUILayout.Height(300));


        if (OrderSelection == 0)

        {
            PopulateItemsListTab();
        }

        if (OrderSelection == 1)
        {
            OrderBySearch();

        }

        if (OrderSelection == 2)
        {
            OrderByType();

        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();




        /*

        DropButton.text(Num);
        if (EditorGUILayout.DropdownButton)
        {


        }
        */
    }



    private void ItemCreationTab()
    {

        string createString = "Create Item";
        string modifyString = "Modify Item";
        string nameString;

        GUILayout.BeginArea(new Rect(400, startY, 300, 600));



        CreateInput.PopulateAll =
        EditorGUILayout.Toggle("Show All Properties", CreateInput.PopulateAll);

        if (CreateInput.PopulateAll == false)
        {
            PopulateAllFalse();
        }
        else
        {
            PopulateAllTrue();
        }




        if (ItemsDatabase.itemsDB[currentSelectedItem].itemID == -1)
        {
            nameString = createString;
        }
        else
        {
            nameString = modifyString;
        }

        Rect h = (Rect)EditorGUILayout.BeginHorizontal(GUILayout.Width(300), GUILayout.Height(40)); // "EvenBackground", 

        if (GUILayout.Button(nameString, GUILayout.Width(100)))
        {

            CreateAtSpot();   // to-do: If any field is missing, don't create and warn the user

        }

        if (GUILayout.Button("Remove Item", GUILayout.Width(100)))
        {
            RemoveItem();
        }


        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(800, startY, 250, 600));

        //   Audio 1
        GUILayout.Label("Sound 1");
        s1Obj = EditorGUILayout.ObjectField(s1Obj, typeof(AudioClip), true);

        // Audio 2
        GUILayout.Label("Sound 2");
        s2Obj = EditorGUILayout.ObjectField(s2Obj, typeof(AudioClip), true);

        //   Audio 3

        GUILayout.Label("Sound 3");
        s3Obj = EditorGUILayout.ObjectField(s3Obj, typeof(AudioClip), true);

        //   Audio 4
        GUILayout.Label("Sound 4");
        s4Obj = EditorGUILayout.ObjectField(s4Obj, typeof(AudioClip), true);

        spriteObj = EditorGUILayout.ObjectField("Sprite", spriteObj, typeof(Sprite), false);

        if (CreateInput.ItemType == (int)ItemType.Helm || CreateInput.ItemType == (int)ItemType.Chest || CreateInput.ItemType == (int)ItemType.Feet || CreateInput.ItemType == (int)ItemType.Legs)
        {
            EditorGUILayout.LabelField("Armor Sprite");
            armorSpriteObj = EditorGUILayout.ObjectField(ItemsDatabase.itemsDB[currentSelectedItem].itemAttachedArmorSprite, armorSpriteObj, typeof(Sprite), false);
        }

        CreateInput.ItemHasProjectile =
            EditorGUILayout.Toggle("Has Projectile", CreateInput.ItemHasProjectile);

        if (CreateInput.ItemHasProjectile)
        {
            projectileSpriteObj = EditorGUILayout.ObjectField("Projectile Sprite", projectileSpriteObj, typeof(Sprite), false);
        }

        GUILayout.EndArea();

    }

    private void PopulateAllTrue()
    {
        GUILayout.Label("Item ID:   " + currentSelectedItem);
        //  CreateInput.ItemID =
        //  EditorGUILayout.IntField("Item ID", CreateInput.ItemID);

        CreateInput.ItemName =
        EditorGUILayout.TextField("Item Name", CreateInput.ItemName);

        CreateInput.ItemSellPrice =
        EditorGUILayout.IntField("Sell Price", CreateInput.ItemSellPrice);

        CreateInput.ItemBuyPrice =
        EditorGUILayout.IntField("Buy Price", CreateInput.ItemBuyPrice);

        itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", itemType);

        CreateInput.ItemMaxStacks =
        EditorGUILayout.IntField("Max Stacks", CreateInput.ItemMaxStacks);

        CreateInput.ItemRarity =
        EditorGUILayout.IntField("Rarity", CreateInput.ItemRarity);

        CreateInput.ItemEquippable =
        EditorGUILayout.Toggle("Equippable", CreateInput.ItemEquippable);

        CreateInput.ItemArmor =
        EditorGUILayout.IntField("Armor", CreateInput.ItemArmor);

        CreateInput.ItemPickaxePower =
        EditorGUILayout.IntField("Pickaxe Power", CreateInput.ItemPickaxePower);

        CreateInput.ItemAxePower =
        EditorGUILayout.IntField("Axe Power", CreateInput.ItemAxePower);

        CreateInput.ItemBaseDamage =
        EditorGUILayout.IntField("Base Damage", CreateInput.ItemBaseDamage);

        CreateInput.ItemCritChance =
        EditorGUILayout.IntField("Critical Strike Chance", CreateInput.ItemCritChance);

        CreateInput.itemTileID =
        EditorGUILayout.IntField("Tile ID", CreateInput.itemTileID);

        EditorGUILayout.LabelField("Attached script:");
        CreateInput.ScriptID =
        EditorGUILayout.Popup(CreateInput.ScriptID, methodsArray);

        CreateInput.ItemConsumablePower =
        EditorGUILayout.IntField("Consumable Power", CreateInput.ItemConsumablePower);

        CreateInput.ItemUseSpeed =
        EditorGUILayout.FloatField("Use Speed", CreateInput.ItemUseSpeed);

        CreateInput.ItemDestroyedOnUse =
        EditorGUILayout.Toggle("Destroyed on use", CreateInput.ItemDestroyedOnUse);


    }


    private void PopulateAllFalse()
    {
        GUILayout.Label("Item ID:   " + currentSelectedItem);
        //  CreateInput.ItemID =
        //  EditorGUILayout.IntField("Item ID", CreateInput.ItemID);

        CreateInput.ItemName =
        EditorGUILayout.TextField("Item Name", CreateInput.ItemName);

        CreateInput.ItemSellPrice =
        EditorGUILayout.IntField("Sell Price", CreateInput.ItemSellPrice);

        CreateInput.ItemBuyPrice =
        EditorGUILayout.IntField("Buy Price", CreateInput.ItemBuyPrice);

        itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", itemType);

        CreateInput.ItemMaxStacks =
        EditorGUILayout.IntField("Max Stacks", CreateInput.ItemMaxStacks);

        CreateInput.ItemRarity =
        EditorGUILayout.IntField("Rarity", CreateInput.ItemRarity);

        CreateInput.ItemEquippable =
        EditorGUILayout.Toggle("Equippable", CreateInput.ItemEquippable);

        switch (itemType)
        {
            case ItemType.Consumable:
                CreateInput.ItemConsumablePower =
            EditorGUILayout.IntField("Consumable Power", CreateInput.ItemConsumablePower);
                break;

            case ItemType.Helm:
                CreateInput.ItemArmor =
            EditorGUILayout.IntField("Armor", CreateInput.ItemArmor);
                break;

            case ItemType.Chest:
                CreateInput.ItemArmor =
            EditorGUILayout.IntField("Armor", CreateInput.ItemArmor);
                break;

            case ItemType.Legs:
                CreateInput.ItemArmor =
            EditorGUILayout.IntField("Armor", CreateInput.ItemArmor);
                break;

            case ItemType.Feet:
                CreateInput.ItemArmor =
            EditorGUILayout.IntField("Armor", CreateInput.ItemArmor);
                break;

            case ItemType.Weapon:
                CreateInput.ItemBaseDamage =
            EditorGUILayout.IntField("Base Damage", CreateInput.ItemBaseDamage);

                CreateInput.ItemUseSpeed =
            EditorGUILayout.FloatField("Use Speed", CreateInput.ItemUseSpeed);

                CreateInput.ItemCritChance =
            EditorGUILayout.IntField("Critical Strike Chance", CreateInput.ItemCritChance);
                break;

            case ItemType.TileBlock:
                CreateInput.itemTileID =
            EditorGUILayout.IntField("Tile ID", CreateInput.itemTileID);
                break;

            case ItemType.Pickaxe:
                CreateInput.ItemBaseDamage =
            EditorGUILayout.IntField("Base Damage", CreateInput.ItemBaseDamage);

                CreateInput.ItemUseSpeed =
            EditorGUILayout.FloatField("Use Speed", CreateInput.ItemUseSpeed);

                CreateInput.ItemPickaxePower =
            EditorGUILayout.IntField("Pickaxe Power", CreateInput.ItemPickaxePower);

                CreateInput.ItemCritChance =
            EditorGUILayout.IntField("Critical Strike Chance", CreateInput.ItemCritChance);
                break;

            case ItemType.PickaxeAxe:
                CreateInput.ItemBaseDamage =
           EditorGUILayout.IntField("Base Damage", CreateInput.ItemBaseDamage);

                CreateInput.ItemUseSpeed =
            EditorGUILayout.FloatField("Use Speed", CreateInput.ItemUseSpeed);

                CreateInput.ItemPickaxePower =
            EditorGUILayout.IntField("Pickaxe Power", CreateInput.ItemPickaxePower);

                CreateInput.ItemAxePower =
            EditorGUILayout.IntField("Axe Power", CreateInput.ItemAxePower);

                CreateInput.ItemCritChance =
            EditorGUILayout.IntField("Critical Strike Chance", CreateInput.ItemCritChance);
                break;

            case ItemType.Axe:
                CreateInput.ItemBaseDamage =
            EditorGUILayout.IntField("Base Damage", CreateInput.ItemBaseDamage);

                CreateInput.ItemAxePower =
            EditorGUILayout.IntField("Axe Power", CreateInput.ItemAxePower);

                CreateInput.ItemCritChance =
            EditorGUILayout.IntField("Critical Strike Chance", CreateInput.ItemCritChance);
                break;

            default:
                break;

        }

        CreateInput.ItemDestroyedOnUse =
    EditorGUILayout.Toggle("Destroyed on use", CreateInput.ItemDestroyedOnUse);


        EditorGUILayout.LabelField("Attached script:");

        CreateInput.ScriptID =
                    EditorGUILayout.Popup(CreateInput.ScriptID, methodsArray);
    }

    private static string[] GetMethodsList(Type type)
    {
        string[] returnString = new string[type.GetMethods().Length - 3];  // -4 ? - 3? i get 4 extra ones and i steal nr 0 for None
        int count = 1;
        returnString[0] = "None";

        foreach (var method in type.GetMethods())
        {

            if (method.Name != "Equals" && method.Name != "GetHashCode" && method.Name != "GetType" && method.Name != "ToString")
            {
                returnString[count] = method.Name;
                count++;
            }
        }

        return returnString;
    }

    private void RemoveItem()
    {
        ItemsDatabase.RemoveSelectedItem(currentSelectedItem);

    }

    private void TempCreateItem()       // to-do: check parameters to see if they are acceptable before creating
    {

        //    Texture2D spriteTexture = (Texture2D)spriteObj;
        //    ItemsEditor.CreateItem(CreateInput.ItemID, CreateInput.ItemName, CreateInput.ItemPrice, spriteTexture, itemType, CreateInput.ItemIcon, CreateInput.ItemWeight, CreateInput.ItemStacks, CreateInput.ItemRarity, CreateInput.ItemStrenght, CreateInput.ItemDexterity, CreateInput.ItemIntelligence, CreateInput.ItemEquippable);

    }

    private void CreateAtSpot()
    {
        Sprite spriteTexture = (Sprite)spriteObj;
        Sprite projectileSpriteTexture = (Sprite)projectileSpriteObj;
        AudioClip s1Clip = (AudioClip)s1Obj;
        AudioClip s2Clip = (AudioClip)s2Obj;
        AudioClip s3Clip = (AudioClip)s3Obj;
        AudioClip s4Clip = (AudioClip)s4Obj;

        string _itemScript;
        string spriteName;
        string armorSpriteName;

        _itemScript = methodsArray[CreateInput.ScriptID];
        float itemUseTime;

        if (CreateInput.ItemType == (int)ItemType.Consumable)
        {

            itemUseTime = 1;
        }
        else
        {
            itemUseTime = CreateInput.ItemUseSpeed;
        }

        if (armorSpriteObj == null)
        {
            armorSpriteName = "";
        }
        else
        {
            armorSpriteName = armorSpriteObj.name;
        }

        if (spriteTexture == null)
        {
            spriteName = "None";

        }
        else
        {
            spriteName = spriteTexture.name;
        }
        ItemsDatabase.CreateItemAtSlot(currentSelectedItem, CreateInput.ItemName, CreateInput.ItemSellPrice, CreateInput.ItemBuyPrice, spriteTexture, spriteName, projectileSpriteTexture, CreateInput.ItemHasProjectile, itemType, CreateInput.ItemMaxStacks, CreateInput.ItemRarity, CreateInput.ItemEquippable, CreateInput.ItemArmor, CreateInput.ItemPickaxePower, CreateInput.ItemAxePower, CreateInput.ItemBaseDamage, CreateInput.itemTileID, _itemScript, CreateInput.ItemConsumablePower, itemUseTime, CreateInput.ItemDestroyedOnUse, CreateInput.ItemCritChance, armorSpriteName, s1Clip, s2Clip, s3Clip, s4Clip, CreateInput.S1Name, CreateInput.S2Name, CreateInput.S3Name, CreateInput.S4Name);
    }
    private void CheckInputData()  // make it return
    {


    }

    private void PopulateItemsListTab()
    {




        for (int i = 0; i < ItemsDatabase.itemsDB.Length; i++)
        {

            if (currentSelectedItem == i)           // if it's selected, highlight line
            {
                GUI.backgroundColor = Color.green;
            }
            else
            {
                GUI.backgroundColor = new Color(0, 0, 0, 0);
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("#" + i + "   " + ItemsDatabase.itemsDB[i].itemName, alignStyle, GUILayout.Width(185)))
            {
                currentSelectedItem = i;
                UpdatePrefs();
            }

            if (currentSelectedItem == i)
            {
                GUI.backgroundColor = Color.green;
            }
            else
            {
                GUI.backgroundColor = new Color(0, 0, 0, 0);
            }

            GUILayout.Label("" + ItemsDatabase.itemsDB[i].itemType, alignStyle, GUILayout.Width(85));
            GUI.backgroundColor = new Color(0, 0, 0, 0);

            if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20)))
            {

            }

            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20)))
            {

            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void UpdatePrefs()
    {
        CreateInput.ItemID = currentSelectedItem;
        CreateInput.ItemName = ItemsDatabase.itemsDB[currentSelectedItem].itemName;
        CreateInput.ItemSellPrice = ItemsDatabase.itemsDB[currentSelectedItem].itemSellPrice;
        CreateInput.ItemBuyPrice = ItemsDatabase.itemsDB[currentSelectedItem].itemBuyPrice;
        CreateInput.ItemType = (int)ItemsDatabase.itemsDB[currentSelectedItem].itemType;
        CreateInput.ItemMaxStacks = ItemsDatabase.itemsDB[currentSelectedItem].itemMaxStack;
        CreateInput.ItemRarity = ItemsDatabase.itemsDB[currentSelectedItem].itemRarity;
        CreateInput.ItemEquippable = ItemsDatabase.itemsDB[currentSelectedItem].itemIsEquippable;
        CreateInput.ItemArmor = ItemsDatabase.itemsDB[currentSelectedItem].itemArmor;
        CreateInput.ItemPickaxePower = ItemsDatabase.itemsDB[currentSelectedItem].itemPickaxePower;
        CreateInput.ItemBaseDamage = ItemsDatabase.itemsDB[currentSelectedItem].itemBaseDamage;
        CreateInput.itemTileID = ItemsDatabase.itemsDB[currentSelectedItem].itemTileId;
        CreateInput.ItemUseSpeed = ItemsDatabase.itemsDB[currentSelectedItem].itemUseSpeed;
        CreateInput.itemAttachedScript = ItemsDatabase.itemsDB[currentSelectedItem].itemAttachedScript;
        CreateInput.ItemDestroyedOnUse = ItemsDatabase.itemsDB[currentSelectedItem].itemDestroyedOnUse;
        CreateInput.ItemCritChance = ItemsDatabase.itemsDB[currentSelectedItem].itemCriticalChance;
        CreateInput.ItemHasProjectile = ItemsDatabase.itemsDB[currentSelectedItem].itemHasProjectile;



        if (ItemsDatabase.itemsDB[currentSelectedItem].itemAttachedScript == "" || ItemsDatabase.itemsDB[currentSelectedItem].itemAttachedScript == "None")
        {
            CreateInput.ScriptID = 0;
        }
        else
        {
            for (int i = 1; i < methodsArray.Length; i++)
            {
                if (ItemsDatabase.itemsDB[currentSelectedItem].itemAttachedScript == methodsArray[i])
                {
                    CreateInput.ScriptID = i;
                    break;
                }
            }
        }


        CreateInput.S1Name = ItemsDatabase.itemsDB[currentSelectedItem].s1Text;
        CreateInput.S2Name = ItemsDatabase.itemsDB[currentSelectedItem].s2Text;
        CreateInput.S3Name = ItemsDatabase.itemsDB[currentSelectedItem].s3Text;
        CreateInput.S4Name = ItemsDatabase.itemsDB[currentSelectedItem].s4Text;

        s1Obj = ItemsDatabase.itemsDB[currentSelectedItem].sound1;
        s2Obj = ItemsDatabase.itemsDB[currentSelectedItem].sound2;
        s3Obj = ItemsDatabase.itemsDB[currentSelectedItem].sound3;
        s4Obj = ItemsDatabase.itemsDB[currentSelectedItem].sound4;

        if (ItemsDatabase.itemsDB[currentSelectedItem].itemAttachedArmorSprite != "")
        {
            armorSpriteObj = Resources.Load<Sprite>("Textures/Player/PlayerSprites/" + ItemsDatabase.itemsDB[currentSelectedItem].itemAttachedArmorSprite);
        }
        else
        {
            armorSpriteObj = null;
        }


        spriteObj = ItemsDatabase.itemsDB[currentSelectedItem].itemSprite;
        spriteTexture = (Sprite)spriteObj;
        projectileSpriteObj = ItemsDatabase.itemsDB[currentSelectedItem].itemProjectileSprite;
        projectileSpriteTexture = (Sprite)projectileSpriteObj;
        itemType = ItemsDatabase.itemsDB[currentSelectedItem].itemType;




    }

    private void OrderBySearch()
    {

        for (int i = 0; i < ItemsDatabase.itemsDB.Length; i++)
        {
            if (ItemsDatabase.itemsDB[i].itemName.IndexOf(CreateInput.SearchString, StringComparison.OrdinalIgnoreCase) != -1)   // .Contains(CreateInput.SearchString)) ItemsDatabase.itemsDB[i].itemName.IndexOf(CreateInput.SearchString, StringComparison.OrdinalIgnoreCase) != -1  
            {
                if (currentSelectedItem == i)
                {
                    GUI.backgroundColor = Color.green;
                }
                else
                {
                    GUI.backgroundColor = new Color(0, 0, 0, 0);
                }

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("#" + i + "   " + ItemsDatabase.itemsDB[i].itemName, alignStyle, GUILayout.Width(185)))
                {
                    currentSelectedItem = i;
                    UpdatePrefs();
                }

                if (currentSelectedItem == i)
                {
                    GUI.backgroundColor = Color.green;
                }
                else
                {
                    GUI.backgroundColor = new Color(0, 0, 0, 0);
                }

                GUILayout.Label("" + ItemsDatabase.itemsDB[i].itemType, alignStyle, GUILayout.Width(85));
                GUI.backgroundColor = new Color(0, 0, 0, 0);

                if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20)))
                {
                }

                if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20)))
                {
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void OrderByType()
    {
        PopulateTypesList();        // is there a better way?

        if (enumSortReverse == false)
        {
            for (int j = 0; j < itemTypesAmount; j++)     // Do this 12 times (Decides the current Type we're comparing)
            {
                for (int i = 0; i < ItemsDatabase.itemsDB.Length; i++)    // Scan the list
                {
                    if (ItemsDatabase.itemsDB[i].itemType == typesList[j])   // if that item's type == the one we're comparing, make button
                    {
                        if (currentSelectedItem == i)
                        {
                            GUI.backgroundColor = Color.green;
                        }
                        else
                        {
                            GUI.backgroundColor = new Color(0, 0, 0, 0);
                        }

                        EditorGUILayout.BeginHorizontal();

                        if (GUILayout.Button("#" + i + "   " + ItemsDatabase.itemsDB[i].itemName, alignStyle, GUILayout.Width(185)))
                        {
                            currentSelectedItem = i;
                            UpdatePrefs();
                        }

                        if (currentSelectedItem == i)
                        {
                            GUI.backgroundColor = Color.green;
                        }
                        else
                        {
                            GUI.backgroundColor = new Color(0, 0, 0, 0);
                        }

                        GUILayout.Label("" + ItemsDatabase.itemsDB[i].itemType, alignStyle, GUILayout.Width(85));
                        GUI.backgroundColor = new Color(0, 0, 0, 0);

                        if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20)))
                        {
                        }

                        if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20)))
                        {
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }
        else
        {
            for (int j = itemTypesAmount - 1; j >= 0; j--)     // Do this 12 times (Decides the current Type we're comparing)
            {
                for (int i = 0; i < ItemsDatabase.itemsDB.Length; i++)    // Scan the list
                {
                    if (ItemsDatabase.itemsDB[i].itemType == typesList[j])   // if that item's type == the one we're comparing, make button
                    {
                        if (currentSelectedItem == i)
                        {
                            GUI.backgroundColor = Color.green;
                        }
                        else
                        {
                            GUI.backgroundColor = new Color(0, 0, 0, 0);
                        }

                        EditorGUILayout.BeginHorizontal();

                        if (GUILayout.Button("#" + i + "   " + ItemsDatabase.itemsDB[i].itemName, alignStyle, GUILayout.Width(185)))
                        {
                            currentSelectedItem = i;
                            UpdatePrefs();
                        }

                        if (currentSelectedItem == i)
                        {
                            GUI.backgroundColor = Color.green;
                        }
                        else
                        {
                            GUI.backgroundColor = new Color(0, 0, 0, 0);
                        }

                        GUILayout.Label("" + ItemsDatabase.itemsDB[i].itemType, alignStyle, GUILayout.Width(85));
                        GUI.backgroundColor = new Color(0, 0, 0, 0);

                        if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20)))
                        {
                        }

                        if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20)))
                        {
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }



    }


    private void PopulateTempOrderList()
    {
        tempList = new Item[ItemsDatabase.itemsDB.Length];
        for (int u = 0; u < tempList.Length; u++)
        {
            tempList[u] = ItemsDatabase.itemsDB[u];
        }
    }

    private void PopulateTypesList()    //find better way
    {
        typesList = new ItemType[18];

        typesList[0] = ItemType.Accessory;
        typesList[1] = ItemType.Ammo;
        typesList[2] = ItemType.Axe;
        typesList[3] = ItemType.Chest;
        typesList[4] = ItemType.Coin;
        typesList[5] = ItemType.Consumable;
        typesList[6] = ItemType.Dye;
        typesList[7] = ItemType.Feet;
        typesList[8] = ItemType.Helm;
        typesList[9] = ItemType.Legs;
        typesList[10] = ItemType.Pickaxe;
        typesList[11] = ItemType.PickaxeAxe;
        typesList[12] = ItemType.Quest;
        typesList[13] = ItemType.Shield;
        typesList[14] = ItemType.Social;
        typesList[15] = ItemType.TileBlock;
        typesList[16] = ItemType.Weapon;
        typesList[17] = ItemType.None;
    }



    //////////////////////////////////////////////   TILES /////////////////////////////////////////////////////


    private void TileListTab()
    {
        // CreateInput.SearchString = ""; ????

        if (currentSelectedTile != -1)
        {
            TileInfoTab();
        }

        alignStyle = new GUIStyle(GUI.skin.button);
        alignStyle.alignment = TextAnchor.MiddleLeft;

        Rect search = EditorGUILayout.BeginHorizontal("Button", GUILayout.Width(360), GUILayout.Height(20));

        CreateInput.SearchString =
            EditorGUILayout.TextField("Search: ", CreateInput.SearchString);

        if (CreateInput.SearchString != "")
        {
            OrderSelection = 2;
        }

        EditorGUILayout.EndHorizontal();

        Rect h = (Rect)EditorGUILayout.BeginHorizontal("Button", GUILayout.Width(360), GUILayout.Height(20));

        GUI.backgroundColor = new Color(0, 0, 0, 0);

        if (GUILayout.Button("   #", alignStyle, GUILayout.Width(40), GUILayout.Height(20)))
        {

            OrderSelection = 0;
        }

        if (GUILayout.Button("Tile Name", alignStyle, GUILayout.Width(130), GUILayout.Height(20)))
        {

            OrderSelection = 1;
        }


        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        Rect r = (Rect)EditorGUILayout.BeginVertical("Button", GUILayout.Width(200), GUILayout.Height(200));
        // EditorGUILayout.BeginVertical();




        scrollPos =
             EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(350), GUILayout.Height(300));


        if (OrderSelection == 0)

        {
            PopulateTileListTab();
        }

        if (OrderSelection == 1)
        {
            // OrderBySearch();

        }

        if (OrderSelection == 2)
        {
            // OrderByType();

        }


        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();

        GUI.backgroundColor = new Color(1, 1, 1, 1);

        if (GUILayout.Button("New Tile", GUILayout.Width(100)))
        {
            CreateNewTile();
        }

    }

    private void CreateNewTile()
    {

        tileDatabase.tileList.Add(new TileData());
        currentSelectedTile = tileDatabase.tileList.Count - 1;
        UpdateTilePrefs();
    }

    private void PopulateTileListTab()
    {
        for (int i = 0; i < tileDatabase.tileList.Count; i++)
        {
            // If the current tile is selected, highlight the line
            if (currentSelectedTile == i)
            {
                GUI.backgroundColor = Color.green;
            }
            else
            {
                GUI.backgroundColor = new Color(0, 0, 0, 0);
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("#" + i + "   " + tileDatabase.tileList[i].tileName, alignStyle, GUILayout.Width(150)))
            {
                currentSelectedTile = i;
                UpdateTilePrefs();
            }

            if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20)))
            {

            }

            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20)))
            {

            }

            if (tileDatabase.tileList[i].tileTextureSheetName == "")
            {

                GUILayout.Label("Missing texture!", redTextStyle);
            }

            EditorGUILayout.EndHorizontal();

        }
    }

    private void TileInfoTab()
    {
        string saveString = "Save Tile";
        string createString = "Create Tile";
        string leftButtonString;

        GUILayout.BeginArea(new Rect(400, startY, 300, 600));

        GUILayout.Label("Tile ID:   " + currentSelectedTile);

        CreateInput.tileName =
            EditorGUILayout.TextField("Tile Name", CreateInput.tileName);

        CreateInput.tileGrip =
        EditorGUILayout.FloatField("Tile Grip (0 to 1)", CreateInput.tileGrip);

        CreateInput.isSolid =
            EditorGUILayout.Toggle("Solid", CreateInput.isSolid);


        Rect h = (Rect)EditorGUILayout.BeginHorizontal(GUILayout.Width(300), GUILayout.Height(40)); // "EvenBackground", 

        if (GUILayout.Button("Save", GUILayout.Width(100)))
        {
            tileDatabase.tileList[currentSelectedTile].tileName = CreateInput.tileName;
            tileDatabase.tileList[currentSelectedTile].tileID = currentSelectedTile;
            tileDatabase.tileList[currentSelectedTile].tileGrip = CreateInput.tileGrip;
            tileDatabase.tileList[currentSelectedTile].isSolid = CreateInput.isSolid;

            if (tileSpriteObj)
            {
                tileDatabase.tileList[currentSelectedTile].tileTextureSheetName = tileSpriteObj.name;
            }


        }

        if (GUILayout.Button("Reset Tile", GUILayout.Width(100)))
        {
            tileDatabase.tileList[currentSelectedTile].tileName = "";
            tileDatabase.tileList[currentSelectedTile].tileID = currentSelectedTile;
            tileDatabase.tileList[currentSelectedTile].tileGrip = 0;
            tileDatabase.tileList[currentSelectedTile].isSolid = false;
            tileDatabase.tileList[currentSelectedTile].tileTextureSheetName = "";
            UpdateTilePrefs();
        }


        GUILayout.EndHorizontal();

        


        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(700, startY, 600, 600));


        tileSpriteObj = EditorGUILayout.ObjectField("", tileSpriteObj, typeof(Sprite), false, GUILayout.Width(300), GUILayout.Height(300));

        GUILayout.EndArea();

    }

    private void UpdateTilePrefs()
    {
        CreateInput.tileName = tileDatabase.tileList[currentSelectedTile].tileName;
        CreateInput.tileGrip = tileDatabase.tileList[currentSelectedTile].tileGrip;
        CreateInput.isSolid = tileDatabase.tileList[currentSelectedTile].isSolid;

        if (tileDatabase.tileList[currentSelectedTile].tileTextureSheetName != "")
        {
            tileSpriteObj = Resources.Load("Textures/TileTextures/" + tileDatabase.tileList[currentSelectedTile].tileTextureSheetName);
        }
        else
        {
            tileSpriteObj = null;
        }
    }

    ///////////////////////////////////////    ATTRIBUTES     ////////////////////////////////////////////////////////////
    

    private void CreateAttributesTab()
    {
        EditorGUILayout.BeginVertical("Button", GUILayout.Width(200), GUILayout.Height(200));

        scrollPos =
             EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(350), GUILayout.Height(300));


        if (OrderSelection == 0)
        {
            PopulateAttributesList();
        }

        if (OrderSelection == 1)
        {

        }


        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void PopulateAttributesList()
    {
        for (int i = 0; i < itemsDatabase.itemAttributeList.Count; i++)
        {
            if (currentSelectedItem == i)           // if it's selected, highlight line
            {
                GUI.backgroundColor = Color.green;
            }
            else
            {
                GUI.backgroundColor = new Color(0, 0, 0, 0);
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("#" + i + "   " + itemsDatabase.itemAttributeList[i].AttributeName, alignStyle, GUILayout.Width(185)))
            {
                currentSelectedItem = i;
                //UpdatePrefs();
            }

            if (currentSelectedItem == i)
            {
                GUI.backgroundColor = Color.green;
            }
            else
            {
                GUI.backgroundColor = new Color(0, 0, 0, 0);
            }

            // the type of the var
            GUILayout.Label("" , alignStyle, GUILayout.Width(85)); // itemsDatabase.itemAttributeList[i].AttributeValue.GetType()
            GUI.backgroundColor = new Color(0, 0, 0, 0);


            EditorGUILayout.EndHorizontal();
        }

    }

    ////////////////////////////////   TILE PREFS    /////////////////////////////////////////////
    
    private void TilePrefsTab()
    {

        if (currentSelectedTilePref != -1)
        {
            TilePrefsDetails();
        }

        EditorGUILayout.BeginVertical("Button", GUILayout.Width(200), GUILayout.Height(200));

        scrollPos =
             EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(350), GUILayout.Height(300));

        PopulateTilePrefsList();



        EditorGUILayout.EndScrollView();


        EditorGUILayout.EndVertical();


        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = new Color(1, 1, 1, 1);

        if (GUILayout.Button("New", alignStyle, GUILayout.Width(185)))
        {
            tileDatabase.tileTextureGroups.Add(new TileTextureGroup(true));
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.BeginArea(new Rect(800, startY, 600, 600));

        GUILayout.BeginVertical();

        GUILayout.Label("Example");

        int count = 1;

        for (int i = 0; i < 8; i++)
        {
            GUILayout.BeginHorizontal();
            for (int u = 0; u < 8; u++)
            {
                GUILayout.Box("" + count, GUILayout.Width(40), GUILayout.Height(40));
                GUILayout.Space(-5);
                count++;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(-5);
        }



        

        GUILayout.EndVertical();

        GUILayout.EndArea();
    }

    private void PopulateTilePrefsList()
    {
        for (int i = 0; i < tileDatabase.tileTextureGroups.Count; i++)
        {
            if (currentSelectedTilePref == i)           // if it's selected, highlight line
            {
                GUI.backgroundColor = Color.green;
            }
            else
            {
                GUI.backgroundColor = new Color(0, 0, 0, 0);
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(tileDatabase.tileTextureGroups[i].groupName, alignStyle, GUILayout.Width(185)))
            {
                currentSelectedTilePref = i;
                UpdateTileGroupsPrefs();
            }

            GUILayout.Label("" + tileDatabase.tileTextureGroups[i].textureMin + ", " + tileDatabase.tileTextureGroups[i].textureMax, alignStyle, GUILayout.Width(85)); // itemsDatabase.itemAttributeList[i].AttributeValue.GetType()
            GUI.backgroundColor = new Color(0, 0, 0, 0);


            EditorGUILayout.EndHorizontal();
        }
    }

    private void TilePrefsDetails()
    {
        GUILayout.BeginArea(new Rect(400, startY, 300, 600));

        CreateInput.TileGroupName =
            EditorGUILayout.TextField("Group Name", CreateInput.TileGroupName);

        CreateInput.TileGroupMin =
        EditorGUILayout.IntField("Group Min", CreateInput.TileGroupMin);

        CreateInput.TileGroupMax =
        EditorGUILayout.IntField("Group Max", CreateInput.TileGroupMax);

        EditorGUILayout.BeginHorizontal(GUILayout.Width(300), GUILayout.Height(40)); // "EvenBackground", 

        if (GUILayout.Button("Save", GUILayout.Width(100)))
        {
            tileDatabase.tileTextureGroups[currentSelectedTilePref].groupName = CreateInput.TileGroupName;
            tileDatabase.tileTextureGroups[currentSelectedTilePref].textureMin = CreateInput.TileGroupMin;
            tileDatabase.tileTextureGroups[currentSelectedTilePref].textureMax = CreateInput.TileGroupMax;
            UpdateTileGroupsPrefs();
        }

        if (GUILayout.Button("Reset Group", GUILayout.Width(100)))
        {
            tileDatabase.tileTextureGroups[currentSelectedTilePref].groupName = "-- Uninitialized --";
            tileDatabase.tileTextureGroups[currentSelectedTilePref].textureMin = 0;
            tileDatabase.tileTextureGroups[currentSelectedTilePref].textureMax = 0;
            UpdateTileGroupsPrefs();
        }


        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    private void UpdateTileGroupsPrefs()
    {
        CreateInput.TileGroupName = tileDatabase.tileTextureGroups[currentSelectedTilePref].groupName;
        CreateInput.TileGroupMin = tileDatabase.tileTextureGroups[currentSelectedTilePref].textureMin;
        CreateInput.TileGroupMax = tileDatabase.tileTextureGroups[currentSelectedTilePref].textureMax;
    }
}

#endif
