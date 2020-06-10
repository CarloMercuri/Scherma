using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class World
{



    // World Variables
    private static decimal tileWorldSize;
    private static float gridWorldSize;
    private static int gridTotalCells;
    private static int worldGridHeight = 50;
    private static int worldGridLenght = 200;
    private static TileProperties tileDatabase;
    public static int horizontalScreenTiles;
    public static int verticalScreenTiles;


    public static void LoadWorldData(int tilePixelSize)
    {
        tileDatabase = Resources.Load<TileProperties>("Databases/TileDB");
        gridTotalCells = 32;
        tileWorldSize = (decimal)((float)tilePixelSize / 100);
        gridWorldSize = (float)(tileWorldSize * 32);

        GetTilesInViewscreen();
    }

    private static void GetTilesInViewscreen()
    {
        // The camera with a size of 1 displays 3.554 total world coordinates in the X axis, and 2 total world coordinates in the Y axis
        float horizontalCoordinatesMultiplier = 3.554f;
        float verticalCoordinatesMultiplier = 2f;

        // Get the size of the camera viewport
        float cameraSize = Camera.main.orthographicSize;

        // Multiply the world coordinates constant times the camera size.
        // Then divide that by the size of the tiles in world coordinates (acquired by dividing the pixel size of a tile by 100) to get 
        // the total horizontal tiles currently displayed with the current camera view size
        horizontalScreenTiles = Mathf.RoundToInt(Mathf.RoundToInt(horizontalCoordinatesMultiplier * cameraSize) / (float)tileWorldSize);
        verticalScreenTiles = Mathf.RoundToInt(Mathf.RoundToInt(verticalCoordinatesMultiplier * cameraSize) / (float)tileWorldSize);

        Debug.Log("World. " + horizontalScreenTiles + " x " + verticalScreenTiles);
    }

    public static float GetTileGrip(int tileID)
    {
        return tileDatabase.tileList[tileID].tileGrip;
    }

    
    /// <summary>
    /// Returns the height of the chunk in it's column
    /// </summary>
    /// <param name="chunkNumber"></param>
    /// <param name="maxVerticalChunks"></param>
    /// <returns></returns>
    public static int GetChunkHeight(int chunkNumber, int maxVerticalChunks)
    {
        int gridFloored = Mathf.FloorToInt(chunkNumber / maxVerticalChunks);
        return chunkNumber - (gridFloored * maxVerticalChunks);
    }

    /// <summary>
    /// Returns the number of the column which contains the chunk, starting from the left side
    /// </summary>
    /// <param name="chunkNumber"></param>
    /// <param name="maxVerticalChunks"></param>
    /// <returns></returns>
    public static int GetColumnNumber(int chunkNumber, int maxVerticalChunks)
    {
        return Mathf.FloorToInt( chunkNumber / maxVerticalChunks);
    }

    /// <summary>
    /// Returns the grid containing the tile with those array coordinates
    /// </summary>
    /// <param name="tileCoords"></param>
    /// <returns></returns>
    public static int TileCoordsToGridNumber(Vec2 tileCoords)
    {
        return (worldGridHeight * Mathf.FloorToInt(tileCoords.x / gridTotalCells) + Mathf.FloorToInt(tileCoords.y / gridTotalCells));
    }

    /// <summary>
    /// Returns the grid containing the tile with those array coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int TileCoordsToGridNumber(int x, int y)
    {
        return (worldGridHeight * Mathf.FloorToInt(x / gridTotalCells) + Mathf.FloorToInt(y / gridTotalCells));
    }
    /// <summary>
    /// Returns the grid containing those world coordinates
    /// </summary>
    /// <param name="worldCoords"></param>
    /// <returns></returns>
    public static int WorldToGridNumber(Vector2 worldCoords)
    {
        int tileX = (int)Math.Floor((decimal)worldCoords.x / tileWorldSize);
        int tileY = (int)Math.Floor((decimal)worldCoords.y / tileWorldSize);

        int GridX = (int)Math.Floor((decimal)tileX / gridTotalCells);
        int GridY = (int)Math.Floor((decimal)tileY / gridTotalCells);

        return ((worldGridHeight * GridX) + GridY);
    }

    /// <summary>
    /// Returns the grid number based on the offset given (horizontal only)
    /// </summary>
    /// <param name="currentGrid"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static int GetHorizontalGridNumber(int currentGrid, int distance)
    {
        return currentGrid + (distance * worldGridHeight);
    }

    /// <summary>
    /// Returns the grid number based on the offsets given
    /// </summary>
    /// <param name="currentGrid"></param>
    /// <param name="offsetX"></param>
    /// <param name="offsetY"></param>
    /// <returns></returns>
    public static int GetOffsetGridNumber(int currentGrid, int offsetX, int offsetY)
    {
        return (currentGrid + (offsetX * worldGridHeight) + offsetY);
    }

    /// <summary>
    /// Returns the world coordinates of the bottom left corner of the specified tile.
    /// </summary>
    /// <param name="tileCoords"></param>
    /// <returns></returns>
    public static Vector2 TileToWorldCoords(Vec2 tileCoords)
    {
        decimal x = tileCoords.x;
        decimal y = tileCoords.y;
        return new Vector2((float)(x * tileWorldSize), (float)(y * tileWorldSize));
    }

    /// <summary>
    /// Returns the world coordinates of the bottom left corner of the grid
    /// </summary>
    /// <param name="gridNumber"></param>
    /// <returns></returns>
    public static Vector3 GridToWorldCoords(int gridNumber)
    {
        // Find how many colums there are
        int flooredGridNr = Mathf.FloorToInt(gridNumber / worldGridHeight);
        // Multiply by the grid size in coordinates (5.12 standard) and get the x coordinates
        float xPos = flooredGridNr * gridWorldSize;

        // The leftover is how many rows from the bottom it is, which multiplied by 5.12 gives you the y coordinates
        float yPos = (gridNumber - (flooredGridNr * worldGridHeight)) * gridWorldSize;

        // 1 is very important for visibility
        return new Vector3(xPos, yPos, -3);
        
    }
    
    /// <summary>
    /// Returns the bottom left tile of the chunk
    /// </summary>
    /// <param name="gridNumber"></param>
    /// <returns></returns>
    public static Vec2 GetChunkStartTile(int gridNumber)
    {
        return new Vec2(Mathf.FloorToInt(gridNumber / worldGridHeight) * gridTotalCells, GetChunkHeight(gridNumber, worldGridHeight) * gridTotalCells);
    }

    /// <summary>
    /// Returns the tile array coordinates from the world coordinates
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static Vec2 WorldToTileCoords(Vector2 pos)
    {
        return new Vec2((int)Math.Floor((decimal)pos.x / tileWorldSize), (int)Math.Floor((decimal)pos.y / tileWorldSize));
        //return new Vec2(Mathf.FloorToInt(pos.x / 0.16f), Mathf.FloorToInt(pos.y / 0.16f));
    }

    /// <summary>
    /// Returns the tile array coordinates from the world coordinates
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <returns></returns>
    public static Vec2 WorldToTileCoords(float posX, float posY)
    {
        return new Vec2((int)Math.Floor((decimal)posX / tileWorldSize), (int)Math.Floor((decimal)posY / tileWorldSize));
        //return new Vec2(Mathf.FloorToInt(posX / 0.16f), Mathf.FloorToInt(posY / 0.16f));
    }

    /// <summary>
    /// Returns the tile's xMin in world coordinates 
    /// </summary>
    /// <param name="tileX"></param>
    /// <returns></returns>
    public static float TileToWorldXMin(int tileX)
    {
        return (float)(tileX * tileWorldSize);
    }

    /// <summary>
    /// Returns the tile's xMax in world coordinates
    /// </summary>
    /// <param name="tileX"></param>
    /// <returns></returns>
    public static float TileToWorldXMax(int tileX)
    {
        return (float)(tileX * tileWorldSize + tileWorldSize);
    }

    /// <summary>
    /// Returns the tile's yMin in world coordinates
    /// </summary>
    /// <param name="tileY"></param>
    /// <returns></returns>
    public static float TileToWorldYMin(int tileY)
    {
        return (float)(tileY * tileWorldSize);
    }

    /// <summary>
    /// Returns the tile's yMax in world coordinates
    /// </summary>
    /// <param name="tileY"></param>
    /// <returns></returns>
    public static float TileToWorldYMax(int tileY)
    {
        return (float)(tileY * tileWorldSize + tileWorldSize);
    }

    /// <summary>
    /// Returns the horizontal distance between two grids
    /// </summary>
    /// <param name="grid1"></param>
    /// <param name="grid2"></param>
    /// <returns></returns>
    public static int HorizontalGridsDistance(int grid1, int grid2)
    {
        int gridOneColumn = grid1 / worldGridHeight;
        int gridTwoColumn = grid2 / worldGridHeight;

        return Mathf.Abs((grid1 / worldGridHeight) - (grid2 / worldGridHeight));
    }

    /// <summary>
    /// Returns the vertical distance between two grids
    /// </summary>
    /// <param name="grid1"></param>
    /// <param name="grid2"></param>
    /// <returns></returns>
    public static int VerticalGridsDistance(int grid1, int grid2)
    {
        return Mathf.Abs( (grid1 - (Mathf.FloorToInt(grid1 / worldGridHeight) * worldGridHeight)) - (grid2 - (Mathf.FloorToInt(grid2 / worldGridHeight) * worldGridHeight)));
    }
    
    /// <summary>
    /// Returns the highest distance between two tiles (real distance)
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <returns></returns>
    public static int HighestTileDistance(Vec2 t1, Vec2 t2)
    {
        return Mathf.Max(Mathf.Abs(t1.x - t2.x), Mathf.Abs(t1.y - t2.y));
    }

    /// <summary>
    /// Returns the GCoords of the tile, given the tile array coordinates
    /// </summary>
    /// <param name="tileX"></param>
    /// <param name="tileY"></param>
    /// <returns></returns>
    public static GCoords TileCoordsToGCoords(int tileX, int tileY)
    {
        int GridX = Mathf.FloorToInt((tileX + 0.05f) / gridTotalCells);
        int GridY = Mathf.FloorToInt((tileY + 0.05f) / gridTotalCells);

        int inGridX = tileX - gridTotalCells * GridX;
        int inGridY = tileY - gridTotalCells * GridY;

        return new GCoords((worldGridHeight * GridX) + GridY, tileX, tileY, inGridX, inGridY);

    }

    /// <summary>
    /// Returns the GCoords of the tile, given the tile array coordinates
    /// </summary>
    /// <param name="coords"></param>
    /// <returns></returns>
    public static GCoords TileCoordsToGCoords(Vec2 coords)
    {
        int GridX = Mathf.FloorToInt((coords.x + 0.05f) / gridTotalCells);
        int GridY = Mathf.FloorToInt((coords.y + 0.05f) / gridTotalCells);

        int inGridX = coords.x - gridTotalCells * GridX;
        int inGridY = coords.y - gridTotalCells * GridY;

        return new GCoords((worldGridHeight * GridX) + GridY, coords.x, coords.y, inGridX, inGridY);

    }

    /// <summary>
    /// Returns GCoords based on world coords
    /// </summary>
    /// <param name="worldCoords"></param>
    /// <returns></returns>
    public static GCoords WorldToGCoords(Vector2 worldCoords)
    {
        // First divide by 0.16f to get the tile count
        int tileX = Mathf.FloorToInt(worldCoords.x / 0.16f);
        int tileY = Mathf.FloorToInt(worldCoords.y / 0.16f);

        // get in which grid it is. +0.05f because of floating point imprecision
        int GridX = Mathf.FloorToInt((tileX + 0.05f) / gridTotalCells);
        int GridY = Mathf.FloorToInt((tileY + 0.05f) / gridTotalCells);


        // Grid count: Bottom left (0,0) and up, then 1 to the right and up
        int inGridX = tileX - gridTotalCells * GridX;
        int inGridY = tileY - gridTotalCells * GridY;

        // Adding 32 for each column // only for old method
        // xFloored *= 32;                                // inGridX

        //return new GCoords((50 * inGridX) + inGridY, xFloored - 32 * (Mathf.FloorToInt(xFloored / 32)) + yFloored - 32 * (Mathf.FloorToInt(yFloored / 32)));
        return new GCoords((worldGridHeight * GridX) + GridY, tileX, tileY, inGridX, inGridY);
    }
}
