using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Matrix : MonoBehaviour
{
    private int boxSize = 40;
    private Sprite sprite;
    private Texture2D texture;
    private SpriteRenderer spriteRenderer;
    private Color pixelColor = new Color(0, 0, 0, 1);
    private int leftBorder = 0, rightBorder = 4, topBorder = 0, bottomBorder = -4;

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
        DrawBlank();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition; 
            mousePos.z = 10;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            if (mousePos.x > leftBorder && mousePos.x < rightBorder && 
                mousePos.y < topBorder && mousePos.y > bottomBorder)
            {
                Vector2 node = mousePos * 100;
                Debug.Log((int)node.x / boxSize + " " + (int)node.y / boxSize);
            }
        }
    }

    private void DrawBlank()
    {
        texture = new Texture2D(boxSize * 10, boxSize * 10);
        for (int i = 0; i < 10 * boxSize; i++)
        {
            for (int j = 0; j < 10 * boxSize; j++)
            {
                texture.SetPixel(j, i, pixelColor);
            }
        }

        texture.Apply();

        Rect rect = new Rect(0f, 0f, texture.width, texture.height);
        Vector2 pivot = new Vector2(0, 1);
        sprite = Sprite.Create(texture, rect, pivot);
        spriteRenderer.sprite = sprite;
    }

    private void DrawPath()
    {
        for (int i = 0; i < 10 * boxSize; i++)
        {
            for (int j = 0; j < 10 * boxSize; j++)
            {
                if(Main.matrix[9 - i / boxSize, j / boxSize] == 1)
                {
                    texture.SetPixel(j, i, Color.white);
                }
            }
        }

        texture.Apply();

        Rect rect = new Rect(0f, 0f, texture.width, texture.height);
        Vector2 pivot = new Vector2(0, 1);
        sprite = Sprite.Create(texture, rect, pivot);
        spriteRenderer.sprite = sprite;

    }
}
