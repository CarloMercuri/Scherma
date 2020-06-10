using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;


public static class MapData
{

    public const int smallWorldGridHeight = 50;
    public const int smallWorldGridLenght = 200;
    private const int chunkSize = 32;

    public static Vector2 playerSpawnCoords;
    public static Chunk[] chunksArray;
    private static int newBlockUpdate = 10;


    // variables for world creation
    public static int worldGridHeight;
    public static int worldGridLenght;
    private static int groundBreakpoint;
    public static int totalVerticalTiles;
    public static int totalHorizontalTiles;
    public static int mapSize; // 1 = small, 2 = medium, 3 = large
    public static string mapName;

    public static int[] breakPointArray;

    // Collections
    public static Tile[,] mapArray;
    public static List<TreeData> treeList;


    public static bool mapCreated = false;

    // References
    public static GameObject chunkControl;
    private static TileProperties tileDatabase;
    //public static GameObject menuControl;
    public static MenuController _menuControl;
    public static GameController gameController;


    private static System.Random rand;


    // Lighting
    public static float SunBrightness = 1f;


    public static void LoadData()
    {
        tileDatabase = Resources.Load<TileProperties>("Databases/TileDB");
        rand = new System.Random();
    }

    public static bool DestroyTile(int x, int y)
    {
        GCoords tileCoords = World.TileCoordsToGCoords(x, y);

        if (!mapArray[tileCoords.tileX, tileCoords.tileY].IsSolid())
            return false;

        //mapArray[tileCoords.tileX, tileCoords.tileY].isSolid = false;
        //ChangeTile(tileCoords.tileX, tileCoords.tileY, TileTexture.FullSquare, TileType.Empty, 1);
        RemoveTile(tileCoords.tileX, tileCoords.tileY);

        TileCheck[] surroundingBlocks = GetNumberSurroundingBlocks(tileCoords);

        if (surroundingBlocks.Length > 0)
            foreach (TileCheck tile in surroundingBlocks)
            {
                if (mapArray[tile.x, tile.y].tileType != (int)TileType.Grass)
                {
                    UpdateTile(new Vec2(tile.x, tile.y));
                }

            }

        CheckForGrass(tileCoords);
        //MeshControl.UpdateGridTextures(tileCoords.gridID);
        //LightControl.UpdateTilesLight(tileCoords.tileX - newBlockUpdate, tileCoords.tileX + newBlockUpdate, tileCoords.tileY - newBlockUpdate, tileCoords.tileY + newBlockUpdate);
        return true;

    }

    public static bool DestroyTile(Vector2 pos)
    {
        GCoords tileCoords = World.WorldToGCoords(pos);

        if (!mapArray[tileCoords.tileX, tileCoords.tileY].IsSolid())
            return false;

        //mapArray[tileCoords.tileX, tileCoords.tileY].isSolid = false;
        //ChangeTile(tileCoords.tileX, tileCoords.tileY, TileTexture.FullSquare, TileType.Empty, 1);
        RemoveTile(tileCoords.tileX, tileCoords.tileY);

        TileCheck[] surroundingBlocks = GetNumberSurroundingBlocks(tileCoords);

        if (surroundingBlocks.Length > 0)
            foreach (TileCheck tile in surroundingBlocks)
            {
                if (mapArray[tile.x, tile.y].tileType != (int)TileType.Grass)
                {
                    UpdateTile(new Vec2(tile.x, tile.y));
                }

            }

        CheckForGrass(tileCoords);
        //MeshControl.UpdateGridTextures(tileCoords.gridID);
        //LightControl.UpdateTilesLight(tileCoords.tileX - newBlockUpdate, tileCoords.tileX + newBlockUpdate, tileCoords.tileY - newBlockUpdate, tileCoords.tileY + newBlockUpdate);
        return true;

    }

    public static bool PlaceBackgroundTile(Vector2 pos, int tileType)
    {
        Vec2 coords = World.WorldToTileCoords(pos);

        // if there's no block in a radius of 1 around, and if there's no background tile on it, return false
        if (!IsBlockOrBackgroundAround(coords, 1) && mapArray[coords.x, coords.y].tileBackgroundType == 0)
            return false;

        //    3 4 5
        //    2   6
        //    1 8 7

        BuildBackgroundTile(coords.x - 1, coords.y - 1, tileType, 1);
        BuildBackgroundTile(coords.x - 1, coords.y, tileType, 2);
        BuildBackgroundTile(coords.x - 1, coords.y + 1, tileType, 3);
        BuildBackgroundTile(coords.x, coords.y + 1, tileType, 4);
        BuildBackgroundTile(coords.x + 1, coords.y + 1, tileType, 5);
        BuildBackgroundTile(coords.x + 1, coords.y, tileType, 6);
        BuildBackgroundTile(coords.x + 1, coords.y - 1, tileType, 7);
        BuildBackgroundTile(coords.x, coords.y - 1, tileType, 8);

        return true;
    }                                                                                 //    3 4 5
                                                                                      //    2   6
    private static void BuildBackgroundTile(int x, int y, int tileType, int section)  //    1 8 7
    {
        int rotation;

        switch(section)
        {
            case 1:
                if(CheckTileTexture(TileTexture.bg_CornerSquare, mapArray[x, y].tileBackgroundTexture))
                {
                    rotation = mapArray[x, y].tileBackgroundRotation;

                    if (rotation == 1)
                        ChangeBackgroundTile(x, y, "bg_FullSquare", tileType, 1);
                    if(rotation == 4)
                        ChangeBackgroundTile(x, y, "bg_HalfSquare", tileType,  3);
                    if(rotation == 2)
                        ChangeBackgroundTile(x, y, "bg_HalfSquare", tileType, 2);
                    if(rotation == 3)
                        ChangeBackgroundTile(x, y, "bg_CornerSquare", tileType, 3);
                }
                break;

            case 2:
                break;

            case 3:
                break;

            case 4:
                break;

            case 5:
                break;

            case 6:
                break;

            case 7:
                break;

            case 8:
                break;

        }

     
 



    }

    private static bool IsBlockAround(Vec2 coords, int distance)
    {
        int tx = coords.x - distance;
        int ty = coords.y - distance;

        for (int x = 0; x < distance * 2; x++)
            for (int y = 0; y < distance * 2; y++)
            {
                if (mapArray[tx + x, ty + y].IsSolid())
                    return true;
            }
        return false;
    }

    private static bool IsBlockOrBackgroundAround(Vec2 coords, int distance)
    {
        int tx = coords.x - distance;
        int ty = coords.y - distance;

        for (int x = 0; x < distance * 2; x++)
            for (int y = 0; y < distance * 2; y++)
            {
                if (mapArray[tx + x, ty + y].IsSolid() || mapArray[tx + x, ty + y].tileBackgroundType != 0)
                    return true;
            }
        return false;
    }

