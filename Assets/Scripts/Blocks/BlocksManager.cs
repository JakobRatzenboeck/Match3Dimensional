using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Linq;

public class BlocksManager : MonoBehaviour
{
    private GameManager GM;
    private GameSettings GS;

    public TMP_Text ScoreText;
    public TMP_Text TimerText;
    private float timer = 0;
    private int[] clock = new int[3];
    public GameField blocks;

    public Vector3 LeftBottomBack;
    public readonly Vector3 BlockSize = new Vector3(0.5f, 0.5f, 0.5f);
    public readonly Vector3 BlockSpacing = new Vector3(0.15f, 0.15f, 0.15f);

    [SerializeField]
    private Transform gFP;
    [SerializeField]
    private Transform sGFP;
    public Block hitBl = null;
    public Block[] BlockPrefabs;
    public GameObject ExplosionPrefab;
    public GameObject positionHolder;
    public Block[] BonusPrefabs;
    public GameObject classicMovementRestraint;
    public GameObject lineHelp;

    private IEnumerator CheckPotentialMatchesCoroutine;
    private IEnumerator AnimatePotentialMatchesCoroutine;

    IEnumerable<Block> potentialMatches;

    public SoundManager soundManager;

    [SerializeField]
    private TMP_Text text;
    [SerializeField]
    private Canvas pauseMenuCanvas;
    [SerializeField]
    private Canvas gameOverScreen;

    void Awake()
    {
        GM = GameManager.Instance;
        GS = GameSettings.Instance;
        gFP.position = new Vector3(-((GameManager.Constants.X - 1) * BlockSize.x / 2), 0.3f, -((GameManager.Constants.Z - 1) * BlockSize.z / 2));
        LeftBottomBack = gFP.position;
        GameObject border = Instantiate(Resources.Load("Prefabs/Blocks/InvertedBlock"), gFP.position, Quaternion.identity) as GameObject;
        border.name = "Gamefieldborder";
        border.transform.localScale = new Vector3((GameManager.Constants.X / 2) * BlockSize.x, (GameManager.Constants.Y / 2) * BlockSize.y, (GameManager.Constants.Z / 2) * BlockSize.z);

        if (GameManager.seed == 0)
            GameManager.seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        GameManager.Random = new System.Random(GameManager.seed);
    }

    void Start()
    {
        if (GM.GameState == GameState.Game)
            InitializeBlockAndSpawnPosition();
        else
            InitializeBlockAndSpawnPositionsFromPremadeLevel(GameManager.textname);
        if (GameManager.help)
            StartCheckForPotentialMatches();
        text.text = GameManager.seed.ToString();
    }

    #region Score control
    private void InitializeScore(int score = 0)
    {
        GameManager.score = score;
        ShowScore();
    }

    private void IncreaseScore(int amount)
    {
        GameManager.score += amount;
        ShowScore();
    }

    private void ShowScore()
    {
        ScoreText.text = "Score: " + GameManager.score.ToString();
    }

    #endregion

    /// <summary>
    /// Gets a Random block from a list
    /// </summary>
    /// <param name="blockPrefabs">The posibilities of blocks which can be picked</param>
    /// <returns>Returns random Block</returns>
    private Block GetRandomBlock(Block[] blockPrefabs)
    {
        return blockPrefabs[GameManager.Random.Next(0, blockPrefabs.Length)];
    }

    /// <summary>
    /// Instatiates and Places a new Block and puts it in the right layer
    /// </summary>
    /// <param name="position">The position the block will be instantiated</param>
    /// <param name="newBlock">The block-type which will be instantiated</param>
    private void InstantiateAndPlaceNewBlock(Vector3Int position, Block newBlock)
    {
        Block blk = Instantiate(newBlock, LeftBottomBack + new Vector3(position.x * BlockSize.x, position.y * BlockSize.y, position.z * BlockSize.z), Quaternion.identity) as Block;
        PositionHolder pH = Instantiate(positionHolder, LeftBottomBack + new Vector3(position.x * BlockSize.x, position.y * BlockSize.y, position.z * BlockSize.z), Quaternion.identity).GetComponent<PositionHolder>();

        blk.name = blk.blockstats.name + position;
        pH.name = position.ToString();

        blk.Assign(newBlock.blockstats._Color, position);
        //blk.cJ.connectedAnchor = position;
        pH.Position = position;

        blocks[position.x, position.y, position.z] = blk;

        #region Managing History
        Transform pT = gFP.transform.Find("Level" + position.y);
        Transform spG = sGFP.transform.Find("Level" + position.y);
        if (pT == null)
        {
            GameObject pG = new GameObject("Level" + position.y);
            spG = new GameObject("Level" + position.y).transform;
            pT = pG.transform;
            pT.parent = gFP;
            spG.transform.parent = sGFP;
            pT.localPosition = Vector3.zero;
        }
        blk.transform.parent = pT;
        pH.transform.parent = spG.transform;
        #endregion
    }

