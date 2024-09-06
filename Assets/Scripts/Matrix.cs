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
        //if (Input.GetMouseButtonDown(0))
        //{
        //    HandleMouseClick(green);
        //}

        if (Input.GetMouseButton(0))
        {
            isHoldingMouse = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHoldingMouse = false;
        }

        //if (Input.GetMouseButtonDown(1))
        //{
        //    HandleMouseClick(red);
        //}

        if (isHoldingMouse)
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        if (mousePos.x > leftBorder && mousePos.x < rightBorder &&
            mousePos.y < topBorder && mousePos.y > bottomBorder)
        {
            Vector2Int node = new Vector2Int((int)mousePos.x , (int)mousePos.y) * 100;
            node.x = Mathf.Abs(node.x);
            node.y = Mathf.Abs(node.y);
            node = node / boxSize;

            if(node.x == currentStart.x && node.y == currentStart.y)
            {
                Debug.Log("yooo");
            }
            else if (node.x == currentGoal.x && node.y == currentGoal.y)
            {
                Debug.Log("ayayayya");
            }

            //DrawNode((int)node.x / boxSize, Mathf.Abs((int)node.y) / boxSize, color);
        }

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
        for (int i = (int)point.y * boxSize; i < (int)point.y * boxSize + boxSize; i++)
        {
            for (int j = (int)point.x * boxSize; j < (int)point.x * boxSize + boxSize; j++)
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
