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





    [SerializeField] private Vector3 ���������ߵ�;
    [SerializeField] private int ƫ��X;
    [SerializeField] private float �޶�Y;

    private void Awake()
    {
        SetActive(startsActive);
    }
    //�ṩ��㣬Ŀ��Ϊ�������Ļ�ϵ�λ�ã���ʼ����ͷ��z��������-1
    public void FromOriToMouse(Transform ori)
    {
        Vector3 origin = new(ori.position.x, ori.position.y+1.8f, ori.position.z - 1);
        // ��ȡ�������Ļ�ϵ�λ��
        Vector3 mousePos = Input.mousePosition;
        // ���������Ļ�ϵ�λ��ת��Ϊ����ռ��е�λ��
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new(mousePos.x, mousePos.y, Camera.main.transform.position.z));
        Vector3 target = new(worldPos.x, worldPos.y, ori.position.z - 1);
        SetActiveAndSetUp(origin, target);
    }
    public void FromOriToDes(Transform ori, Transform des)
    {
        SetActive(true);
        ���������ߵ� = new Vector3((des.position.x + ori.position.x)/2, (des.position.y+ori.position.y)/2, des.position.z - 1);
        SetUp(ori.position, des.position);
    }
    public void SetActiveAndSetUp(Vector3 origin, Vector3 target)
    {
        SetActive(true);
        ���챴������(origin, target);
        SetUp(origin, target);
    }
    public void DeActive()
    {
        SetActive(false);
    }

    private void SetUp(Vector3 origin, Vector3 target)
    {
        arrowBase.transform.position = ���������ߵ�;
        List < Vector3 > points = ���챴��������(origin, target);
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


    private void ���챴������(Vector3 origin, Vector3 target)
    {
        ���������ߵ�=new Vector3(origin .x- (target.x - origin.x) / 7f * ƫ��X, �޶�Y+(target.y - origin.y) / 10f,0);
    }
    private List<Vector3> ���챴��������(Vector3 origin, Vector3 target)
    {
        // �����㼯�б�
        var points = new List<Vector3>();
        //if (ArePointsColinear(origin, ���������ߵ�, target))
        //{
        //    for (int i = 0; i < circles.Count; i++)
        //    {
        //        float t = i / (float)(circles.Count - 1);
        //        Vector3 point = Vector3.Lerp(origin, target, t);
        //        points.Add(point);
        //    }
        //}
        //else
        //{// ���ױ��������߹�ʽ��B(t) = (1-t)^2*P0 + 2(1-t)t*P1 + t^2*P2
            points.Add(origin);
            for (int i = 1; i < circles.Count; i++)
            {
                float t = i / (float)(circles.Count - 1);
                float oneMinusT = 1 - t;
                Vector3 point =
                    (oneMinusT * oneMinusT) * origin +
                    (2 * oneMinusT * t) * ���������ߵ� +
                    (t * t) * target;

                points.Add(point);
            }
            // Ԥ���㲢����յ�ȷ������
            points.Add(target);
        //} 
        return points;
    }


    private bool ArePointsColinear(Vector3 a, Vector3 b, Vector3 c)
    {
        // ��������AB��AC
        Vector3 ab = b - a;
        Vector3 ac = c - a;

        // ������������ӽ������������ߣ�
        Vector3 cross = Vector3.Cross(ab, ac);
        return cross.sqrMagnitude < 0.01f;
    }
}