    public static bool PlaceTile(Vector2 pos, int tileType)
    {
        GCoords tileInfo = World.WorldToGCoords(pos);
 
        if (mapArray[tileInfo.tileX, tileInfo.tileY].IsSolid())
        {
            if(Debugging.debuggingBlockPlacement == true)
            {
                Debug.Log(Debugging.ERR_SELECTED_IS_SOLID);
            }
           return false;
        }
           

        if (tileType == 0)
        {
            if (Debugging.debuggingBlockPlacement == true)
                Debug.Log(Debugging.ERR_BLOCK_TYPE_INVISIBLE);

            return false;
        }

        // Check that there's blocks nearby to grab to

        for (int x = -1; x < 2; x++)
        {
            if (mapArray[tileInfo.tileX + x, tileInfo.tileY + 1].IsSolid())
                break;
            if (mapArray[tileInfo.tileX + x, tileInfo.tileY].IsSolid())
                break;
            if (mapArray[tileInfo.tileX + x, tileInfo.tileY - 1].IsSolid())
                break;

            if (x >= 1)
            {
                if (Debugging.debuggingBlockPlacement == true)
                    Debug.Log(Debugging.ERR_NO_ADJACENT_BLOCK);
                return false;
            }
        }

        mapArray[tileInfo.tileX, tileInfo.tileY].tileType = (byte)tileType;
        UpdateTile(new Vec2(tileInfo.tileX, tileInfo.tileY));
        UpdateNearbyTiles(tileInfo);

  

       // LightControl.UpdateTilesLight(tileInfo.tileX - newBlockUpdate, tileInfo.tileX + newBlockUpdate, tileInfo.tileY - newBlockUpdate, tileInfo.tileY + newBlockUpdate);
       // LightControl.UpdateColumnSunLight(new Vec2(tileInfo.tileX, tileInfo.tileY));
        return true;
    }

