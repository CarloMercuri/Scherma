using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshControl
{
    public static Material material;

    private static Vector2[] tileGrassUV;
    private static Vector2[] tileEarthUV;

    private static Color baseColor = new Color(1, 1, 1);

    // Mesh variables
    //private static Mesh mesh = new Mesh();
    private static List<Vector3> _vertices = new List<Vector3>();
    private static List<Vector2> _uv = new List<Vector2>();
    private static List<int> _triangles = new List<int>();
    private static List<Color> _colors = new List<Color>();
    private static int tileX;
    private static int tileY;
    private static Color newColor;
    private static float tileBrightness;
    private static float meshVerticesSeparation = 0.161f;
    private static int trianglesCount = 0;
    private static int chunkMeshVerticesZ = 0;
    private static int chunkMeshVerticesZOffset = 1;
    private static int chunkMeshLayerSeparation = 4096;

    private static Vector2 uv1;
    private static Vector2 uv2;
    private static Vector2 uv3;
    private static Vector2 uv4;

    private static Vector2 pos;
    private static Vec2 startingTile;

    // PLAYER MESH
    private static Rect playerRects;
    private static int playerMeshFaces = 10;


    // GameObjects
    public static GameObject playerMeshObject;
    private static GameObject meshObject;

    private const float vertexPadding = 0.16f;

    private static Rect setMeshRect;



    

    private static int chunkSize = 32;


    


    private static void ClearMeshVars()
    {
        //mesh.Clear();
        trianglesCount = 0;
        _vertices.Clear();
        _triangles.Clear();
        _uv.Clear();
        _colors.Clear();
    }

    public static void UpdateTile(int tileX, int tileY)
    {
        Rect rect;
        rect = Texturing.GetTileTexture(MapData.mapArray[tileX, tileY].tileType, MapData.mapArray[tileX, tileY].tileTexture);
        GCoords tileInfo = World.TileCoordsToGCoords(tileX, tileY);
        Mesh mesh = MapData.chunksArray[tileInfo.gridID].gridObject.GetComponent<MeshFilter>().mesh;

        int tileID = tileInfo.inGridX * 32 + tileInfo.inGridY;
        tileID *= 4;

        Vector2[] uvTemp = new Vector2[MapData.chunksArray[tileInfo.gridID].gridObject.GetComponent<MeshFilter>().mesh.uv.Length];
        uvTemp = MapData.chunksArray[tileInfo.gridID].gridObject.GetComponent<MeshFilter>().mesh.uv;


        switch (MapData.mapArray[tileX, tileY].tileRotation)
        {
            case 1:
                uvTemp[tileID] = new Vector2(rect.xMin, rect.yMin);
                uvTemp[tileID + 1] = new Vector2(rect.xMax, rect.yMin);
                uvTemp[tileID + 2] = new Vector2(rect.xMax, rect.yMax);
                uvTemp[tileID + 3] = new Vector2(rect.xMin, rect.yMax);
                break;
            case 2:
                uvTemp[tileID] = new Vector2(rect.xMax, rect.yMin);
                uvTemp[tileID + 1] = new Vector2(rect.xMax, rect.yMax);
                uvTemp[tileID + 2] = new Vector2(rect.xMin, rect.yMax);
                uvTemp[tileID + 3] = new Vector2(rect.xMin, rect.yMin);
                break;
            case 3:
                uvTemp[tileID] = new Vector2(rect.xMax, rect.yMax);
                uvTemp[tileID + 1] = new Vector2(rect.xMin, rect.yMax);
                uvTemp[tileID + 2] = new Vector2(rect.xMin, rect.yMin);
                uvTemp[tileID + 3] = new Vector2(rect.xMax, rect.yMin);
                break;
            case 4:
                uvTemp[tileID] = new Vector2(rect.xMin, rect.yMax);
                uvTemp[tileID + 1] = new Vector2(rect.xMin, rect.yMin);
                uvTemp[tileID + 2] = new Vector2(rect.xMax, rect.yMin);
                uvTemp[tileID + 3] = new Vector2(rect.xMax, rect.yMax);
                break;
        }

        // Water Layer
        rect = Texturing.GetTileTexture(0, 1);
        tileID += chunkMeshLayerSeparation;

        uvTemp[tileID] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[tileID + 1] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[tileID + 2] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[tileID + 3] = new Vector2(rect.xMin, rect.yMax);

        // Background Layer
        rect = Texturing.GetTileTexture(MapData.mapArray[tileX, tileY].tileBackgroundType, MapData.mapArray[tileX, tileY].tileBackgroundTexture);
        tileID += chunkMeshLayerSeparation;

        uvTemp[tileID] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[tileID + 1] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[tileID + 2] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[tileID + 3] = new Vector2(rect.xMin, rect.yMax);


        MapData.chunksArray[tileInfo.gridID].gridObject.GetComponent<MeshFilter>().mesh.uv = uvTemp;
    }

    public static void UpdateTile(GCoords tileInfo)
    {
        Rect rect;
        rect = Texturing.GetTileTexture(MapData.mapArray[tileInfo.tileX, tileInfo.tileY].tileType, MapData.mapArray[tileInfo.tileX, tileInfo.tileY].tileTexture);

        Mesh mesh = MapData.chunksArray[tileInfo.gridID].gridObject.GetComponent<MeshFilter>().mesh;

        int tileID = tileInfo.inGridX * 32 + tileInfo.inGridY;
        tileID *= 4;

        Vector2[] uvTemp = new Vector2[MapData.chunksArray[tileInfo.gridID].gridObject.GetComponent<MeshFilter>().mesh.uv.Length];
        uvTemp = MapData.chunksArray[tileInfo.gridID].gridObject.GetComponent<MeshFilter>().mesh.uv;


        switch (MapData.mapArray[tileInfo.tileX, tileInfo.tileY].tileRotation)
        {
            case 1:
                uvTemp[tileID] = new Vector2(rect.xMin, rect.yMin);
                uvTemp[tileID + 1] = new Vector2(rect.xMax, rect.yMin);
                uvTemp[tileID + 2] = new Vector2(rect.xMax, rect.yMax);
                uvTemp[tileID + 3] = new Vector2(rect.xMin, rect.yMax);
                break;
            case 2:
                uvTemp[tileID] = new Vector2(rect.xMax, rect.yMin);
                uvTemp[tileID + 1] = new Vector2(rect.xMax, rect.yMax);
                uvTemp[tileID + 2] = new Vector2(rect.xMin, rect.yMax);
                uvTemp[tileID + 3] = new Vector2(rect.xMin, rect.yMin);
                break;
            case 3:
                uvTemp[tileID] = new Vector2(rect.xMax, rect.yMax);
                uvTemp[tileID + 1] = new Vector2(rect.xMin, rect.yMax);
                uvTemp[tileID + 2] = new Vector2(rect.xMin, rect.yMin);
                uvTemp[tileID + 3] = new Vector2(rect.xMax, rect.yMin);
                break;
            case 4:
                uvTemp[tileID] = new Vector2(rect.xMin, rect.yMax);
                uvTemp[tileID + 1] = new Vector2(rect.xMin, rect.yMin);
                uvTemp[tileID + 2] = new Vector2(rect.xMax, rect.yMin);
                uvTemp[tileID + 3] = new Vector2(rect.xMax, rect.yMax);
                break;
        }

        // Water Layer
        rect = Texturing.GetTileTexture(0, 1);
        tileID += chunkMeshLayerSeparation;

        uvTemp[tileID] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[tileID + 1] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[tileID + 2] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[tileID + 3] = new Vector2(rect.xMin, rect.yMax);

        // Background Layer
        rect = Texturing.GetTileTexture(MapData.mapArray[tileX, tileY].tileBackgroundType, MapData.mapArray[tileX, tileY].tileBackgroundTexture);
        tileID += chunkMeshLayerSeparation;

        uvTemp[tileID] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[tileID + 1] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[tileID + 2] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[tileID + 3] = new Vector2(rect.xMin, rect.yMax);

        MapData.chunksArray[tileInfo.gridID].gridObject.GetComponent<MeshFilter>().mesh.uv = uvTemp;
    }

    public static void UpdateGridLight(int gridNumber)
    {
        if (gridNumber < 0 || gridNumber > MapData.chunksArray.Length)
            return;

        if (MapData.chunksArray[gridNumber].gridObject == null)
            return;

        Color[] colorTemp = MapData.chunksArray[gridNumber].gridObject.GetComponent<MeshFilter>().mesh.colors;
        int tileID;
        float brightness;
        // Bottom left tile in the mapArray array
        Vec2 gridStart = World.GetChunkStartTile(gridNumber);

        newColor = Color.clear;


        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++) 
            {
                if(Debugging.disableLighting == true)
                {
                    brightness = 1;
                }
                else
                {
                    brightness = MapData.mapArray[gridStart.x + x, gridStart.y + y].tileBrightness;
                }

               
                newColor.r = brightness;
                newColor.g = brightness;
                newColor.b = brightness;

                tileID = x * 32 + y;
                tileID *= 4;

                // Tile Layer
                colorTemp[tileID] = newColor;
                colorTemp[tileID + 1] = newColor;
                colorTemp[tileID + 2] = newColor;
                colorTemp[tileID + 3] = newColor;

                // Water Layer
                tileID += chunkMeshLayerSeparation;

                colorTemp[tileID] = newColor;
                colorTemp[tileID + 1] = newColor;
                colorTemp[tileID + 2] = newColor;
                colorTemp[tileID + 3] = newColor;

                // Background Layer
                tileID += chunkMeshLayerSeparation;

                colorTemp[tileID] = newColor;
                colorTemp[tileID + 1] = newColor;
                colorTemp[tileID + 2] = newColor;
                colorTemp[tileID + 3] = newColor;
            }
        }
        MapData.chunksArray[gridNumber].gridObject.GetComponent<MeshFilter>().mesh.colors = colorTemp;
    }

    public static void UpdateGridTextures(int gridNumber)
    {
        
        Vector2[] uvTemp = MapData.chunksArray[gridNumber].gridObject.GetComponent<MeshFilter>().mesh.uv;
        int tileID;
        Vec2 gridStart = World.GetChunkStartTile(gridNumber);
        Rect rect;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                rect = Texturing.GetTileTexture(MapData.mapArray[gridStart.x + x, gridStart.y + y].tileType, MapData.mapArray[gridStart.x + x, gridStart.y + y].tileTexture);
                tileID = x * 32 + y;
                tileID *= 4;

                // Tile Layer
                switch (MapData.mapArray[gridStart.x + x, gridStart.y + y].tileRotation)
                {
                    case 1:
                        uvTemp[tileID] = new Vector2(rect.xMin, rect.yMin);
                        uvTemp[tileID + 1] = new Vector2(rect.xMax, rect.yMin);
                        uvTemp[tileID + 2] = new Vector2(rect.xMax, rect.yMax);
                        uvTemp[tileID + 3] = new Vector2(rect.xMin, rect.yMax);
                        break;
                    case 2:
                        uvTemp[tileID] = new Vector2(rect.xMax, rect.yMin);
                        uvTemp[tileID + 1] = new Vector2(rect.xMax, rect.yMax);
                        uvTemp[tileID + 2] = new Vector2(rect.xMin, rect.yMax);
                        uvTemp[tileID + 3] = new Vector2(rect.xMin, rect.yMin);
                        break;
                    case 3:
                        uvTemp[tileID] = new Vector2(rect.xMax, rect.yMax);
                        uvTemp[tileID + 1] = new Vector2(rect.xMin, rect.yMax);
                        uvTemp[tileID + 2] = new Vector2(rect.xMin, rect.yMin);
                        uvTemp[tileID + 3] = new Vector2(rect.xMax, rect.yMin);
                        break;
                    case 4:
                        uvTemp[tileID] = new Vector2(rect.xMin, rect.yMax);
                        uvTemp[tileID + 1] = new Vector2(rect.xMin, rect.yMin);
                        uvTemp[tileID + 2] = new Vector2(rect.xMax, rect.yMin);
                        uvTemp[tileID + 3] = new Vector2(rect.xMax, rect.yMax);
                        break;
                }

                // Water Layer - temporary solution (show empty)
                rect = Texturing.GetTileTexture(0, 1);
                tileID += chunkMeshLayerSeparation;
                uvTemp[tileID] = new Vector2(rect.xMin, rect.yMin);
                uvTemp[tileID + 1] = new Vector2(rect.xMax, rect.yMin);
                uvTemp[tileID + 2] = new Vector2(rect.xMax, rect.yMax);
                uvTemp[tileID + 3] = new Vector2(rect.xMin, rect.yMax);

                // Background layer
                rect = Texturing.GetTileTexture(MapData.mapArray[gridStart.x + x, gridStart.y + y].tileBackgroundType, MapData.mapArray[gridStart.x + x, gridStart.y + y].tileBackgroundTexture);
                tileID += chunkMeshLayerSeparation;
                uvTemp[tileID] = new Vector2(rect.xMin, rect.yMin);
                uvTemp[tileID + 1] = new Vector2(rect.xMax, rect.yMin);
                uvTemp[tileID + 2] = new Vector2(rect.xMax, rect.yMax);
                uvTemp[tileID + 3] = new Vector2(rect.xMin, rect.yMax);
            }
        }
        MapData.chunksArray[gridNumber].gridObject.GetComponent<MeshFilter>().mesh.uv = uvTemp;
    }

    public static Mesh InitializeChunkMesh(int chunkNumber) // 0 = normal tiles, 1 = background
    {

        int verticesZ = chunkMeshVerticesZ;

        ClearMeshVars();

        // The tile from MapData.mapArray that is going to be the bottom left (0, 0) tile of the chunk
        startingTile = World.GetChunkStartTile(chunkNumber);

        // Front Tiles Layer
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                pos.x = x;
                pos.y = y;
                CreateMesh(chunkNumber, startingTile, verticesZ, 0);
            }
        }

        verticesZ += chunkMeshVerticesZOffset;

        // Water Layer
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                pos.x = x;
                pos.y = y;
                CreateMesh(chunkNumber, startingTile, verticesZ, 1);
            }
        }

        verticesZ += chunkMeshVerticesZOffset;

        // Background tiles Layer
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                pos.x = x;
                pos.y = y;
                CreateMesh(chunkNumber, startingTile, verticesZ, 2);
            }
        }



        Mesh mesh = new Mesh
        {
            vertices = _vertices.ToArray(),
            triangles = _triangles.ToArray(),
            uv = _uv.ToArray(),
            colors = _colors.ToArray()
        };

        Debug.Log(mesh.colors.Length);
        
        mesh.RecalculateNormals();
        return mesh;
    }

    private static void CreateMesh(int chunkNumber, Vec2 startingTile, int verticesZ, int layer) // 0 = front tiles, 1 = water, 2 = background tiles
    {
        // reference to the tile under scrutiny
        tileX = startingTile.x + (int)pos.x;
        tileY = startingTile.y + (int)pos.y;

        pos.x *= 0.16f;
        pos.y *= 0.16f;

        Vector3 vertices1;
        Vector3 vertices2;
        Vector3 vertices3;
        Vector3 vertices4;

        vertices1.x = pos.x;
        vertices1.y = pos.y;
        vertices1.z = verticesZ;

        vertices2.x = pos.x + meshVerticesSeparation;
        vertices2.y = pos.y;
        vertices2.z = verticesZ;

        vertices3.x = pos.x + meshVerticesSeparation;
        vertices3.y = pos.y + meshVerticesSeparation;
        vertices3.z = verticesZ;

        vertices4.x = pos.x;
        vertices4.y = pos.y + meshVerticesSeparation;
        vertices4.z = verticesZ;

        _vertices.Add(vertices1);
        _vertices.Add(vertices2);
        _vertices.Add(vertices3);
        _vertices.Add(vertices4);

        switch (layer)
        {
            // Tile Layer
            case 0:
                setMeshRect = Texturing.GetTileTexture(MapData.mapArray[tileX, tileY].tileType, MapData.mapArray[tileX, tileY].tileTexture);
                setUVs(setMeshRect, MapData.mapArray[tileX, tileY].tileRotation);
                break;
            // water. This is temporary
            case 1:
                setMeshRect = Texturing.GetTileTexture(0, 1);
                setUVs(setMeshRect, MapData.mapArray[tileX, tileY].tileRotation);
                break;
                // Background Layer
            case 2:
                setMeshRect = Texturing.GetTileTexture(MapData.mapArray[tileX, tileY].tileBackgroundType, MapData.mapArray[tileX, tileY].tileBackgroundTexture);
                setUVs(setMeshRect, MapData.mapArray[tileX, tileY].tileBackgroundRotation);
                break;
        }

        newColor = Color.clear;


        if (Debugging.disableLighting == false)
        {
            tileBrightness = MapData.mapArray[tileX, tileY].tileBrightness;
        }
        else
        {
            tileBrightness = 1;
        }




        newColor.r = tileBrightness;
        newColor.b = tileBrightness;
        newColor.g = tileBrightness;



        _colors.Add(newColor);
        _colors.Add(newColor);
        _colors.Add(newColor);
        _colors.Add(newColor);



        _triangles.Add(trianglesCount + 0);
        _triangles.Add(trianglesCount + 3);
        _triangles.Add(trianglesCount + 2);
        _triangles.Add(trianglesCount + 2);
        _triangles.Add(trianglesCount + 1);
        _triangles.Add(trianglesCount + 0);


        trianglesCount += 4;
    }

    private static void setUVs(Rect setMeshRect, int rotation)
    {
        switch (rotation)
        {
            case 1:
                uv1.x = setMeshRect.xMin;
                uv1.y = setMeshRect.yMin;

                uv2.x = setMeshRect.xMax;
                uv2.y = setMeshRect.yMin;

                uv3.x = setMeshRect.xMax;
                uv3.y = setMeshRect.yMax;

                uv4.x = setMeshRect.xMin;
                uv4.y = setMeshRect.yMax;
                break;
            case 2:
                uv1.x = setMeshRect.xMax;
                uv1.y = setMeshRect.yMin;

                uv2.x = setMeshRect.xMax;
                uv2.y = setMeshRect.yMax;

                uv3.x = setMeshRect.xMin;
                uv3.y = setMeshRect.yMax;

                uv4.x = setMeshRect.xMin;
                uv4.y = setMeshRect.yMin;
                break;
            case 3:
                uv1.x = setMeshRect.xMax;
                uv1.y = setMeshRect.yMax;

                uv2.x = setMeshRect.xMin;
                uv2.y = setMeshRect.yMax;

                uv3.x = setMeshRect.xMin;
                uv3.y = setMeshRect.yMin;

                uv4.x = setMeshRect.xMax;
                uv4.y = setMeshRect.yMin;
                break;
            case 4:
                uv1.x = setMeshRect.xMin;
                uv1.y = setMeshRect.yMax;

                uv2.x = setMeshRect.xMin;
                uv2.y = setMeshRect.yMin;

                uv3.x = setMeshRect.xMax;
                uv3.y = setMeshRect.yMin;

                uv4.x = setMeshRect.xMax;
                uv4.y = setMeshRect.yMax;
                break;
        }

        _uv.Add(uv1);
        _uv.Add(uv2);
        _uv.Add(uv3);
        _uv.Add(uv4);

    }

    public static void UpdatePlayerColors()
    {
        Color[] tempColors = playerMeshObject.GetComponent<MeshFilter>().mesh.colors;


                if(Inventory.armor_slot[1, 0] == -1)
                {
                   
                    tempColors[8] = Inventory.playerDefaultBodyColor;
                    tempColors[9] = Inventory.playerDefaultBodyColor;
                    tempColors[10] = Inventory.playerDefaultBodyColor;
                    tempColors[11] = Inventory.playerDefaultBodyColor;
                }
                else
                {
                    tempColors[8] = baseColor;
                    tempColors[9] = baseColor;
                    tempColors[10] = baseColor;
                    tempColors[11] = baseColor;
                }

                if (Inventory.armor_slot[2, 0] == -1)
                {
                    tempColors[24] = Inventory.playerDefaultLegsColor;
                    tempColors[25] = Inventory.playerDefaultLegsColor;
                    tempColors[26] = Inventory.playerDefaultLegsColor;
                    tempColors[27] = Inventory.playerDefaultLegsColor;
                }
                else
                {
                    tempColors[24] = baseColor;
                    tempColors[25] = baseColor;
                    tempColors[26] = baseColor;
                    tempColors[27] = baseColor;
                }
        


        playerMeshObject.GetComponent<MeshFilter>().mesh.colors = tempColors;
    }

    public static void UpdatePlayerColors(CharacterData cData, GameObject playerMeshObj)
    {
        Color[] tempColors = playerMeshObj.GetComponent<MeshFilter>().mesh.colors;

        // SkinHand
        tempColors[0] = cData.playerSkinColor;
        tempColors[1] = cData.playerSkinColor;
        tempColors[2] = cData.playerSkinColor;
        tempColors[3] = cData.playerSkinColor;

        // TextureArm
        tempColors[4] = cData.playerArmColor;
        tempColors[5] = cData.playerArmColor;
        tempColors[6] = cData.playerArmColor;
        tempColors[7] = cData.playerArmColor;

        // TextureBody (Shirt)
        tempColors[8] = cData.playerShirtColor;
        tempColors[9] = cData.playerShirtColor;
        tempColors[10] = cData.playerShirtColor;
        tempColors[11] = cData.playerShirtColor;

        // Shoes
        tempColors[16] = cData.playerShoesColor;
        tempColors[17] = cData.playerShoesColor;
        tempColors[18] = cData.playerShoesColor;
        tempColors[19] = cData.playerShoesColor;

        // Hair
        tempColors[20] = cData.playerHairColor;
        tempColors[21] = cData.playerHairColor;
        tempColors[22] = cData.playerHairColor;
        tempColors[23] = cData.playerHairColor;

        // TextureLegs
        tempColors[24] = cData.playerPantsColor;
        tempColors[25] = cData.playerPantsColor;
        tempColors[26] = cData.playerPantsColor;
        tempColors[27] = cData.playerPantsColor;

        // Eyes
        tempColors[28] = cData.playerEyesColor;
        tempColors[29] = cData.playerEyesColor;
        tempColors[30] = cData.playerEyesColor;
        tempColors[31] = cData.playerEyesColor;

        // SkinHead
        tempColors[32] = cData.playerSkinColor;
        tempColors[33] = cData.playerSkinColor;
        tempColors[34] = cData.playerSkinColor;
        tempColors[35] = cData.playerSkinColor;

        // SkinLegs
        tempColors[36] = cData.playerSkinColor;
        tempColors[37] = cData.playerSkinColor;
        tempColors[38] = cData.playerSkinColor;
        tempColors[39] = cData.playerSkinColor;

        playerMeshObj.GetComponent<MeshFilter>().mesh.colors = tempColors;
    }


    // SkinHand, TextureArm, TextureBody, TextureHead, Shoes/Feet, Hair, TextureLegs, Eyes, SkinHead, SkinLegs
    public static void UpdatePlayerMesh(int bodyStep, int generalStep, bool isFacingRight)
    {
        Vector2[] uvTemp = playerMeshObject.GetComponent<MeshFilter>().mesh.uv;
 

        // SkinHand


        if (Inventory.armor_slot[1, 0] == -1)
        {
            playerRects = Texturing.GetPlayerTextureRect(Inventory.playerSkinHandTexture, bodyStep);
        }
        else
        {
            playerRects = Texturing.GetPlayerTextureRect(Inventory.playerEmptyTexture, bodyStep);
        }

            if (isFacingRight)
            {
                uvTemp[0] = new Vector2(playerRects.xMin, playerRects.yMin);
                uvTemp[1] = new Vector2(playerRects.xMax, playerRects.yMin);
                uvTemp[2] = new Vector2(playerRects.xMax, playerRects.yMax);
                uvTemp[3] = new Vector2(playerRects.xMin, playerRects.yMax);
            }
            else
            {
                uvTemp[0] = new Vector2(playerRects.xMax, playerRects.yMin);
                uvTemp[1] = new Vector2(playerRects.xMin, playerRects.yMin);
                uvTemp[2] = new Vector2(playerRects.xMin, playerRects.yMax);
                uvTemp[3] = new Vector2(playerRects.xMax, playerRects.yMax);
            }


        

        // TextureArm
        playerRects = Texturing.GetPlayerTextureRect(Inventory.playerDefaultArmTexture, bodyStep);

        if (isFacingRight)
        {
            uvTemp[4] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[5] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[6] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[7] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[4] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[5] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[6] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[7] = new Vector2(playerRects.xMax, playerRects.yMax);
        }

        // TextureBody
        playerRects = Texturing.GetPlayerTextureRect(Inventory.playerBodyTexture, bodyStep);

        if (isFacingRight)
        {
            uvTemp[8] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[9] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[10] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[11] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[8] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[9] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[10] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[11] = new Vector2(playerRects.xMax, playerRects.yMax);
        }

        // TextureHead
        playerRects = Texturing.GetPlayerTextureRect(Inventory.playerHeadTexture, generalStep);

        if (isFacingRight)
        {
            uvTemp[12] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[13] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[14] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[15] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[12] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[13] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[14] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[15] = new Vector2(playerRects.xMax, playerRects.yMax);
        }

        // Shoes / Feet

        if(Inventory.armor_slot[2, 0] == -1)
        {
            playerRects = Texturing.GetPlayerTextureRect(Inventory.playerDefaultShoesTexture, generalStep);
        }
        else
        {
            playerRects = Texturing.GetPlayerTextureRect(Inventory.playerEmptyTexture, generalStep);
        }


        if (isFacingRight)
        {
            uvTemp[16] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[17] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[18] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[19] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[16] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[17] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[18] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[19] = new Vector2(playerRects.xMax, playerRects.yMax);
        }

        // Hair

        if (Inventory.armor_slot[0, 0] == -1)
        {
            playerRects = Texturing.GetPlayerTextureRect(Inventory.playerDefaultHairTexture, generalStep);
        }
        else
        {
            playerRects = Texturing.GetPlayerTextureRect(Inventory.playerEmptyTexture, generalStep);
        }
        

        if (isFacingRight)
        {
            uvTemp[20] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[21] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[22] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[23] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[20] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[21] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[22] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[23] = new Vector2(playerRects.xMax, playerRects.yMax);
        }

        // TextureLegs
        playerRects = Texturing.GetPlayerTextureRect(Inventory.playerLegsTexture, generalStep);

        if (isFacingRight)
        {
            uvTemp[24] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[25] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[26] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[27] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[24] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[25] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[26] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[27] = new Vector2(playerRects.xMax, playerRects.yMax);
        }

        // Eyes
        playerRects = Texturing.GetPlayerTextureRect(Inventory.playerDefaultEyesTexture, generalStep);

        if (isFacingRight)
        {
            uvTemp[28] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[29] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[30] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[31] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[28] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[29] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[30] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[31] = new Vector2(playerRects.xMax, playerRects.yMax);
        }

        // SkinHead
        playerRects = Texturing.GetPlayerTextureRect(Inventory.playerSkinHeadTexture, generalStep);

        if (isFacingRight)
        {
            uvTemp[32] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[33] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[34] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[35] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[32] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[33] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[34] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[35] = new Vector2(playerRects.xMax, playerRects.yMax);
        }

        // SkinLegs
        playerRects = Texturing.GetPlayerTextureRect(Inventory.playerSkinLegsTexture, generalStep);

        if (isFacingRight)
        {
            uvTemp[36] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[37] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[38] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[39] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[36] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[37] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[38] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[39] = new Vector2(playerRects.xMax, playerRects.yMax);
        }



        playerMeshObject.GetComponent<MeshFilter>().mesh.uv = uvTemp;

    }

    public static void UpdatePlayerMesh(CharacterData cData, GameObject playerMeshObj)
    {
        Vector2[] uvTemp = playerMeshObj.GetComponent<MeshFilter>().mesh.uv;

        bool isFacingRight = cData.isFacingRight;
        int bodyStep = cData.playerBodyStep;
        int generalStep = cData.playerGeneralStep;
        

        // SkinHand
        playerRects = Texturing.GetPlayerTextureRect(cData.playerSkinHandTexture, bodyStep);

        if (isFacingRight)
        {
            uvTemp[0] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[1] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[2] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[3] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[0] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[1] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[2] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[3] = new Vector2(playerRects.xMax, playerRects.yMax);
        }




        // TextureArm
        playerRects = Texturing.GetPlayerTextureRect(cData.playerTextureArmTexture, bodyStep);

        if (isFacingRight)
        {
            uvTemp[4] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[5] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[6] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[7] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[4] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[5] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[6] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[7] = new Vector2(playerRects.xMax, playerRects.yMax);
        }

        // TextureBody
        playerRects = Texturing.GetPlayerTextureRect(cData.playerTextureBodyTexture, bodyStep);

        if (isFacingRight)
        {
            uvTemp[8] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[9] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[10] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[11] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[8] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[9] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[10] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[11] = new Vector2(playerRects.xMax, playerRects.yMax);
        }

        // TextureHead
        playerRects = Texturing.GetPlayerTextureRect(cData.playerTextureHeadTexture, generalStep);

        if (isFacingRight)
        {
            uvTemp[12] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[13] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[14] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[15] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[12] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[13] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[14] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[15] = new Vector2(playerRects.xMax, playerRects.yMax);
        }

        // Shoes / Feet

        playerRects = Texturing.GetPlayerTextureRect(cData.playerShoesTexture, generalStep);



        if (isFacingRight)
        {
            uvTemp[16] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[17] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[18] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[19] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[16] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[17] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[18] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[19] = new Vector2(playerRects.xMax, playerRects.yMax);
        }

        // Hair

        playerRects = Texturing.GetPlayerTextureRect(cData.playerHairTexture, generalStep);


        if (isFacingRight)
        {
            uvTemp[20] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[21] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[22] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[23] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[20] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[21] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[22] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[23] = new Vector2(playerRects.xMax, playerRects.yMax);
        }

        // TextureLegs
        playerRects = Texturing.GetPlayerTextureRect(cData.playerTextureLegsTexture, generalStep);

        if (isFacingRight)
        {
            uvTemp[24] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[25] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[26] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[27] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[24] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[25] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[26] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[27] = new Vector2(playerRects.xMax, playerRects.yMax);
        }

        // Eyes
        playerRects = Texturing.GetPlayerTextureRect(cData.playerEyesTexture, generalStep);

        if (isFacingRight)
        {
            uvTemp[28] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[29] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[30] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[31] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[28] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[29] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[30] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[31] = new Vector2(playerRects.xMax, playerRects.yMax);
        }

        // SkinHead
        playerRects = Texturing.GetPlayerTextureRect(cData.playerSkinHeadTexture, generalStep);

        if (isFacingRight)
        {
            uvTemp[32] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[33] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[34] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[35] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[32] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[33] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[34] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[35] = new Vector2(playerRects.xMax, playerRects.yMax);
        }

        // SkinLegs
        playerRects = Texturing.GetPlayerTextureRect(cData.playerSkinLegsTexture, generalStep);

        if (isFacingRight)
        {
            uvTemp[36] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[37] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[38] = new Vector2(playerRects.xMax, playerRects.yMax);
            uvTemp[39] = new Vector2(playerRects.xMin, playerRects.yMax);
        }
        else
        {
            uvTemp[36] = new Vector2(playerRects.xMax, playerRects.yMin);
            uvTemp[37] = new Vector2(playerRects.xMin, playerRects.yMin);
            uvTemp[38] = new Vector2(playerRects.xMin, playerRects.yMax);
            uvTemp[39] = new Vector2(playerRects.xMax, playerRects.yMax);
        }



        playerMeshObj.GetComponent<MeshFilter>().mesh.uv = uvTemp;

    }

    public static Mesh InitializePlayerMesh()
    {
        

        List<Vector3> _verts = new List<Vector3>();
        List<int> _tris = new List<int>();
        List<Color> _cols = new List<Color>();
        Color baseColor = new Color(1, 1, 1);
        float faceZ = 0;

        Vector3 vertices1;
        Vector3 vertices2;
        Vector3 vertices3;
        Vector3 vertices4;
        int trianglesCount = 0;

        // SkinHand, TextureArm, TextureBody, TextureHead, Hair, TextureLegs, Eyes, SkinHead, SkinLegs, Shoes/Feet 
        for (int i = 0; i < playerMeshFaces; i++)
        {
            vertices1.x = 0;
            vertices1.y = 0;
            vertices1.z = faceZ;

            vertices2.x = 0.20f;
            vertices2.y = 0;
            vertices2.z = faceZ;

            vertices3.x = 0.20f;
            vertices3.y = 0.56f;
            vertices3.z = faceZ;

            vertices4.x = 0;
            vertices4.y = 0.56f;
            vertices4.z = faceZ;

            _verts.Add(vertices1);
            _verts.Add(vertices2);
            _verts.Add(vertices3);
            _verts.Add(vertices4);

            faceZ += 1;

            _tris.Add(trianglesCount + 0);
            _tris.Add(trianglesCount + 3);
            _tris.Add(trianglesCount + 2);
            _tris.Add(trianglesCount + 2);
            _tris.Add(trianglesCount + 1);
            _tris.Add(trianglesCount + 0);

            trianglesCount += 4;

            _cols.Add(baseColor);
            _cols.Add(baseColor);
            _cols.Add(baseColor);
            _cols.Add(baseColor);
        }



        Vector2[] uvTemp = new Vector2[4 * playerMeshFaces];

        Vector2 v2Zero = new Vector2(0, 0);

        for (int i = 0; i < uvTemp.Length; i++)
        {
            uvTemp[i] = v2Zero;
        }

        Mesh mesh = new Mesh
        {
            vertices = _verts.ToArray(),
            triangles = _tris.ToArray(),
            uv = uvTemp,
            colors = _cols.ToArray()
        };

        mesh.RecalculateNormals();
        return mesh;

    }

    public static Mesh InitializePlayerTestMesh()
    {
        List<Vector3> _verts = new List<Vector3>();
        List<int> _tris = new List<int>();
        List<Color> _cols = new List<Color>();
        Color baseColor = new Color(1, 1, 1);
        float faceZ = 0;

        Vector3 vertices1;
        Vector3 vertices2;
        Vector3 vertices3;
        Vector3 vertices4;
        int trianglesCount = 0;

        // SkinHand, TextureArm, TextureBody, TextureHead, Hair, TextureLegs, Eyes, SkinHead, SkinLegs, Shoes/Feet 
        for (int i = 0; i < playerMeshFaces; i++)
        {
            vertices1.x = 0;
            vertices1.y = 0;
            vertices1.z = faceZ;

            vertices2.x = 0.20f;
            vertices2.y = 0;
            vertices2.z = faceZ;

            vertices3.x = 0.20f;
            vertices3.y = 0.56f;
            vertices3.z = faceZ;

            vertices4.x = 0;
            vertices4.y = 0.56f;
            vertices4.z = faceZ;

            _verts.Add(vertices1);
            _verts.Add(vertices2);
            _verts.Add(vertices3);
            _verts.Add(vertices4);

            faceZ += 1;

            _tris.Add(trianglesCount + 0);
            _tris.Add(trianglesCount + 3);
            _tris.Add(trianglesCount + 2);
            _tris.Add(trianglesCount + 2);
            _tris.Add(trianglesCount + 1);
            _tris.Add(trianglesCount + 0);

            trianglesCount += 4;

            _cols.Add(baseColor);
            _cols.Add(baseColor);
            _cols.Add(baseColor);
            _cols.Add(baseColor);
        }



        Vector2[] uvTemp = new Vector2[4 * playerMeshFaces];

        Vector2 v2Zero = new Vector2(0, 0);

        for (int i = 0; i < uvTemp.Length; i++)
        {
            uvTemp[i] = v2Zero;
        }

        Mesh mesh = new Mesh
        {
            vertices = _verts.ToArray(),
            triangles = _tris.ToArray(),
            uv = uvTemp,
            colors = _cols.ToArray()
        };

        mesh.RecalculateNormals();
        return mesh;

    }

    // SkinHand, TextureArm, TextureBody, TextureHead, Hair, TextureLegs, Eyes, SkinHead, SkinLegs, Shoes/Feet 
    public static void LoadDefaultCharacterTexture()
    {
        Vector2[] uvTemp = playerMeshObject.GetComponent<MeshFilter>().mesh.uv;
        Color[] colTemp = playerMeshObject.GetComponent<MeshFilter>().mesh.colors;

        // Skin Hand
        Rect rect = Texturing.GetPlayerTextureRect(Inventory.playerSkinHandTexture, 1);

        uvTemp[0] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[1] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[2] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[3] = new Vector2(rect.xMin, rect.yMax);

        colTemp[0] = Inventory.playerSkinColor;
        colTemp[1] = Inventory.playerSkinColor;
        colTemp[2] = Inventory.playerSkinColor;
        colTemp[3] = Inventory.playerSkinColor;

        // Texture Arm
        rect = Texturing.GetPlayerTextureRect(Inventory.playerDefaultArmTexture, 1);

        uvTemp[4] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[5] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[6] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[7] = new Vector2(rect.xMin, rect.yMax);

        colTemp[4] = Inventory.playerDefaultArmColor;
        colTemp[5] = Inventory.playerDefaultArmColor;
        colTemp[6] = Inventory.playerDefaultArmColor;
        colTemp[7] = Inventory.playerDefaultArmColor;

        // Texture Body
        rect = Texturing.GetPlayerTextureRect(Inventory.playerDefaultBodyTexture, 1);

        uvTemp[8] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[9] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[10] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[11] = new Vector2(rect.xMin, rect.yMax);

        colTemp[8] = Inventory.playerDefaultBodyColor;
        colTemp[9] = Inventory.playerDefaultBodyColor;
        colTemp[10] = Inventory.playerDefaultBodyColor;
        colTemp[11] = Inventory.playerDefaultBodyColor;

        // Texture Head
        rect = Texturing.GetPlayerTextureRect(Inventory.playerDefaultHeadTexture, 1);

        uvTemp[12] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[13] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[14] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[15] = new Vector2(rect.xMin, rect.yMax);

        colTemp[12] = Inventory.playerDefaultHeadColor;
        colTemp[13] = Inventory.playerDefaultHeadColor;
        colTemp[14] = Inventory.playerDefaultHeadColor;
        colTemp[15] = Inventory.playerDefaultHeadColor;

        // Shoes/Feet
        rect = Texturing.GetPlayerTextureRect(Inventory.playerDefaultShoesTexture, 1);

        uvTemp[16] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[17] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[18] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[19] = new Vector2(rect.xMin, rect.yMax);

        colTemp[16] = Inventory.playerDefaultShoesColor;
        colTemp[17] = Inventory.playerDefaultShoesColor;
        colTemp[18] = Inventory.playerDefaultShoesColor;
        colTemp[19] = Inventory.playerDefaultShoesColor;

        // Hair
        rect = Texturing.GetPlayerTextureRect(Inventory.playerDefaultHairTexture, 1);

        uvTemp[20] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[21] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[22] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[23] = new Vector2(rect.xMin, rect.yMax);

        colTemp[20] = Inventory.playerDefaultHairColor;
        colTemp[21] = Inventory.playerDefaultHairColor;
        colTemp[22] = Inventory.playerDefaultHairColor;
        colTemp[23] = Inventory.playerDefaultHairColor;

        // TextureLegs
        rect = Texturing.GetPlayerTextureRect(Inventory.playerLegsTexture, 1);

        uvTemp[24] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[25] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[26] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[27] = new Vector2(rect.xMin, rect.yMax);

        colTemp[24] = Inventory.playerDefaultLegsColor;
        colTemp[25] = Inventory.playerDefaultLegsColor;
        colTemp[26] = Inventory.playerDefaultLegsColor;
        colTemp[27] = Inventory.playerDefaultLegsColor;

        // Eyes
        rect = Texturing.GetPlayerTextureRect(Inventory.playerDefaultEyesTexture, 1);

        uvTemp[28] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[29] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[30] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[31] = new Vector2(rect.xMin, rect.yMax);

        colTemp[28] = Inventory.playerEyesColor;
        colTemp[29] = Inventory.playerEyesColor;
        colTemp[30] = Inventory.playerEyesColor;
        colTemp[31] = Inventory.playerEyesColor;

        // SkinHead
        rect = Texturing.GetPlayerTextureRect(Inventory.playerSkinHeadTexture, 1);

        uvTemp[32] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[33] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[34] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[35] = new Vector2(rect.xMin, rect.yMax);

        colTemp[32] = Inventory.playerSkinColor;
        colTemp[33] = Inventory.playerSkinColor;
        colTemp[34] = Inventory.playerSkinColor;
        colTemp[35] = Inventory.playerSkinColor;

        // SkinLegs
        rect = Texturing.GetPlayerTextureRect(Inventory.playerSkinLegsTexture, 1);

        uvTemp[36] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[37] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[38] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[39] = new Vector2(rect.xMin, rect.yMax);

        colTemp[36] = Inventory.playerSkinColor;
        colTemp[37] = Inventory.playerSkinColor;
        colTemp[38] = Inventory.playerSkinColor;
        colTemp[39] = Inventory.playerSkinColor;

        playerMeshObject.GetComponent<MeshFilter>().mesh.uv = uvTemp;
        playerMeshObject.GetComponent<MeshFilter>().mesh.colors = colTemp;


    }

    public static void LoadCharacterDataMesh(GameObject playerMeshObj, CharacterData cData)
    {
        Vector2[] uvTemp = playerMeshObj.GetComponent<MeshFilter>().mesh.uv;
        Color[] colTemp = playerMeshObj.GetComponent<MeshFilter>().mesh.colors;

        // Skin Hand
        Rect rect = Texturing.GetPlayerTextureRect(cData.playerSkinHandTexture, 1);

        uvTemp[0] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[1] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[2] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[3] = new Vector2(rect.xMin, rect.yMax);

        colTemp[0] = cData.playerSkinColor;
        colTemp[1] = cData.playerSkinColor;
        colTemp[2] = cData.playerSkinColor;
        colTemp[3] = cData.playerSkinColor;

        // Texture Arm
        rect = Texturing.GetPlayerTextureRect(cData.playerTextureArmTexture, 1);

        uvTemp[4] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[5] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[6] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[7] = new Vector2(rect.xMin, rect.yMax);

        colTemp[4] = cData.playerArmColor;
        colTemp[5] = cData.playerArmColor;
        colTemp[6] = cData.playerArmColor;
        colTemp[7] = cData.playerArmColor;

        // Texture Body
        rect = Texturing.GetPlayerTextureRect(cData.playerTextureBodyTexture, 1);

        uvTemp[8] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[9] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[10] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[11] = new Vector2(rect.xMin, rect.yMax);

        colTemp[8] = cData.playerShirtColor;
        colTemp[9] = cData.playerShirtColor;
        colTemp[10] = cData.playerShirtColor;
        colTemp[11] = cData.playerShirtColor;

        // Texture Head
        rect = Texturing.GetPlayerTextureRect(cData.playerTextureHeadTexture, 1);

        uvTemp[12] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[13] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[14] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[15] = new Vector2(rect.xMin, rect.yMax);

        colTemp[12] = cData.playerHeadColor;
        colTemp[13] = cData.playerHeadColor;
        colTemp[14] = cData.playerHeadColor;
        colTemp[15] = cData.playerHeadColor;

        // Shoes/Feet
        rect = Texturing.GetPlayerTextureRect(cData.playerShoesTexture, 1);

        uvTemp[16] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[17] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[18] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[19] = new Vector2(rect.xMin, rect.yMax);

        colTemp[16] = cData.playerShoesColor;
        colTemp[17] = cData.playerShoesColor;
        colTemp[18] = cData.playerShoesColor;
        colTemp[19] = cData.playerShoesColor;

        // Hair
        rect = Texturing.GetPlayerTextureRect(cData.playerHairTexture, 1);

        uvTemp[20] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[21] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[22] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[23] = new Vector2(rect.xMin, rect.yMax);

        colTemp[20] = cData.playerHairColor;
        colTemp[21] = cData.playerHairColor;
        colTemp[22] = cData.playerHairColor;
        colTemp[23] = cData.playerHairColor;

        // TextureLegs
        rect = Texturing.GetPlayerTextureRect(cData.playerTextureLegsTexture, 1);

        uvTemp[24] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[25] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[26] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[27] = new Vector2(rect.xMin, rect.yMax);

        colTemp[24] = cData.playerPantsColor;
        colTemp[25] = cData.playerPantsColor;
        colTemp[26] = cData.playerPantsColor;
        colTemp[27] = cData.playerPantsColor;

        // Eyes
        rect = Texturing.GetPlayerTextureRect(cData.playerEyesTexture, 1);

        uvTemp[28] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[29] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[30] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[31] = new Vector2(rect.xMin, rect.yMax);

        colTemp[28] = cData.playerEyesColor;
        colTemp[29] = cData.playerEyesColor;
        colTemp[30] = cData.playerEyesColor;
        colTemp[31] = cData.playerEyesColor;

        // SkinHead
        rect = Texturing.GetPlayerTextureRect(cData.playerSkinHeadTexture, 1);

        uvTemp[32] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[33] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[34] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[35] = new Vector2(rect.xMin, rect.yMax);

        colTemp[32] = cData.playerSkinColor;
        colTemp[33] = cData.playerSkinColor;
        colTemp[34] = cData.playerSkinColor;
        colTemp[35] = cData.playerSkinColor;

        // SkinLegs
        rect = Texturing.GetPlayerTextureRect(cData.playerSkinLegsTexture, 1);

        uvTemp[36] = new Vector2(rect.xMin, rect.yMin);
        uvTemp[37] = new Vector2(rect.xMax, rect.yMin);
        uvTemp[38] = new Vector2(rect.xMax, rect.yMax);
        uvTemp[39] = new Vector2(rect.xMin, rect.yMax);

        colTemp[36] = cData.playerSkinColor;
        colTemp[37] = cData.playerSkinColor;
        colTemp[38] = cData.playerSkinColor;
        colTemp[39] = cData.playerSkinColor;

        playerMeshObj.GetComponent<MeshFilter>().mesh.uv = uvTemp;
        playerMeshObj.GetComponent<MeshFilter>().mesh.colors = colTemp;


    }



    

    private static Vector2 PixelsToUVCoords(int x, int y, int width, int height)
    {
        return new Vector2((float)x / width, (float)y / height);
    }

    private static Vector2[] getUVRect(int x, int y, int width, int height, int textureWidth, int textureHeight)
    {

        /* 0, 1 2
        *  1, 1 3
        *  0, 0 1
        *  1, 0 4
        */

        return new Vector2[]
        {
           PixelsToUVCoords(x, y + height, textureWidth, textureHeight),
           PixelsToUVCoords(x + width, y + height, textureWidth, textureHeight),
           PixelsToUVCoords(x, y, textureWidth, textureHeight),
           PixelsToUVCoords(x + width, y, textureWidth, textureHeight),

        };
    }


    private static void ApplyUVToArray(Vector2[] uv, ref Vector2[] mainUV)
    {
        // should remove when safe
        if (uv == null || uv.Length < 4 || mainUV == null || mainUV.Length < 4) throw new System.Exception();
        mainUV[0] = uv[0];
        mainUV[1] = uv[1];
        mainUV[2] = uv[2];
        mainUV[3] = uv[3];
    }
}
