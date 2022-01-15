using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PositionHolder : MonoBehaviour
{
    public Vector3Int Position { get; set; }
    private BlocksManager bm;
    private GameField field;

    void Awake()
    {
        bm = FindObjectOfType<BlocksManager>();
        field = bm.blocks;
    }

    void OnTriggerEnter(Collider other)
    {
        Block otherBlock = other.GetComponent<Block>();
        if (other.tag.Equals(GameManager.Constants._SelectedBlockTag))
        {
            if (!otherBlock.moves.Contains(Position))
            {
                Block.SwapPosition(otherBlock, field[Position.x, Position.y, Position.z]);
                field[Position.x, Position.y, Position.z].transform.DOMove(bm.LeftBottomBack + Vector3.Scale(field[Position.x, Position.y, Position.z]._position, bm.BlockSize), GameManager.Constants.AnimationDuration, true);
            }
            else if (otherBlock.moves.Count > 1 && otherBlock.moves[otherBlock.moves.Count - 2] == Position)
            {
                otherBlock.RevertMove(otherBlock.moves.Count - 2, false);
            }
            if (otherBlock == field[Position.x, Position.y, Position.z])
            {
                otherBlock.ChangeMovementTypeTo(ConfigurableJointMotion.Free);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == field[Position.x, Position.y, Position.z] && other.tag == GameManager.Constants._SelectedBlockTag)
        {
            RestrictMovement(other.GetComponent<Block>());
        }
    }


    /// <summary>
    /// Restricts the movement of a block, so the block can only be moved in one direction
    /// </summary>
    /// <param name="block">the block which movement should be restricted</param>
    private void RestrictMovement(Block block)
    {
        float x = Mathf.Abs((bm.LeftBottomBack.x + (block._position.x * bm.BlockSpacing.x) - block.transform.position.x)),
              y = Mathf.Abs((bm.LeftBottomBack.y + (block._position.y * bm.BlockSpacing.y) - block.transform.position.y)),
              z = Mathf.Abs((bm.LeftBottomBack.z + (block._position.z * bm.BlockSpacing.z) - block.transform.position.z));

        block.ChangeMovementTypeTo(ConfigurableJointMotion.Limited);
        if (Mathf.Max(x, y, z) == x)
        {
            block.cJ.xMotion = ConfigurableJointMotion.Free;
        }
        else if (Mathf.Max(x, y, z) == y)
        {
            block.cJ.yMotion = ConfigurableJointMotion.Free;
        }
        else if (Mathf.Max(x, y, z) == z)
        {
            block.cJ.zMotion = ConfigurableJointMotion.Free;
        }
        else
        {
            Debug.LogError($"Something went wrong here!(X: {x};  Y: {y};  Z: {z};  Mathf.Max{Mathf.Max(x, y, z)})");
            Debug.LogError($"xMotion: {block.cJ.xMotion};  yMotion: {block.cJ.yMotion};  zMotion: {block.cJ.zMotion}");
        }

    }
}
