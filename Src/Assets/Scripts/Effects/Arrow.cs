using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;


//ѡ��Ŀ��ļ�ͷ
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
    //�ṩ��㣬Ŀ��Ϊ�������Ļ�ϵ�λ�ã���ʼ����ͷ��z��������-1
    public void FromOriToMouse(Transform ori)
    {
        Vector3 origin = new(ori.position.x, ori.position.y, ori.position.z - 1);
        // ��ȡ�������Ļ�ϵ�λ��
        Vector3 mousePos = Input.mousePosition;
        // ���������Ļ�ϵ�λ��ת��Ϊ����ռ��е�λ��
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
        // ��ȡSprite��ǰλ��
        Vector2 currentPosition = watcher.position;

        // ���㷽������
        Vector2 direction = targetPosition - currentPosition;

        // ʹ��Atan2����Ƕȣ����ȣ���Ȼ��ת��Ϊ��
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        angle -= 90;

        // Ӧ����ת
        watcher.rotation = Quaternion.Euler(0, 0, angle);
    }
}
