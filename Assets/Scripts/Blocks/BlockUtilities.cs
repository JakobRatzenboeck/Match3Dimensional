using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class BlockUtilities
{
    static int indexer = 0;


    // Will change the Effect from opacity to emission or something else
    /// <summary>
    /// Will let a random possible match 'blink'
    /// </summary>
    /// <param name="potentialMatches">The Blocks of the potential match</param>
    /// <returns>Returns a yield WaitForSeconds</returns>
    public static IEnumerator AnimatePotentialMatches(IEnumerable<Block> potentialMatches)
    {
        Color baseColor = potentialMatches.First().blockstats._Color;
        Material[] mats = new Material[potentialMatches.Count()];
        Block[] potMatches = potentialMatches.ToArray();
        int count = potentialMatches.Count();
        for (int i = 0; i < count; i++)
            mats[i] = potMatches[i].GetComponent<MeshRenderer>().material;

        for (float i = 0f; i <= 1f; i += 0.1f)
        {
            for (int e = 0; e < count; e++)
            {
                mats[e].SetColor("_EmissionColor", baseColor * i);
                mats[e].EnableKeyword("_EMISSION");
            }
            yield return new WaitForSeconds(GameManager.Constants.OpacityAnimationFrameDelay);
        }
        for (float i = 1f; i >= 0f; i -= 0.1f)
        {
            for (int e = 0; e < count; e++)
            {
                mats[e].SetColor("_EmissionColor", baseColor * i);
                mats[e].EnableKeyword("_EMISSION");
            }
            yield return new WaitForSeconds(GameManager.Constants.OpacityAnimationFrameDelay);
        }

    }

    /// <summary>
    /// Looks if two blocks are neighbors.
    /// </summary>
    /// <param name="b1">One block. </param>
    /// <param name="b2">The other block. </param>
    /// <returns>Returns true if they are neighbors, false if not. </returns>
    public static bool AreNeighbors(Block b1, Block b2)
    {
        return (new List<float> { 0, 45 }.Contains(Vector3.Dot(b1._position + Vector3.up, b2._position + Vector3.up)))
                        && Vector3.Distance(b1._position, b2._position) <= 1 && b1.blockstats._Color == b2.blockstats._Color;
    }

    /// <summary>
    /// Gathers potential matches and then returns one.
    /// </summary>
    /// <param name="field">The array in which the potential match. </param>
    /// <returns>Returns a random potential match. </returns>
    public static IEnumerable<Block> GetPotentialMatches(GameField field)
    {
        //list that will contain all the matches we find
        List<List<Block>> matches = new List<List<Block>>();

        for (int x = 0; x < GameManager.Constants.Y; x++)
            for (int y = 0; y < GameManager.Constants.X; y++)
                for (int z = 0; z < GameManager.Constants.Z; z++)
                {
                    var matches2 = CheckNeighbors(x, y, z, null, field);

                    if (matches2 != null) matches.Add(matches2.ToList());

                    //if >= 3 matches, return a random one
                    if (matches.Count >= 3)
                        return matches[GameManager.Random.Next(0, matches.Count - 1)];

                    if (x >= GameManager.Constants.X / 2 && matches.Count > 0 && matches.Count <= 2)
                        return matches[GameManager.Random.Next(0, matches.Count - 1)];
                }
        return null;
    }

    /// <summary>
    /// Searching if there is a posible match with the neighbors
    /// </summary>
    /// <param name="x">The X-coordinate. </param>
    /// <param name="y">The Y-coordinate. </param>
    /// <param name="z">The Z-coordinate. </param>
    /// <param name="blacklistedAxis">Axes where there may be a neighbor but no possible match. </param>
    /// <param name="field">The field where the blocks are. </param>
    /// <returns>Retruns one posible match or null. </returns>
    public static IEnumerable<Block> CheckNeighbors(int x, int y, int z, SortedDictionary<int, Axis> blacklist, GameField field)
    {
        List<Block> matches = new List<Block>();
        matches.Add(field[x, y, z]);
        Axis blacklistedAxes = Axis.None;
        Axis lineAxis = Axis.None;
        int[] dirs = new int[] { -1, 1 };
        int dir = 1;
        try
        {
            dir = new int[] { -1, 1 }[GameManager.Random.Next(0, 1)];
        }
        catch
        {
            Debug.LogError(GameManager.Random);
        }
        if (blacklist == null) blacklist = new SortedDictionary<int, Axis>();
        blacklist.TryGetValue(dir, out blacklistedAxes);
        // if there are no matches
        if ((int)blacklistedAxes == 7)
        {
            dir *= -1;
            blacklist.TryGetValue(dir, out blacklistedAxes);
        }
        lineAxis = (Axis)7;
        lineAxis &= ~blacklistedAxes;


        #region 0X0\0X0\X00
        matches.Add(SearchNeighborInLine(x, y, z, 1 * dir, field, lineAxis, out lineAxis));
        matches.Add(SearchNeighborDiagonal(x, y, z, 2 * dir, field, lineAxis, out lineAxis));

        if (matches.Contains(null))
        {
            matches = new List<Block>() { field[x, y, z] };
        }
        else
        {
            return matches;
        }
        #endregion

        #region├ 0X0\X0X\0X0
        matches.Add(SearchNeighborDiagonal(x, y, z, 1 * dir, field, lineAxis, out lineAxis));
        matches.Add(SearchNeighborInLine(x, y, z, 2 * dir, field, lineAxis, out lineAxis));

        if (matches.Contains(null))
        {
            matches = new List<Block>() { field[x, y, z] };
        }
        else
        {
            return matches;
        }
        #endregion

        #region 0X0\0X0\000\0X0
        matches.Add(SearchNeighborInLine(x, y, z, 1 * dir, field, lineAxis, out lineAxis));
        if (GameManager.Mode == GameMode.HuniePopMode)
        {
            int bound = 0;
            if (lineAxis == Axis.X) bound = dir == -1 ? x : GameManager.Constants.X - x;
            if (lineAxis == Axis.Y) bound = dir == -1 ? y : GameManager.Constants.Y - y;
            if (lineAxis == Axis.Z) bound = dir == -1 ? z : GameManager.Constants.Z - z;

            for (int i = 3; i < bound; i++)
            {
                matches.Add(SearchNeighborInLine(x, y, z, i * dir, field, blacklistedAxes, out lineAxis));
            }
        }
        else
        {
            matches.Add(SearchNeighborInLine(x, y, z, 3 * dir, field, lineAxis, out lineAxis));
        }
        #endregion

        if (matches.Contains(null))
        {
            matches = null;
        }

        //Tries other axises just to be save
        if (matches == null && indexer < 6)
        {
            blacklistedAxes |= lineAxis;
            if (!blacklist.ContainsKey(dir))
            {
                blacklist.Add(dir, blacklistedAxes);
            }
            else
            {
                blacklist[dir] = blacklistedAxes;
            }
            indexer++;
            CheckNeighbors(x, y, z, blacklist, field);
        }
        indexer = 0;
        return matches;
    }

    /// <summary>
    /// Searches a block ajacent, but not in line, to the block in question.
    /// </summary>
    /// <param name="x">The x-coordinate of the Position. </param>
    /// <param name="y">The y-coordinate of the Position. </param>
    /// <param name="z">The z-coordinate of the Position. </param>
    /// <param name="matchPos">The distance relative to the primary block. </param>
    /// <param name="field">In which gamefield the blocks are. </param>
    /// <param name="lineAxis">On which axes or axis it can look for the same type. </param>
    /// <param name="lineAxisOut">On which axis it searched. </param>
    /// <returns>Returns a possible canditade for a match. </returns>
    public static Block SearchNeighborDiagonal(int x, int y, int z, int matchPos, GameField field, Axis lineAxis, out Axis lineAxisOut)
    {
        SortedDictionary<Axis, List<Block>> candidates = new SortedDictionary<Axis, List<Block>>();
        List<Block> values = new List<Block>();
        Block neighbor = null;
        int[] bounds = new int[] { -1, 2, -1, 2, -1, 2 };
        if (GameManager.Mode == GameMode.HuniePopMode) bounds = new int[] { -x, GameManager.Constants.X, -y, GameManager.Constants.Y, -z, GameManager.Constants.Z };

        if (((int)lineAxis) == 3)
            lineAxis = (Axis)GameManager.Random.Next(1, 2);
        if ((int)lineAxis == 5)
            lineAxis = (Axis)new List<int> { 1, 4 }[GameManager.Random.Next(0, 1)];
        if ((int)lineAxis == 6)
            lineAxis = (Axis)new List<int> { 2, 4 }[GameManager.Random.Next(0, 1)];
        if (((int)lineAxis) == 7)
            lineAxis = (Axis)new List<int> { 1, 2, 4 }[GameManager.Random.Next(0, 2)];

        lineAxisOut = lineAxis;

        if (field[x, y, z] != null)
        {
            //If the Axis is blacklisted, so it wouldn't run uselessly
            if ((lineAxis & Axis.X) == Axis.X)
            {
                // checks the up and down of the X-dispotitioned
                for (int yneighbor = bounds[2]; yneighbor < bounds[3]; yneighbor++)
                {
                    if (yneighbor == 0) yneighbor++;

                    if (PositionInBoundsBlockNotNull(new Vector3Int(x + matchPos, y + yneighbor, z), field) && field[x, y, z].blockstats._Color == field[x + matchPos, y + yneighbor, z].blockstats._Color)
                    {
                        values.Add(field[x + matchPos, y + yneighbor, z]);
                    }
                }
                // checks the before and behind of the X-dispotitioned
                for (int zneighbor = bounds[4]; zneighbor < bounds[5]; zneighbor++)
                {
                    if (zneighbor == 0) zneighbor++;

                    if (PositionInBoundsBlockNotNull(new Vector3Int(x + matchPos, y, z + zneighbor), field) && field[x, y, z].blockstats._Color == field[x + matchPos, y, z + zneighbor].blockstats._Color)
                    {
                        values.Add(field[x + matchPos, y, z + zneighbor]);
                    }
                }
                if (values.Count > 0)
                {
                    candidates.Add(lineAxis, values);
                }
                values = new List<Block>();
            }
            //If the Axis is blacklisted, so it wouldn't run uselessly
            if (lineAxis == Axis.Y)
            {
                // checks the left and right of the Y-dispotitioned
                for (int xneighbor = bounds[0]; xneighbor < bounds[1]; xneighbor++)
                {
                    if (PositionInBoundsBlockNotNull(new Vector3Int(x + xneighbor, y + matchPos, z), field) && field[x, y, z].blockstats._Color == field[x + xneighbor, y + matchPos, z].blockstats._Color)
                    {
                        values.Add(field[x + xneighbor, y + matchPos, z]);
                    }
                }
                // checks the before and behind of the Y-dispotitioned
                for (int zneighbor = bounds[4]; zneighbor < bounds[5]; zneighbor++)
                {
                    if (zneighbor == 0) zneighbor++;

                    if (PositionInBoundsBlockNotNull(new Vector3Int(x, y + matchPos, z + zneighbor), field) && field[x, y, z].blockstats._Color == field[x, y + matchPos, z + zneighbor].blockstats._Color)
                    {
                        values.Add(field[x, y + matchPos, z + zneighbor]);
                    }
                }
                if (values.Count > 0)
                {
                    candidates.Add(lineAxis, values);
                }
                values = new List<Block>();
            }
            //If the Axis is blacklisted, so it wouldn't run uselessly
            if (lineAxis == Axis.Z)
            {
                // checks the left and right of the Z-dispotitioned
                for (int xneighbor = bounds[0]; xneighbor < bounds[1]; xneighbor++)
                {
                    if (xneighbor == 0) xneighbor++;

                    if (PositionInBoundsBlockNotNull(new Vector3Int(x + xneighbor, y, z + matchPos), field) && field[x, y, z].blockstats._Color == field[x + xneighbor, y, z + matchPos].blockstats._Color)
                    {
                        values.Add(field[x + xneighbor, y, z + matchPos]);
                    }
                }
                // checks the up and down of the Z-dispotitioned
                for (int yneighbor = bounds[2]; yneighbor < bounds[3]; yneighbor++)
                {
                    if (yneighbor == 0) yneighbor++;

                    if (PositionInBoundsBlockNotNull(new Vector3Int(x, y + yneighbor, z + matchPos), field) && field[x, y, z].blockstats._Color == field[x, y + yneighbor, z + matchPos].blockstats._Color)
                    {
                        values.Add(field[x, y + yneighbor, z + matchPos]);
                    }
                }
                if (values.Count > 0)
                {
                    candidates.Add(lineAxis, values);
                }
                values = new List<Block>();
            }
        }

        if (candidates.Count > 0)
        {
            candidates.TryGetValue(lineAxis, out values);
            neighbor = values[GameManager.Random.Next(0, values.Count - 1)];
        }

        return neighbor;
    }

    /// <summary>
    /// Searches a block ajacent to the block in question.
    /// </summary>
    /// <param name="x">The x-coordinate of the Position. </param>
    /// <param name="y">The y-coordinate of the Position. </param>
    /// <param name="z">The z-coordinate of the Position. </param>
    /// <param name="matchPos">The Position of the possible match if is part of [(x,y,z)/pos/pos]. </param>
    /// <param name="field">The whole gamefield. </param>
    /// <param name="blacklistedAxis">Axes where there may be a neighbor but no possible match. </param>
    /// <param name="lineAxis">The axis the second same-colored block is. </param>
    /// <returns>Returns the second same-colored block. </returns>
    public static Block SearchNeighborInLine(int x, int y, int z, int matchPos, GameField field, Axis lineAxis, out Axis lineAxisOut)
    {
        SortedDictionary<Axis, List<Block>> candidates = new SortedDictionary<Axis, List<Block>>();
        List<Block> values = new List<Block>();
        Block neighbor = null;

        if (((int)lineAxis) == 3)
            lineAxis = (Axis)GameManager.Random.Next(1, 2);
        if ((int)lineAxis == 5)
            lineAxis = (Axis)new List<int> { 1, 4 }[GameManager.Random.Next(0, 1)];
        if ((int)lineAxis == 6)
            lineAxis = (Axis)new List<int> { 2, 4 }[GameManager.Random.Next(0, 1)];
        if (((int)lineAxis) == 7)
            lineAxis = (Axis)new List<int> { 1, 2, 4 }[GameManager.Random.Next(0, 2)];

        lineAxisOut = lineAxis;

        if (field[x, y, z] != null)
        {
            if (lineAxis == Axis.X)
            {
                if (PositionInBoundsBlockNotNull(new Vector3Int(x + matchPos, y, z), field) && field[x, y, z].blockstats._Color == field[x + matchPos, y, z].blockstats._Color)
                    values.Add(field[x + matchPos, y, z]);
                if (values.Count > 0)
                {
                    candidates.Add(lineAxis, values);
                }
                values = new List<Block>();
            }
            if (lineAxis == Axis.Y)
            {
                if (PositionInBoundsBlockNotNull(new Vector3Int(x, y + matchPos, z), field) && field[x, y, z].blockstats._Color == field[x, y + matchPos, z].blockstats._Color)
                    values.Add(field[x, y + matchPos, z]);
                if (values.Count > 0)
                {
                    candidates.Add(lineAxis, values);
                }
                values = new List<Block>();
            }
            if (lineAxis == Axis.Z)
            {
                if (PositionInBoundsBlockNotNull(new Vector3Int(x, y, z + matchPos), field) && field[x, y, z].blockstats._Color == field[x, y, z + matchPos].blockstats._Color)
                    values.Add(field[x, y, z + matchPos]);
                if (values.Count > 0)
                {
                    candidates.Add(lineAxis, values);
                }
                values = new List<Block>();
            }
        }

        if (candidates.Count > 0)
        {
            candidates.TryGetValue(lineAxis, out values);
            neighbor = values[GameManager.Random.Next(0, values.Count - 1)];
        }

        return neighbor;
    }

    /// <summary>
    /// Checks if given position is InBounds of the gamefield and if the position in the field is not null.
    /// </summary>
    /// <param name="pos">The Position which will be checked. </param>
    /// <param name="field">The field in which the nullcheck should happen. </param>
    /// <returns>Returns <code>true</code> if both the Position is in bounds and the place of the position in the field is not null. </returns>
    public static bool PositionInBoundsBlockNotNull(Vector3Int pos, GameField field)
    {
        return (pos.x >= 0 && pos.x < GameManager.Constants.X) && (pos.y >= 0 && pos.y < GameManager.Constants.Y) && (pos.z >= 0 && pos.z < GameManager.Constants.Z) && field[pos.x, pos.y, pos.z] != null;
    }

}