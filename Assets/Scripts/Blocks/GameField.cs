using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Flags]
public enum Axis { None = 0, X = 1, Y = 2, Z = 4 }

public class GameField
{
    /// <summary>
    /// The Array where all blocks are saved for easyer access
    /// </summary>
    private Block[,,] blocks = new Block[GameManager.Constants.X, GameManager.Constants.Y, GameManager.Constants.Z];
    /// <summary>
    /// For Switching Blocks that may or may not need to move back to their original position
    /// </summary>
    private SortedDictionary<int, List<Block>> backupMoves = new SortedDictionary<int, List<Block>>();

    public Block this[int xAxis, int yAxis, int zAxis]
    {
        get
        {
            try { return blocks[xAxis, yAxis, zAxis]; }
            catch { throw; }
        }
        set
        {
            blocks[xAxis, yAxis, zAxis] = value;
            value._position = new Vector3Int(xAxis,yAxis,zAxis);
        }
    }

    public Vector3Int GetBlocksSize()
    {
        return new Vector3Int(blocks.GetUpperBound(0), blocks.GetUpperBound(1), blocks.GetUpperBound(2));
    }


    /// <summary>
    /// Swaps to blocks position(in the blocks and the blocks on the array)
    /// </summary>
    /// <param name="b1">The primary block</param>
    /// <param name="b2">The secondary block</param>
    /// <param name="undo">If the move should be undone</param>
    public void Swap(Block b1, Block b2, bool undo = false)
    {
        if (!undo)
            backupMoves.Add(backupMoves.Count, new List<Block> { b1, b2 });
        else
            backupMoves.Remove(backupMoves.Count - 1);

        Vector3Int b1Pos = b1._position;
        Vector3Int b2Pos = b2._position;

        var temp = blocks[b1Pos.x, b1Pos.y, b1Pos.z];
        blocks[b1Pos.x, b1Pos.y, b1Pos.z] = blocks[b2Pos.x, b2Pos.y, b2Pos.z];
        blocks[b2Pos.x, b2Pos.y, b2Pos.z] = temp;

        Block.SwapPosition(b1, b2);
    }

    /// <summary>
    /// Unswaps the LAST move
    /// </summary>
    public void UndoSwap()
    {
        if (backupMoves == null || backupMoves.Count <= 0)
            throw new System.Exception("There is no Backup!!!!");
        else
            Swap(backupMoves[backupMoves.Count - 1][0], backupMoves[backupMoves.Count - 1][1], true);
    }

