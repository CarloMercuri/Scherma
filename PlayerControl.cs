using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading;
using System;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

public class PlayerControl : MonoBehaviour
{   
    // OBJECT - CLASS REFERENCES
    private GameObject _chunkControl;
    private PlayerAnimator _animator;
    private SandboxRB sRB;
    private ChunkControl _cC;
    public LightSource _playerLight;
    private GameObject _mainCamera;

    public Vec2 playerPosition;

 
    

    // DEBUG VARIABLES



    // MESH 
    private Material playerMaterial;
    private GameObject meshObject;

    public int playerZ = -18;

    // INVENTORY & UI //
    private bool inventoryActive = false;
    private MethodInfo[] itemUseMethods;

    // SYSTEMS
    private Vector2 _mousePos;
    

    // PLAYER VARIABLES
    public string PlayerName { get; set; }
    public int PlayerMaxHealth { get; set; }
    public int PlayerMaxMana { get; set; }
    public bool Softcore{ get; set; }
    public int PlayerArmor { get; set; }
    public int MeleeSpeedBonus { get; set; }
    public int MiningDistance { get; set; }

    // MOVEMENT RELATED VARIABLES //
    private Vector2 movement;
    public bool IsFacingRight { get; set; }
    public bool facingSnapShot;
    private int hMov;
    public float playerSpeed;
    public float jumpStrenght;

    public UIControl _ui;

    // CAMERA VARIABLES
    private Vector3 cameraVector;

    // ACTIONS
    public float globalCooldown = 0;
    public bool isGlobalCooldownOver { get { return CheckGlobalCoolDown(); } }



    void Awake()
    {
        _mousePos = new Vector2();
        //// TEMPORARY
        PlayerName = "Drewon";
        PlayerMaxHealth = 100;
        PlayerMaxMana = 20;
        Softcore = false;

        // References
        _animator = GetComponent<PlayerAnimator>();
        sRB = gameObject.GetComponent<SandboxRB>();
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        meshObject = transform.GetChild(1).gameObject;
        MeshControl.playerMeshObject = meshObject;
        playerMaterial = Resources.Load<Material>("Textures/Materials/PlayerMaterial");
        _chunkControl = GameObject.FindGameObjectWithTag("GameController");
        _chunkControl.GetComponent<ChunkControl>().playerCharacter = this.gameObject;
        _cC = _chunkControl.GetComponent<ChunkControl>();
        MapData.chunkControl = _chunkControl; // change
        Inventory.SetPlayerObject(this.gameObject);


        IsFacingRight = true;
        facingSnapShot = true;
        cameraVector = new Vector3(0, 0, -25);

        itemUseMethods = GetMethodsList(typeof(ItemScripts));


        MiningDistance = 5;
        _playerLight = new LightSource(15, playerPosition, Color.clear);
        LightControl.dynamicLights.Add(_playerLight);

    }



    public void InitializeSpriteMesh()
    {
        playerMaterial.mainTexture = Texturing.playerSpriteSheet;
        meshObject.GetComponent<MeshRenderer>().material = playerMaterial;

        meshObject.GetComponent<MeshFilter>().mesh = MeshControl.InitializePlayerTestMesh();

        MeshControl.LoadDefaultCharacterTexture();
        MeshControl.UpdatePlayerColors();
    }

   


    void Update()
    {
        // reset the horizontal and vertical speed
        hMov = 0;

        // Grab data
        _mousePos = GetMousePos();
        playerPosition = World.WorldToTileCoords(transform.position);
        LightControl.dynamicLights[0].position = playerPosition;
        CheckInputs();

        // run the lights - CHANGE LATER
        //LightControl.CalculateTilesLight(playerPosition);


        // Movement
        movement = sRB.MoveCharacter(hMov, playerSpeed);
        _mainCamera.transform.position = transform.position;

        if (hMov > 0)
        {
            IsFacingRight = true;
        }
        else if (hMov < 0)
        {
            IsFacingRight = false;
        }

        if (facingSnapShot != IsFacingRight)
        {
            SwitchFacingDirection(IsFacingRight);
            facingSnapShot = !facingSnapShot;
        }

        _animator.ProcessAnimation(hMov, movement);
 
    }

    private void SwitchFacingDirection(bool isFacingRight)
    {
        _animator.SwitchDirection(IsFacingRight);
    }

