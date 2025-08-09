using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;

public class TileGFX : MonoBehaviour
{
    Tile tile;
    public Color defaultColor;

    public void Awake()
    {
        tile = GetComponent<Tile>();
    }

    [SerializeField, FoldoutGroup("References")] SpriteRenderer border;
    [SerializeField, FoldoutGroup("References")] SpriteRenderer innerBorder;
    [SerializeField, FoldoutGroup("References")] SpriteRenderer outerBorder;
    [SerializeField, FoldoutGroup("References")] SpriteRenderer aimingBorder;
    [SerializeField, FoldoutGroup("References")] SpriteRenderer fill;
    [SerializeField, FoldoutGroup("References")] SpriteRenderer pathUp;
    [SerializeField, FoldoutGroup("References")] SpriteRenderer pathLeft;
    [SerializeField, FoldoutGroup("References")] SpriteRenderer pathDown;
    [SerializeField, FoldoutGroup("References")] SpriteRenderer pathRight;
    [SerializeField, FoldoutGroup("References")] SpriteRenderer pathDestination;
    

    public void ResetGraphics()
    {
        border.enabled = true;
        border.color = defaultColor;
        innerBorder.enabled = false;
        outerBorder.enabled = false;
        aimingBorder.enabled = false;

        pathUp.enabled = false;
        pathLeft.enabled = false;
        pathDown.enabled = false;
        pathRight.enabled = false;
        pathDestination.enabled = false;

        if(!tile.IsWalkable)
            fill.color = Color.gray;
        else
            fill.enabled = false;
    }

    public void SetOuterBorderColor(Color color)
    {
        outerBorder.enabled = true;
        outerBorder.color = color;
    }
    public void SetBorderColor(Color color)
    {
        border.enabled = true;
        border.color = color;
    }
    public void SetInnerBorderColor(Color color)
    {
        innerBorder.enabled = true;
        innerBorder.color = color;
    }
    public void SetFillColor(Color color)
    {
        fill.enabled = true;
        fill.color = color;
    }
    public void SetAimingColor(Color color)
    {
        aimingBorder.enabled = true;
        aimingBorder.color = color;
    }
    public void SetPathDestination()
    {
        pathDestination.enabled = true;
    }

    public void SetPathPreview(Vector2Int before, Vector2Int after)
    {
        EnableDirection(before);
        EnableDirection(after);

        void EnableDirection(Vector2Int direction)
        {
            if(direction == Vector2Int.up)
                pathUp.enabled = true;
            else if (direction == Vector2Int.left)
                pathLeft.enabled = true;
            else if (direction == Vector2Int.down)
                pathDown.enabled = true;
            else if (direction == Vector2Int.right)
                pathRight.enabled = true;
        }
    }
}
