using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Block : MonoBehaviour
{
    public Blockstats blockstats;
    public Color _color;
    public Vector3Int _position;
    private XRGrabInteractable grabable;
    public ConfigurableJoint cJ;
    public List<Vector3Int> moves;
    private GameObject classicMovement;
    private BlocksManager bm;
    private GameField field;

    void Awake()
    {
        _position = blockstats._Position;
        if (!blockstats) throw new System.MissingFieldException("No blockstats found for " + this);
        GetComponent<MeshRenderer>().material.color = blockstats._Color;
        if (transform.childCount >= 1)
            for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).GetComponent<MeshRenderer>().material.color = blockstats._Color;
        grabable = GetComponent<XRGrabInteractable>();
        grabable.interactionManager = FindObjectOfType<XRInteractionManager>();
        cJ = GetComponent<ConfigurableJoint>();
        moves = new List<Vector3Int>() { _position };
        bm = FindObjectOfType<BlocksManager>();
        field = bm.blocks;
    }

    void Start()
    {
        grabable.hoverEntered.AddListener(OnHoverEntered);
        grabable.hoverExited.AddListener(OnHoverExit);
        grabable.selectEntered.AddListener(OnGrab);
        grabable.selectExited.AddListener(OnDeselect);
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
    /// <param name="color">The new color the block will have. </param>
    /// <param name="position">The new postion the block will have. </param>
    public void Assign(Color color, Vector3Int position)
    {
        _color = color;
        _position = position;
    }

    /// <summary>
    /// Swaps the position of given blocks.
    /// </summary>
    /// <param name="a">The primary block of the two switching. </param>
    /// <param name="b">The secondary block of the two switching. </param>
    public static void SwapPosition(Block a, Block b)
    {
        Vector3Int temp = a._position;
        a._position = b._position;
        b._position = temp;
        if (!a.moves.Contains(a._position))
        {
            a.moves.Add(a._position);
            b.moves.Add(b._position);
        }
        else
        {
            a.moves.Remove(a._position);
            b.moves.Add(b._position);
        }
    }

    /// <summary>
    /// When the player hovers over this block this method will highlight it.
    /// (Will include a visual Help to know which row, line, column the player is in)
    /// </summary>
    /// <param name="args0"></param>
    public void OnHoverEntered(HoverEnterEventArgs args0)
    {
        Material mat = GetComponent<MeshRenderer>().material;
        mat.SetColor("_EmissionColor", _color * 10);
        mat.EnableKeyword("_EMISSION");
    }

    /// <summary>
    /// When the player stops hovering over the block this method will stop highlighting the block.
    /// </summary>
    /// <param name="args0"></param>
    public void OnHoverExit(HoverExitEventArgs args0)
    {
        Material mat = GetComponent<MeshRenderer>().material;
        mat.SetColor("_EmissionColor", _color * 0);
        mat.EnableKeyword("_EMISSION");
    }

    /// <summary>
    /// When the block is Grabed this Method will be called.
    /// </summary>
    public void OnGrab(SelectEnterEventArgs args0)
    {
        bm.hitBl = this;
        bm.StopCheckForPotentialMatches();

        if (GameManager.Mode == GameMode.Classic)
        {
            classicMovement = Instantiate(Resources.Load("Prefabs/Blocks/InvertedBlock") as GameObject, transform.position, Quaternion.identity);
        }
        gameObject.tag = GameManager.Constants._SelectedBlockTag;
        cJ.xMotion = ConfigurableJointMotion.Free;
        cJ.yMotion = ConfigurableJointMotion.Free;
        cJ.zMotion = ConfigurableJointMotion.Free;
    }

    /// <summary>
    /// When the Player is letting go of this block.
    /// </summary>
    /// <param name="args0"></param>
    public void OnDeselect(SelectExitEventArgs args0)
    {
        if (classicMovement != null)
        {
            Destroy(classicMovement);
        }
        if (!BlockUtilities.AreNeighbors(this, bm.blocks[moves[moves.Count - 1].x, moves[moves.Count - 1].y, moves[moves.Count - 1].z]))
        {
            RevertMove();
        }
        else
        {
            StartCoroutine(bm.FindMatchesAndCollapse(this));
        }
    }

    /// <summary>
    /// Reverts a certain amount of Moves or all. (position of the block[Data&Visual])
    /// </summary>
    /// <param name="revertmove">How many moves should be reverted. (min,max - moves.count(), 0) </param>
    /// <param name="animateGrabed">If the grabbed block should also be animated. </param>
    public void RevertMove(int revertmove = 0, bool animateGrabbed = true)
    {
        Block secondBlock;
        if (animateGrabbed)
            this.transform.DOMove(bm.LeftBottomBack + Vector3.Scale(moves[revertmove], bm.BlockSize), GameManager.Constants.AnimationDuration, true);
        for (int ri = moves.Count - 1; ri > revertmove; ri--)
        {
            secondBlock = bm.blocks[moves[ri].x, moves[ri].y, moves[ri].z];
            secondBlock.transform.DOMove(bm.LeftBottomBack + Vector3.Scale(moves[ri], bm.BlockSize), GameManager.Constants.AnimationDuration, true);
            SwapPosition(this, secondBlock);
        }
        gameObject.tag = "Untagged";
    }

    /// <summary>
    /// Changes the Movement of all axes to wished motion.
    /// </summary>
    /// <param name="motion">The wished motion. </param>
    public void ChangeMovementTypeTo(ConfigurableJointMotion motion)
    {
        cJ.xMotion = motion;
        cJ.yMotion = motion;
        cJ.zMotion = motion;
    }

    /// <summary>
    /// Unsubcribes from all Scripted XRGrabInteractable Events.
    /// </summary>
    internal void UnsubscribeInteractebleEvents()
    {
        grabable.firstHoverEntered.RemoveAllListeners();
        grabable.lastHoverExited.RemoveAllListeners();
        grabable.selectEntered.RemoveAllListeners();
        grabable.selectExited.RemoveAllListeners();
    }

    void OnDestroy()
    {
        UnsubscribeInteractebleEvents();
    }
}
