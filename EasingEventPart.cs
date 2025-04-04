using System;
using System.Collections.Generic;
using System.Linq;

namespace Easing;

/// <summary>
/// 一个缓动动画的事件片段，用于在指定时间范围内根据缓动函数计算属性值的变化
/// </summary>
public class EasingEventPart
{
    /// <summary>
    /// 事件片段的起始时间
    /// </summary>
    public float TimeStart { get; set; }

    /// <summary>
    /// 事件片段的结束时间
    /// </summary>
    public float TimeEnd { get; set; }

    /// <summary>
    /// 属性在起始时间的值
    /// </summary>
    public float ValueStart { get; set; }

    /// <summary>
    /// 属性在结束时间的值
    /// </summary>
    public float ValueEnd { get; set; }

    /// <summary>
    /// 使用的缓动函数类型
    /// </summary>
    public EaseType EaseType { get; set; }

    /// <summary>
    /// 一个缓动动画的事件片段，用于在指定时间范围内根据缓动函数计算属性值的变化
    /// </summary>
    /// <exception cref="ArgumentException">当 timeEnd 小于等于 timeStart 时抛出</exception>
    public EasingEventPart(float timeStart, float timeEnd, float valueStart, float valueEnd, EaseType easeType)
    {
        if (timeEnd <= timeStart)
            throw new ArgumentException("End time must be greater than start time");

        TimeStart = timeStart;
        TimeEnd = timeEnd;
        ValueStart = valueStart;
        ValueEnd = valueEnd;
        EaseType = easeType;
    }

    /// <summary>
    /// 在指定时间点计算当前属性值
    /// </summary>
    public float GetValue(float t)
    {
        float timeRatio = (t - TimeStart) / (TimeEnd - TimeStart);
        float easedValue = Ease.GetEase(EaseType, timeRatio);
        return ValueStart + (ValueEnd - ValueStart) * easedValue;
    }

    /// <summary>
    /// 将当前缓动事件在指定时间点分割为两个连续的事件片段
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">time 不在 (TimeStart, TimeEnd) 区间内时抛出</exception>
    public virtual EasingEventPart[] Divid(float time)
    {
        if (time <= TimeStart || time >= TimeEnd)
            throw new ArgumentOutOfRangeException(nameof(time), "Split time must be between TimeStart and TimeEnd.");

        float midValue = GetValue(time);

        return [
            new EasingEventPart(TimeStart, time, ValueStart, midValue, EaseType),
            new EasingEventPart(time, TimeEnd, midValue, ValueEnd, EaseType)
        ];
    }
}

/// <summary>
/// 扩展的缓动事件片段，支持基于速度的位移计算和预计算存储优化
/// </summary>
public class SpeedEventPart : EasingEventPart
{
    /// <summary>
    /// 计算位移时使用的时间步长
    /// </summary>
    public static float DeltaTime { get; set; } = 0.01f;

    /// <summary>
    /// 预计算位移的存储块长度
    /// </summary>
    public static float StoredLength { get; set; } = 0.1f;

    /// <summary>
    /// 存储预计算的位移块数据
    /// </summary>
    private List<float> StoredDisplacements;

    /// <summary>
    /// 扩展的缓动事件片段，支持基于速度的位移计算和预计算存储优化
    /// </summary>
    public SpeedEventPart(float timeStart, float timeEnd, float valueStart, float valueEnd, EaseType easeType)
    : base(timeStart, timeEnd, valueStart, valueEnd, easeType)
        => InitStoredDisplacements();

    /// <summary>
    /// 计算指定时间区间内的位移
    /// </summary>
    /// <exception cref="InvalidOperationException">当前事件持续时间无效（timeStart > timeEnd）时抛出 </exception>
    /// <exception cref="ArgumentOutOfRangeException">timeStart 或 timeEnd 不在 [TimeStart, TimeEnd] 区间内时抛出</exception>
    public float CalculateDisplacement(float timeStart, float timeEnd)
    {
        if (timeStart > timeEnd)
            throw new InvalidOperationException("Invalid event timing. TimeStart must precede TimeEnd.");

        if (timeStart < TimeStart || timeStart > TimeEnd)
            throw new ArgumentOutOfRangeException(nameof(timeStart), "Start time must be between TimeStart and TimeEnd.");

        if (timeEnd < TimeStart || timeEnd > TimeEnd)
            throw new ArgumentOutOfRangeException(nameof(timeEnd), "End time must be between TimeStart and TimeEnd.");

        float displacement = 0;
        float time = timeStart;

        // 使用固定时间步长进行积分
        while (time + DeltaTime < timeEnd)
        {
            displacement += GetValue(time) * DeltaTime;
            time += DeltaTime;
        }
        // 计算剩余时间的位移
        displacement += GetValue(time) * (timeEnd - time);

        return displacement;
    }

