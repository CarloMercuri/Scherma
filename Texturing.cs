using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Texturing
{

    // Directory paths
    private const string _path = "Textures/TileTextures/";
    private const string _tilesTexturesPath = "Textures/TileTextures/";
    private const string _playerTexturesPath = "Textures/Player/PlayerSprites";

    // Public sheets variables
    public static Texture2D tileSpriteSheet { get { return tilesTextures; } }
    public static Texture2D playerSpriteSheet { get { return playerTextures; } }

    // Player textures
    public static Rect[] playerRects;
    private static Rect[,] playerIdToRect;
    private static List<Texture2D> playerSplitTextures = new List<Texture2D>();
    private static Dictionary<string, int> playerTextureDict;
    private static Texture2D playerTextures;
    private static int playerSheetSlots = 20;
    private static int playerSheetGridYSize = 56;
    private static int playerSheetGridXSize = 40;

    // Tile Textures
    public static Rect[] tileRects;
    private static TileSprite[] tileSpritesArray;
    private static List<Texture2D> splitTextures = new List<Texture2D>();
    private static Dictionary<string, int> tileTexturesDict;
    private static Texture2D tilesTextures;
    private static int tileSheetGridSize = 16; // Make dynamic
    private static int tileSheetSquaresCount = 16; // Make dynamic
    private static TileProperties tileDatabase;



    public static void LoadTileTextures()
    {
        tileTexturesDict = new Dictionary<string, int>();
        // Load the tiles Database
        tileDatabase = Resources.Load<TileProperties>("Databases/TileDB");

        // Load the texture sheets in a predetermined order
        Texture2D[] _tileSheets = GetTexturesArray();


        // Start filling up the names in the tileSpritesArray
        tileSpritesArray = new TileSprite[tileDatabase.tileList.Count];

        for (int i = 0; i < tileSpritesArray.Length; i++)
        {
            tileSpritesArray[i] = new TileSprite(tileDatabase.tileList[i].tileName, null);
        }

        // To store the count of the tile textures of every tile type
        int[] tileTexturesCount = new int[tileSpritesArray.Length];

        // Split each texture sheet into the sub textures, and add them to splitTextures list
        for (int i = 0; i < _tileSheets.Length; i++)
        {
            if (_tileSheets[i].name.IndexOf("Tree") > -1)
            {
            }

            // Populate splitTextures list
            tileTexturesCount[i] = SplitTileSheetDynamic(_tileSheets[i]);
        }

        // Convert the list into an array
        Texture2D[] textures = splitTextures.ToArray();
        // Clear the list in case of future use
        splitTextures.Clear();

        // Create the texture atlas
        tilesTextures = new Texture2D(1, 1);
        tileRects = tilesTextures.PackTextures(textures, 0);

        // So the textures are sharp
        tilesTextures.filterMode = FilterMode.Point;

        // Fill the tile sprites array
        int currentIndex = 0;

        for (int i = 0; i < tileSpritesArray.Length; i++)
        {
            tileSpritesArray[i].tileTextureRects = new Rect[tileTexturesCount[i] + 1];
            tileSpritesArray[i].tileTextureRects[0] = tileRects[0]; // the texture type 0 of every tile type should be the Empty one

            for (int u = 0; u < tileTexturesCount[i]; u++)
            {
                tileSpritesArray[i].tileTextureRects[u + 1] = tileRects[currentIndex];
                currentIndex++;

            }
        }

        FillTileTexturesDictionary();

    }

    private static void FillTileTexturesDictionary()
    {
        for (int i = 0; i < tileSpritesArray.Length; i++)
        {
            tileTexturesDict.Add(tileSpritesArray[i].tileName, i);
        }
    }


    private static Texture2D[] GetTexturesArray()
    {
        // Load tiles in the same order as the database

        Texture2D[] tempArray = new Texture2D[tileDatabase.tileList.Count];

        for (int i = 0; i < tileDatabase.tileList.Count; i++)
        {
            if(tileDatabase.tileList[i].tileTextureSheetName != "")
            {
                tempArray[i] = Resources.Load<Texture2D>("Textures/TileTextures/" + tileDatabase.tileList[i].tileTextureSheetName);
            }
            else
            {
                tempArray[i] = Resources.Load<Texture2D>("Textures/TileTextures/tiles_Empty");
            }

            
        }

        return tempArray;

    }

    private static Texture2D FindPlayerTexture(string textureName, int step)
    {
        foreach (Texture2D t in playerSplitTextures)
        {
            if(t.name == (textureName + "_" + step))
            {
                return t;
            }
        }

        return null;
    }

  
    private static int SplitTileSheetDynamic(Texture2D tileSheet)
    {
        // The whole point of gotTransparent is that the texture with ID 0 is going to be an empty texture, and I only need one of those
        bool gotTransparent = false;

        // Make it start from 1.
        int textureNumber = 1;


        // Count the textures that have been cut, to feed to []tileTexturesCount in the main method
        int textureCount = 0;
        
        for (int y = 0; y < (tileSheet.height + 1) / 17; y++)
        {
            for (int x = 0; x < (tileSheet.width + 1 ) / 17; x++)
            {

                // Simplified: GetPixels x: 17 * x // Starts at 0 then jumps 17 pixels because of the red lines. Y: 119 - (17 * y). Has to start at 119 because 135-16 = 119 (16 because there's no red line on top)
                Color[] pixels = tileSheet.GetPixels((tileSheetGridSize + 1) * x, (tileSheet.height - tileSheetGridSize) - (tileSheetGridSize + 1) * y, tileSheetGridSize, tileSheetGridSize);

                if (IsTileSquareTransparent(pixels))
                    if (gotTransparent == false)
                    {
                        gotTransparent = true;
                    }
                    else
                    {
                        continue;
                    }


                // Add the texture to the list, and name it "texturename_xxx"
                splitTextures.Add(new Texture2D(tileSheetGridSize, tileSheetGridSize));
                splitTextures[splitTextures.Count - 1].SetPixels(pixels);  
                splitTextures[splitTextures.Count - 1].filterMode = FilterMode.Point; // So the tiles are pixel sharp instead of being blurry
                splitTextures[splitTextures.Count - 1].Apply();
                splitTextures[splitTextures.Count - 1].name = tileSheet.name + "_" + (textureNumber);

                textureCount++;
                textureNumber++;
            }
        }

        return textureCount;
    }


    public static void LoadPlayerTextures()
    {

        playerTextureDict = new Dictionary<string, int>();

        // Load the texture sheets in a predetermined order
        Texture2D[] _playerTileSheet = Resources.LoadAll<Texture2D>(_playerTexturesPath); //

        playerIdToRect = new Rect[_playerTileSheet.Length, playerSheetSlots];
        int playerSheetCount = 0;


        foreach (Texture2D playerSheet in _playerTileSheet)
        {
            playerTextureDict.Add(playerSheet.name, playerSheetCount);
            SplitPlayerTextureSheet(playerSheet);
            playerSheetCount++;
        }

        Texture2D[] _textures = playerSplitTextures.ToArray();

        playerTextures = new Texture2D(1, 1);
        playerRects = playerTextures.PackTextures(_textures, 0);
        playerTextures.filterMode = FilterMode.Point;


        for (int i = 0; i < _playerTileSheet.Length; i++)
        {
            for (int u = 0; u < playerSheetSlots; u++)
            {
                playerIdToRect[i, u] = playerRects[(i * 20) + u];
            }
        }


    }

    private static void SplitPlayerTextureSheet(Texture2D texture)
    {
        int textureNumber = 1;

        for (int y = 0; y < playerSheetSlots; y++)
        {

            //Color[] pixels = texture.GetPixels(0, 56 * y, playerSheetGridXSize, playerSheetGridYSize);
            Color[] pixels = texture.GetPixels(0, (texture.height / 20) * y, playerSheetGridXSize, playerSheetGridYSize);


            playerSplitTextures.Add(new Texture2D(playerSheetGridXSize, playerSheetGridYSize));
            playerSplitTextures[playerSplitTextures.Count - 1].SetPixels(pixels);     // y * (tileSheetGridSize / 2
            playerSplitTextures[playerSplitTextures.Count - 1].filterMode = FilterMode.Point;
            playerSplitTextures[playerSplitTextures.Count - 1].Apply();
            playerSplitTextures[playerSplitTextures.Count - 1].name = texture.name + "_" + (textureNumber);
            textureNumber++;

        }
    }

    public static int CheckHairStylesAmount()
    {
        int hairStylesAmount = 0;
        int hairNr = 1;

        for (int i = 0; i < playerTextureDict.Count; i++)
        {
            if (playerTextureDict.ContainsKey("Player_Hair_" + hairNr))
            {
                hairStylesAmount++;
                hairNr++;
            }
            
        }

       
        return hairStylesAmount;
    }

    private static string GetTileSheetName(string texture)
    {
        int underscore = texture.IndexOf('_');
        underscore++;

        string newName = StringTools.FirstLetterToUpperCase(texture.Substring(underscore, texture.Length - underscore));
        return newName;
        
    }


    /// <summary>
    /// Get the player texture ID from a string
    /// </summary>
    /// <param name="textureName"></param>
    /// <returns></returns>
    public static int GetPlayerTextureID(string textureName)
    {
        return playerTextureDict[textureName];
    }

    /// <summary>
    /// Get the texture rect given the ID of the texture and the step
    /// </summary>
    /// <param name="textureType"></param>
    /// <param name="textureStep"></param>
    /// <returns></returns>
    public static Rect GetPlayerTextureRect(int textureType, int textureStep)
    {
        return playerIdToRect[textureType, textureStep];
    }

    /// <summary>
    /// Get the texture rect given the name of the texture and the step
    /// </summary>
    /// <param name="textureName"></param>
    /// <param name="textureStep"></param>
    /// <returns></returns>
    public static Rect GetPlayerTextureRect(string textureName, int textureStep)
    {
        return playerIdToRect[playerTextureDict[textureName], textureStep];
    }


    /// <summary>
    /// Returns true if all the pixels in the color array are transparent
    /// </summary>
    /// <param name="colorArray"></param>
    /// <returns></returns>
    private static bool IsTileSquareTransparent(Color[] colorArray)
    {
        foreach (Color c in colorArray)
        {
            if (c.a != 0)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Get the rects for the texture given an ID
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="tileTexture"></param>
    /// <returns></returns>
    /// 
    public static Rect GetTileTexture(int tileType, int tileTexture)
    {
        return tileSpritesArray[tileType].tileTextureRects[tileTexture];
    }

    public static Rect GetTileTexture(string tileName, int tileTexture)
    {

        if(tileTexturesDict.TryGetValue(tileName, out int index))
        {
            return tileSpritesArray[index].tileTextureRects[tileTexture];
        }
        else
        {
            return tileSpritesArray[0].tileTextureRects[1]; // return empty tile
        }
        
    }

    public static int GetTileTextureID(string tileName)
    {
        if(tileTexturesDict.ContainsKey(tileName))
        {
            return tileTexturesDict[tileName];
        }
        else
        {
            Debug.Log("Dictionary does not contain the key: " + tileName);
            return 0;

        }
     
    }


    public static string GetTextureName(int ID)
    {
        return tileSpritesArray[ID].tileName;
    }

    /*
    [Obsolete("Deprecated. Use IDs instead")]
    public static Rect getTextureFromString(string textureName)
    {
        return tilesTextureDict[textureName];
    }
    */

}


