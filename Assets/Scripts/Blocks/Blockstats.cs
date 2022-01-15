using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType { NORMAL = 0, HBOMB = 1, VBOMB = 2, DBOMB = 4, BOMB = 8, SBOMB = 16 }

[CreateAssetMenu(menuName = "Blockstats")]
public class Blockstats : ScriptableObject
{
    /// <summary>
    /// The color it 'reacts' to
    /// </summary>
    public Color _Color;
    /// <summary>
    /// 0 being the noraml puzzle-block
    /// 1 being the horizonal bomb
    /// 2 being the vertical bomb
    /// 2 being the depth bomb
    /// 4 being the X-+-Bomb
    /// 8 being the Same-color-bomb
    /// </summary>
    public BlockType _Type;
    /// <summary>
    /// The position of the block in the game
    /// </summary>
    public Vector3Int _Position;
}