    /// <summary>
    /// Searches for adjacent block-color on the x-axis
    /// </summary>
    /// <param name="bk">The Block in question of making a connected line</param>
    /// <returns>Returns a connected line of blocks if existing</returns>
    private IEnumerable<Block> GetXMatches(Block bk)
    {
        List<Block> matches = new List<Block>();
        matches.Add(bk);
        var block = bk;
        if (block._position.x >= 0 && block._position.x < GameManager.Constants.X)
        {
            if (block._position.x > 0)
            {
                for (int column = block._position.x - 1; column >= 0; column--)
                {
                    if (GetBlocksBlock(block._position, Axis.X, x: column) != null &&
                        GetBlocksBlock(block._position, Axis.X, x: column).IsSameType(block))
                    {
                        matches.Add(GetBlocksBlock(block._position, Axis.X, x: column));
                    }
                    else
                        break;
                }
            }
            if (block._position.x < GameManager.Constants.X - 1)
            {
                for (int column = block._position.x + 1; column < GameManager.Constants.X; column++)
                {
                    if (GetBlocksBlock(block._position, Axis.X, x: column) != null &&
                        GetBlocksBlock(block._position, Axis.X, x: column).IsSameType(block))
                    {
                        matches.Add(GetBlocksBlock(block._position, Axis.X, x: column));
                    }
                    else
                        break;
                }
            }
        }
        else throw new System.Exception($"{block.gameObject}s x-position({block._position.x}) is invalid!");

        if (matches.Count < GameManager.Constants.MinimumMatches)
            matches.Clear();

        return matches;
    }
    /// <summary>
    /// Searches for adjacent block-color on the y-axis
    /// </summary>
    /// <param name="bk">The Block in question of making a connected line</param>
    /// <returns>Returns a connected line of blocks if existing</returns>
    private IEnumerable<Block> GetYMatches(Block bk)
    {
        List<Block> matches = new List<Block>();
        matches.Add(bk);
        var block = bk;

        if (block._position.y >= 0 && block._position.y < GameManager.Constants.X)
        {
            if (block._position.y > 0)
            {
                for (int row = block._position.y - 1; row >= 0; row--)
                {
                    if (GetBlocksBlock(block._position, Axis.Y, y: row) != null &&
                        GetBlocksBlock(block._position, Axis.Y, y: row).IsSameType(block))
                    {
                        matches.Add(GetBlocksBlock(block._position, Axis.Y, y: row));
                    }
                    else
                        break;
                }
            }
            if (block._position.y < GameManager.Constants.Y - 1)
            {
                for (int row = block._position.y + 1; row < GameManager.Constants.Y; row++)
                {
                    if (GetBlocksBlock(block._position, Axis.Y, y: row) != null &&
                        GetBlocksBlock(block._position, Axis.Y, y: row).IsSameType(block))
                    {
                        matches.Add(GetBlocksBlock(block._position, Axis.Y, y: row));
                    }
                    else
                        break;
                }
            }
        }
        else throw new System.Exception($"{block.gameObject}s y-position({block._position.y}) is invalid!");

        if (matches.Count < GameManager.Constants.MinimumMatches)
            matches.Clear();

        return matches;
    }
    /// <summary>
    /// Searches for adjacent block-color on the z-axis
    /// </summary>
    /// <param name="bk">The Block in question of making a connected line</param>
    /// <returns>Returns a connected line of blocks if existing</returns>
    private IEnumerable<Block> GetZMatches(Block bk)
    {
        List<Block> matches = new List<Block>();
        matches.Add(bk);
        var block = bk;
        if (block._position.z >= 0 && block._position.z < GameManager.Constants.Z)
        {
            if (block._position.z > 0)
            {
                for (int depht = block._position.z - 1; depht >= 0; depht--)
                {
                    if (GetBlocksBlock(block._position, Axis.Z, z: depht) != null &&
                        GetBlocksBlock(block._position, Axis.Z, z: depht).IsSameType(block))
                    {
                        matches.Add(GetBlocksBlock(block._position, Axis.Z, z: depht));
                    }
                    else
                        break;
                }
            }
            if (block._position.z < GameManager.Constants.Z - 1)
            {
                for (int row = block._position.z + 1; row < GameManager.Constants.Z; row++)
                {
                    if (GetBlocksBlock(block._position, Axis.Z, z: row) != null &&
                        GetBlocksBlock(block._position, Axis.Z, z: row).IsSameType(block))
                    {
                        matches.Add(GetBlocksBlock(block._position, Axis.Z, z: row));
                    }
                    else
                        break;
                }
            }
        }
        else throw new System.Exception($"{block.gameObject}s y-position({block._position.z}) is invalid!");

        if (matches.Count < GameManager.Constants.MinimumMatches)
            matches.Clear();

        return matches;
    }

    /// <summary>
    /// Get's an entire line, because of a line-bonus-block
    /// </summary>
    /// <param name="b">The block which is the line-bonus-type</param>
    /// <param name="axis">The axis the line goes</param>
    /// <returns>Returns a 'List' of Blocks in the line</returns>
    private IEnumerable<Block> GetEntireLine(Block b, Axis axis)
    {
        List<Block> matches = new List<Block>();
        Vector3Int pos = b._position;
        if (axis.Equals(Axis.X))
            for (int i = 0; i <= blocks.GetUpperBound(0); i++)
            {
                matches.Add(GetBlocksBlock(pos, axis, x: i));
            }
        else if (axis.Equals(Axis.Y))
            for (int i = 0; i < blocks.GetUpperBound(1); i++)
            {
                matches.Add(GetBlocksBlock(pos, axis, y: i));
            }
        else if (axis.Equals(Axis.Z))
            for (int i = 0; i < blocks.GetUpperBound(2); i++)
            {
                matches.Add(GetBlocksBlock(pos, axis, z: i));
            }
        return matches;
    }

