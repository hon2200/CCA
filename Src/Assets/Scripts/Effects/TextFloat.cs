using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CustomFont : MonoBehaviour
{
    public TMP_Text tmp;
    public float jitterIntensity = 4f;  // 抖动强度
    public float jitterSpeed = 1f;        // 抖动速度
    public float twistIntensity = 0.25f;  // 扭曲强度
    public float omiga = 0.5f; //波动频率
    public float k = 0.5f; //波动波矢
    public float amplitude = 0.02f; //振幅

    private Vector3[][] originalVertices; // 存储原始顶点
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

            // 获取原始顶点位置（避免累积偏移）
            Vector3[] originalVerts = new Vector3[4];
            for (int j = 0; j < 4; j++)
            {
                originalVerts[j] = originalVertices[charInfo.materialReferenceIndex][vertexIndex + j];
            }

            // 计算扭曲中心（字符中心点）
            Vector3 center = (originalVerts[0] + originalVerts[2]) * 0.5f;

            for (int j = 0; j < 4; j++)
            {
                Vector3 orig = originalVerts[j];
                float timeOffset = Time.time * jitterSpeed + i * 0.2f; // 每个字符不同相位

                // 随机抖动（基于 Perlin 噪声，平滑随机）
                float randomX = Mathf.PerlinNoise(timeOffset, j * 10) * 2f - 1f;
                float randomY = Mathf.PerlinNoise(timeOffset + 5f, j * 10) * 2f - 1f;
                Vector3 jitter = new Vector3(randomX, randomY, 0) * jitterIntensity * 0.01f;

                // 扭曲效果（顶点绕中心旋转）
                Vector3 dir = orig - center;
                float angle = Mathf.PerlinNoise(timeOffset * 0.3f, j * 5) * twistIntensity;
                Quaternion twistRot = Quaternion.Euler(0, 0, angle * 10f);
                Vector3 twistedPos = center + twistRot * dir;

                //波浪效果
                Vector3 wavePos = new(0, Mathf.Sin(Time.time * omiga + orig.x * k) * amplitude, 0);

                // 最终位置 = 原始位置 + 抖动 + 扭曲
                verts[vertexIndex + j] = twistedPos + jitter + wavePos;
            }
        }

        // 更新所有网格
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            tmp.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}