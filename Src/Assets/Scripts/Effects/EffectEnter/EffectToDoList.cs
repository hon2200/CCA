using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;




public class EffectToDoList : MonoSingleton<EffectToDoList>
{
    private Queue<EffectEvent> _effectQueue = new Queue<EffectEvent>();
    private bool _isPlaying = false;

    #region 队列功能函数
    // 添加特效事件到队列
    public void EnqueueEffect(EffectEvent effectEvent)
    {
        _effectQueue.Enqueue(effectEvent);

        // 如果没有正在播放，开始播放队列
        if (!_isPlaying)
        {
            StartCoroutine(PlayQueue());
        }
    }

    // 批量添加特效事件
    public void EnqueueEffects(List<EffectEvent> effectEvents)
    {
        foreach (var effectEvent in effectEvents)
        {
            _effectQueue.Enqueue(effectEvent);
        }

        if (!_isPlaying && _effectQueue.Count > 0)
        {
            StartCoroutine(PlayQueue());
        }
    }

    // 清空队列
    public void ClearQueue()
    {
        _effectQueue.Clear();
        _isPlaying = false;
        StopAllCoroutines();
    }

    // 获取队列长度
    public int GetQueueCount()
    {
        return _effectQueue.Count;
    }

    #endregion
    // 播放队列中的特效
    private IEnumerator PlayQueue()
    {
        _isPlaying = true;

        while (_effectQueue.Count > 0)
        {
            EffectEvent currentEvent = _effectQueue.Dequeue();

            // 等待延迟
            if (currentEvent.Delay > 0)
            {
                yield return new WaitForSeconds(currentEvent.Delay);
            }

            // 执行特效
            currentEvent.OnStart?.Invoke();

            // 等待特效持续时间
            if (currentEvent.Duration > 0)
            {
                yield return new WaitForSeconds(currentEvent.Duration);
            }
        }

        _isPlaying = false;
    }
}