    public void UpdatePlayerArmor(int slotNr)
    {
        PlayerArmor = 0;

        if(Inventory.armor_slot[0, 0] >= 0)
        {
            PlayerArmor += ItemsDatabase.itemsDB[Inventory.armor_slot[0, 0]].itemArmor;
        }

        if (Inventory.armor_slot[1, 0] >= 0)
        {
            PlayerArmor += ItemsDatabase.itemsDB[Inventory.armor_slot[1, 0]].itemArmor;
        }

        if (Inventory.armor_slot[2, 0] >= 0)
        {
            PlayerArmor += ItemsDatabase.itemsDB[Inventory.armor_slot[2, 0]].itemArmor;
        }

        if(Inventory.armor_slot[slotNr, 0] != -1)
        {
            if(ItemsDatabase.itemsDB[Inventory.armor_slot[slotNr, 0]].itemAttachedArmorSprite != "")
            {
                switch(slotNr)
                {
                    case 0: // helm
                        Inventory.playerHeadTexture = ItemsDatabase.itemsDB[Inventory.armor_slot[slotNr, 0]].itemAttachedArmorSprite;
                        
                        break;

                    case 1: // body
                        Inventory.playerBodyTexture = ItemsDatabase.itemsDB[Inventory.armor_slot[slotNr, 0]].itemAttachedArmorSprite;
                        MeshControl.UpdatePlayerColors();
                        break;

                    case 2: // legs
                        Inventory.playerLegsTexture = ItemsDatabase.itemsDB[Inventory.armor_slot[slotNr, 0]].itemAttachedArmorSprite;
                        MeshControl.UpdatePlayerColors();
                        break;
                }
            }
            else
            {
                switch (slotNr)
                {
                    case 0: // helm
                        Inventory.playerHeadTexture = Inventory.playerEmptyTexture;
                        break;

                    case 1: // body
                        Inventory.playerBodyTexture = Inventory.playerDefaultBodyTexture;
                        MeshControl.UpdatePlayerColors();
                        break;

                    case 2: // legs
                        Inventory.playerLegsTexture = Inventory.playerDefaultLegsTexture;
                        MeshControl.UpdatePlayerColors();
                        break;
                }
            }
        }
        else
        {
            switch (slotNr)
            {
                case 0: // helm
                    Inventory.playerHeadTexture = Inventory.playerEmptyTexture;
                    break;

                case 1: // body
                    Inventory.playerBodyTexture = Inventory.playerDefaultBodyTexture;
                    MeshControl.UpdatePlayerColors();
                    break;

                case 2: // legs
                    Inventory.playerLegsTexture = Inventory.playerDefaultLegsTexture;
                    MeshControl.UpdatePlayerColors();
                    break;
            }
        }

       // Debug.Log($"Helm: {Inventory.playerHeadTexture}, Body: {Inventory.playerBodyTexture}, legs: {Inventory.playerLegsTexture}");



    }



    public bool ActionUseItem(int itemID, int actionBarSlot, Vector3 mousePos)
    {
  
        if(ItemsDatabase.itemsDB[itemID].itemAttachedScript != "None")
        {
            for (int i = 0; i < itemUseMethods.Length; i++)
            {
                if(ItemsDatabase.itemsDB[itemID].itemAttachedScript == itemUseMethods[i].Name)
                {
                    switch(ItemsDatabase.itemsDB[itemID].itemAttachedScript)
                    {
                        case "ShootBow":
                            if ((bool)itemUseMethods[i].Invoke(null, new object[] { itemID, mousePos }))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        case "BasicMine":
                            if ((bool)itemUseMethods[i].Invoke(null, new object[] { itemID, mousePos }))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        default:
                            if ((bool)itemUseMethods[i].Invoke(null, new object[] { itemID }))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                         
                    }
                    
                    
                    
                }
                
            }
            return false;
        }
        else
        {
         
            return false;
        }
       
    }

    public void ActionUseItemFromDragged(int itemID)
    {
        if (ItemsDatabase.itemsDB[itemID].itemAttachedScript != "None")
        {
            for (int i = 0; i < itemUseMethods.Length; i++)
            {
                if (ItemsDatabase.itemsDB[itemID].itemAttachedScript == itemUseMethods[i].Name)
                {
                    itemUseMethods[i].Invoke(null, new object[] { itemID });
                }
            }
        }
    }

    // Get the list of methods from a class
    private MethodInfo[] GetMethodsList(Type type)
    {
        MethodInfo[] methods = new MethodInfo[type.GetMethods().Length - 4];  // -4 ? - 3? i get 4 extra ones and i steal nr 0 for None
        int count = 0;

        foreach (var method in type.GetMethods())
        {
            if (method.Name != "Equals" && method.Name != "GetHashCode" && method.Name != "GetType" && method.Name != "ToString")
            {
                methods[count] = method;
                count++;
            }
        }

        return methods;
    }

    private void FillItemMethodsList()
    {
        ShowMethods(typeof(ItemScripts));

    }

    private void ShowMethods(Type type)
    {
        foreach (var method in type.GetMethods())
        {
            if(method.Name != "Equals" && method.Name != "GetHashCode" && method.Name != "GetType" && method.Name != "ToString")
            {
                method.Invoke(null, new object[] {1, "Hello" });
            }
           
            //Debug.Log(method.Name);
        }
    }

