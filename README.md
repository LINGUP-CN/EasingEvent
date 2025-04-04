# EasingEvent - 动画缓动与速度事件处理

![.NET Version](https://img.shields.io/badge/.NET-%3E%3D8.0-blue)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## 项目简介

EasingEvent 是一个动画缓动计算库，提供缓动动画时间线管理方案。适用于游戏开发、UI动画等场景。

## 示例

```csharp
// 创建10秒的缓动事件
var speedEvent = new SpeedEvent(10);

// 在5秒处分割事件
speedEvent.Divid(5f);

// 更改后半段（第7.5秒所在的一段）缓动类型并初始化位移缓存（必须）
var part = speedEvent.FindPart(7.5f);
part.EaseType = EaseType.InSine;
part.InitStoredDisplacements();

// 获取2秒到8秒的位移
Console.WriteLine(speedEvent.GetDisplacement(2, 8));
```
