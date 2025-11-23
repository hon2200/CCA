using UnityEngine;
using System.Collections.Generic;
using TMPro;

//����Ķ���
public class CustomFont : MonoBehaviour
{
    public TMP_Text tmp;
    public float jitterIntensity = 4f;  // ����ǿ��
    public float jitterSpeed = 1f;        // �����ٶ�
    public float twistIntensity = 0.25f;  // Ť��ǿ��
    public float omiga = 0.5f; //����Ƶ��
    public float k = 0.5f; //������ʸ
    public float amplitude = 0.02f; //���

    private Vector3[][] originalVertices; // �洢ԭʼ����
    private bool initialized = false;

    void Start()
    {
        InitializeOriginalVertices();
    }

    void InitializeOriginalVertices()
    {
        tmp.ForceMeshUpdate();
        var textInfo = tmp.textInfo;
        originalVertices = new Vector3[textInfo.meshInfo.Length][];

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            originalVertices[i] = (Vector3[])textInfo.meshInfo[i].vertices.Clone();
        }
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        tmp.ForceMeshUpdate();
        var textInfo = tmp.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            int vertexIndex = charInfo.vertexIndex;

            // ��ȡԭʼ����λ�ã������ۻ�ƫ�ƣ�
            Vector3[] originalVerts = new Vector3[4];
            for (int j = 0; j < 4; j++)
            {
                originalVerts[j] = originalVertices[charInfo.materialReferenceIndex][vertexIndex + j];
            }

            // ����Ť�����ģ��ַ����ĵ㣩
            Vector3 center = (originalVerts[0] + originalVerts[2]) * 0.5f;

            for (int j = 0; j < 4; j++)
            {
                Vector3 orig = originalVerts[j];
                float timeOffset = Time.time * jitterSpeed + i * 0.2f; // ÿ���ַ���ͬ��λ

                // ������������� Perlin ������ƽ�������
                float randomX = Mathf.PerlinNoise(timeOffset, j * 10) * 2f - 1f;
                float randomY = Mathf.PerlinNoise(timeOffset + 5f, j * 10) * 2f - 1f;
                Vector3 jitter = new Vector3(randomX, randomY, 0) * jitterIntensity * 0.01f;

                // Ť��Ч����������������ת��
                Vector3 dir = orig - center;
                float angle = Mathf.PerlinNoise(timeOffset * 0.3f, j * 5) * twistIntensity;
                Quaternion twistRot = Quaternion.Euler(0, 0, angle * 10f);
                Vector3 twistedPos = center + twistRot * dir;

                //����Ч��
                Vector3 wavePos = new(0, Mathf.Sin(Time.time * omiga + orig.x * k) * amplitude, 0);

                // ����λ�� = ԭʼλ�� + ���� + Ť��
                verts[vertexIndex + j] = twistedPos + jitter + wavePos;
            }
        }

        // ������������
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            tmp.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}