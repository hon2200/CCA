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

    private void Awake()
    {
        SetActive(startsActive);
    }
    //提供起点，目标为鼠标在屏幕上的位置，初始化箭头。z方向会调整-1
    public void FromOriToMouse(Transform ori)
    {
        Vector3 origin = new(ori.position.x, ori.position.y, ori.position.z - 1);
        // 获取鼠标在屏幕上的位置
        Vector3 mousePos = Input.mousePosition;
        // 将鼠标在屏幕上的位置转换为世界空间中的位置
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new(mousePos.x, mousePos.y, Camera.main.transform.position.z));
        Vector3 target = new(worldPos.x, worldPos.y, ori.position.z - 1);
        SetActiveAndSetUp(origin, target);
    }
    public void FromOriToDes(Transform ori,Transform des)
    {
        SetActive(true);
        SetUp(ori.position, des.position);
    }
    public void SetActiveAndSetUp(Vector3 origin, Vector3 target)
    {
        SetActive(true);
        SetUp(origin, target);
    }
    public void DeActive()
    {
        SetActive(false);
    }


    private void SetUp(Vector3 origin, Vector3 target)
    {
        arrowBase.transform.position = origin;
        int count = circles.Count;
        float length = (target - origin).magnitude;
        for (int i = 0; i < count; i++)
        {
            Vector3 position =new Vector3(0, 1, 0) * length / count * (i);
            circles[i].transform.localPosition = position;
        }
        Vector3 position_ = new Vector3(0, 1, 0) * length;
        end.transform.localPosition = position_;
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
}
