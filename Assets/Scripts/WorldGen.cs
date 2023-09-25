using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[ExecuteAlways]
public class WorldGen : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private BlockList blockList;
    [SerializeField] private GameObject worldObj;

    [SerializeField] private GameObject chunkPrefab;
    [SerializeField] private List<GameObject> chunkList;
    
    [SerializeField] private WorldSize worldSize;

    [SerializeField] private Enums.GenerationType worldGenType;
    [SerializeField] private int seed = 0;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private TMP_InputField worldHeight;
    [SerializeField] private TMP_InputField chunkWidth;
    [SerializeField] private TMP_InputField worldLength;
    [SerializeField] private TMP_InputField worldWidth;
    [SerializeField] private TMP_InputField seedText;
    [SerializeField] private GameObject loadingPopup;
    #endregion

    private bool generating = false;
    private int blockCount = 0;

    #region Inspector Buttons
    [SerializeField, ButtonInvoke(nameof(GenerateWorld), displayIn: ButtonInvoke.DisplayIn.PlayAndEditModes)] private bool generateWorld;
    [SerializeField, ButtonInvoke(nameof(ChangeSeed), displayIn: ButtonInvoke.DisplayIn.PlayAndEditModes)] private bool changeSeed;
    #endregion

    #region Seed
    public void ChangeSeed()
    {
        seed = Random.Range(1, 10000000);
        seedText.text = seed.ToString();
    }
    #endregion

    #region Block Determiners
    public Enums.BlockType DetermineBlockRandom(int x, int y, int z)
    {
        return (Random.Range(0, 2) == 0) ? Enums.BlockType.AIR : Enums.BlockType.STONE;
    }

    public Enums.BlockType DetermineBlockSin(int x, int y, int z)
    {
        float surfaceHeight = 10 + (Mathf.Sin(x * 0.2f) * 10);
        return (y > surfaceHeight) ? Enums.BlockType.AIR : Enums.BlockType.STONE;
    }

    public Enums.BlockType DetermineBlockValleys(int x, int y, int z)
    {
        float xOffset = Mathf.Sin(x * 0.1f) * 10;
        float zOffset = Mathf.Sin(z * 0.1f) * 10;
        float surfaceHeight = 10 + xOffset + zOffset;
        return (y > surfaceHeight) ? Enums.BlockType.AIR : Enums.BlockType.STONE;
    }

    public Enums.BlockType DetermineBlockWaves(int x, int y, int z)
    {
        var xOffset = (Mathf.Sin(x * 0.2f) * 3) + (Mathf.Sin(x * 0.9f) * 1) + (Mathf.Sin(x * 0.1f) * 7);
        var zOffset = (Mathf.Sin(z * 0.2f) * 3) + (Mathf.Sin(z * 0.9f) * 1) + (Mathf.Sin(z * 0.1f) * 7);
        var surfaceHeight = 10 + xOffset + zOffset;
        return (y > surfaceHeight) ? Enums.BlockType.AIR : Enums.BlockType.STONE;
    }

    public Enums.BlockType DetermineBlockPerlinNoise(int x, int y, int z)
    {
        var xOffset = Mathf.Sin(x * 0.2f) * 10;
        var zOffset = Mathf.Sin(z * 0.2f) * 10;
        var surfaceHeight = (10 + xOffset + zOffset) * Mathf.PerlinNoise(x, z);
        return (y > surfaceHeight) ? Enums.BlockType.AIR : Enums.BlockType.STONE;
    }

    public Enums.BlockType DetermineBlockWavedNoise(int x, int y, int z)
    {
        var xOffset = (Mathf.Sin(x * 3f) * 0.25) + (Mathf.Sin(x * 0.9f) * 1) + (Mathf.Sin(x * 0.1f) * 7);
        var zOffset = (Mathf.Sin(z * 3f) * 0.25) + (Mathf.Sin(z * 0.9f) * 1) + (Mathf.Sin(z * 0.1f) * 7);
        var surfaceHeight = (10 + xOffset + zOffset) * Mathf.PerlinNoise(x, z);
        return (y > surfaceHeight) ? Enums.BlockType.AIR : Enums.BlockType.STONE;
    }

    public Enums.BlockType DetermineBlockTesting(int x, int y, int z)
    {
        Random.InitState(seed);
        var xOffset = ((Mathf.Sin(x * 0.9f) * 0.2f) + (Mathf.Sin(x * 0.2f) * 3) + (Mathf.Sin(x * 0.7f) * 0.9) + Mathf.Sin(x * 0.4f) + (Mathf.Sin(x * 0.2f) * 0.1f)) * Random.Range(0.8f, 1.2f);
        var zOffset = ((Mathf.Sin(z * 0.4f) * 0.8f) + (Mathf.Sin(z * 0.7f) * 0.4f) + (Mathf.Sin(z * 0.2f) * 2) + (Mathf.Sin(z) * 0.7f) + (Mathf.Sin(z * 0.8f) * 0.6f)) * Random.Range(0.8f, 1.2f);
        double surfaceHeight = (worldSize.y - (worldSize.y / 4) + xOffset + zOffset) * Mathf.PerlinNoise(x, z) * 1.5f;
        float perlinOffset = Random.Range(0.01f, 0.08f);

        if (y < surfaceHeight && y > surfaceHeight - 1)
        {
            //we don't want floating grass if the block below is air.
            //I thought the terrain looked a little more interesting without it. Though it does cause terrain generation that would be harder to traverse for a player.
            if (Perlin3D(x * perlinOffset, (y - 1) * perlinOffset, z * perlinOffset) > 0.5f)
            {
                return Enums.BlockType.AIR;
            }
            else
            {
                //since surfaceHeight is a double, it needs to check if we are near surface height.
                return Enums.BlockType.GRASS;
            }
        }

        if (y > surfaceHeight)
        {
            return Enums.BlockType.AIR;
        }

        if(y > surfaceHeight - 5)
        {
            //same as grass.
            if (Perlin3D(x * perlinOffset, (y - 1) * perlinOffset, z * perlinOffset) > 0.5f)
            {
                return Enums.BlockType.AIR;
            }
            else
            {
                return Enums.BlockType.DIRT;
            }
        }

        if (Perlin3D(x * perlinOffset, y * perlinOffset, z * perlinOffset) > 0.5f)
        {
            return Enums.BlockType.AIR;
        }
        else
        {
            return Enums.BlockType.STONE;
        }
    }
    #endregion

    public static float Perlin3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yx = Mathf.PerlinNoise(y, x);
        float yz = Mathf.PerlinNoise(y, z);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);

        float xyz = xy + yz + yx + xz + zy + zx;
        return xyz / 6f;
    }

    private void Update()
    {
        if (!generating)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pauseMenu.SetActive(!pauseMenu.activeSelf);
                Camera.main.GetComponent<CameraMovement>().enabled = !pauseMenu.activeSelf;
                Camera.main.GetComponent<CameraMovement>().SetCursorVisibility();
            }
        }
    }

    private void SpawnBlock(int x, int y, int z, GameObject parent, int xOffset, int zOFfset)
    {
        Enums.BlockType blockType = Enums.BlockType.AIR;
        switch (worldGenType)
        {
            case Enums.GenerationType.RANDOM:
                blockType = DetermineBlockRandom(x, y, z);
                break;
            case Enums.GenerationType.SIN:
                blockType = DetermineBlockSin(x, y, z);
                break;
            case Enums.GenerationType.VALLEY:
                blockType = DetermineBlockValleys(x, y, z);
                break;
            case Enums.GenerationType.OCTAVE:
                blockType = DetermineBlockWaves(x, y, z);
                break;
            case Enums.GenerationType.PERLIN_NOISE:
                blockType = DetermineBlockPerlinNoise(x, y, z);
                break;
            case Enums.GenerationType.OCTAVE_PERLIN:
                blockType = DetermineBlockWavedNoise(x, y, z);
                break;
            case Enums.GenerationType.SURFACEDRESSING:
                blockType = DetermineBlockTesting(x, y, z);
                break;
        }

        //save time by not instantiating air blocks since they aren't visible.
        //if this were a real project I would probably include them to determine where a player was looking when placing blocks, but for a demo of generation I felt it was unnecessary.
        if (blockType != Enums.BlockType.AIR)
        {
            blockCount++;
            GameObject block = Instantiate(blockList.blocks[(int)blockType], new Vector3(x - xOffset, y, z - zOFfset), blockList.blocks[(int)blockType].transform.rotation, parent.transform);
            //Unity gets really laggy when there are a bunch of meshes. For the sake of simplicity I combine all meshes of the same material in each chunk into 1 mesh.
            //this is done in the MeshCombine.cs script
            parent.GetComponent<MeshCombiner>().AddMeshToCombine(block.GetComponent<MeshFilter>());

            //I decided to keep the blocks as part of the chunks so that their colliders would still be around to be walked on/destroyed by the player if I decided to include those features
            //unfortunately this makes saving the scene take a while for big worlds
        }
    }

    public void GenerateWorld()
    {
        if (Application.isPlaying)
        {
            worldSize = new WorldSize(int.Parse(worldWidth.text), int.Parse(worldLength.text), int.Parse(worldHeight.text), int.Parse(chunkWidth.text));
            seed = int.Parse(seedText.text);
        }
        

        generating = true;
        loadingPopup.SetActive(true);
        pauseMenu.SetActive(false);

        GameObject[] worlds = GameObject.FindGameObjectsWithTag("World");
        foreach (GameObject obj in worlds)
        {
            DestroyImmediate(obj);
        }


        chunkList.Clear();

        GameObject world = Instantiate(worldObj, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

        blockCount = 0;
        for(int worldWidth = 0; worldWidth < worldSize.worldWidth; worldWidth++)
        {
            for(int worldLength = 0; worldLength < worldSize.worldLength; worldLength++)
            {
                GameObject nextChunk = Instantiate(chunkPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), world.transform);

                for (int x = worldWidth * worldSize.chunkSize; x < worldSize.chunkSize * (worldWidth + 1); x++)
                {
                    for (int z = worldLength * worldSize.chunkSize; z < worldSize.chunkSize * (worldLength + 1); z++)
                    {
                        for (int y = -20; y < worldSize.y; y++)
                        {
                            SpawnBlock(x, y, z, nextChunk, worldWidth * worldSize.chunkSize, worldLength * worldSize.chunkSize);
                        }
                    }
                }

                nextChunk.GetComponent<MeshCombiner>().CombineMeshes();
                chunkList.Add(nextChunk);
            }
        }

        Debug.Log(blockCount + " Blocks Placed");
        FixChunkPlacement();
        MeshCombiner combiner = world.GetComponent<MeshCombiner>();
        DestroyImmediate(combiner);
        loadingPopup.SetActive(false);
        generating = false;
    }

    //since the chunks instantiate all in the same location, the actual layout doesn't get properly reflected without moving them.
    private void FixChunkPlacement()
    {
        int x = 0;
        int z = 0;
        for(int i = 0; i < chunkList.Count; i++)
        {
            if(i % worldSize.worldLength != 0 && i != 0)
            {
                z += worldSize.chunkSize;
                
            }
            else
            {
                z = 0;
                x += worldSize.chunkSize;
            }
            chunkList[i].transform.position = new Vector3(x-worldSize.chunkSize, 0, z);
        }
    }

    //I didn't want to make another script just for exiting so I put it here (used in the pause menu)
    public void ExitGame()
    {
        Application.Quit();
    }
}
