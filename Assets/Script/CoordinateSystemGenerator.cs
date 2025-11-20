using UnityEngine;
using UnityEngine.UI;

public class CoordinateSystemGenerator : MonoBehaviour
{
    [Header("坐标系设置")]
    public int gridSize = 5; // 坐标系范围（-gridSize 到 gridSize）
    public float cellSize = 100f; // 每个格子的像素大小
    public Color axisColor = Color.black; // 坐标轴颜色
    public Color gridColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); // 网格线颜色
    public Color textColor = Color.black; // 坐标文本颜色
    public Font textFont; // 坐标文本字体（可在 Project 窗口中拖拽字体文件）

    private Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        GenerateCoordinateSystem();
    }

    void GenerateCoordinateSystem()
    {
        // 生成 X 轴和 Y 轴（轴线比网格线粗）
        GenerateAxisLine(Vector2.left * gridSize * cellSize, Vector2.right * gridSize * cellSize, axisColor, 2f); // X 轴
        GenerateAxisLine(Vector2.down * gridSize * cellSize, Vector2.up * gridSize * cellSize, axisColor, 2f); // Y 轴

        // 生成网格线和坐标文本
        for (int i = -gridSize; i <= gridSize; i++)
        {
            // 生成垂直网格线（平行于 Y 轴）
            Vector2 vStart = new Vector2(i * cellSize, -gridSize * cellSize);
            Vector2 vEnd = new Vector2(i * cellSize, gridSize * cellSize);
            GenerateAxisLine(vStart, vEnd, gridColor, 1f);

            // 生成水平网格线（平行于 X 轴）
            Vector2 hStart = new Vector2(-gridSize * cellSize, i * cellSize);
            Vector2 hEnd = new Vector2(gridSize * cellSize, i * cellSize);
            GenerateAxisLine(hStart, hEnd, gridColor, 1f);

            // 生成坐标文本（跳过原点重复文本）
            if (i != 0)
            {
                // X 轴坐标文本（在 X 轴上方）
                GenerateCoordinateText(new Vector2(i * cellSize, 10f), i.ToString(), textColor);
                // Y 轴坐标文本（在 Y 轴右侧）
                GenerateCoordinateText(new Vector2(10f, i * cellSize), i.ToString(), textColor);
            }
        }

        // 生成原点文本
        GenerateCoordinateText(new Vector2(10f, 10f), "0", textColor);
    }

    // 生成单条轴线/网格线
    void GenerateAxisLine(Vector2 startPos, Vector2 endPos, Color color, float width)
    {
        GameObject lineObj = new GameObject("AxisLine");
        lineObj.transform.SetParent(transform); // 作为 Canvas 的子对象
        lineObj.transform.localPosition = Vector3.zero; // 相对于 Canvas 定位

        Image lineImage = lineObj.AddComponent<Image>();
        lineImage.color = color;

        // 计算线的长度和角度
        float length = Vector2.Distance(startPos, endPos);
        Vector2 direction = (endPos - startPos).normalized;

        // 设置线的 RectTransform
        RectTransform rect = lineObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(length, width);
        rect.anchoredPosition = (startPos + endPos) / 2f; // 居中在起止点中间
        rect.rotation = Quaternion.FromToRotation(Vector2.right, direction);
    }

    // 生成坐标文本
    void GenerateCoordinateText(Vector2 pos, string text, Color color)
    {
        GameObject textObj = new GameObject("CoordinateText");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = Vector3.zero;

        Text textComp = textObj.AddComponent<Text>();
        textComp.text = text;
        textComp.color = color;
        textComp.font = textFont ?? Resources.GetBuiltinResource<Font>("Arial.ttf"); // 默认使用 Arial 字体
        textComp.fontSize = 14;
        textComp.alignment = TextAnchor.MiddleLeft;

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(50f, 20f); // 文本框大小
    }
}