    /// <summary>
    /// 更新预计算的位移存储块
    /// </summary>
    public void InitStoredDisplacements()
    {
        float time = TimeStart;
        StoredDisplacements = [];

        if (EaseType != EaseType.None && EaseType != EaseType.Linear)
        {
            // 按存储块长度预计算位移
            while (time + StoredLength < TimeEnd)
            {
                StoredDisplacements.Add(CalculateDisplacement(time, time + StoredLength));
                time += StoredLength;
            }
            // 添加最后一个不完整的块
            StoredDisplacements.Add(CalculateDisplacement(time, TimeEnd));
        }
    }

    /// <summary>
    /// 获取指定时间区间内的位移（结合预计算数据和实时计算）
    /// </summary>
    public float GetDisplacement(float timeStart, float timeEnd)
    {
        // 参数有效性检查
        if (timeStart >= timeEnd) return 0;
        timeStart = Math.Clamp(timeStart, TimeStart, TimeEnd);
        timeEnd = Math.Clamp(timeEnd, TimeStart, TimeEnd);

        int i = GetStoredDisplacementIndex(timeStart);
        int j = GetStoredDisplacementIndex(timeEnd);

        if (i == j)
            return CalculateDisplacement(timeStart, timeEnd);

        if (EaseType == EaseType.None)
            return ValueStart * (timeEnd - TimeStart);
        else if (EaseType == EaseType.Linear)
        {
            float vStart = GetValue(timeStart);
            float vEnd = GetValue(timeEnd);
            return (vStart + vEnd) / 2 * (timeEnd - timeStart);
        }
        else
        {
            float displacement = 0;

            // 处理起始部分块
            displacement += GetDisplacementPartRight(timeStart);

            // 累加完整块
            for (int k = i + 1; k < j; k++)
                displacement += StoredDisplacements[k];

            // 处理结束部分块
            displacement += GetDisplacementPartLeft(timeEnd);

            return displacement;
        }
    }

    public float GetTotalDisplacement()
        => StoredDisplacements.Sum();

    /// <summary>
    /// 获取指定时间点对应的存储块索引
    /// </summary>
    private int GetStoredDisplacementIndex(float time)
        => (int)((time - TimeStart) / StoredLength);

    /// <summary>
    /// 计算从指定时间到所在存储块结束的位移
    /// </summary>
    private float GetDisplacementPartLeft(float time)
    {
        int index = GetStoredDisplacementIndex(time);
        float start = TimeStart + index * StoredLength;
        return CalculateDisplacement(start, time);
    }

    /// <summary>
    /// 计算从指定时间到所在存储块结束的位移
    /// </summary>
    private float GetDisplacementPartRight(float time)
    {
        int index = GetStoredDisplacementIndex(time);
        float end = Math.Min(TimeStart + (index + 1) * StoredLength, TimeEnd);
        return CalculateDisplacement(time, end);
    }

    /// <summary>
    /// 将当前缓动事件在指定时间点分割为两个连续的事件片段
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">time 不在 (TimeStart, TimeEnd) 区间内时抛出</exception>
    public override SpeedEventPart[] Divid(float time)
    {
        if (time <= TimeStart || time >= TimeEnd)
            throw new ArgumentOutOfRangeException(nameof(time), "Split time must be between TimeStart and TimeEnd.");

        float midValue = GetValue(time);

        SpeedEventPart[] DividedParts = [
            new SpeedEventPart(TimeStart, time, ValueStart, midValue, EaseType),
            new SpeedEventPart(time, TimeEnd, midValue, ValueEnd, EaseType)
        ];

        DividedParts[0].InitStoredDisplacements();
        DividedParts[1].InitStoredDisplacements();

        return DividedParts;
    }
}