    /// <summary>
    /// Destroys all blocks in the field
    /// </summary>
    private void DestroyAllBlocks()
    {
        for (int x = 0; x < GameManager.Constants.X; x++)
            for (int y = 0; y < GameManager.Constants.Y; y++)
                for (int z = 0; z < GameManager.Constants.Z; z++)
                {
                    Destroy(blocks[x, y, z]);
                }
    }

    /// <summary>
    /// Fills the Gamefield with blocks and destroys if there are still some
    /// </summary>
    public void InitializeBlockAndSpawnPosition()
    {
        InitializeScore();
        List<Block> blockPrefabs = new List<Block>();
        blockPrefabs.AddRange(BlockPrefabs);

        if (blocks != null)
            DestroyAllBlocks();
        blocks = new GameField();
        for (int x = 0; x < GameManager.Constants.X; x++)
            for (int y = 0; y < GameManager.Constants.Y; y++)
                for (int z = 0; z < GameManager.Constants.Z; z++)
                {
                    Block newBlock = GetRandomBlock(blockPrefabs.ToArray());
                    blockPrefabs.Remove(newBlock);

                    while (x >= 2 && blocks[x - 1, y, z].IsSameType(newBlock) && blocks[x - 2, y, z].IsSameType(newBlock))
                    {
                        newBlock = GetRandomBlock(blockPrefabs.ToArray());
                        blockPrefabs.Remove(newBlock);
                    }
                    while (y >= 2 && blocks[x, y - 1, z].IsSameType(newBlock) && blocks[x, y - 2, z].IsSameType(newBlock))
                    {
                        newBlock = GetRandomBlock(blockPrefabs.ToArray());
                        blockPrefabs.Remove(newBlock);
                    }
                    while (z >= 2 && blocks[x, y, z - 1].IsSameType(newBlock) && blocks[x, y, z - 2].IsSameType(newBlock))
                    {
                        newBlock = GetRandomBlock(blockPrefabs.ToArray());
                        blockPrefabs.Remove(newBlock);
                    }
                    InstantiateAndPlaceNewBlock(new Vector3Int(x, y, z), newBlock);
                    blockPrefabs.Clear(); blockPrefabs.AddRange(BlockPrefabs);
                }
    }