    /// <summary>
    /// Get's a 3x3x3-block of blocks, because of a bomb-bonus-block
    /// </summary>
    /// <param name="b">The block with the bomb-bonus-type</param>
    /// <returns>Returns a big block(3x3x3) of blocks</returns>
    private IEnumerable<Block> GetEntireBigBlock(Block b)
    {
        List<Block> matches = new List<Block>();
        for (Axis axis = Axis.X; axis < Axis.None; axis++)
        {
            for (int i = -1; i < 2; i++)
            {
                if (GetAdjacentBlock(b._position, i, axis) != null)
                {
                    matches.Add(GetAdjacentBlock(b._position, i, axis));
                }
            }
        }
        return matches;
    }

    /// <summary>
    /// Get's all blocks of the same color, because of a special-bonus-block
    /// </summary>
    /// <param name="b">the block with the special-bonus</param>
    /// <returns>Returns all blocks of the same color as the input-block</returns>
    private IEnumerable<Block> GetEntireBlockColor(Block b)
    {
        List<Block> matches = new List<Block>();
        for (int x = 0; x <= blocks.GetUpperBound(0); x++)
            for (int y = 0; y <= blocks.GetUpperBound(1); y++)
                for (int z = 0; z <= blocks.GetUpperBound(2); z++)
                {
                    if (b.IsSameType(blocks[x, y, z]))
                    {
                        matches.Add(blocks[x, y, z]);
                    }

                }
        return matches;
    }

    private Block ContainsLineBonus(List<Block> matches, BlockType bonus)
    {
        if (matches.Count >= GameManager.Constants.MinimumMatches)
        {
            foreach (var bl in matches)
            {
                if ((bl.blockstats._Type & bonus) == bonus)
                {
                    return bl;
                }
            }
        }
        return null;
    }

    public MatchesInfo GetMatches(Block block)
    {
        MatchesInfo info = new MatchesInfo();

        List<Block> matches = new List<Block>();
        matches.AddRange(GetXMatches(block));
        matches.AddRange(GetYMatches(block));
        matches.AddRange(GetZMatches(block));

        if (ContainsLineBonus(matches, BlockType.HBOMB) != null)
        {
            matches.AddRange(GetEntireLine(ContainsLineBonus(matches, BlockType.HBOMB), Axis.X));
            if ((info.BonusesContained & BlockType.HBOMB) == BlockType.HBOMB)
                info.BonusesContained |= BlockType.HBOMB;
        }
        if (ContainsLineBonus(matches, BlockType.VBOMB) != null)
        {
            matches.AddRange(GetEntireLine(ContainsLineBonus(matches, BlockType.VBOMB), Axis.Y));
            if ((info.BonusesContained & BlockType.VBOMB) == BlockType.VBOMB)
                info.BonusesContained |= BlockType.VBOMB;
        }
        if (ContainsLineBonus(matches, BlockType.DBOMB) != null)
        {
            matches.AddRange(GetEntireLine(ContainsLineBonus(matches, BlockType.DBOMB), Axis.Z));
            if ((info.BonusesContained & BlockType.DBOMB) == BlockType.DBOMB)
                info.BonusesContained |= BlockType.DBOMB;
        }
        if (ContainsLineBonus(matches, BlockType.BOMB) != null)
        {
            matches.AddRange(GetEntireBigBlock(ContainsLineBonus(matches, BlockType.BOMB)));
            if ((info.BonusesContained & BlockType.BOMB) == BlockType.BOMB)
                info.BonusesContained |= BlockType.BOMB;
        }
        if (ContainsLineBonus(matches, BlockType.SBOMB) != null)
        {
            matches.AddRange(GetEntireBlockColor(ContainsLineBonus(matches, BlockType.SBOMB)));
            if ((info.BonusesContained & BlockType.SBOMB) == BlockType.SBOMB)
                info.BonusesContained |= BlockType.SBOMB;
        }

        info.AddBlockRange(matches);
        return info;
    }
    public IEnumerable<Block> GetMatches(IEnumerable<Block> blocks)
    {
        List<Block> matches = new List<Block>();
        foreach (var blk in blocks)
        {
            matches.AddRange(GetMatches(blk).MatchedBlock);
        }
        return matches.Distinct();
    }