    private Vector3 GetMousePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 200, 1 << LayerMask.NameToLayer("MouseCoordsRaycast")))
        {
           
            return hit.point;
        }
        else
        {
            return new Vector3(0, 0);
        }
    }



    private bool CheckGlobalCoolDown()
    {
        if (Time.time > globalCooldown)
            return true;
        return false;
        
    }

    private void SetGlobalCooldown(int itemID)
    {
        globalCooldown = Time.time + ItemsDatabase.itemsDB[itemID].itemUseSpeed;
    }


    // Need to reorganize this
    private void CheckInputs()
    {

        if (Input.GetKeyDown(KeyCode.N))
        {
            

        }

        if (Input.GetMouseButton(0))
        {





            if (!EventSystem.current.IsPointerOverGameObject())             // if i am clicking on an UI element
                if (isGlobalCooldownOver)
                {

                    // if(isDragging == false)
                    if (Inventory.ab_slot[UI.actionBarSelected, 0] != -1)
                        if (ActionUseItem(Inventory.ab_slot[UI.actionBarSelected, 0], UI.actionBarSelected, GetMousePos()))
                        {
                            int item = Inventory.ab_slot[UI.actionBarSelected, 0];
                            SetGlobalCooldown(item);

                            if (ItemsDatabase.itemsDB[Inventory.ab_slot[UI.actionBarSelected, 0]].itemDestroyedOnUse)
                            {
                                Inventory.ab_slot[UI.actionBarSelected, 1] -= 1;
                                if (Inventory.ab_slot[UI.actionBarSelected, 1] <= 0) // to implement: check if itemid.isdestroyed on use
                                    Inventory.ab_slot[UI.actionBarSelected, 0] = -1; // remove item
                            }
                        }
                }

        }

        if (Input.GetKeyUp(KeyCode.V))
        {



        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (sRB.IsGrounded())
            {
                sRB.AddForce(new Vector2(0, jumpStrenght));

            }

        }

        // check mouse wheel
        if (Input.GetAxis("MouseWheel") < 0)
        {
            if (isGlobalCooldownOver)
                if (UI.actionBarSelected + 1 > 9)
                {
                    UI.ChangeSelectedActionBar(0);
                }
                else
                {
                    UI.ChangeSelectedActionBar(UI.actionBarSelected + 1);
                }

        }
        //
        if (Input.GetAxis("MouseWheel") > 0)
        {
            if (isGlobalCooldownOver)
                if (UI.actionBarSelected - 1 < 0)
                {
                    UI.ChangeSelectedActionBar(9);
                }
                else
                {
                    UI.ChangeSelectedActionBar(UI.actionBarSelected - 1);
                }


        }


        // check if player pressed number keys
        if (isGlobalCooldownOver)
            for (int i = 0; i < 10; i++)
            {
                if (Input.GetKeyDown("" + i))
                {
                    if (i == 0)
                    {
                        UI.ChangeSelectedActionBar(9);
                    }
                    else
                    {
                        UI.ChangeSelectedActionBar(i - 1);
                    }


                }
            }

        if (Input.GetKeyDown(KeyCode.O))
        {
            sRB.AddForce(new Vector2(5, 10));
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            CheckTileInfo();
        }





        

        if (Input.GetKeyDown(KeyCode.W))
        {
            CheckTileInfo();
        }

        if (Input.GetKeyUp(KeyCode.U))
        {
            
        }

        if (Input.GetKey(KeyCode.Q))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 200, 1 << LayerMask.NameToLayer("MouseCoordsRaycast")))
            {
                MapData.PlaceTile(hit.point, 5);
            }


            // gameObject.transform.Translate(mov);
        }

        if (Input.GetKey(KeyCode.R))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 200, 1 << LayerMask.NameToLayer("MouseCoordsRaycast")))
            {
                MapData.DestroyTile(hit.point);
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            hMov = -1;

        }

        if (Input.GetKey(KeyCode.D))
        {
            hMov = 1;

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryActive = !inventoryActive;
            UI.SwitchInventoryState(inventoryActive);
        }


        if (Input.GetKeyDown(KeyCode.P))
        {
            FilesHandler.SaveMap();
            //gameObject.transform.Translate(movy);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            MapData.SunBrightness -= 0.1f;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Vec2 tile = World.WorldToTileCoords(GetMousePos());   

            if(MapData.mapArray[tile.x, tile.y].tileType == (int)TileType.Grass)
            {
                MapData.CreateTree(tile, false);
            }
        }

        if (Input.GetKeyUp(KeyCode.J))
        {
           // _playerLight = LightControl.CreatePlayerLight(World.WorldToTileCoords(transform.position), 20);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            FillItemMethodsList();
        }

    }

    private void CheckTileInfo()
    {
        Vec2 tilev2 = World.WorldToTileCoords(_mousePos);

        Debug.Log($"tileType: {MapData.mapArray[tilev2.x, tilev2.y].tileType} ({Texturing.GetTextureName(MapData.mapArray[tilev2.x, tilev2.y].tileType)})," +
            $" Tile Background Type: {MapData.mapArray[tilev2.x, tilev2.y].tileBackgroundType}," +
            $" tileTexture: {MapData.mapArray[tilev2.x, tilev2.y].tileTexture}," +
            $" isSolid: {MapData.mapArray[tilev2.x, tilev2.y].IsSolid()}," +
            $" is Light Source: {LightControl.IsTileAmbientLightSource(tilev2.x, tilev2.y)}," +
            $" brightness: {MapData.mapArray[tilev2.x, tilev2.y].tileBrightness}," +
            $" is empty: {LightControl.IsAreaEmpty(tilev2.x, tilev2.y)}");
    }

}
