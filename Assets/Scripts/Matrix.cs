using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public class Matrix : MonoBehaviour
{
    private int boxSize = 40;
    private Sprite sprite;
    private Texture2D texture;
    private SpriteRenderer spriteRenderer;
    private Color red, green, white;
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
        white = Color.white;
        white.a = .5f;
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
        Vector2 pos = GetMousePosInMatrix();

        if (pos == currentStart)
        {
            heldColor = green;
            return true;
        }
        else if(pos == currentGoal)
        {
            heldColor = red;
            return true;
        }
        else
            return false;
    }

    private Vector2 GetMousePosInMatrix()
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
        heldNode = GetMousePosInMatrix();
        // Outside of boundaries
        if(heldNode == -Vector2.one)
            return;

        DrawNode((int)heldNode.x, (int)heldNode.y, heldColor);
        OnMouseButtonDown?.Invoke(currentStart, currentGoal);
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
                if (Main.matrix[9 - i / boxSize, j / boxSize] == 1)
                {
                    texture.SetPixel(j, i, white);
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
        else
            point = currentStart;

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
        else
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