    public void Remove(Block block)
    {
        blocks[block._position.x, block._position.y, block._position.z] = null;
    }

    public AlteredBlockInfo Collapse(IEnumerable<Vector2Int> level)
    {
        AlteredBlockInfo collapseInfo = new AlteredBlockInfo();

        foreach (var place in level)
            for (int y = 0; y < GameManager.Constants.Y; y++)
                if (blocks[place.x, y, place.y] == null)
                {
                    for (int y2 = y + 1; y2 < GameManager.Constants.Y; y2++)
                    {
                        collapseInfo.MaxDistance = y2 - y;
                        //if (row2 - row > collapseInfo.MaxDistance)
                        if (blocks[place.x, y2, place.y] != null)
                        {
                            blocks[place.x, y, place.y] = blocks[place.x, y2, place.y];
                            blocks[place.x, y2, place.y] = null;
                            blocks[place.x, y, place.y]._position.y = y;

                            collapseInfo.AddBlock(blocks[place.x, y, place.y]);
                            break;
                        }

                    }
                }
        return collapseInfo;
    }

    public IEnumerable<Vector3Int> GetEmptyItemsOnColumn(Vector2Int column)
    {
        List<Vector3Int> emptyItems = new List<Vector3Int>();
        for (int y = 0; y < GameManager.Constants.Y; y++)
        {
            if (blocks[column.x, y, column.y] == null)
                emptyItems.Add(new Vector3Int(column.x, y, column.y));
        }
        return emptyItems;
    }


    /// <summary>
    /// Returns the block of given Position
    /// </summary>
    /// <param name="blockPos">The original position of the block</param>
    /// <param name="x">If the x-Value is diferent from the blockPos</param>
    /// <param name="y">If the y-Value is diferent from the blockPos</param>
    /// <param name="z">If the z-Value is diferent from the blockPos</param>
    /// <returns>A Block of the wanted Position.</returns>
    public Block GetBlocksBlock(Vector3Int blockPos, Axis axis, int x = 0, int y = 0, int z = 0)
    {
        Vector3Int pos = new Vector3Int();
        pos.x = axis == Axis.X ? x : blockPos.x;
        pos.y = axis == Axis.Y ? y : blockPos.y;
        pos.z = axis == Axis.Z ? z : blockPos.z;
        try
        {
            return blocks[pos.x, pos.y, pos.z];
        }
        catch (System.Exception) { throw; }
    }

    /// <summary>
    /// Searches a block at a modified Vector
    /// </summary>
    /// <param name="blockPos">The original position</param>
    /// <param name="value">the distance of the adjacent Block (should be 1 or -1 to be adjacent)</param>
    /// <param name="axis">the general axis the adjacent Block lies on</param>
    /// <returns>Returns a block which is side by side,but not diagonal, with the blockPos.</returns>
    public Block GetAdjacentBlock(Vector3Int blockPos, int value, Axis axis = Axis.None)
    {
        Vector3Int pos = new Vector3Int();
        pos.x = axis == Axis.X ? blockPos.x + value : blockPos.x;
        pos.y = axis == Axis.Y ? blockPos.y + value : blockPos.y;
        pos.z = axis == Axis.Z ? blockPos.z + value : blockPos.z;
        try
        {
            return blocks[pos.x, pos.y, pos.z];
        }
        catch (System.Exception) { throw; }
    }
}
