using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class RoverController : MonoBehaviour
{
    [Header("初始状态")]
    public int initialX = 1; // 初始x坐标（对应火星车世界坐标）
    public int initialY = 2; // 初始y坐标
    public Direction initialDirection = Direction.N; // 初始朝向

    [Header("移动参数")]
    public float moveDuration = 0.5f; // 移动一格的时间（秒，控制动画速度）
    public float rotateDuration = 0.3f; // 旋转90°的时间（秒）

    public GameObject backGround;
    public RectTransform backgroundRectTrans;

    // 朝向枚举（对应旋转角度：N=0°, E=90°, S=180°, W=270°）
    public enum Direction { N, E, S, W }
    public Direction currentDirection { get; private set; } // 当前朝向
    public int currentRotationAngle;
    public float currentX { get; private set; } // 当前x坐标
    public float currentY { get; private set; } // 当前y坐标

    private RectTransform rectTrans; // 火星车的UI坐标组件
    private MapGenerator mapGenerator; // 地图生成器（用于获取格子大小）
    private int gridSize; // 每个格子的像素大小（和背景一致）

    public GameObject button;

    void Awake()
    {
        backgroundRectTrans = backGround.GetComponent<RectTransform>();
        rectTrans = GetComponent<RectTransform>();
        // 获取地图生成器组件（确保Background和MarsRover在同一个Canvas下）
        mapGenerator = FindObjectOfType<MapGenerator>();
        if (mapGenerator != null)
            gridSize = mapGenerator.gridSize;
        else
            gridSize = 100; //  fallback值
    }

    void Start()
    {
        // 初始化火星车状态为（0，0）
        currentX = -backgroundRectTrans.rect.width/2+gridSize/2;
        currentY = -backgroundRectTrans.rect.height/2+gridSize/2;
        //转换为初始坐标
        currentX+= initialX * gridSize;
        currentY+= initialY * gridSize;

        currentDirection = initialDirection;
        currentRotationAngle=currentDirection switch
        {
            Direction.N => 0,
            Direction.E => 270,
            Direction.S => 180,
            Direction.W => 90,
            _ => 0
        };
        // 初始位置和朝向赋值
        UpdateRoverPosition();
        UpdateRoverRotation();
    }

    // 更新火星车的UI位置（世界坐标→UI像素坐标）
    void UpdateRoverPosition()
    {
        // 世界坐标 (x,y) → Background 像素坐标：x*gridSize, y*gridSize
        rectTrans.anchoredPosition = new Vector2(currentX, currentY);
    }

    // 更新火星车的旋转（朝向→角度）
    void UpdateRoverRotation()
    {
        float angle = 0f;
        switch (currentDirection)
        {
            case Direction.N: angle = 0f; break;
            case Direction.E: angle = 270f; break;
            case Direction.S: angle = 180f; break;
            case Direction.W: angle = 90f; break;
        }
        rectTrans.localEulerAngles = new Vector3(0f, 0f, angle);
    }

    // 左转指令（逆时针旋转90°）
    public void TurnLeft()
    {
        Debug.Log("左转");
        currentRotationAngle += 90;
        StartCoroutine(RotateCoroutine()); // 平滑旋转动画
    }

    // 右转指令（顺时针旋转90°）
    public void TurnRight()
    {
        Debug.Log("右转");
        currentRotationAngle -= 90;
        StartCoroutine(RotateCoroutine()); // 平滑旋转动画
    }

    // 平滑旋转协程
    IEnumerator RotateCoroutine()
    {
        float startAngle = rectTrans.localEulerAngles.z;
        float elapsed = 0f;
        while (elapsed < rotateDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / rotateDuration);
            rectTrans.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(startAngle, (float)currentRotationAngle, t));
            yield return null;
        }
        
        // 确保最终角度准确
        if(currentRotationAngle >= 360)
            currentRotationAngle -= 360;
        else if(currentRotationAngle < 0)
            currentRotationAngle += 360;
        rectTrans.localEulerAngles = new Vector3(0f, 0f, currentRotationAngle);
    }
    
    // 前进指令（沿当前朝向移动一格）
    public void Move()
    {
        // 根据朝向更新世界坐标
        switch (currentRotationAngle) { 
            case 0: // 北
                Debug.Log("向北前进1格");
                currentY += gridSize;
                break;
            case 270: // 东
                Debug.Log("向东前进1格");
                currentX += gridSize;
                break;
            case 180: // 南
                Debug.Log("向南前进1格");
                currentY -= gridSize;
                break;
            case 90: // 西
                Debug.Log("向西前进1格");
                currentX -= gridSize;
                break;
        }

        StartCoroutine(MoveCoroutine());
    }

    // 平滑移动协程
    IEnumerator MoveCoroutine()
    {
        Vector2 startPos = rectTrans.anchoredPosition;
        Vector2 targetPos = new Vector2(currentX, currentY);

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            rectTrans.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        // 确保最终位置准确
        rectTrans.anchoredPosition = targetPos;
    }
    
    // 新增：封装方法，用于UI事件绑定
    public void StartExecuteCommands(string commands)
    {
        Image buttonImage = button.GetComponent<Image>();
        Button buttonComp = button.GetComponent<Button>();
        TMP_Text text = button.GetComponentInChildren<TMP_Text>();

        text.enabled = false; // 隐藏文字
        buttonImage.enabled = false; // 隐藏按钮
        buttonComp.interactable = false; // 禁用按钮

        StartCoroutine(ExecuteCommands(commands));
    }

    // 执行指令字符串（如"LMLMLMLMM"）
    public IEnumerator ExecuteCommands(string commands)
    {
        foreach (char cmd in commands)
        {
            switch (cmd)
            {
                case 'L': TurnLeft(); break;
                case 'R': TurnRight(); break;
                case 'M': Move(); break;
                default: Debug.LogWarning("未知指令：" + cmd); break;
            }
            // 等待指令执行完成（移动/旋转动画结束）
            yield return new WaitForSeconds(Mathf.Max(moveDuration, rotateDuration));
        }
        currentDirection=currentRotationAngle switch
        {
            0 => Direction.N,
            90 => Direction.W,
            180 => Direction.S,
            270 => Direction.E,
            _ => currentDirection
        };

        //-backgroundRectTrans.rect.width / 2 + gridSize / 2;
        // -backgroundRectTrans.rect.height / 2 + gridSize / 2;
        float zeroX = -backgroundRectTrans.rect.width / 2 + gridSize / 2;
        float zeroY = -backgroundRectTrans.rect.height / 2 + gridSize / 2;

        float lastX = (currentX - zeroX) / gridSize;
        float lastY = (currentY - zeroY) / gridSize;
        Debug.Log("指令执行完成！最终状态：" + lastX + "," + lastY + "," + currentDirection);
    }

    // 外部接口：重置火星车到初始状态
    public void ResetRover()
    {
        currentX = initialX;
        currentY = initialY;
        currentDirection = initialDirection;
        UpdateRoverPosition();
        UpdateRoverRotation();
    }
}