using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public class Matrix : MonoBehaviour
{
    private int boxSize = 40;
    private Sprite sprite;
    private Texture2D texture;
    private SpriteRenderer spriteRenderer;
    private Color red, green, yellow, gray = Color.gray;
    private int leftBorder = 0, rightBorder = 4, topBorder = 0, bottomBorder = -4;
    private Vector2Int currentStart, currentGoal;
    public static event Action<Vector2, Vector2> OnMouseButtonDown;
    private bool isHoldingMouse = false;
    private Color heldColor;
    private Vector2 heldNode;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        Main.OnCalculation += DrawPath;
    }

    private void OnDisable()
    {
        Main.OnCalculation -= DrawPath;
    }

    private void Start()
    {
        red = Color.red;
        red.a = 0.5f;
        green = Color.green;
        green.a = 0.5f;
        yellow = Color.yellow;
        yellow.a = .5f;
        texture = new Texture2D(boxSize * 10, boxSize * 10);
        texture.wrapMode = TextureWrapMode.Clamp;
        currentStart = new Vector2Int(int.MaxValue, int.MaxValue);
        currentGoal = new Vector2Int(int.MaxValue, int.MaxValue);
        DrawBlank();
        DrawNode(0, 0, green);
        DrawNode(9, 9, red);
        CreateSprite();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && CheckIfHoldingAnyNode())
        {
            isHoldingMouse = true;
        }

        if(Input.GetMouseButtonDown(0) && !CheckIfHoldingAnyNode())
        {
            HandleObstacle();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHoldingMouse = false;
        }

        if (isHoldingMouse)
        {
            HandleMouseClick();
        }
    }

    private bool CheckIfHoldingAnyNode()
    {
        Vector2 index = TurnMousePosIntoMatrixIndex();

        if (index == currentStart)
        {
            heldColor = green;
            return true;
        }
        else if(index == currentGoal)
        {
            heldColor = red;
            return true;
        }
        else
            return false;
    }

    private Vector2 TurnMousePosIntoMatrixIndex()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        if (mousePos.x > leftBorder && mousePos.x < rightBorder &&
            mousePos.y < topBorder && mousePos.y > bottomBorder)
        {
            mousePos = mousePos * 100;
            mousePos = mousePos / boxSize;
            mousePos = new Vector2(Mathf.Floor(Mathf.Abs(mousePos.x)), Mathf.Floor(Mathf.Abs(mousePos.y)));

            return mousePos;
        }

        return -Vector2.one;
    }

    private void HandleMouseClick()
    {
        heldNode = TurnMousePosIntoMatrixIndex();
        // Outside of boundaries
        if(heldNode == -Vector2.one)
            return;

        if (Main.matrix[(int)heldNode.y, (int)heldNode.x] == -1)
            return;

        DrawNode((int)heldNode.x, (int)heldNode.y, heldColor);
        OnMouseButtonDown?.Invoke(currentStart, currentGoal);
    }

    private void HandleObstacle()
    {
        Vector2 index = TurnMousePosIntoMatrixIndex();

        // Outside of boundaries
        if (index.x < 0 || index.x > Main.width || index.y > Main.height|| index.y < 0)
            return;

        // -1 represents blocked node, 0 represents passable node
        switch (Main.matrix[(int)index.y, (int)index.x])
        {
            case -1:
                Main.matrix[(int)index.y, (int)index.x] = 0;
                DrawNode((int)index.x, (int)index.y, Color.black);
                break;
            case 0:
                Main.matrix[(int)index.y, (int)index.x] = -1;
                DrawNode((int)index.x, (int)index.y, gray);
                break;
            default:
                return;
        }
    }

    private void DrawBlank()
    {
        for (int i = 0; i < 10 * boxSize; i++)
        {
            for (int j = 0; j < 10 * boxSize; j++)
            {
                texture.SetPixel(j, i, Color.black);
            }
        }

        texture.Apply();
    }

    private void DrawPath()
    {
        DrawBlank();

        for (int i = 0; i < 10 * boxSize; i++)
        {
            for (int j = 0; j < 10 * boxSize; j++)
            {
                switch (Main.matrix[9 - i / boxSize, j / boxSize])
                {
                    case 1:
                        texture.SetPixel(j, i, yellow);
                        break;
                    case -1:
                        texture.SetPixel(j, i, gray);
                        break;
                    default:
                        continue;
                }
            }
        }

        DrawNode(currentStart.x, currentStart.y, green);
        DrawNode(currentGoal.x, currentGoal.y, red);

        texture.Apply();
    }

    // x and y are the coordinates of the node
    private void DrawNode(int x, int y, Color color)
    {
        Vector2Int point;
        if (color == red)
            point = currentGoal;
        else if(color == green)
            point = currentStart;
        else
            point = new Vector2Int(x, y);

        // Draw the previous node to black
        for (int i = point.y * boxSize; i < point.y * boxSize + boxSize; i++)
        {
            for (int j = point.x * boxSize; j < point.x * boxSize + boxSize; j++)
            {
                texture.SetPixel(j, texture.height - 1 - i, Color.black);
            }
        }

        if (point == currentStart)
            currentStart = new Vector2Int(x, y);
        else if(point == currentGoal)
            currentGoal = new Vector2Int(x, y);

        for (int i = y * boxSize; i < y * boxSize + boxSize; i++)
        {
            for (int j = x * boxSize; j < x * boxSize + boxSize; j++)
            {
                texture.SetPixel(j, texture.height - 1 - i, color);
            }
        }

        texture.Apply();
    }

    private void CreateSprite()
    {
        Rect rect = new Rect(0f, 0f, texture.width, texture.height);
        Vector2 pivot = new Vector2(0, 1);
        sprite = Sprite.Create(texture, rect, pivot);
        spriteRenderer.sprite = sprite;
    }
}
