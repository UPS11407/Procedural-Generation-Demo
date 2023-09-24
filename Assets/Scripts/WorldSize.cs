using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldSize
{
    public int y;
    [Tooltip("large chunk size with small width and length can be used to help increase performance")]
    public int chunkSize;
    [Tooltip("large chunk size with small width and length can be used to help increase performance")]
    public int worldWidth;
    [Tooltip("large chunk size with small width and length can be used to help increase performance")]
    public int worldLength;

    public WorldSize(int worldWidth, int worldLength, int worldHeight, int chunkSize)
    {
        this.worldWidth = worldWidth;
        this.worldLength = worldLength;
        this.y = worldHeight;
        this.chunkSize = chunkSize;
    }
}
