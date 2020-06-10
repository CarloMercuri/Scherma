using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LightControl
{
    public static GameController _gameController;

    private static int maxDynamicLights = 1000;
    public static List<LightSource> dynamicLights = new List<LightSource>();
    private static List<LightSource> tempDynamicLights = new List<LightSource>();

    private static LightSource playerLight;
    private static List<int> affectedGrids = new List<int>();
    private static int offScreenTiles = 20;
    
    private static int xMin;
    private static int xMax;
    private static int yMin;
    private static int yMax;

    private static float sunLightDecrements = 0.2f;
    private static float blocksHitDecrements = 0.1f;



    public static void LoadLightingSystem()
    {

    }

    // COMMENTED OUT TO NOT USE THE OLD SYSTEM: PlayerControl in update, Gamecontroller //MapData.CreateBaseLighting();
    public static void CalculateTilesLight(Vec2 position)
    {
        // Clear the temporary list
        tempDynamicLights.Clear();

        // Set the min&maxes
        xMin = position.x - (World.horizontalScreenTiles / 2)  - offScreenTiles;
        xMax = position.x + (World.horizontalScreenTiles / 2) + offScreenTiles;
        yMin = position.y - (World.verticalScreenTiles / 2) - offScreenTiles;
        yMax = position.y + (World.verticalScreenTiles / 2) + offScreenTiles;

        if (xMin < 20) xMin = 20;
        if (xMax > MapData.totalHorizontalTiles - 20) xMax = MapData.totalHorizontalTiles - 20;
        if (yMin < 20) yMin = 20;
        if (yMax > MapData.totalVerticalTiles - 20) yMax = MapData.totalVerticalTiles - 20;

        // Search in the dynamic lights list and add it to a temporary list
        
        foreach (LightSource ls in dynamicLights)
        {
            if(ls.position.x >= xMin && ls.position.x <= xMax && ls.position.y >= yMin && ls.position.y <= yMax)
            {
                tempDynamicLights.Add(ls);
            }
        }
        
        // BIG THING !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // First we reset the brightness to 0. Need to find a way to do it in the same pass.
        for (int x = xMin; x < xMax; x++)
        {
            for (int y = yMin; y < yMax; y++)
            {
                ChangeTileBrightness(x, y, 0, true);
            }
        }

         /*
         First we calculate ambient light. Run the whole screen + some tiles outside.
         That's in case there is a light source just outside the screen, which would affect the tiles inside the screen
         Running horizontally, column by column from bottom up.
         */
        for (int x = xMin; x < xMax; x++)
        {
            for (int y = yMin; y < yMax; y++)
            {
                // First we check if that tile has a dynamic light (player torch, normal torch, etc)
                if (IsTileDynamicLightSource(x, y, out LightSource source))
                {
                    ChangeTileBrightness(x, y, MapData.SunBrightness, false);
                    ApplyDynamicLightMatrix(source);
                    continue; // no need to apply ambient light from that tile
                }

                // if it's an empty tile, with no background (sky tile) or a tree, we count it as an ambient light source
                if (IsTileAmbientLightSource(x, y))
                {
                    // Not sure if I should keep this or not, but it's here in case it's a tree. Not sure if it helps or not.
                    ChangeTileBrightness(x, y, MapData.SunBrightness, false);

                    // if the tile is surrounded by empty tiles, no point in doing it, and we skip it.
                    if (!IsAreaEmpty(x, y))
                    {
                        ApplySunLightMatrix(x, y);
                    }
                }
            }
        }


        // TESTING, TEMPORARY. Slows down a lot
        int gridNumber = World.TileCoordsToGridNumber(position);


        UnityThread.executeInUpdate(() =>
        {
            MeshControl.UpdateGridLight(gridNumber);
            MeshControl.UpdateGridLight(World.GetHorizontalGridNumber(gridNumber, -1));
            MeshControl.UpdateGridLight(World.GetHorizontalGridNumber(gridNumber, -1) - 1);
            MeshControl.UpdateGridLight(World.GetHorizontalGridNumber(gridNumber, -1) + 1);
            MeshControl.UpdateGridLight(World.GetHorizontalGridNumber(gridNumber, +1));
            MeshControl.UpdateGridLight(World.GetHorizontalGridNumber(gridNumber, +1) + 1);
            MeshControl.UpdateGridLight(World.GetHorizontalGridNumber(gridNumber, +1) - 1);
            MeshControl.UpdateGridLight(gridNumber + 1);
            MeshControl.UpdateGridLight(gridNumber - 1);
        });

        
        
        
        
    }

    /// <summary>
    /// Returns true if all the tiles around are empty
    /// </summary>
    /// <param name="tileX"></param>
    /// <param name="tileY"></param>
    /// <returns></returns>
    public static bool IsAreaEmpty(int tileX, int tileY)
    {
        // Start from the top right
        tileX--;
        tileY++;
        int currentTileY = tileY;

        for (int x = 0; x < 3; x++)
        {
            currentTileY = tileY;

            for (int y = 0; y < 3; y++)
            {
                if(MapData.mapArray[tileX, currentTileY].tileType != 0 || MapData.mapArray[tileX, currentTileY].tileBackgroundType != 0)
                {
                    return false;
                }
                currentTileY--;
            }

            tileX++;
        }

        return true;
    }

    // DIAMOND
    private static void ApplyDynamicLightMatrix(LightSource source)
    {
        /* Overview: We check each tile in a diamond-shaped matrix around the light source.
         * For each one, the brightness starts at 1. Then we decrease it by some amount (obtained via
         * GetDistanceDecrements(), because it depends on the radius of the light source. The larger the 
         * radius, the smaller the decrements need to be.
         * Then we do a raycast from the center of the source to that tile, using a modified version
         * of the Bresenham algorithm, and we decrease the tile brightness even further for each tile that is in the path.
         */

        // I should see if I can optimize this a bit, or at least reorganize
        float distanceDecrements = GetDistanceDecrements(source.radius);
        int x = source.position.x - source.radius;
        int y = source.position.y;
        int blocksHitCount = 0;
        int matrix_X_loop = source.radius;
        int matrix_X = matrix_X_loop;
        int endX = source.position.x + source.radius;

        // middle and up
        for (int i = 0; i < source.radius + 1; i++)
        {
            
            for (int _x = x; _x < endX; _x++)
            {
                List<Vec2> points = RayCast2DSB.RayCast(new Vec2(_x, y), source.position);

                if (points.Count > 1)
                {
                    points.RemoveAt(points.Count - 1);
                    points.RemoveAt(0);
                }


                for (int u = 0; u < points.Count; u++)
                {
                    if (MapData.mapArray[points[u].x, points[u].y].IsSolid())
                    {
                        blocksHitCount++;
                    }
                }

                ChangeTileBrightness(_x, y, (1 - GetDistanceInMatrix(matrix_X, i) * distanceDecrements) - blocksHitCount * blocksHitDecrements, false); // ChangeTileBrightness(_x, y, 1 - (blocksHitCount * 0.1f), false);
                //MapData.mapArray[_x, _y].tileBrightness = 1 - (blocksHitCount * 0.1f);
                blocksHitCount = 0;
                matrix_X--;
            }

            matrix_X_loop--;
            matrix_X = matrix_X_loop;
            y++;
            x++;
            endX--;
        }

        // starting one row down
        x = source.position.x - source.radius;
        x++;
        y = source.position.y - 1;
        endX = source.position.x + source.radius;
        endX--;
        matrix_X_loop = source.radius - 1;
        matrix_X = matrix_X_loop;

        // down
        for (int i = 0; i < source.radius; i++)
        {
            for (int _x = x; _x < endX; _x++)
            {
                List<Vec2> points = RayCast2DSB.RayCast(new Vec2(_x, y), source.position);

                if (points.Count > 1)
                {
                    points.RemoveAt(points.Count - 1);
                    points.RemoveAt(0);
                }


                for (int u = 0; u < points.Count; u++)
                {
                    if (MapData.mapArray[points[u].x, points[u].y].IsSolid())
                    {
                        blocksHitCount++;
                    }
                }


                ChangeTileBrightness(_x, y, (1 - GetDistanceInMatrix(matrix_X, i) * distanceDecrements) - blocksHitCount * blocksHitDecrements, false);
                //MapData.mapArray[_x, _y].tileBrightness = 1 - (blocksHitCount * 0.1f);
                blocksHitCount = 0;
                matrix_X--;
            }

            matrix_X_loop--;
            matrix_X = matrix_X_loop;
            y--;
            x++;
            endX--;
        }

    }

    private static float GetDistanceDecrements(int radius)
    {
        switch(radius)
        {
            case 10:
                return 0.1f;
            case 15:
                return 0.07f;
            case 20:
                return 0.05f;
        }

        if(radius > 20)
        {
            return 0.03f;
        }
        else
        {
            return 0.1f;
        }

    }

    private static int GetDistanceInMatrix(int x, int y)
    {
        return Mathf.Abs(x) + Mathf.Abs(y);
    }

    private static void ApplySunLightMatrix(int x, int y)
    {

        /*
            Apply a diamond shaped matrix, starting with full brightness in the middle and decreasing around. Like this (numbers are brightness)
            Right now im just assuming it starts from 1, with 0.2 decreases

                                    [0.0]
                               [0.0][0.2][0.0]
                          [0.0][0.2][0.4][0.2][0.0]
                     [0.0][0.2][0.4][0.6][0.4][0.2][0.0]
                [0.0][0.2][0.4][0.6][0.8][0.6][0.4][0.2][0.0]
           [0.0][0.2][0.4][0.6][0.8][1.0][0.8][0.6][0.4][0.2][0.0]
                [0.0][0.2][0.4][0.6][0.8][0.6][0.4][0.2][0.0]
                     [0.0][0.2][0.4][0.6][0.4][0.2][0.0]
                          [0.0][0.2][0.4][0.2][0.0]
                               [0.0][0.2][0.0]
                                    [0.0]


        To do so, I start from the very center, and do the two horizontal lines to the right and left, decreasing
        everytime by the amount specified.
        Then, I take the center tile again, go one up, and do the horizontal sides again, and so on. Then I do the same but downwards

        */

        float centerBrightness = MapData.SunBrightness;
        float currentBrightness = centerBrightness;

        int centerX = x;
        int centerY = y;
        int currentX;

        // The max amount of decrements possible before it reaches 0
        int maxSteps = Mathf.FloorToInt(centerBrightness / sunLightDecrements);

        // sides
        for (int i = 0; i < maxSteps + 1; i++)
        {
            ChangeTileBrightness(centerX + i, centerY, currentBrightness, false);
            ChangeTileBrightness(centerX - i, centerY, currentBrightness, false);
            currentBrightness = MapData.SunBrightness - (sunLightDecrements * i);

 
        }


        // up
        for (int up = 0; up < maxSteps; up++)
        {
            centerY++;
            currentX = centerX;
            currentBrightness = MapData.SunBrightness - (sunLightDecrements * up);

            ChangeTileBrightness(currentX, centerY, currentBrightness, false);

            for (int u = 1; u < maxSteps; u++)
            {
                currentBrightness -= sunLightDecrements;
                ChangeTileBrightness(currentX - u, centerY, currentBrightness, false);
                ChangeTileBrightness(currentX + u, centerY, currentBrightness, false);
            }
        }

        currentBrightness = centerBrightness;
        centerY = y;

        // down
        for (int down = 0; down < maxSteps; down++)
        {
            centerY--;
            currentX = centerX;
            currentBrightness = MapData.SunBrightness - (sunLightDecrements * down);

            ChangeTileBrightness(currentX, centerY, currentBrightness, false);

            for (int u = 1; u < maxSteps; u++)
            {
                currentBrightness -= sunLightDecrements;
                ChangeTileBrightness(currentX - u, centerY, currentBrightness, false);
                ChangeTileBrightness(currentX + u, centerY, currentBrightness, false);
            }
        }

    }

    public static void PlaceLightSource(Vec2 tile)
    {

    }



    /// <summary>
    /// If false, only applies the requested brightness if it's higher than the current tile's brightness
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="brightness"></param>
    /// <param name="forceBrightness"></param>
    private static void ChangeTileBrightness(int x, int y, float brightness, bool forceBrightness)
    {

        if (brightness < 0)
            brightness = 0;

        if(MapData.mapArray[x, y].tileType == 0 & MapData.mapArray[x, y].tileBackgroundType == 0)
        {
            MapData.mapArray[x, y].tileBrightness = MapData.SunBrightness;
            return;
        }

        if(MapData.mapArray[x, y].tileType == 7 || MapData.mapArray[x, y].tileType == 8)
        {
            MapData.mapArray[x, y].tileBrightness = MapData.SunBrightness;
            return;
        }


        if(forceBrightness)
        {
            MapData.mapArray[x, y].tileBrightness = brightness;
        }
        else
        {
            if(MapData.mapArray[x, y].tileBrightness < brightness)
            {
                MapData.mapArray[x, y].tileBrightness = brightness;
            }
        }



    }

    /// <summary>
    /// Returns true if that tile has a dynamic light source (torch).
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsTileDynamicLightSource(int x, int y, out LightSource source)
    {
        for (int i = 0; i < tempDynamicLights.Count; i++)
        {
            if (tempDynamicLights[i].position.x == x && tempDynamicLights[i].position.y == y)
            {
                source = tempDynamicLights[i];
                tempDynamicLights.RemoveAt(i);
                return true;
            }
        }

        source = null;
        return false;
    }

    /*
    public static bool IsTileDynamicLightSource(int x, int y, out int radius, out Color color)
    {
        for (int i = 0; i < tempDynamicLights.Count; i++)
        {
            if(tempDynamicLights[i].position.x == x && tempDynamicLights[i].position.y == y)
            {
                radius = tempDynamicLights[i].radius;
                color = tempDynamicLights[i].color;
                tempDynamicLights.RemoveAt(i);
                return true;
            }
        }

        radius = 0;
        color = Color.clear;
        return false;
    }
    */

    /// <summary>
    /// Returns true if the tile is empty, or it's a tree
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool IsTileAmbientLightSource(int x, int y)
    {
        bool isEmpty;
        bool isTree;

        if (MapData.mapArray[x, y].tileType == 0 && MapData.mapArray[x, y].tileBackgroundType == 0)
        {
            isEmpty = true;
        }
        else
        {
            isEmpty = false;
        }

        if(MapData.mapArray[x, y].tileType == 7 || MapData.mapArray[x, y].tileType == 8)
        {
            isTree = true;
        }
        else
        {
            isTree = false;
        }

        if(isEmpty || isTree)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private static bool FindTempLightSource(LightSource test, out LightSource result)
    {
 
        foreach (LightSource ls in tempDynamicLights)
        {
            if (ls == test)
            {
                result = ls;
                return true;
            }
        }

        result = null;
        return false;
    }

    private static bool FindTempLightSourceIndex(LightSource test, out int id)
    {
        for (int i = 0; i < tempDynamicLights.Count; i++)
        {
            if(tempDynamicLights[i] == test)
            {
                id = i;
                return true;
            }
        }

        id = -1;
        return false;
    }


}
