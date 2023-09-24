using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WorldSettings
{
    public WorldSize generationSettings;
    public string seed;
}

//simple script that lets me store cool seeds in the inspector window of a gameobject.
public class CoolSeeds : MonoBehaviour
{
    public List<WorldSettings> seeds;
}
