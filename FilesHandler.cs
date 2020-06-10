using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class FilesHandler
{
    /*      PLAYER FILE STRUCT
     *      string PlayerName
     *      int PlayerMaxHealth
     *      int PlayerMaxMana
     *      bool SoftCore => byte. 00000001 true 00000000 false
     *      ab_slot[0, 0]
     *      ab_slot[0, 1]
     *      inv_slot[0, 0]
     *      inv_slot[0, 1]
     *      coin_slot
     *      ammo_slot
     *      trash_slot  
     *      --armor_slot--
     *      head
     *      chest
     *      legs
     *      accessory_slot
     *      social_slot
     *      dye_slot
     *      playerHeadTexture
     *      playerBodyTexture
     *      playerLegsTexture
     *      playerEyesTexture
     *      playerBaseCharacterTexture

     
    */
    public static void LoadPlayerInGame(string playerName, GameObject playerObj)
    {
        PlayerControl player = playerObj.GetComponent<PlayerControl>();
        PacketBuffer buffer = new PacketBuffer();
        buffer.WriteBytes(File.ReadAllBytes(Application.streamingAssetsPath + "/Players/" + playerName + ".plr"));

        player.PlayerName = buffer.ReadString();
        player.PlayerMaxHealth = buffer.ReadInteger();
        player.PlayerMaxMana = buffer.ReadInteger();
        byte softcore = buffer.ReadByte();
        byte mask = 1;

        if((softcore & mask) == 1)
        {
            player.Softcore = true;
        }
        else
        {
            player.Softcore = false;
        }

        for (int i = 0; i < UI._actionBarSlots; i++)
        {
            Inventory.ab_slot[i, 0] = buffer.ReadInteger();
            Inventory.ab_slot[i, 1] = buffer.ReadInteger();
        }

        for (int i = 0; i < UI._inventorySlots; i++)
        {
            Inventory.inv_slot[i, 0] = buffer.ReadInteger();
            Inventory.inv_slot[i, 1] = buffer.ReadInteger();
        }

        for (int i = 0; i < UI._coinSlots; i++)
        {
            Inventory.coin_slot[i, 0] = buffer.ReadInteger();
            Inventory.coin_slot[i, 1] = buffer.ReadInteger();
        }

        for (int i = 0; i < UI._ammoSlots; i++)
        {
            Inventory.ammo_slot[i, 0] = buffer.ReadInteger();
            Inventory.ammo_slot[i, 1] = buffer.ReadInteger();
        }

        for (int i = 0; i < UI._trashSlots; i++)
        {
            Inventory.trash_slot[i, 0] = buffer.ReadInteger();
            Inventory.trash_slot[i, 1] = buffer.ReadInteger();
        }

        Inventory.armor_slot[0, 0] = buffer.ReadInteger();
        Inventory.armor_slot[0, 1] = buffer.ReadInteger();
        Inventory.armor_slot[1, 0] = buffer.ReadInteger();
        Inventory.armor_slot[1, 1] = buffer.ReadInteger();
        Inventory.armor_slot[2, 0] = buffer.ReadInteger();
        Inventory.armor_slot[2, 1] = buffer.ReadInteger();

        for (int i = 0; i < UI._accessorySlots; i++)
        {
            Inventory.accessory_slot[i, 0] = buffer.ReadInteger();
            Inventory.accessory_slot[i, 1] = buffer.ReadInteger();
        }

        for (int i = 0; i < UI._socialSlots; i++)
        {
            Inventory.social_slot[i, 0] = buffer.ReadInteger();
            Inventory.social_slot[i, 1] = buffer.ReadInteger();
        }

        for (int i = 0; i < UI._dyeSlots; i++)
        {
            Inventory.dye_slot[i, 0] = buffer.ReadInteger();
            Inventory.dye_slot[i, 1] = buffer.ReadInteger();
        }
/*
        Inventory.playerHeadTexture = buffer.ReadInteger();
        Inventory.playerBodyTexture = buffer.ReadInteger();
        Inventory.playerLegsTexture = buffer.ReadInteger();
        //Inventory.playerEyesTexture = buffer.ReadInteger();
        Inventory.playerBaseCharacterTexture = buffer.ReadInteger();
        */

    }

    public static void DeleteCharacter(string playerName)
    {
        string[] playerNames = CheckPlayerFiles();

        foreach (string name in playerNames)
        {
            if (playerName == name)
            {
                File.Delete(Application.streamingAssetsPath + "/Players/" + playerName + ".player");
            }
        }
    }

    public static void DeleteMap(string mapName)
    {
          string[] mapNames = CheckMapFiles();

        foreach(string name in mapNames)
        {
            if(mapName == name)
            {
                File.Delete(Application.streamingAssetsPath + "/Saves/" + mapName + ".wrl");
            }
        }
    }


    public static void SavePlayer(GameObject playerObj)
    {
        PlayerControl player = playerObj.GetComponent<PlayerControl>();

        PacketBuffer buffer = new PacketBuffer();

        

        buffer.WriteString(player.PlayerName);
        buffer.WriteInteger(player.PlayerMaxHealth);
        buffer.WriteInteger(player.PlayerMaxMana);

        byte softcore = 1;

        if(!player.Softcore)
        {
            var writeByte = (byte)(softcore >> 1);
            buffer.WriteByte(writeByte);
        }
        else
        {
            buffer.WriteByte(softcore);
        }

        for (int i = 0; i < UI._actionBarSlots; i++)
        {
            buffer.WriteInteger(Inventory.ab_slot[i, 0]);
            buffer.WriteInteger(Inventory.ab_slot[i, 1]);
        }

        for (int i = 0; i < UI._inventorySlots; i++)
        {
            buffer.WriteInteger(Inventory.inv_slot[i, 0]);
            buffer.WriteInteger(Inventory.inv_slot[i, 1]);
        }

        for (int i = 0; i < UI._coinSlots; i++)
        {
            buffer.WriteInteger(Inventory.coin_slot[i, 0]);
            buffer.WriteInteger(Inventory.coin_slot[i, 1]);
        }

        for (int i = 0; i < UI._ammoSlots; i++)
        {
            buffer.WriteInteger(Inventory.ammo_slot[i, 0]);
            buffer.WriteInteger(Inventory.ammo_slot[i, 1]);
        }

        for (int i = 0; i < UI._trashSlots; i++)
        {
            buffer.WriteInteger(Inventory.trash_slot[i, 0]);
            buffer.WriteInteger(Inventory.trash_slot[i, 1]);
        }


            buffer.WriteInteger(Inventory.armor_slot[0, 0]);
            buffer.WriteInteger(Inventory.armor_slot[0, 1]);
            buffer.WriteInteger(Inventory.armor_slot[1, 0]);
            buffer.WriteInteger(Inventory.armor_slot[1, 1]);
            buffer.WriteInteger(Inventory.armor_slot[2, 0]);
            buffer.WriteInteger(Inventory.armor_slot[2, 1]);

        for (int i = 0; i < UI._accessorySlots; i++)
        {
            buffer.WriteInteger(Inventory.accessory_slot[i, 0]);
            buffer.WriteInteger(Inventory.accessory_slot[i, 1]);
        }

        for (int i = 0; i < UI._socialSlots; i++)
        {
            buffer.WriteInteger(Inventory.social_slot[i, 0]);
            buffer.WriteInteger(Inventory.social_slot[i, 1]);
        }

        for (int i = 0; i < UI._dyeSlots; i++)
        {
            buffer.WriteInteger(Inventory.dye_slot[i, 0]);
            buffer.WriteInteger(Inventory.dye_slot[i, 1]);
        }

        /*
        buffer.WriteInteger(Inventory.playerHeadTexture);
        buffer.WriteInteger(Inventory.playerBodyTexture);
        buffer.WriteInteger(Inventory.playerLegsTexture);
        buffer.WriteInteger(Texturing.GetPlayerTextureID(Inventory.playerEyesTexture));
        buffer.WriteInteger(Inventory.playerBaseCharacterTexture);
        */  
        byte[] byteArray = buffer.ToArray();

        if (!Directory.Exists(Application.streamingAssetsPath + "/Players"))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath + "/Players");
        }

        File.WriteAllBytes(Application.streamingAssetsPath + "/Players/" + player.PlayerName + ".player", byteArray); // player.PlayerName + ".plr"



    }

    public static CharacterData LoadPlayerCharacterData(string playerName)
    {
        CharacterData cData = new CharacterData();
        PacketBuffer buffer = new PacketBuffer();
        buffer.WriteBytes(File.ReadAllBytes(Application.streamingAssetsPath + "/Players/" + playerName + ".player"));

        cData.saveSlot = buffer.ReadInteger();

        cData.playerName = buffer.ReadString();
        cData.playerMaxHealth = buffer.ReadInteger();
        cData.playerCurrentHealth = buffer.ReadInteger();
        cData.playerMaxMana = buffer.ReadInteger();
        cData.playerCurrentMana = buffer.ReadInteger();

        byte softcore = buffer.ReadByte();
        byte mask = 1;

        if ((softcore & mask) == 1)
        {
            cData.playerSoftcore = true;
        }
        else
        {
            cData.playerSoftcore = false;
        }

        cData.playerSkinHandTexture = buffer.ReadString();
        cData.playerSkinHeadTexture = buffer.ReadString();
        cData.playerSkinLegsTexture = buffer.ReadString();
        cData.playerSkinBodyTexture = buffer.ReadString();
        cData.playerTextureArmTexture = buffer.ReadString();
        cData.playerTextureBodyTexture = buffer.ReadString();
        cData.playerTextureHeadTexture = buffer.ReadString();
        cData.playerShoesTexture = buffer.ReadString();
        cData.playerHairTexture = buffer.ReadString();
        cData.playerTextureLegsTexture = buffer.ReadString();
        cData.playerEyesTexture = buffer.ReadString();

        // COLORS

        float r;
        float g;
        float b;

        r = buffer.ReadFloat();
        g = buffer.ReadFloat();
        b = buffer.ReadFloat();

        cData.playerSkinColor = new Color(r, g, b);

        r = buffer.ReadFloat();
        g = buffer.ReadFloat();
        b = buffer.ReadFloat();

        cData.playerArmColor = new Color(r, g, b);

        r = buffer.ReadFloat();
        g = buffer.ReadFloat();
        b = buffer.ReadFloat();

        cData.playerShirtColor = new Color(r, g, b);

        r = buffer.ReadFloat();
        g = buffer.ReadFloat();
        b = buffer.ReadFloat();

        cData.playerShoesColor = new Color(r, g, b);

        r = buffer.ReadFloat();
        g = buffer.ReadFloat();
        b = buffer.ReadFloat();

        cData.playerHairColor = new Color(r, g, b);

        r = buffer.ReadFloat();
        g = buffer.ReadFloat();
        b = buffer.ReadFloat();

        cData.playerPantsColor = new Color(r, g, b);

        r = buffer.ReadFloat();
        g = buffer.ReadFloat();
        b = buffer.ReadFloat();

        cData.playerHeadColor = new Color(r, g, b);

        r = buffer.ReadFloat();
        g = buffer.ReadFloat();
        b = buffer.ReadFloat();

        cData.playerEyesColor = new Color(r, g, b);


        cData.inv_slot = new int[40, 2];
        cData.ab_slot = new int[10, 2];
        cData.coin_slot = new int[4, 2];
        cData.ammo_slot = new int[4, 2];
        cData.trash_slot = new int[1, 2];
        cData.armor_slot = new int[3, 2];
        cData.accessory_slot = new int[5, 2];
        cData.social_slot = new int[8, 2];
        cData.dye_slot = new int[8, 2];

        for (int i = 0; i < UI._actionBarSlots; i++)
        {
            cData.ab_slot[i, 0] = buffer.ReadInteger();
            cData.ab_slot[i, 1] = buffer.ReadInteger();
        }

        for (int i = 0; i < UI._inventorySlots; i++)
        {
            cData.inv_slot[i, 0] = buffer.ReadInteger();
            cData.inv_slot[i, 1] = buffer.ReadInteger();
        }

        for (int i = 0; i < UI._coinSlots; i++)
        {
            cData.coin_slot[i, 0] = buffer.ReadInteger();
            cData.coin_slot[i, 1] = buffer.ReadInteger();
        }

        for (int i = 0; i < UI._ammoSlots; i++)
        {
            cData.ammo_slot[i, 0] = buffer.ReadInteger();
            cData.ammo_slot[i, 1] = buffer.ReadInteger();
        }

        for (int i = 0; i < UI._trashSlots; i++)
        {
            cData.trash_slot[i, 0] = buffer.ReadInteger();
            cData.trash_slot[i, 1] = buffer.ReadInteger();
        }

        for (int i = 0; i < UI._armorSlots; i++)
        {
            cData.armor_slot[i, 0] = buffer.ReadInteger();
            cData.armor_slot[i, 1] = buffer.ReadInteger();
        }

        for (int i = 0; i < UI._accessorySlots; i++)
        {
            cData.accessory_slot[i, 0] = buffer.ReadInteger();
            cData.accessory_slot[i, 1] = buffer.ReadInteger();
        }

        for (int i = 0; i < UI._socialSlots; i++)
        {
            cData.social_slot[i, 0] = buffer.ReadInteger();
            cData.social_slot[i, 1] = buffer.ReadInteger();
        }

        for (int i = 0; i < UI._dyeSlots; i++)
        {
            cData.dye_slot[i, 0] = buffer.ReadInteger();
            cData.dye_slot[i, 1] = buffer.ReadInteger();
        }

        return cData;
    }

    public static void SavePlayer(CharacterData cData)
    {
        PacketBuffer buffer = new PacketBuffer();

        buffer.WriteInteger(cData.saveSlot);
        buffer.WriteString(cData.playerName);
        buffer.WriteInteger(cData.playerMaxHealth);
        buffer.WriteInteger(cData.playerCurrentHealth);
        buffer.WriteInteger(cData.playerMaxMana);
        buffer.WriteInteger(cData.playerCurrentMana);

        byte softcore = 1;

        if (!cData.playerSoftcore)
        {
            var writeByte = (byte)(softcore >> 1);
            buffer.WriteByte(writeByte);
        }
        else
        {
            buffer.WriteByte(softcore);
        }

        buffer.WriteString(cData.playerSkinHandTexture);
        buffer.WriteString(cData.playerSkinHeadTexture);
        buffer.WriteString(cData.playerSkinLegsTexture);
        buffer.WriteString(cData.playerSkinBodyTexture);
        buffer.WriteString(cData.playerTextureArmTexture);
        buffer.WriteString(cData.playerTextureBodyTexture);
        buffer.WriteString(cData.playerTextureHeadTexture);
        buffer.WriteString(cData.playerShoesTexture);
        buffer.WriteString(cData.playerHairTexture);
        buffer.WriteString(cData.playerTextureLegsTexture);
        buffer.WriteString(cData.playerEyesTexture);

        // COLORS - r g b
        // playerSkinColor
        buffer.WriteFloat(cData.playerSkinColor.r);
        buffer.WriteFloat(cData.playerSkinColor.g);
        buffer.WriteFloat(cData.playerSkinColor.b);

        //playerArmColor
        buffer.WriteFloat(cData.playerArmColor.r);
        buffer.WriteFloat(cData.playerArmColor.g);
        buffer.WriteFloat(cData.playerArmColor.b);

        //playerShirtColor
        buffer.WriteFloat(cData.playerShirtColor.r);
        buffer.WriteFloat(cData.playerShirtColor.g);
        buffer.WriteFloat(cData.playerShirtColor.b);

        //playerShoesColor
        buffer.WriteFloat(cData.playerShoesColor.r);
        buffer.WriteFloat(cData.playerShoesColor.g);
        buffer.WriteFloat(cData.playerShoesColor.b);

        //playerHairColor
        buffer.WriteFloat(cData.playerHairColor.r);
        buffer.WriteFloat(cData.playerHairColor.g);
        buffer.WriteFloat(cData.playerHairColor.b);

        //playerPantsColor
        buffer.WriteFloat(cData.playerPantsColor.r);
        buffer.WriteFloat(cData.playerPantsColor.g);
        buffer.WriteFloat(cData.playerPantsColor.b);

        //playerHeadColor
        buffer.WriteFloat(cData.playerHeadColor.r);
        buffer.WriteFloat(cData.playerHeadColor.g);
        buffer.WriteFloat(cData.playerHeadColor.b);

        //playerEyesColor
        buffer.WriteFloat(cData.playerEyesColor.r);
        buffer.WriteFloat(cData.playerEyesColor.g);
        buffer.WriteFloat(cData.playerEyesColor.b);

        // ab_slot[10, 2]
        for (int i = 0; i < 10; i++)
        {
            buffer.WriteInteger(cData.ab_slot[i, 0]);
            buffer.WriteInteger(cData.ab_slot[i, 1]);
        }

        // inv_slot[10, 2]
        for (int i = 0; i < 40; i++)
        {
            buffer.WriteInteger(cData.inv_slot[i, 0]);
            buffer.WriteInteger(cData.inv_slot[i, 1]);
        }

        // coin_slot[4, 2]
        for (int i = 0; i < 4; i++)
        {
            buffer.WriteInteger(cData.coin_slot[i, 0]);
            buffer.WriteInteger(cData.coin_slot[i, 1]);
        }

        // ammo_slot[4, 2]
        for (int i = 0; i < 4; i++)
        {
            buffer.WriteInteger(cData.ammo_slot[i, 0]);
            buffer.WriteInteger(cData.ammo_slot[i, 1]);
        }

        // trash_slot[1, 2]
            buffer.WriteInteger(cData.trash_slot[0, 0]);
            buffer.WriteInteger(cData.trash_slot[0, 1]);

        // armor_slot[3, 2]
        for (int i = 0; i < 3; i++)
        {
            buffer.WriteInteger(cData.armor_slot[i, 0]);
            buffer.WriteInteger(cData.armor_slot[i, 1]);
        }

        // accessory_slot[5, 2]
        for (int i = 0; i < 5; i++)
        {
            buffer.WriteInteger(cData.accessory_slot[i, 0]);
            buffer.WriteInteger(cData.accessory_slot[i, 1]);
        }

        // social_slot[8, 2]
        for (int i = 0; i < 8; i++)
        {
            buffer.WriteInteger(cData.social_slot[i, 0]);
            buffer.WriteInteger(cData.social_slot[i, 1]);
        }

        // dye_slot[8, 2]
        for (int i = 0; i < 8; i++)
        {
            buffer.WriteInteger(cData.dye_slot[i, 0]);
            buffer.WriteInteger(cData.dye_slot[i, 1]);
        }




        byte[] byteArray = buffer.ToArray();

        if (!Directory.Exists(Application.streamingAssetsPath + "/Players"))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath + "/Players");
        }

        File.WriteAllBytes(Application.streamingAssetsPath + "/Players/" + cData.playerName + ".player", byteArray); // player.PlayerName + ".plr"



    }

    public static string[] CheckMapFiles()
    {
        string[] fullNames = Directory.GetFiles(Application.streamingAssetsPath + "/Saves/", "*.wrl").Select(Path.GetFileName).ToArray();

        string[] cutNames = new string[fullNames.Length];

        for (int i = 0; i < fullNames.Length; i++)
        {
            int index = fullNames[i].IndexOf('.'); // fix

            cutNames[i] = fullNames[i].Substring(0, index);
        }

        return cutNames;
    }

    public static string[] CheckPlayerFiles()
    {
        string[] fullNames = Directory.GetFiles(Application.streamingAssetsPath + "/Players/", "*.player").Select(Path.GetFileName).ToArray();

        string[] cutNames = new string[fullNames.Length];

        for (int i = 0; i < fullNames.Length; i++)
        {
            int index = fullNames[i].IndexOf('.'); // fix

            cutNames[i] = fullNames[i].Substring(0, index);
        }

        return cutNames;

    }

    

    public static void LoadMap(string mapName)
    {
        int skips = 0;

        MapData.mapArray = null;

        PacketBuffer buffer = new PacketBuffer();
        buffer.WriteBytes(File.ReadAllBytes(Application.streamingAssetsPath + "/Saves/" + mapName + ".wrl"));

        MapData.mapSize = buffer.ReadByte();
        MapData.mapName = buffer.ReadString();


        MapData.totalHorizontalTiles = MapData.GetTotalHorizontalTiles(MapData.mapSize);
        MapData.totalVerticalTiles = MapData.GetTotalVerticalTiles(MapData.mapSize);
  
        MapData.mapArray = new Tile[MapData.totalHorizontalTiles, MapData.totalVerticalTiles];
        Tile copyPasteTile = new Tile(1, 1, 1, 1, 1, 1, 1, 1);

        bool done = false;

        for (int x = 0; x < MapData.totalHorizontalTiles; x++)
            for (int y = 0; y < MapData.totalVerticalTiles; y++)
            {


                if (skips > 0)
                {
                    if (!done)
                    {
                        done = true;
                    }

                    MapData.mapArray[x, y] = copyPasteTile;
                    skips--;
                    continue;
                }

                byte tileType = buffer.ReadByte();
                byte tileTexture = buffer.ReadByte();
                byte tileBackgroundType = buffer.ReadByte();
                byte tileBackgroundTexture = buffer.ReadByte();
                byte tileBackgroundRotation = buffer.ReadByte();
                byte tileRotation = buffer.ReadByte();



                copyPasteTile = new Tile(tileType, tileTexture, tileRotation, (short)x, (short)y, tileBackgroundType, tileBackgroundTexture, tileBackgroundRotation);


                MapData.mapArray[x, y] = new Tile(copyPasteTile);

                byte skipsByte = buffer.ReadByte();

                if(skipsByte > 0)
                {
                    skips = skipsByte;
                }
                
            }

        buffer.Dispose();
        MapData.chunksArray = null;
        MapData.chunksArray = new Chunk[200 * 50];
        MapData.PopulateChunksLists();
        

    }

    public static void SaveMap()
    {
        int skips = 0;

        PacketBuffer buffer = new PacketBuffer();
        byte worldSize = (byte)MapData.mapSize;


        // Combine bytes
        buffer.WriteByte(worldSize);
        buffer.WriteString(MapData.mapName);



        for (int x = 0; x < MapData.totalHorizontalTiles; x++)
            for (int y = 0; y < MapData.totalVerticalTiles; y++)
            {
                if(skips > 0)
                {
                    skips--;
                    continue;
                }

                buffer.WriteByte(MapData.mapArray[x, y].tileType);
                buffer.WriteByte(MapData.mapArray[x, y].tileTexture);
                buffer.WriteByte(MapData.mapArray[x, y].tileBackgroundType);
                buffer.WriteByte(MapData.mapArray[x, y].tileBackgroundTexture);
                buffer.WriteByte(MapData.mapArray[x, y].tileBackgroundRotation);
                buffer.WriteByte(MapData.mapArray[x, y].tileRotation);

                int identicalTiles = FindIdenticalTiles(x, y);

                byte tileSkips = (byte)identicalTiles;

                // identifying byte 
                buffer.WriteByte(tileSkips);

                if(identicalTiles > 0)
                {
                    skips = identicalTiles;

                }
           
            }


        byte[] byteArray = buffer.ToArray();

        if (!Directory.Exists(Application.streamingAssetsPath + "/Saves"))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath + "/Saves");
        }

        File.WriteAllBytes(Application.streamingAssetsPath + "/Saves/" + MapData.mapName +  ".wrl", byteArray);

        buffer.Dispose();
    }

    private static int FindIdenticalTiles(int _x, int _y)
    {
        int countDown = 255;
        int identicalTiles = 0;

        Tile tileUnderScrutiny = MapData.mapArray[_x, _y];

        // Going to start checking from the next tile

        bool doneFirstPass = false;
        Vec2 nextTile = JumpToNextTile(_x, _y);

        // QUADRUPLE CHECK THIS
        for (int x = nextTile.x; x < MapData.totalHorizontalTiles; x++)
        {
            if(doneFirstPass)
            {
                nextTile.y = 0;
            }

            for (int y = nextTile.y; y < MapData.totalVerticalTiles; y++)
            {


                if (countDown <= 0)
                {
                    return identicalTiles;
                }

                // the tile under scrutiny atm is the one before _x and _y
                if (tileUnderScrutiny == (MapData.mapArray[x, y]))
                {
                    // change it to the tile of the current for loop iteration, so the next loop its gonna be the next tile
                    //tileUnderScrutiny = MapData.mapArray[x, y];
                    countDown--;
                    identicalTiles++;
                }
                else
                {
                    return identicalTiles;
                }
            }

            doneFirstPass = true;
        }
            

        return 0;
    }

    private static Vec2 JumpToNextTile(int x, int y)
    {

        y++;

        if (y == MapData.totalVerticalTiles)
        {
            x++;
            y = 0;
            return new Vec2(x, y);
        }

        
        return new Vec2(x, y);

        
    }
}