    /// <summary>
    /// Checks for potential matches and animates them
    /// </summary>
    /// <returns>Returns a corutine-able procedure</returns>
    private IEnumerator CheckPotentialMatches()
    {
        yield return new WaitForSeconds(GameManager.Constants.WaitBeforePotentialMatchesCheck);
        potentialMatches = BlockUtilities.GetPotentialMatches(blocks);
        if (potentialMatches != null)
        {
            while (true)
            {
                AnimatePotentialMatchesCoroutine = BlockUtilities.AnimatePotentialMatches(potentialMatches);
                StartCoroutine(AnimatePotentialMatchesCoroutine);
                yield return new WaitForSeconds(GameManager.Constants.WaitBeforePotentialMatchesCheck);
            }
        }
        else
        {
            StopAllCoroutines();
            gameOverScreen.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Resets and disables the emission of blocks in the potential match
    /// </summary>
    private void ResetEmissionOnPotentialMatches()
    {
        if (potentialMatches != null)
        {
            foreach (var item in potentialMatches)
            {
                if (item == null) break;
                Color c = item.blockstats._Color;
                item.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", c);
                item.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                item.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
            }
        }
    }

    /// <summary>
    /// Starts corutiene to look for potential matches
    /// </summary>
    private void StartCheckForPotentialMatches()
    {
        StopCheckForPotentialMatches();
        CheckPotentialMatchesCoroutine = CheckPotentialMatches();
        StartCoroutine(CheckPotentialMatchesCoroutine);
    }

    /// <summary>
    /// Stops all coroutines for potential matches
    /// </summary>
    public void StopCheckForPotentialMatches()
    {
        if (AnimatePotentialMatchesCoroutine != null)
            StopCoroutine(AnimatePotentialMatchesCoroutine);
        if (CheckPotentialMatchesCoroutine != null)
            StopCoroutine(CheckPotentialMatchesCoroutine);
        ResetEmissionOnPotentialMatches();
    }

    #region Remove & Create Explosion

    /// <summary>
    /// Creates a explosion at the position where it removes(destroys) a block
    /// </summary>
    /// <param name="item">The block to be removed</param>
    private void RemoveFromScene(Block item)
    {
        GameObject explosion = GetExplosion();
        var newExplosion = Instantiate(explosion, item.transform.position, Quaternion.identity);
        Destroy(newExplosion, GameManager.Constants.ExplosionDuration);
        Destroy(item);
    }

    /// <summary>
    /// Gets the only Explostion-Prefab
    /// </summary>
    /// <returns>Returns the Explostion-Prefab</returns>
    private GameObject GetExplosion()
    {
        return ExplosionPrefab;
    }


    #endregion

    /// <summary>
    /// Animates blocks moving down
    /// </summary>
    /// <param name="moveBlocks">the blocks it's moving</param>
    /// <param name="distance">the distance it needs to move</param>
    private void MoveAndAnimate(IEnumerable<Block> moveBlocks, int distance)
    {
        Transform pos = null;
        foreach (var item in moveBlocks)
        {
            if (pos == null)
                pos = item.transform;
            pos.transform.DOMove(pos.position + Vector3.down * distance, GameManager.Constants.MoveAnimationMinDuration * distance);
            if (pos != null)
                pos = null;
        }
    }

    /// <summary>
    /// Creates new blocks to fill empty places after a match
    /// </summary>
    /// <param name="columnsWithMissingBlock">The columns where a block is "missing"(x,y)</param>
    /// <returns>Reruns the new blocks</returns>
    private AlteredBlockInfo CreateNewBlockInSpecificColumns(IEnumerable<Vector2Int> columnsWithMissingBlock)
    {
        AlteredBlockInfo newBlockInfo = new AlteredBlockInfo();

        foreach (Vector2Int column in columnsWithMissingBlock)
        {
            var emptyItems = blocks.GetEmptyItemsInColumn(column);
            foreach (var item in emptyItems)
            {
                var go = GetRandomBlock(BlockPrefabs);
                Block newBlock = Instantiate(go, new Vector3(column.x, GameManager.Constants.Y * BlockSize.y, column.y), Quaternion.identity).GetComponent<Block>();

                newBlock.Assign(go.blockstats._Color, go._position);

                if (GameManager.Constants.Y - item.y > newBlockInfo.MaxDistance)
                    newBlockInfo.MaxDistance = GameManager.Constants.Y - item.y;

                blocks[item.x, item.y, item.z] = newBlock;
                newBlockInfo.AddBlock(newBlock);
            }
        }

        return newBlockInfo;
    }

    #region Create&Get Bonus

    /// <summary>
    /// Creates the bonus which will be granted
    /// </summary>
    /// <param name="hitBlCache">The main block</param>
    /// <param name="bonus">The type of bonus</param>
    private void CreateBonus(Block hitBlCache, BlockType bonus)
    {
        Block Bonus = Instantiate(GetBonusFromType(hitBlCache.blockstats._Type, hitBlCache.blockstats._Color), LeftBottomBack + new Vector3(hitBlCache._position.x * BlockSize.x,
            hitBlCache._position.y * BlockSize.y, hitBlCache._position.z * BlockSize.z), Quaternion.identity).GetComponent<Block>();

        blocks[hitBlCache._position.x, hitBlCache._position.y, hitBlCache._position.z] = Bonus;
        var BonusBlock = Bonus;
        Bonus.Assign(hitBlCache.blockstats._Color, hitBlCache._position);
        BonusBlock.blockstats._Type |= bonus;
    }

    /// <summary>
    /// Searches for the right block of specific type and color
    /// </summary>
    /// <param name="type">What type of bonus it is</param>
    /// <param name="color">What color-type the bonus is</param>
    /// <returns>Returns the specific bonus-block</returns>
    private Block GetBonusFromType(BlockType type, Color color)
    {
        foreach (var item in BonusPrefabs)
        {
            if ((item.blockstats._Type & type) == type && item.blockstats._Color == color)
                return item;
        }
        throw new System.Exception("Something went wrong");
    }

    /// <summary>
    /// Decides which bonus will be granted, from what matches where made(Only one bonus for a move)
    /// </summary>
    /// <param name="hitBlChache">The main block of the match (one of which was moved)</param>
    /// <param name="totalMatches">All the blocks which are part of the match</param>
    private void GetSupposedBonus(Block hitBlChache, IEnumerable<Block> totalMatches)
    {
        BlockType supposedBonus = BlockType.NORMAL;
        if (totalMatches.Where(item => (item.blockstats._Color == hitBlChache.blockstats._Color)
        && (item._position.x == hitBlChache._position.x && item._position.y == hitBlChache._position.y)).Count() == 4)
            supposedBonus = BlockType.DBOMB;
        if (totalMatches.Where(item => (item.blockstats._Color == hitBlChache.blockstats._Color)
        && (item._position.x == hitBlChache._position.x && item._position.z == hitBlChache._position.z)).Count() == 4)
            supposedBonus = BlockType.VBOMB;
        if (totalMatches.Where(item => (item.blockstats._Color == hitBlChache.blockstats._Color)
        && (item._position.y == hitBlChache._position.y && item._position.z == hitBlChache._position.z)).Count() == 4)
            supposedBonus = BlockType.HBOMB;

        if (totalMatches.Where(item => (item.blockstats._Color == hitBlChache.blockstats._Color)
                            && ((item._position.x == hitBlChache._position.x && item._position.y == hitBlChache._position.y)
                            || (item._position.x == hitBlChache._position.x && item._position.z == hitBlChache._position.z)
                            || (item._position.y == hitBlChache._position.y && item._position.z == hitBlChache._position.z))).Count() >= 5)
            supposedBonus = BlockType.SBOMB;
        var blockPerColor = from Block in totalMatches group Block by Block.blockstats._Color into blockGroup select new { Color = blockGroup.Key, Count = blockGroup.Count() };
        if (blockPerColor.Where(item => item.Count == 3).Count() >= 2)
            supposedBonus = BlockType.BOMB;

        CreateBonus(hitBlChache, supposedBonus);
    }

    #endregion

    private void OnApplicationPause(bool pause)
    {
        pauseMenuCanvas.gameObject.SetActive(!pauseMenuCanvas.gameObject.activeSelf);
    }

    void LateUpdate()
    {
        // Updates the clock
        if (!pauseMenuCanvas.gameObject.activeSelf && !gameOverScreen.gameObject.activeSelf)
            timer += Time.deltaTime;
        if (timer >= 1)
        {
            clock[2] += 1;
            if (clock[2] > 59) { clock[1] += 1; clock[2] = 0; }
            if (clock[1] > 59) { clock[0] += 1; clock[1] = 0; }
            string timertext = clock[0] == 0 ? $"{clock[1].ToString("D2")}:{clock[2].ToString("D2")}" : $"{clock[0].ToString("D2")}:{clock[1].ToString("D2")}:{clock[2].ToString("D2")}";
            TimerText.text = timertext;
            timer -= 1;
        }
    }

    /// <summary>
    /// Checks for and collapses Matches.
    /// </summary>
    /// <param name="hitBl2">The block which will be switched with the moving. </param>
    /// <returns>Returns an IEnumerator. </returns>
    public IEnumerator FindMatchesAndCollapse(Block hitBl2)
    {
        var hitBlMatchesInfo = blocks.GetMatches(hitBl);
        var hitBl2MatchesInfo = blocks.GetMatches(hitBl2);

        var totalMatches = hitBlMatchesInfo.MatchedBlock.Union(hitBl2MatchesInfo.MatchedBlock).Distinct();

        if (totalMatches.Count() < GameManager.Constants.MinimumMatches)
        {
            hitBl.RevertMove();
            yield return new WaitForSeconds(GameManager.Constants.AnimationDuration * hitBl.moves.Count);
        }

        bool addBonus = totalMatches.Count() >= GameManager.Constants.MinimumMatchesForBonus &&
            ((hitBl.blockstats._Type & BlockType.NORMAL) == BlockType.NORMAL) &&
            ((hitBl2.blockstats._Type & BlockType.NORMAL) == BlockType.NORMAL);

        Block hitBlCache = null;
        if (addBonus)
            hitBlCache = hitBlMatchesInfo.MatchedBlock.Count() > 0 ? hitBl : hitBl2;

        int timesRun = 1;
        while (totalMatches.Count() >= GameManager.Constants.MinimumMatches)
        {
            IncreaseScore((totalMatches.Count() - 2) * GameManager.Constants.Match3Score);
            if (timesRun >= 2)
                IncreaseScore(GameManager.Constants.SubsequentMatchScore);

            soundManager.PlayCrincle();

            foreach (var item in totalMatches)
            {
                blocks.Remove(item);
                RemoveFromScene(item);
            }

            if (addBonus)
                GetSupposedBonus(hitBlCache, totalMatches);

            addBonus = false;
            var columns = totalMatches.Select(bl => new Vector2Int(bl._position.x, bl._position.z)).Distinct();

            var collapsedBlockInfo = blocks.Collapse(columns);
            var newBlockInfo = CreateNewBlockInSpecificColumns(columns);

            int maxDistance = Mathf.Max(collapsedBlockInfo.MaxDistance, newBlockInfo.MaxDistance);

            MoveAndAnimate(newBlockInfo.AlteredBlock, maxDistance);
            MoveAndAnimate(collapsedBlockInfo.AlteredBlock, maxDistance);

            yield return new WaitForSeconds(GameManager.Constants.MoveAnimationMinDuration * maxDistance);

            totalMatches = blocks.GetMatches(collapsedBlockInfo.AlteredBlock).Union(blocks.GetMatches(newBlockInfo.AlteredBlock)).Distinct();

            timesRun++;
        }
        StartCheckForPotentialMatches();

    }

    #region Premade/Loaded Levels

    /// <summary>
    /// Gets the specified block.
    /// </summary>
    /// <param name="info">the info of the block. (color type) </param>
    /// <returns>Returns a Block-reference to be instantiated. </returns>
    private Block GetSpecificBlockOrBonusForPremadeLevel(string info)
    {
        var tokens = info.Split('_');

        if (tokens.Count() == 1 || tokens[1].Trim() == "N")
        {
            foreach (var item in BlockPrefabs)
            {
                if (ColorUtility.ToHtmlStringRGB(item.blockstats._Color) == tokens[0])
                    return item;
            }
        }
        else if (tokens.Count() == 2 && tokens[1].Trim() != "N")
        {
            foreach (var item in BonusPrefabs)
            {
                if (ColorUtility.ToHtmlStringRGB(item.blockstats._Color) == tokens[0] && item.blockstats._Type.ToString().StartsWith(tokens[1].Trim().ToUpper()))
                    return item;
            }
        }

        throw new System.Exception($"Wrong color({info}), your premade level is corupted!!");
    }

    /// <summary>
    /// Creates a gamefield after the given "Save-File".
    /// </summary>
    /// <param name="levelToLoad">The saved level which should be loaded. </param>
    public void InitializeBlockAndSpawnPositionsFromPremadeLevel(string levelToLoad)
    {
        TextAsset txt = Resources.Load(GameManager.Constants._SavedFilesPath + levelToLoad) as TextAsset;
        string level = txt.text.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries)[0];

        GameManager.seed = int.Parse(level.Split(';')[1]);
        GameManager.Random = new System.Random(GameManager.seed);
        GameManager.Constants.X = int.Parse(level.Split(';')[2]);
        GameManager.Constants.Y = int.Parse(level.Split(';')[3]);
        GameManager.Constants.Z = int.Parse(level.Split(';')[4]);

        InitializeScore(int.Parse(level.Split(';')[5]));
        clock[0] = (int.Parse(level.Split(';')[6].Split(':')[0]));
        clock[1] = (int.Parse(level.Split(';')[6].Split(':')[1]));
        clock[2] = (int.Parse(level.Split(';')[6].Split(':')[2]));
        var premadeLevel = FillShapesArrayFromResourcesData(levelToLoad);

        if (blocks != null)
            DestroyAllBlocks();

        blocks = new GameField();

        for (int x = 0; x < GameManager.Constants.X; x++)
            for (int y = 0; y < GameManager.Constants.Y; y++)
                for (int z = 0; z < GameManager.Constants.Z; z++)
                {
                    Block newblock = null;
                    newblock = GetSpecificBlockOrBonusForPremadeLevel(premadeLevel[x, y, z]);
                    InstantiateAndPlaceNewBlock(new Vector3Int(x, y, z), newblock);
                }
    }

    /// <summary>
    /// Parses the TextAsset to a readable array for further use.
    /// </summary>
    /// <param name="levelToLoad">The file which should be parsed. </param>
    /// <returns>Returns a array with the blocks already in position. (only now need be instatiated) </returns>
    public static string[,,] FillShapesArrayFromResourcesData(string levelToLoad)
    {
        string[,,] blocks = new string[GameManager.Constants.X, GameManager.Constants.Y, GameManager.Constants.Z];
        TextAsset txt = Resources.Load(GameManager.Constants._SavedFilesPath + levelToLoad) as TextAsset;
        string level = txt.text;

        string[] lines = level.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int y = GameManager.Constants.Y - 1; y >= 0; y--)
        {
            for (int z = GameManager.Constants.Z - 1; z >= 0; z--)
            {
                string[] fields = lines[1 + y + (GameManager.Constants.Y * z)].Split('|');
                for (int x = GameManager.Constants.X - 1; x >= 0; x--)
                {
                    blocks[x, y, z] = fields[x].Split('/')[0];
                }
            }
        }
        return blocks;
    }

    #endregion
}