    public static bool IsTileSolid(Tile tile)
    {
        if (tile.tileType != 0 && tile.tileType != 7 && tile.tileType != 8)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private static void UpdateNearbyTiles(GCoords tileInfo)
    {

        /*
        int tx = tileInfo.tileX - 1;
        int ty = tileInfo.tileY - 1;

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (mapArray[tx + x, ty + y].tileType == (int)TileType.Grass)
                {
                    UpdateGrassTile(new Vec2(tx + x, ty + y));
                }
                else
                {
                    Debug.Log("n");
                    UpdateTile(new Vec2(tx + x, ty + y));
                }
                    
            }

        }
        */

        TileCheck[] surroundingBlocks = GetNumberSurroundingBlocks(tileInfo);

        if (surroundingBlocks.Length > 0)
            foreach (TileCheck tile in surroundingBlocks)
            {
                if (mapArray[tile.x, tile.y].tileType != (int)TileType.Grass)
                {
                    UpdateTile(new Vec2(tile.x, tile.y));
                }

            }

        CheckForGrass(tileInfo);

    }

    private static void CheckForGrass(GCoords tileInfo)
    {

        int tx = tileInfo.tileX - 1;
        int ty = tileInfo.tileY - 1;

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (mapArray[tx + x, ty + y].tileType == (int)TileType.Grass)
                    UpdateGrassTile(new Vec2(tx + x, ty + y));
            }

        }
    }

    public static void CreateTree(Vec2 tile, bool isFromMenu)
    {
        List<int> affectedGrids = new List<int>();

        if(!isFromMenu)
        {
            if (!IsAreaFlat(tile, 2))
            {
                return;
            }
        }


        // Create roots
        if (rand.Next(0, 2) == 0) // only 1 root
        {
            if(rand.Next(0, 2) == 0) // 1 root to the left
            {
                mapArray[tile.x - 1, tile.y + 1].tileType = (byte)Texturing.GetTileTextureID("Tree_1");
                mapArray[tile.x - 1, tile.y + 1].tileTexture = GetTileTexture("Tree_Body_Roots_Left");
                mapArray[tile.x - 1, tile.y + 1].tileRotation = 1;

                if (!affectedGrids.Contains(World.TileCoordsToGridNumber(tile.x - 1, tile.y + 1)))
                    affectedGrids.Add(World.TileCoordsToGridNumber(tile.x - 1, tile.y + 1));

                mapArray[tile.x, tile.y + 1].tileType = (byte)Texturing.GetTileTextureID("Tree_1");
                mapArray[tile.x, tile.y + 1].tileTexture = GetTileTexture("Tree_Body_Base_Left");
                mapArray[tile.x, tile.y + 1].tileRotation = 1;

                if (!affectedGrids.Contains(World.TileCoordsToGridNumber(tile.x, tile.y + 1)))
                    affectedGrids.Add(World.TileCoordsToGridNumber(tile.x, tile.y + 1));
            }
            else // 1 root to the right
            {
                mapArray[tile.x + 1, tile.y + 1].tileType = (byte)Texturing.GetTileTextureID("Tree_1");
                mapArray[tile.x + 1, tile.y + 1].tileTexture = GetTileTexture("Tree_Body_Roots_Right");
                mapArray[tile.x + 1, tile.y + 1].tileRotation = 1;

                if (!affectedGrids.Contains(World.TileCoordsToGridNumber(tile.x + 2, tile.y + 1)))
                    affectedGrids.Add(World.TileCoordsToGridNumber(tile.x + 2, tile.y + 1));

                mapArray[tile.x, tile.y + 1].tileType = (byte)Texturing.GetTileTextureID("Tree_1");
                mapArray[tile.x, tile.y + 1].tileTexture = GetTileTexture("Tree_Body_Base_Right");
                mapArray[tile.x, tile.y + 1].tileRotation = 1;

                if (!affectedGrids.Contains(World.TileCoordsToGridNumber(tile.x, tile.y + 1)))
                    affectedGrids.Add(World.TileCoordsToGridNumber(tile.x, tile.y + 1));
            }
        }
        else // 2 roots
        {
            mapArray[tile.x - 1, tile.y + 1].tileType = (byte)Texturing.GetTileTextureID("Tree_1");
            mapArray[tile.x - 1, tile.y + 1].tileTexture = GetTileTexture("Tree_Body_Roots_Left");
            mapArray[tile.x - 1, tile.y + 1].tileRotation = 1;

            if (!affectedGrids.Contains(World.TileCoordsToGridNumber(tile.x - 1, tile.y + 1)))
                affectedGrids.Add(World.TileCoordsToGridNumber(tile.x - 1, tile.y + 1));

            mapArray[tile.x + 1, tile.y + 1].tileType = (byte)Texturing.GetTileTextureID("Tree_1");
            mapArray[tile.x + 1, tile.y + 1].tileTexture = GetTileTexture("Tree_Body_Roots_Right");
            mapArray[tile.x + 1, tile.y + 1].tileRotation = 1;

            if (!affectedGrids.Contains(World.TileCoordsToGridNumber(tile.x - 1, tile.y + 1)))
                affectedGrids.Add(World.TileCoordsToGridNumber(tile.x - 1, tile.y + 1));

            mapArray[tile.x, tile.y + 1].tileType = (byte)Texturing.GetTileTextureID("Tree_1");
            mapArray[tile.x, tile.y + 1].tileTexture = GetTileTexture("Tree_Body_Base_Both");
            mapArray[tile.x, tile.y + 1].tileRotation = 1;

            if (!affectedGrids.Contains(World.TileCoordsToGridNumber(tile.x, tile.y + 1)))
                affectedGrids.Add(World.TileCoordsToGridNumber(tile.x, tile.y + 1));
        }

        // Already placed the roots and base
        tile.y += 2;


        // Randomizing the height of the tree
        int treeHeight = rand.Next(14, 19);
        
        int remainingBranches = 4;

        int branchPause = 0;

        for (int y = 0; y < treeHeight; y++)
        {
            
            if(rand.Next(0, 11) >= 6 && y > 6 && remainingBranches > 0 && branchPause <= 0) // Branch. Making sure it doesen't place a branch at bottom level
            {
                if(rand.Next(0, 2) == 0) // branch on the left
                {
                    mapArray[tile.x, tile.y].tileType = (byte)Texturing.GetTileTextureID("Tree_1");
                    mapArray[tile.x, tile.y].tileTexture = GetTileTexture("Tree_Body_Hole");
                    mapArray[tile.x, tile.y].tileRotation = 1;

                    if (!affectedGrids.Contains(World.TileCoordsToGridNumber(tile.x, tile.y)))
                        affectedGrids.Add(World.TileCoordsToGridNumber(tile.x, tile.y));

                    int branchRootTexture = GetTreeBranchRoot();

                    mapArray[tile.x - 1, tile.y].tileType = (byte)Texturing.GetTileTextureID("Tree_Branches_1");
                    mapArray[tile.x - 1, tile.y].tileTexture = (byte)branchRootTexture;
                    mapArray[tile.x - 1, tile.y].tileRotation = 1;

                    if (!affectedGrids.Contains(World.TileCoordsToGridNumber(tile.x - 1, tile.y)))
                        affectedGrids.Add(World.TileCoordsToGridNumber(tile.x - 1, tile.y));

                    mapArray[tile.x - 2, tile.y].tileType = (byte)Texturing.GetTileTextureID("Tree_Branches_1");
                    mapArray[tile.x - 2, tile.y].tileTexture = (byte)(branchRootTexture - 1);
                    mapArray[tile.x - 2, tile.y].tileRotation = 1;

                    if (!affectedGrids.Contains(World.TileCoordsToGridNumber(tile.x - 2, tile.y)))
                        affectedGrids.Add(World.TileCoordsToGridNumber(tile.x - 2, tile.y));

                    remainingBranches--;
                }
                else // branch on the right
                {
                    mapArray[tile.x, tile.y].tileType = (byte)Texturing.GetTileTextureID("Tree_1");
                    mapArray[tile.x, tile.y].tileTexture = GetTileTexture("Tree_Body_Hole");
                    mapArray[tile.x, tile.y].tileRotation = 3;


                    if (!affectedGrids.Contains(World.TileCoordsToGridNumber(tile.x, tile.y)))
                        affectedGrids.Add(World.TileCoordsToGridNumber(tile.x, tile.y));

                    int branchRootTexture = GetTreeBranchRoot();

                    mapArray[tile.x + 1, tile.y].tileType = (byte)Texturing.GetTileTextureID("Tree_Branches_1");
                    mapArray[tile.x + 1, tile.y].tileTexture = (byte)branchRootTexture;
                    mapArray[tile.x + 1, tile.y].tileRotation = 3;


                    if (!affectedGrids.Contains(World.TileCoordsToGridNumber(tile.x + 1, tile.y)))
                        affectedGrids.Add(World.TileCoordsToGridNumber(tile.x + 1, tile.y));

                    mapArray[tile.x + 2, tile.y].tileType = (byte)Texturing.GetTileTextureID("Tree_Branches_1");
                    mapArray[tile.x + 2, tile.y].tileTexture = (byte)(branchRootTexture - 1);
                    mapArray[tile.x + 2, tile.y].tileRotation = 3;

                    if (!affectedGrids.Contains(World.TileCoordsToGridNumber(tile.x + 2, tile.y)))
                        affectedGrids.Add(World.TileCoordsToGridNumber(tile.x + 2, tile.y));

                    remainingBranches--;
                }

                branchPause = 3;
            }
            else // Normal body
            {
                mapArray[tile.x, tile.y].tileType = (byte)Texturing.GetTileTextureID("Tree_1");
                mapArray[tile.x, tile.y].tileTexture = GetTileTexture("Tree_Body");
                mapArray[tile.x, tile.y].tileRotation = 1;

                if (!affectedGrids.Contains(World.TileCoordsToGridNumber(tile.x, tile.y)))
                    affectedGrids.Add(World.TileCoordsToGridNumber(tile.x, tile.y));

                if (branchPause > 0)
                    branchPause--;
            }

            tile.y++;
        }

        // Create the tree top
        /*
        LightControl.UpdateTilesLight(tile.x - 2, tile.x + 2, tile.y, tile.y + 20);
        LightControl.UpdateColumnSunLight(new Vec2(tile.x, tile.y));
        LightControl.UpdateColumnSunLight(new Vec2(tile.x - 1, tile.y));
        LightControl.UpdateColumnSunLight(new Vec2(tile.x - 2, tile.y));
        LightControl.UpdateColumnSunLight(new Vec2(tile.x + 1, tile.y));
        LightControl.UpdateColumnSunLight(new Vec2(tile.x + 2, tile.y));
        */

        if(!isFromMenu)
        {
            foreach (int grid in affectedGrids)
            {
                MeshControl.UpdateGridTextures(grid);
            }
        }


    }

    
    private static void UpdateTile(Vec2 tile)
    {
        TileCheck[] tiles = GetNumberSurroundingBlocks(tile);

        switch (tiles.Length)
        {
            case 0:
                ChangeTile(tile.x, tile.y, "LonelyBlock", 1);
                break;

            case 1:
                // If there's only one block around, it has to be the cap one, now need to check if the adjacent block is earth or not.
                ChangeTile(tile.x, tile.y, "Cap", tiles[0].tileNumber);

                break;
            case 2:
                Vec2 tN = new Vec2(tiles[0].tileNumber, tiles[1].tileNumber);

                switch (tN.x)
                {
                    case 1:
                        if (tN.y == 2)  // means it's a 90' corner, down and left
                        {
                            //ChangeTile(tile.x, tile.y, TileTexture.RoughSlope, 2);
                            ChangeTile(tile.x, tile.y, "Corner", 1);
                            break;
                        }

                        if (tN.y == 3) // down and up
                        {
                            ChangeTile(tile.x, tile.y, "Highway", 1);
                            break;
                        }

                        if (tN.y == 4) // means it's a 90' corner, down and right
                        {
                            //ChangeTile(tile.x, tile.y, TileTexture.RoughSlope, 1);
                            ChangeTile(tile.x, tile.y, "Corner", 4);
                            break;
                        }
                        break;
                    case 2:
                        if (tN.y == 3) // 90' corner, left and up
                        {
                            ChangeTile(tile.x, tile.y, "Corner", 2);
                            break;
                        }
                        if (tN.y == 4) // left and right
                        {
                            ChangeTile(tile.x, tile.y, "Highway", 2);
                            break;
                        }
                        break;
                    case 3:
                        if (tN.y == 4) // has to be. 90' corner, up and right
                        {
                            ChangeTile(tile.x, tile.y, "Corner", 3);
                            break;
                        }
                        break;
                    default:
                        break;
                }

                break;



            case 3:
                Vec3 tN3 = new Vec3(tiles[0].tileNumber, tiles[1].tileNumber, tiles[2].tileNumber);

                switch (tN3.x)
                {
                    case 1:
                        //      X
                        //    X T O 
                        //      X

                        if (tN3.y == 2 && tN3.z == 3)
                        {
                            ChangeTile(tile.x, tile.y, "Flat", 2);
                            break;
                        }
                        if (tN3.y == 2 && tN3.z == 4)
                        {
                            ChangeTile(tile.x, tile.y, "Flat", 1);
                            break;
                        }

                        //       X
                        //     O T X
                        //       X
                        if (tN3.y == 3 && tN3.z == 4)
                        {
                            ChangeTile(tile.x, tile.y, "Flat", 4);
                            break;
                        }
                        break;
                    case 2: // can only be 2, 3, 4
                        ChangeTile(tile.x, tile.y, "Flat", 3);

                        break;

                    default:
                        break;

                }


                break;

            //      X
            //   X  O  X
            //      X
            //
            case 4:

                ChangeTile(tile.x, tile.y, "FullSquare", 1);

                break;

        }
    }

    private static void UpdateGrassTile(Vec2 tile)
    {
        TileCheck[] tiles = GetNumberSurroundingBlocks(tile);

        switch(tiles.Length)
        {
            case 0:
                ChangeTile(tile.x, tile.y, "LonelyBlock", 1);
                break;

            case 1:
                // If there's only one block around, it has to be the cap one, now need to check if the adjacent block is earth or not.
                ChangeTile(tile.x, tile.y, "Cap", tiles[0].tileNumber);
               
                break;
            case 2:
                Vec2 tN = new Vec2(tiles[0].tileNumber, tiles[1].tileNumber);

                switch (tN.x)
                {
                    case 1:
                        if (tN.y == 2)  // means it's a 90' corner, down and left
                        {
                            //ChangeTile(tile.x, tile.y, TileTexture.RoughSlope, 2);
                            ChangeTile(tile.x, tile.y, "Corner", 1);
                            break;
                        }

                        if (tN.y == 3) // down and up
                        {
                            ChangeTile(tile.x, tile.y, "Highway", 1);
                            break;
                        }

                        if (tN.y == 4) // means it's a 90' corner, down and right
                        {
                            //ChangeTile(tile.x, tile.y, TileTexture.RoughSlope, 1);
                            ChangeTile(tile.x, tile.y, "Corner", 4);
                            break;
                        }
                        break;
                    case 2:
                        if (tN.y == 3) // 90' corner, left and up
                        {
                            ChangeTile(tile.x, tile.y, "RoughSlope", 3);
                            break;
                        }
                        if (tN.y == 4) // left and right
                        {
                            ChangeTile(tile.x, tile.y, "Highway", 2);
                            break;
                        }
                        break;
                    case 3:
                        if (tN.y == 4) // has to be. 90' corner, up and right
                        {
                            ChangeTile(tile.x, tile.y, "RoughSlope", 4);
                            break;
                        }
                        break;
                    default:
                        break;
                }

                break;



            case 3:
                Vec3 tN3 = new Vec3(tiles[0].tileNumber, tiles[1].tileNumber, tiles[2].tileNumber);

                switch (tN3.x)
                {
                    case 1:
                        //      X
                        //    X T O 
                        //      X

                        if (tN3.y == 2 && tN3.z == 3)
                        {
                            ChangeTile(tile.x, tile.y, "Flat", 2);
                            break;
                        }
                        if (tN3.y == 2 && tN3.z == 4)
                        {
                            ChangeTile(tile.x, tile.y, "Flat", 1);
                            break;
                        }

                        //       X
                        //     O T X
                        //       X
                        if (tN3.y == 3 && tN3.z == 4)
                        {
                            ChangeTile(tile.x, tile.y, "Flat", 4);
                            break;
                        }
                        break;
                    case 2: // can only be 2, 3, 4
                        ChangeTile(tile.x, tile.y, "Flat", 3);

                        break;

                    default:
                        break;

                }


                break;

                //      X
                //   X  O  X
                //      X
                //
            case 4:

                // if all 3 above are empty.  Should add OR ARE TREES. It's not gonna happen with this style
                
                if (!mapArray[tile.x - 1, tile.y + 1].IsSolid() && !mapArray[tile.x, tile.y + 1].IsSolid() && !mapArray[tile.x + 1, tile.y + 1].IsSolid())
                    ChangeTile(tile.x, tile.y, "FullSquare", TileType.Earth);

                // X X X
                if (mapArray[tile.x - 1, tile.y + 1].IsSolid() && mapArray[tile.x, tile.y + 1].IsSolid() && mapArray[tile.x + 1, tile.y + 1].IsSolid())
                    ChangeTile(tile.x, tile.y, "FullSquare", TileType.Earth);
                // X o X
                if (mapArray[tile.x - 1, tile.y + 1].IsSolid() && !mapArray[tile.x, tile.y + 1].IsSolid() && mapArray[tile.x + 1, tile.y + 1].IsSolid())
                    ChangeTile(tile.x, tile.y, "Flat");

                // o X X
                if (!mapArray[tile.x - 1, tile.y + 1].IsSolid() && mapArray[tile.x, tile.y + 1].IsSolid() && mapArray[tile.x + 1, tile.y + 1].IsSolid())
                {
                    if(mapArray[tile.x - 1, tile.y + 1].tileType != (int)TileType.Tree)
                    {
                        if(mapArray[tile.x - 1, tile.y].tileType != (int)TileType.Grass)
                        {
                            ChangeTile(tile.x, tile.y, "FullSquare", TileType.Earth);
                            return;
                        }
                        ChangeTile(tile.x, tile.y, "Grass_HalfSpoutLeft");
                    }
                }

                // o X o
                if (!mapArray[tile.x - 1, tile.y + 1].IsSolid() && mapArray[tile.x, tile.y + 1].IsSolid() && !mapArray[tile.x + 1, tile.y + 1].IsSolid())
                {
                    if (mapArray[tile.x, tile.y + 1].tileType != (int)TileType.Tree)
                    {
                        // nongrass, grass
                        if (mapArray[tile.x - 1, tile.y].tileType != (int)TileType.Grass && mapArray[tile.x + 1, tile.y].tileType == (int)TileType.Grass)
                        {
                            ChangeTile(tile.x, tile.y, "Grass_HalfSpoutRight");
                            return;
                        }

                        if (mapArray[tile.x - 1, tile.y].tileType == (int)TileType.Grass && mapArray[tile.x + 1, tile.y].tileType != (int)TileType.Grass)
                        {
                            ChangeTile(tile.x, tile.y, "Grass_HalfSpoutLeft");
                            return;
                        }

                        ChangeTile(tile.x, tile.y, "Grass_Spout");
                    }
                }

                // X X o
                if (mapArray[tile.x - 1, tile.y + 1].IsSolid() && mapArray[tile.x, tile.y + 1].IsSolid() && !mapArray[tile.x + 1, tile.y + 1].IsSolid())
                {
                    if (mapArray[tile.x, tile.y + 1].tileType != (int)TileType.Tree)
                    {
                        // decisions decisions
                        /*
                        if(mapArray[tile.x + 1, tile.y].tileType != (int)TileType.Grass)
                        {
                            ChangeTile(tile.x, tile.y, TileTexture.FullSquare, TileType.Earth, 1);
                            return;
                        }
                        */

                        ChangeTile(tile.x, tile.y, "Grass_HalfSpoutRight", 1);
                    }
                    
                }



                break;

        }
    }

    private static void ChangeBackgroundTile(int x, int y, string tileTexture, TileType tileType, int tileRotation)
    {
        mapArray[x, y].tileBackgroundType = (byte)tileType;
        mapArray[x, y].tileBackgroundTexture = GetTileTexture(tileTexture);
        mapArray[x, y].tileBackgroundRotation = (byte)tileRotation;
        //MeshControl.UpdateBackgroundTile(x, y);
    }

    private static void ChangeBackgroundTile(int x, int y, string tileTexture, int tileType, int tileRotation)
    {
        mapArray[x, y].tileBackgroundType = (byte)tileType;
        mapArray[x, y].tileBackgroundTexture = GetTileTexture(tileTexture);
        mapArray[x, y].tileBackgroundRotation = (byte)tileRotation;
        //MeshControl.UpdateBackgroundTile(x, y);
    }

    private static void ChangeBackgroundTile(int x, int y, string tileTexture, int tileRotation)
    {
        mapArray[x, y].tileBackgroundTexture = GetTileTexture(tileTexture);
        mapArray[x, y].tileBackgroundRotation = (byte)tileRotation;
        //MeshControl.UpdateBackgroundTile(x, y);
    }

    private static void ChangeTile(int x, int y, string tileTexture)
    {
        mapArray[x, y].tileTexture = GetTileTexture(tileTexture);
        MeshControl.UpdateTile(x, y);
    }

    private static void ChangeTile(int x, int y, string tileTexture, int tileRotation)
    {

        mapArray[x, y].tileTexture = GetTileTexture(tileTexture);
        mapArray[x, y].tileRotation = (byte)tileRotation;
        MeshControl.UpdateTile(x, y);
    }

    /*
    private static void ChangeTile(int x, int y, TileTexture tileTexture, int tileRotation)
    {
        if (mapArray[x, y].IsSolid() == false)
            mapArray[x, y].IsSolid(true);
        // if mapArray[x, y].tileType != tree
        mapArray[x, y].tileTexture = GetTileTexture(tileTexture);
        mapArray[x, y].tileRotation = (byte)tileRotation;
        MeshControl.UpdateTile(x, y);
    }
    */

    private static void ChangeTile(int x, int y, string tileTexture, TileType tileType)
    {
        mapArray[x, y].tileTexture = GetTileTexture(tileTexture);
        mapArray[x, y].tileType = (byte)tileType;
        MeshControl.UpdateTile(x, y);
    }

    private static void ChangeTile(int x, int y, string tileTexture, string tileName)
    {
        mapArray[x, y].tileTexture = GetTileTexture(tileTexture);
        mapArray[x, y].tileType = (byte)Texturing.GetTileTextureID(tileName);
        MeshControl.UpdateTile(x, y);
    }

    private static void ChangeTile(int x, int y, string tileTexture, TileType tileType, int tileRotation)
    {
        mapArray[x, y].tileTexture = GetTileTexture(tileTexture);
        mapArray[x, y].tileType = (byte)tileType;
        mapArray[x, y].tileRotation = (byte)tileRotation;
        MeshControl.UpdateTile(x, y);
    }

    private static void ChangeTile(int x, int y, string tileTexture, string tileName, int tileRotation)
    {
        mapArray[x, y].tileTexture = GetTileTexture(tileTexture);
        mapArray[x, y].tileType = (byte)Texturing.GetTileTextureID(tileName);
        mapArray[x, y].tileRotation = (byte)tileRotation;
        MeshControl.UpdateTile(x, y);
    }


    private static void RemoveTile(int x, int y)
    {
        mapArray[x, y].tileTexture = 1;
        mapArray[x, y].tileType = 0;
        mapArray[x, y].tileRotation = 1;
        MeshControl.UpdateTile(x, y);
    }

    private static TileCheck[] GetNumberSurroundingBlocks(GCoords tileInfo)
    {
        TileCheck testVec1 = new TileCheck(tileInfo.tileX, tileInfo.tileY - 1, 1);
        TileCheck testVec2 = new TileCheck(tileInfo.tileX - 1, tileInfo.tileY, 2);
        TileCheck testVec3 = new TileCheck(tileInfo.tileX, tileInfo.tileY + 1, 3);
        TileCheck testVec4 = new TileCheck(tileInfo.tileX + 1, tileInfo.tileY, 4);

        List<TileCheck> vecsList = new List<TileCheck>();


        if (mapArray[testVec1.x, testVec1.y].IsSolid()) vecsList.Add(testVec1);
        if (mapArray[testVec2.x, testVec2.y].IsSolid()) vecsList.Add(testVec2);
        if (mapArray[testVec3.x, testVec3.y].IsSolid()) vecsList.Add(testVec3);
        if (mapArray[testVec4.x, testVec4.y].IsSolid()) vecsList.Add(testVec4);

        return vecsList.ToArray();
    }

    
    private static TileCheck[] GetNumberSurroundingBlocks(Vec2 tile)
    {
        TileCheck testVec1 = new TileCheck(tile.x, tile.y - 1, 1);
        TileCheck testVec2 = new TileCheck(tile.x - 1, tile.y, 2);
        TileCheck testVec3 = new TileCheck(tile.x, tile.y + 1, 3);
        TileCheck testVec4 = new TileCheck(tile.x + 1, tile.y, 4);

        List<TileCheck> vecsList = new List<TileCheck>();


        if (mapArray[testVec1.x, testVec1.y].IsSolid()) vecsList.Add(testVec1);
        if (mapArray[testVec2.x, testVec2.y].IsSolid()) vecsList.Add(testVec2);
        if (mapArray[testVec3.x, testVec3.y].IsSolid()) vecsList.Add(testVec3);
        if (mapArray[testVec4.x, testVec4.y].IsSolid()) vecsList.Add(testVec4);

        return vecsList.ToArray();
    }

    /// <summary>
    /// Populates the chunks list
    /// </summary>
    public static void PopulateChunksLists()
    {
        for (int i = 0; i < chunksArray.Length ; i++)  // worldGridHeight * worldGridLenght
        {
            chunksArray[i] = new Chunk(i, null, null);
        }
    }


    public static void ResetWorldData()
    {
        mapArray = null;
        chunksArray = null;
    }


    /// <summary>
    /// The big red button
    /// </summary>
    /// <param name="worldSize"></param>
    /// <param name="worldName"></param>
    public static void CreateWorld(int worldSize, string worldName)  // 1: small, 2: medium, 3: large
    {
        mapArray = null;



        switch (worldSize)
        {
            case 1:
                worldGridHeight = smallWorldGridHeight;
                worldGridLenght = smallWorldGridLenght;
                playerSpawnCoords = new Vector2(513, 158);
                groundBreakpoint = 960;
                totalHorizontalTiles = 32 * worldGridLenght;
                totalVerticalTiles = 32 * worldGridHeight;
                break;

            default:    // default should be medium
                worldGridHeight = smallWorldGridHeight;
                worldGridLenght = smallWorldGridLenght;
                playerSpawnCoords = new Vector2(513, 158);
                groundBreakpoint = 960;
                totalHorizontalTiles = 32 * worldGridLenght;
                totalVerticalTiles = 32 * worldGridHeight;
                break;
        }

        mapArray = new Tile[totalHorizontalTiles, totalVerticalTiles];
        //chunksArray = new Chunk[worldGridHeight * worldGridLenght];

        mapSize = worldSize;

        CreateBreakpointArray();
      
        CreateBaseMap(worldSize);
        AddTrees();

       // PopulateChunksLists();
        
        mapCreated = true;

        mapName = worldName;
        
        FilesHandler.SaveMap();


        UnityThread.executeInUpdate(() =>
        {
            //chunkControl.GetComponent<ChunkControl>().LoadMainEvent();
            _menuControl.DoneCreatingWorld();
        });
        



    }

    private static void AddTrees()
    {
        int minimumTreeSeparation = 4;

        int treePause = 0;

        // start from the 10th tile and end 10 tiles from the end
        for (int i = 10; i < totalHorizontalTiles - 10; i++)
        {
            

            if(treePause > 0)
            {
                treePause--;
                continue;
            }

            if (mapArray[i, breakPointArray[i]].tileType == Texturing.GetTileTextureID("Grass") && CheckTileTexture(TileTexture.Flat, mapArray[i, breakPointArray[i]].tileTexture))
            {
                if(IsAreaFlat(new Vec2(i, breakPointArray[i]), 3))
                {
                    CreateTree(new Vec2(i, breakPointArray[i]), true);
                    treePause = rand.Next(3, 7);
                }
            }
        }
    }

    private static bool IsAreaFlat(Vec2 tile, int radius)
    {
        tile.x -= radius;

        for (int i = 0; i < radius * 2 + 1; i++)
        {
            if(tile.x > totalHorizontalTiles || tile.x < 0)
            {
                return false;
            }
            if(!CheckTile(tile, "Grass", "Flat"))
            {
                return false;
            }

            tile.x++;
        }

        return true;
    }


    /// <summary>
    /// Creates all the basic colums, and refines grass
    /// </summary>
    /// <param name="worldSize"></param>
    private static void CreateBaseMap(int worldSize)
    {
        for (int i = 0; i < worldGridLenght * 32; i++)
        {
            CreateBaseColumn(i);
        }

        for (int i = 1; i < totalHorizontalTiles - 1; i++)
          RefineGrassTile(new Vec2(i, breakPointArray[i]));

  
    }

    /// <summary>
    /// Creates a very basic column given the breakpoint array
    /// </summary>
    /// <param name="columnNumber"></param>
    private static void CreateBaseColumn(int columnNumber)
    {
        for (int i = 0; i < totalVerticalTiles; i++)
        {
            if (i < 600)
            {
                mapArray[columnNumber, i] = new Tile(5, 1, 1, (short)columnNumber, (short)i, (byte)TileType.bg_Stone, (byte)2, 1); // , (int)TileType.bg_Stone, GetTileTexture(TileTexture.bg_FullSquare), 1
            }

            if (i >= 600 && i < breakPointArray[columnNumber])
            {
                mapArray[columnNumber, i] = new Tile(1, (byte)rand.Next(1, 5), 1, (short)columnNumber, (short)i, (byte)TileType.bg_Stone, (byte)2, 1);
            }

            if (i == breakPointArray[columnNumber])
            {
                //mapArray[columnNumber, i] = new Tile(2, GetTileTexture(TileTexture.Flat), true, 1, (short)columnNumber, (short)i);
                mapArray[columnNumber, i] = new Tile(2, GetTileTexture("Flat"), 1, (short)columnNumber, (short)i);
            }

            if (i > breakPointArray[columnNumber])
            {
                mapArray[columnNumber, i] = new Tile(0, 1, 1, (short)columnNumber, (short)i);
            }
        }


    }

    /// <summary>
    /// Fixes the orientation and texture of the grass tile
    /// </summary>
    /// <param name="currentTile"></param>
    private static void RefineGrassTile(Vec2 currentTile)
    {

        Tile rightTile = GetHorizontalTile(currentTile, 1);
        Tile leftTile = GetHorizontalTile(currentTile, -1);



        if (rightTile.IsSolid() == false && leftTile.IsSolid() == false)
        {
            mapArray[currentTile.x, currentTile.y].tileTexture = GetTileTexture("Cap");

            if(GetVerticalTile(currentTile, 1).IsSolid() == true && GetVerticalTile(currentTile, -1).IsSolid() == false)
            {
                mapArray[currentTile.x, currentTile.y].tileRotation = 3;
                
            }

            if (GetVerticalTile(currentTile, -1).IsSolid())
                mapArray[GetVerticalTileCoords(currentTile, -1).x, GetVerticalTileCoords(currentTile, -1).y].tileTexture = GetTileTexture("Flat_UnderCap");
            return;
        }

        if (rightTile.IsSolid() == false)
        {
            mapArray[currentTile.x, currentTile.y].tileTexture = GetTileTexture("RoughSlope");
            mapArray[currentTile.x, currentTile.y].tileRotation = 2;

            if (mapArray[currentTile.x + 1, currentTile.y - 1].IsSolid())
            {
                mapArray[currentTile.x, currentTile.y - 1].tileType = 2;
                mapArray[currentTile.x, currentTile.y - 1].tileTexture = GetTileTexture("Grass_HalfSpoutRight");
                //mapArray[currentTile.x, currentTile.y - 1].tileRotation = 2;
            }
            else
            {
                mapArray[currentTile.x, currentTile.y - 1].tileType = 2;
                mapArray[currentTile.x, currentTile.y - 1].tileTexture = GetTileTexture("Flat");
                mapArray[currentTile.x, currentTile.y - 1].tileRotation = 2;
            }


        }

        if (leftTile.IsSolid() == false)
        {
            mapArray[currentTile.x, currentTile.y].tileTexture = GetTileTexture("RoughSlope");
            mapArray[currentTile.x, currentTile.y].tileRotation = 1;

            if(mapArray[currentTile.x -1, currentTile.y - 1].IsSolid())
            {
                mapArray[currentTile.x, currentTile.y - 1].tileType = 2;
                mapArray[currentTile.x, currentTile.y - 1].tileTexture = GetTileTexture("Grass_HalfSpoutLeft");
                //mapArray[currentTile.x, currentTile.y - 1].tileRotation = 1;
            }
            else
            {
                mapArray[currentTile.x, currentTile.y - 1].tileType = 2;
                mapArray[currentTile.x, currentTile.y - 1].tileTexture = GetTileTexture("Flat");
                mapArray[currentTile.x, currentTile.y - 1].tileRotation = 4;
            }

           
        }

        if (rightTile.IsSolid() == true && rightTile.tileType == 1)
        {
            Vec2 rTile = GetHorizontalTileCoords(currentTile, 1);
            mapArray[rTile.x, rTile.y].tileType = 2;
            mapArray[rTile.x, rTile.y].tileTexture = GetTileTexture("Grass_HalfSpoutLeft");
        }

        if (leftTile.IsSolid() == true && leftTile.tileType == 1)
        {
            Vec2 lTile = GetHorizontalTileCoords(currentTile, -1);
            mapArray[lTile.x, lTile.y].tileType = 2;
            mapArray[lTile.x, lTile.y].tileTexture = GetTileTexture("Grass_HalfSpoutLeft");
            mapArray[lTile.x, lTile.y].tileRotation = 2;
        }
    }

    /// <summary>
    /// Returns the total horizontal tiles in each row
    /// </summary>
    /// <param name="mapSize"></param>
    /// <returns></returns>
    public static int GetTotalHorizontalTiles(int mapSize) // 1 small 2 medium 3 large
    {
        switch(mapSize)
        {
            case 1:
                return smallWorldGridLenght * 32;

            default:
                return 0;
        }
    }

    /// <summary>
    /// Returns the total number of vertical tiles in each column
    /// </summary>
    /// <param name="mapSize"></param>
    /// <returns></returns>
    public static int GetTotalVerticalTiles(int mapSize) // 1 small 2 medium 3 large
    {
        switch (mapSize)
        {
            case 1:
                return smallWorldGridHeight * 32;

            default:
                return 0;
        }
    }

    /// <summary>
    /// positive distance for up, negative for down
    /// </summary>
    /// <param name="tileNumber"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private static Tile GetVerticalTile(Vec2 tileCoords, int distance)
    {
        return mapArray[tileCoords.x, tileCoords.y + distance];
    }

    /// <summary>
    /// positive distance for up, negative for down
    /// </summary>
    /// <param name="tileNumber"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private static Vec2 GetVerticalTileCoords(Vec2 tileCoords, int distance)
    {
        return new Vec2(tileCoords.x, tileCoords.y + distance);
    }


    /// <summary>
    /// positive distance for right, negative for left
    /// </summary>
    /// <param name="tileNumber"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private static Tile GetHorizontalTile(Vec2 tileCoords, int distance)
    {
        return mapArray[tileCoords.x + distance, tileCoords.y];
    }

    /// <summary>
    /// positive distance for right, negative for left
    /// </summary>
    /// <param name="tileNumber"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private static Vec2 GetHorizontalTileCoords(Vec2 tileCoords, int distance)
    {
        return new Vec2(tileCoords.x + distance, tileCoords.y);
    }

    /// <summary>
    /// Creates the "Ground" level, with perlin noise
    /// </summary>
    private static void CreateBreakpointArray()
    {
        breakPointArray = new int[totalHorizontalTiles];

        
        for (int i = 0; i < breakPointArray.Length; i++)
        {
            breakPointArray[i] = groundBreakpoint + (int)(Mathf.PerlinNoise(i / 40f, 0) * 20) + (int)(Mathf.PerlinNoise(i / 30f, 0) * 10);
        }
        
    }

    /// <summary>
    /// Checks if the given tile is of a certain type
    /// </summary>
    /// <param name="textureCheck"></param>
    /// <param name="tileTexture"></param>
    /// <returns></returns>
    private static bool CheckTileTexture(TileTexture textureCheck, int tileTexture)
    {
        switch(textureCheck)
        {
            case TileTexture.FullSquare:
                if (tileTexture < 5)
                    return true;
                break;
            case TileTexture.LonelyBlock:
                if (tileTexture > 4 && tileTexture < 9)
                    return true;
                break;
            case TileTexture.Cap:
                if (tileTexture > 12 && tileTexture < 17)
                    return true;
                break;
            case TileTexture.Flat:
                if (tileTexture > 16 && tileTexture < 21)
                    return true;
                break;
            case TileTexture.RoughSlope:
                if (tileTexture > 20 && tileTexture < 25)
                    return true;
                break;
            case TileTexture.Corner:
                if (tileTexture > 24 && tileTexture < 29)
                    return true;
                break;
            case TileTexture.StraightSlope:
                if (tileTexture > 28 && tileTexture < 33)
                    return true;
                break;
            case TileTexture.Highway:
                if (tileTexture > 32 && tileTexture < 37)
                    return true;
                break;
            case TileTexture.Cap_AboveEarth:
                if (tileTexture > 36 && tileTexture < 41)
                    return true;
                break;
            case TileTexture.Flat_Earth:
                if (tileTexture > 40 && tileTexture < 45)
                    return true;
                break;
            case TileTexture.LonelyBlock_Earth:
                if (tileTexture > 44 && tileTexture < 49)
                    return true;
                break;
            case TileTexture.Cap_Earth:
                if (tileTexture > 48 && tileTexture < 53)
                    return true;
                break;
            case TileTexture.Highway_Earth:
                if (tileTexture > 52 && tileTexture < 57)
                    return true;
                break;
            case TileTexture.Flat_UnderCap:
                if (tileTexture > 56 && tileTexture < 59)
                    return true;
                break;
            case TileTexture.Flat_UnderCapCorner:
                if (tileTexture > 58 && tileTexture < 61)
                    return true;
                break;
            case TileTexture.Grass_CornerGrass:
                if (tileTexture > 60 && tileTexture < 65)
                    return true;
                break;
            case TileTexture.Grass_Spout:
                if (tileTexture > 56 && tileTexture < 59)
                    return true;
                break;
            case TileTexture.Grass_HalfSpoutLeft:
                if (tileTexture > 58 && tileTexture < 61)
                    return true;
                break;
            case TileTexture.Grass_HalfSpoutRight:
                if (tileTexture > 60 && tileTexture < 63)
                    return true;
                break;
            case TileTexture.bg_FullSquare:
                if (tileTexture < 5)
                    return true;
                break;
            case TileTexture.bg_HalfSquare:
                if (tileTexture > 4 && tileTexture < 9)
                    return true;
                break;
            case TileTexture.bg_CornerSquare:
                if (tileTexture > 8 && tileTexture < 13)
                    return true;
                break;

        }

        return false;
    }

    private static bool CheckTile(Vec2 tileLoc, string tileType, string tileTexture)
    {
        if(mapArray[tileLoc.x, tileLoc.y].tileType == Texturing.GetTileTextureID(tileType))
        {
            TileTextureGroup tG = tileDatabase.tileTextureGroups.Find(x => x.groupName.Equals(tileTexture));

            if (tG != null)
            {
                if(mapArray[tileLoc.x, tileLoc.y].tileTexture >= tG.textureMin && mapArray[tileLoc.x, tileLoc.y].tileTexture <= tG.textureMax)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false; // couldn't find anything
            }
        }
        else
        {
            return false;
        }
    }
    
    /*
    /// <summary>
    /// Returns a random choice of the specific texture
    /// </summary>
    /// <param name="tTexture"></param>
    /// <returns></returns>
    private static byte GetTileTexture(TileTexture tTexture)
    {
        System.Random rand = new System.Random();
        switch (tTexture)
        {
            case TileTexture.FullSquare:
                return (byte)rand.Next(1, 5);

            case TileTexture.LonelyBlock:
                return (byte)rand.Next(5, 9);

            case TileTexture.Cap:
                return (byte)rand.Next(13, 15);

            case TileTexture.Flat:
                return (byte)rand.Next(17, 19);

            case TileTexture.RoughSlope:
                return (byte)rand.Next(21, 25);

            case TileTexture.Corner:
                return (byte)rand.Next(25, 29);

            case TileTexture.StraightSlope:
                return (byte)rand.Next(29, 33);

            case TileTexture.Highway:
                return (byte)rand.Next(33, 37);

            case TileTexture.Cap_AboveEarth:
                return (byte)rand.Next(37, 41);

            case TileTexture.Flat_Earth:
                return (byte)rand.Next(41, 45);

            case TileTexture.LonelyBlock_Earth:
                return (byte)rand.Next(45, 49);

            case TileTexture.Cap_Earth:
                return (byte)rand.Next(49, 53);

            case TileTexture.Highway_Earth:
                return (byte)rand.Next(53, 57);

            case TileTexture.Flat_UnderCap:
                return (byte)rand.Next(57, 59);

            case TileTexture.Flat_UnderCapCorner:
                return (byte)rand.Next(59, 61);

            case TileTexture.Grass_CornerGrass:
                return (byte)rand.Next(63, 65);

            case TileTexture.Grass_Spout:
                return (byte)rand.Next(57, 59);

            case TileTexture.Grass_HalfSpoutLeft:
                return (byte)rand.Next(59, 61);

            case TileTexture.Grass_HalfSpoutRight:
                return (byte)rand.Next(61, 63);

            case TileTexture.bg_FullSquare:
                return (byte)rand.Next(1, 6);

            case TileTexture.bg_HalfSquare:
                return (byte)rand.Next(5, 9);

            case TileTexture.bg_CornerSquare:
                return (byte)rand.Next(9, 13); 

            case TileTexture.Tree_Body:
                return (byte)rand.Next(1, 9);

            case TileTexture.Tree_Body_Hole:
                return (byte)rand.Next(9, 17);

            case TileTexture.Tree_Body_Base_Left:
                return (byte)rand.Next(17, 25);

            case TileTexture.Tree_Body_Base_Right:
                return (byte)rand.Next(25, 33);

            case TileTexture.Tree_Body_Roots_Left:
                return (byte)rand.Next(33, 41);

            case TileTexture.Tree_Body_Roots_Right:
                return (byte)rand.Next(41, 49);

            case TileTexture.Tree_Cap:
                return (byte)rand.Next(49, 57);

            default:
                return 1;
        }
    }
    */


    

    private static byte GetTileTexture(string typeName)
    {
        TileTextureGroup tG = tileDatabase.tileTextureGroups.Find(x => x.groupName.Equals(typeName));

        if (tG != null)
        {
            return (byte)rand.Next(tG.textureMin, tG.textureMax + 1);
        }

        return 0;

    }

    private static int GetTreeBranchRoot()
    {
        return rand.Next(1, 9) * 2; // return a random even number between 2 and 16
    }
}
