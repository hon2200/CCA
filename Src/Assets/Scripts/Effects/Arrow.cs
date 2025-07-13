using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;


//选择目标的箭头
public class Arrow : MonoSingleton<Arrow>
{
    public GameObject arrowBase;
    private bool startsActive = false;
    private bool isActive;
    [SerializeField]
    public List<GameObject> circles;
    [SerializeField]
    public GameObject end;





    [SerializeField] private Vector3 贝塞尔曲线点;
    [SerializeField] private int 偏置X;
    [SerializeField] private float 限定Y;

    private void Awake()
    {
        SetActive(startsActive);
    }
    //提供起点，目标为鼠标在屏幕上的位置，初始化箭头。z方向会调整-1
    public void FromOriToMouse(Transform ori)
    {
        Vector3 origin = new(ori.position.x, ori.position.y+1.8f, ori.position.z - 1);
        // 获取鼠标在屏幕上的位置
        Vector3 mousePos = Input.mousePosition;
        // 将鼠标在屏幕上的位置转换为世界空间中的位置
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new(mousePos.x, mousePos.y, Camera.main.transform.position.z));
        Vector3 target = new(worldPos.x, worldPos.y, ori.position.z - 1);
        SetActiveAndSetUp(origin, target);
    }
    public void FromOriToDes(Transform ori, Transform des)
    {
        SetActive(true);
        贝塞尔曲线点 = new Vector3((des.position.x + ori.position.x)/2, (des.position.y+ori.position.y)/2, des.position.z - 1);
        SetUp(ori.position, des.position);
    }
    public void SetActiveAndSetUp(Vector3 origin, Vector3 target)
    {
        SetActive(true);
        构造贝塞尔点(origin, target);
        SetUp(origin, target);
    }
    public void DeActive()
    {
        SetActive(false);
    }

    private void SetUp(Vector3 origin, Vector3 target)
    {
        arrowBase.transform.position = 贝塞尔曲线点;
        List < Vector3 > points = 构造贝塞尔曲线(origin, target);
        Debug.Log("points.Count=" + points.Count);
        for (int i = 0; i < circles.Count; i++)
        {
            Vector3 localPos = arrowBase.transform.InverseTransformPoint(points[i]);
            circles[i].transform.localPosition = localPos;
        }
        end.transform.localPosition = arrowBase.transform.InverseTransformPoint(points[circles.Count-1]);
        PointToTarget(arrowBase.transform, target);
    }
    private void SetActive(bool status)
    {
        isActive = status;
        arrowBase.gameObject.SetActive(status);

    }
    private void PointToTarget(Transform watcher, Vector2 targetPosition)
    {
        // 获取Sprite当前位置
        Vector2 currentPosition = watcher.position;

        // 计算方向向量
        Vector2 direction = targetPosition - currentPosition;

        // 使用Atan2计算角度（弧度），然后转换为度
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        angle -= 90;

        // 应用旋转
        watcher.rotation = Quaternion.Euler(0, 0, angle);
    }


    private void 构造贝塞尔点(Vector3 origin, Vector3 target)
    {
        贝塞尔曲线点=new Vector3(origin .x- (target.x - origin.x) / 7f * 偏置X, 限定Y+(target.y - origin.y) / 10f,0);
    }
    private List<Vector3> 构造贝塞尔曲线(Vector3 origin, Vector3 target)
    {
        // 创建点集列表
        var points = new List<Vector3>();
        //if (ArePointsColinear(origin, 贝塞尔曲线点, target))
        //{
        //    for (int i = 0; i < circles.Count; i++)
        //    {
        //        float t = i / (float)(circles.Count - 1);
        //        Vector3 point = Vector3.Lerp(origin, target, t);
        //        points.Add(point);
        //    }
        //}
        //else
        //{// 二阶贝塞尔曲线公式：B(t) = (1-t)^2*P0 + 2(1-t)t*P1 + t^2*P2
            points.Add(origin);
            for (int i = 1; i < circles.Count; i++)
            {
                float t = i / (float)(circles.Count - 1);
                float oneMinusT = 1 - t;
                Vector3 point =
                    (oneMinusT * oneMinusT) * origin +
                    (2 * oneMinusT * t) * 贝塞尔曲线点 +
                    (t * t) * target;

                points.Add(point);
            }
            // 预计算并添加终点确保精度
            points.Add(target);
        //} 
        return points;
    }


    private bool ArePointsColinear(Vector3 a, Vector3 b, Vector3 c)
    {
        // 计算向量AB和AC
        Vector3 ab = b - a;
        Vector3 ac = c - a;

        // 计算叉积（如果接近零向量，则共线）
        Vector3 cross = Vector3.Cross(ab, ac);
        return cross.sqrMagnitude < 0.01f;
    }
}
