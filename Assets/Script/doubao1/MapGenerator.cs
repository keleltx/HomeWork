using UnityEngine;
using TMPro;
using UnityEngine.UI; // 新增这一行，引入UI命名空间

[RequireComponent(typeof(Image))]
public class MapGenerator : MonoBehaviour
{
    [Header("地图参数")]
    public int maxX = 5; // 用户设置的最大X坐标（对应火星车的x范围：0~maxX）
    public int maxY = 5; // 用户设置的最大Y坐标（对应火星车的y范围：0~maxY）
    public int gridSize = 100; // 每个格子的像素大小（控制背景整体尺寸）
    public Color axisColor = Color.black; // 坐标轴颜色
    public Color gridColor = new Color(0.8f, 0.8f, 0.8f); // 网格线颜色
    public TMP_Text gridLabelPrefab; // 坐标刻度文字预制体

    private Image backgroundImage;
    private Texture2D mapTexture;
    private int textureWidth; // 背景纹理宽度（= maxX * gridSize）
    private int textureHeight; // 背景纹理高度（= maxY * gridSize）

    public GameObject backGround;
    public RectTransform backgroundRectTrans;
    void Awake()
    {
        maxX++;
        maxY++;
        backgroundRectTrans = backGround.GetComponent<RectTransform>();
        backgroundImage = GetComponent<Image>();
        // 计算纹理尺寸（每个格子对应gridSize像素）
        textureWidth = maxX * gridSize;
        textureHeight = maxY * gridSize;
        // 生成背景纹理
        GenerateMapTexture();
        // 生成坐标刻度
        GenerateGridLabels();
    }

    // 生成带网格和坐标轴的纹理
    void GenerateMapTexture()
    {
        mapTexture = new Texture2D(textureWidth, textureHeight);
        mapTexture.filterMode = FilterMode.Point; // 像素化显示（适合坐标图）
        mapTexture.wrapMode = TextureWrapMode.Clamp;

        // 填充背景色（白色）
        Color[] pixels = new Color[textureWidth * textureHeight];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        mapTexture.SetPixels(pixels);

        // 绘制坐标轴（x轴、y轴，粗线：3像素）
        int axisLineWidth = 3;
        // 绘制x轴（底部）
        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < axisLineWidth; y++)
            {
                if (y < textureHeight)
                    mapTexture.SetPixel(x, y, axisColor);
            }
        }
        // 绘制y轴（左侧）
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < axisLineWidth; x++)
            {
                if (x < textureWidth)
                    mapTexture.SetPixel(x, y, axisColor);
            }
        }

        // 绘制网格线（竖线：x方向，横线：y方向）
        int gridLineWidth = 2;
        // 竖线（每个x坐标对应一条线）
        for (int x = 0; x <= maxX; x++)
        {
            int pixelX = x * gridSize;
            for (int y = 0; y < textureHeight; y++)
            {
                for (int w = 0; w < gridLineWidth; w++)
                {
                    int currentX = pixelX + w;
                    if (currentX < textureWidth)
                        mapTexture.SetPixel(currentX, y, gridColor);
                }
            }
        }
        // 横线（每个y坐标对应一条线）
        for (int y = 0; y <= maxY; y++)
        {
            int pixelY = y * gridSize;
            for (int x = 0; x < textureWidth; x++)
            {
                for (int w = 0; w < gridLineWidth; w++)
                {
                    int currentY = pixelY + w;
                    if (currentY < textureHeight)
                        mapTexture.SetPixel(x, currentY, gridColor);
                }
            }
        }

        // 应用纹理修改
        mapTexture.Apply();
        // 将纹理赋值给背景Image
        backgroundImage.sprite = Sprite.Create(mapTexture, new Rect(0, 0, textureWidth, textureHeight), Vector2.zero);
        // 调整Background的RectTransform尺寸（适配纹理）
        RectTransform rectTrans = GetComponent<RectTransform>();
        rectTrans.sizeDelta = new Vector2(textureWidth, textureHeight);
    }

    // 生成坐标刻度文字（x轴底部、y轴左侧）
    void GenerateGridLabels()
    {
        // 生成x轴刻度（0,1,2,...,maxX）
        for (int x = 0; x < maxX; x++)
        {
            TMP_Text label = Instantiate(gridLabelPrefab, transform);
            label.text = x.ToString();
            // 计算文字位置（x轴刻度在x坐标正下方，y=0 - 20像素（避免遮挡坐标轴））
            float posX = x * gridSize-backgroundRectTrans.rect.width/2+gridSize/2;
            float posY = -20f-backgroundRectTrans.rect.height/2;
            label.rectTransform.anchoredPosition = new Vector2(posX, posY);
        }

        // 生成y轴刻度（0,1,2,...,maxY）
        for (int y = 0; y < maxY; y++)
        {
            TMP_Text label = Instantiate(gridLabelPrefab, transform);
            label.text = y.ToString();
            // 计算文字位置（y轴刻度在y坐标左侧，x=0 - 30像素）
            float posX = -30f - backgroundRectTrans.rect.width / 2;
            float posY = y * gridSize - backgroundRectTrans.rect.height / 2 + gridSize / 2;
            label.rectTransform.anchoredPosition = new Vector2(posX, posY);
        }

        // 生成每个格子的坐标标签
        for (int x=0;x<maxX ;x++ ) 
        {
            float posX = x * gridSize - backgroundRectTrans.rect.width / 2 + gridSize / 2;
            for(int y = 0; y < maxY ;y++)
            {
                float posY = y * gridSize - backgroundRectTrans.rect.height / 2 + gridSize / 2;
                //posY -= (float)(gridSize * 0.35);
                TMP_Text label = Instantiate(gridLabelPrefab, transform);
                label.fontSize = 14;
                label.color = Color.green;
                label.text = new Vector2Int(x, y).ToString();
                label.rectTransform.anchoredPosition = new Vector2(posX, posY);
            }
        }
    }

    // 提供外部接口：更新地图大小（如果需要动态修改）
    public void UpdateMapSize(int newMaxX, int newMaxY)
    {
        maxX = newMaxX;
        maxY = newMaxY;
        // 重新生成背景和刻度
        GenerateMapTexture();
        GenerateGridLabels();
        // 销毁旧的刻度文字（避免重复）
        foreach (Transform child in transform)
        {
            if (child.GetComponent<TMP_Text>() != null)
                Destroy(child.gameObject);
        }
        GenerateGridLabels();
    }
}