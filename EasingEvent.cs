using System;
using System.Collections.Generic;
using System.Linq;

namespace Easing;

/// <summary>
/// 一个缓动事件序列，管理由多个缓动事件片段组成的动画时间线
/// </summary>
public class EasingEvent
{
    /// <summary>
    /// 事件总时长
    /// </summary>
    public float TimeLength;

    /// <summary>
    /// 事件片段链表
    /// </summary>
    private LinkedList<EasingEventPart> EventParts = [];

    /// <summary>
    /// 一个缓动事件序列，管理由多个缓动事件片段组成的动画时间线
    /// </summary>
    public EasingEvent(float timeLength)
    {
        TimeLength = timeLength;
        InitEvent();
    }

    /// <summary>
    /// 初始化默认事件片段
    /// </summary>
    public void InitEvent()
    {
        EventParts = [];
        EventParts.AddLast(new EasingEventPart(0, TimeLength, 0, 0, EaseType.Linear));
    }

    /// <summary>
    /// 查找包含指定时间点的事件片段
    /// </summary>
    public EasingEventPart FindPart(float time)
        => EventParts.First(part => time > part.TimeStart && time < part.TimeEnd);

    /// <summary>
    /// 在指定时间点分割事件片段
    /// </summary>
    public void Divid(float time)
    {
        EasingEventPart DividingPart = FindPart(time);
        EasingEventPart[] DividedPart = DividingPart.Divid(time);
        LinkedListNode<EasingEventPart> ReplaceNode = EventParts.Find(DividingPart);

        ReplaceNode.Value = DividedPart[0];
        EventParts.AddAfter(ReplaceNode, DividedPart[1]);
    }
}

/// <summary>
/// 带有速度计算的事件序列
/// </summary>
public class SpeedEvent
{
    /// <summary>
    /// 事件总时长
    /// </summary>
    public float TimeLength;

    /// <summary>
    /// 事件片段链表
    /// </summary>
    private LinkedList<SpeedEventPart> EventParts = [];

    /// <summary>
    /// 带有速度计算的事件序列
    /// </summary>
    public SpeedEvent(float timeLength)
    {
        TimeLength = timeLength;
        InitEvent();
    }

    /// <summary>
    /// 初始化默认事件片段
    /// </summary>
    public void InitEvent()
    {
        EventParts = [];
        EventParts.AddLast(new SpeedEventPart(0, TimeLength, 0, 0, EaseType.Linear));
    }

    /// <summary>
    /// 查找包含指定时间点的事件片段
    /// </summary>
    public SpeedEventPart FindPart(float time)
    {
        if (time < 0 || time > TimeLength)
            throw new ArgumentOutOfRangeException(nameof(time), "Time must be between 0 and TimeLength.");
        return EventParts.First(part => time >= part.TimeStart && time <= part.TimeEnd);
    }

    /// <summary>
    /// 在指定时间点分割事件片段
    /// </summary>
    public void Divid(float time)
    {
        SpeedEventPart DividingPart = FindPart(time);
        SpeedEventPart[] DividedPart = DividingPart.Divid(time);
        LinkedListNode<SpeedEventPart> ReplaceNode = EventParts.Find(DividingPart);

        ReplaceNode.Value = DividedPart[0];
        EventParts.AddAfter(ReplaceNode, DividedPart[1]);
    }

    /// <summary>
    /// 计算指定时间区间内的位移
    /// </summary>
    /// <exception cref="InvalidOperationException">当前事件持续时间无效（timeStart > timeEnd）时抛出 </exception>
    /// <exception cref="ArgumentOutOfRangeException">timeStart 或 timeEnd 不在 [TimeStart, TimeEnd] 区间内时抛出</exception>
    public float GetDisplacement(float timeStart, float timeEnd)
    {
        if (timeStart > timeEnd)
            throw new InvalidOperationException("Invalid event timing. TimeStart must precede TimeEnd.");

        if (timeStart < 0 || timeStart > TimeLength)
            throw new ArgumentOutOfRangeException(nameof(timeStart), "Start time must be between TimeStart and TimeEnd.");

        if (timeEnd < 0 || timeEnd > TimeLength)
            throw new ArgumentOutOfRangeException(nameof(timeEnd), "End time must be between TimeStart and TimeEnd.");

        LinkedListNode<SpeedEventPart> partNodeStart = EventParts.Find(FindPart(timeStart));
        LinkedListNode<SpeedEventPart> partNodeEnd = EventParts.Find(FindPart(timeEnd));

        if (partNodeStart == partNodeEnd)
            return partNodeStart.Value.GetDisplacement(timeStart, timeEnd);
        else
        {
            float displacement = 0;
            LinkedListNode<SpeedEventPart> partNode = partNodeStart;

            while (partNode != partNodeEnd)
                displacement += partNode.Value.GetTotalDisplacement();
            displacement += partNodeEnd.Value.GetDisplacement(partNodeEnd.Value.TimeStart, timeEnd);

            return displacement;
        }
    }
}
