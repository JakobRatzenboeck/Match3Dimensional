using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Blockstats blockstats;
    public Color _color;
    public Vector3Int _position;

    void Awake()
    {
        _position = blockstats._Position;
        if (!blockstats) throw new System.MissingFieldException("No blockstats found for " + this);
        GetComponent<MeshRenderer>().material.color = blockstats._Color;
        if (transform.childCount >= 1)
            for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).GetComponent<MeshRenderer>().material.color = blockstats._Color;
    }

    /// <summary>
    /// Checks if given other block is the same colorType as this one.
    /// </summary>
    /// <param name="otherBlock">The other block which needs to be checked. </param>
    /// <returns>Returns true if they have the same color, false if not. </returns>
    public bool IsSameType(Block otherBlock)
    {
        if (otherBlock == null || !(otherBlock is Block))
            throw new System.ArgumentException("otherBlock");
        return (this.blockstats._Color == otherBlock.blockstats._Color);
    }

    /// <summary>
    /// Assigns a new color and position to a block.
    /// </summary>
    /// <param name="color">The new color the block will have.</param>
    /// <param name="position">The new postion the block will have.</param>
    public void Assign(Color color, Vector3Int position)
    {
        _color = color;
        _position = position;
    }

    /// <summary>
    /// Swaps the position of given blocks.
    /// </summary>
    /// <param name="a">The primary block of the two switching.</param>
    /// <param name="b">The secondary block of the two switching.</param>
    public static void SwapPosition(Block a, Block b)
    {
        Vector3Int temp = a._position;
        a._position = b._position;
        b._position = temp;
    }

}
