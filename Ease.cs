using System;

namespace Easing;

/// <summary>
/// 定义所有支持的缓动函数类型
/// </summary>
public enum EaseType
{
    None, Linear,
    InSine, OutSine, InOutSine,
    InQuad, OutQuad, InOutQuad,
    InCubic, OutCubic, InOutCubic,
    InQuart, OutQuart, InOutQuart,
    InQuint, OutQuint, InOutQuint,
    InExpo, OutExpo, InOutExpo,
    InCirc, OutCirc, InOutCirc,
    InBack, OutBack, InOutBack,
    InElastic, OutElastic, InOutElastic,
    InBounce, OutBounce, InOutBounce
}

/// <summary>
/// 缓动函数计算器，提供动画插值曲线计算方法
/// </summary>
public static class Ease
{
    private const float C1 = 1.70158f;
    private const float C2 = C1 * 1.525f;
    private const float C3 = C1 + 1;
    private const float C4 = (2 * MathF.PI) / 3;
    private const float C5 = (2 * MathF.PI) / 4.5f;
    private const float N1 = 7.5625f;
    private const float D1 = 2.75f;

    /// <summary>
    /// 根据缓动类型获取对应的插值计算结果
    /// </summary>
    public static float GetEase(EaseType easeType, float x)
        => easeType switch
        {
            EaseType.Linear => x,
            EaseType.InSine => InSine(x),
            EaseType.OutSine => OutSine(x),
            EaseType.InOutSine => InOutSine(x),
            EaseType.InQuad => InQuad(x),
            EaseType.OutQuad => OutQuad(x),
            EaseType.InOutQuad => InOutQuad(x),
            EaseType.InCubic => InCubic(x),
            EaseType.OutCubic => OutCubic(x),
            EaseType.InOutCubic => InOutCubic(x),
            EaseType.InQuart => InQuart(x),
            EaseType.OutQuart => OutQuart(x),
            EaseType.InOutQuart => InOutQuart(x),
            EaseType.InQuint => InQuint(x),
            EaseType.OutQuint => OutQuint(x),
            EaseType.InOutQuint => InOutQuint(x),
            EaseType.InExpo => InExpo(x),
            EaseType.OutExpo => OutExpo(x),
            EaseType.InOutExpo => InOutExpo(x),
            EaseType.InCirc => InCirc(x),
            EaseType.OutCirc => OutCirc(x),
            EaseType.InOutCirc => InOutCirc(x),
            EaseType.InBack => InBack(x),
            EaseType.OutBack => OutBack(x),
            EaseType.InOutBack => InOutBack(x),
            EaseType.InElastic => InElastic(x),
            EaseType.OutElastic => OutElastic(x),
            EaseType.InOutElastic => InOutElastic(x),
            EaseType.InBounce => InBounce(x),
            EaseType.OutBounce => OutBounce(x),
            EaseType.InOutBounce => InOutBounce(x),
            _ => 0
        };

    public static float InSine(float x)
        => 1 - MathF.Cos(x * MathF.PI / 2);

    public static float OutSine(float x)
        => MathF.Sin(x * MathF.PI / 2);

    public static float InOutSine(float x)
        => -(MathF.Cos(MathF.PI * x) - 1) / 2;

    public static float InQuad(float x)
        => x * x;

    public static float OutQuad(float x)
        => 1 - (1 - x) * (1 - x);

    public static float InOutQuad(float x)
        => x < 0.5 ? 2 * x * x : 1 - MathF.Pow(-2 * x + 2, 2) / 2;

    public static float InCubic(float x)
        => x * x * x;

    public static float OutCubic(float x)
        => 1 - MathF.Pow(1 - x, 3);

    public static float InOutCubic(float x)
        => x < 0.5 ? 4 * x * x * x : 1 - MathF.Pow(-2 * x + 2, 3) / 2;

    public static float InQuart(float x)
        => x * x * x * x;

    public static float OutQuart(float x)
        => 1 - MathF.Pow(1 - x, 4);

    public static float InOutQuart(float x)
        => x < 0.5 ? 8 * x * x * x * x : 1 - MathF.Pow(-2 * x + 2, 4) / 2;

    public static float InQuint(float x)
        => x * x * x * x * x;

    public static float OutQuint(float x)
        => 1 - MathF.Pow(1 - x, 5);

    public static float InOutQuint(float x)
        => x < 0.5 ? 16 * x * x * x * x * x : 1 - MathF.Pow(-2 * x + 2, 5) / 2;

    public static float InExpo(float x)
        => x == 0 ? 0 : MathF.Pow(2, 10 * x - 10);

    public static float OutExpo(float x)
        => x == 1 ? 1 : 1 - MathF.Pow(2, -10 * x);

    public static float InOutExpo(float x)
        => x == 0
            ? 0
            : x == 1
                ? 1
                : x < 0.5
                    ? MathF.Pow(2, 20 * x - 10) / 2
                    : (2 - MathF.Pow(2, -20 * x + 10)) / 2;

    public static float InCirc(float x)
        => 1 - MathF.Sqrt(1 - x * x);

    public static float OutCirc(float x)
        => MathF.Sqrt(1 - MathF.Pow(x - 1, 2));

    public static float InOutCirc(float x)
        => x < 0.5
            ? (1 - MathF.Sqrt(1 - MathF.Pow(2 * x, 2))) / 2
            : (MathF.Sqrt(1 - MathF.Pow(-2 * x + 2, 2)) + 1) / 2;

    public static float InBack(float x)
        => C3 * x * x * x - C1 * x * x;

    public static float OutBack(float x)
        => 1 + C3 * MathF.Pow(x - 1, 3) + C1 * MathF.Pow(x - 1, 2);

    public static float InOutBack(float x)
        => x < 0.5
            ? (MathF.Pow(2 * x, 2) * (C2 + 1) * 2 * x - C2) / 2
            : (MathF.Pow(2 * x - 2, 2) * ((C2 + 1) * (x * 2 - 2) + C2) + 2) / 2;

    public static float InElastic(float x)
        => x switch
        {
            0 => 0,
            1 => 1,
            _ => -MathF.Pow(2, 10 * x - 10) * MathF.Sin((x * 10 - 10.75f) * C4)
        };

    public static float OutElastic(float x)
        => x switch
        {
            0 => 0,
            1 => 1,
            _ => MathF.Pow(2, -10 * x) * MathF.Sin((x * 10 - 0.75f) * C4) + 1
        };

    public static float InOutElastic(float x)
        => x == 0
            ? 0
            : x == 1
                ? 1
                : x < 0.5
                    ? -(MathF.Pow(2, 20 * x - 10) * MathF.Sin((20 * x - 11.125f) * C5)) / 2
                    : (MathF.Pow(2, -20 * x + 10) * MathF.Sin((20 * x - 11.125f) * C5)) / 2 + 1;

    public static float InBounce(float x)
        => 1 - OutBounce(1 - x);

    public static float OutBounce(float x)
    {
        switch (x)
        {
            case < 1 / D1: return N1 * x * x;
            case < 2 / D1: return N1 * (x -= 1.5f / D1) * x + 0.75f;
            default:
                return x < 2.5 / D1
                    ? N1 * (x -= 2.25f / D1) * x + 0.9375f
                    : N1 * (x -= 2.625f / D1) * x + 0.984375f;
        }
    }

    public static float InOutBounce(float x)
        => x < 0.5
            ? (1 - OutBounce(1 - 2 * x)) / 2
            : (1 + OutBounce(2 * x - 1)) / 2;
}
