using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchesInfo
{
    private List<Block> matchedBlocks;

    /// <summary>
    /// Returns all blocks who are part of the match
    /// </summary>
    public IEnumerable<Block> MatchedBlock
    {
        get
        {
            return matchedBlocks;
        }
    }

    /// <summary>
    /// Adds a block to the match
    /// </summary>
    /// <param name="bl">The block who is part of a match</param>
    public void AddBlock(Block bl)
    {
        if (!matchedBlocks.Contains(bl))
            matchedBlocks.Add(bl);
    }

    /// <summary>
    /// Adds blocks to the match
    /// </summary>
    /// <param name="bls">Many blocks which are part of a match</param>
    public void AddBlockRange(IEnumerable<Block> bls)
    {
        foreach (Block bl in bls)
        {
            AddBlock(bl);
        }
    }

    /// <summary>
    /// Initaliation of the class
    /// </summary>
    public MatchesInfo()
    {
        matchedBlocks = new List<Block>();
        BonusesContained = BlockType.NORMAL;
    }

    /// <summary>
    /// Shows/Sets if and which type of bonus is contained in the match
    /// </summary>
    public BlockType BonusesContained { get; set; }
}
