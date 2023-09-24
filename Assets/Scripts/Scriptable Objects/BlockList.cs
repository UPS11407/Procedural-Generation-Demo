using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockList.asset", menuName = "BlockList")]
//used to keep a nice scriptable object with all the valid generation blocks in it.
public class BlockList : ScriptableObject
{
    public GameObject[] blocks;
}