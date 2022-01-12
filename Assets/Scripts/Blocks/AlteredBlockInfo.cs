using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlteredBlockInfo 
{
    /// <summary>
    /// the blocks which need to be altered
    /// </summary>
    private List<Block> newBlock { get; set; }
    /// <summary>
    /// The distance the blocks need to travel
    /// </summary>
    public int MaxDistance { get; set; }

    /// <summary>
    /// Shows all blocks which need to be altered
    /// </summary>
    public IEnumerable<Block> AlteredBlock
    {
        get
        {
            return newBlock;
        }
    }

    /// <summary>
    /// Adds a block to the list
    /// </summary>
    /// <param name="block">The block to be added</param>
    public void AddBlock(Block block)
    {
        if (!newBlock.Contains(block))
            newBlock.Add(block);
    }

    public AlteredBlockInfo()
    {
        newBlock = new List<Block>();
    }
}
