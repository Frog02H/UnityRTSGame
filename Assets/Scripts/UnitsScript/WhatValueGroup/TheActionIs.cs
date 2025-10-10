using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheActionIs
{
    /* tai 是 The Action Is 的 缩写， 有利于共享行为判断的数据 */

    // 展示生死状态
    public bool isFaint = false;
    public bool isRespawn = false;
    // 此单位 寻常行为 Action 的 启动、更新、关闭判断
    public bool isIdle = false;
    public bool isMove = false;
    public bool isHit = false;
    public bool isAttack = false;
    public bool isGuarding = false;
    // 此单位 与 其他单位 的 持续性 互动判断
    public bool isVehicle = false;
    // 此单位 寻常行为 Action 的 执行可行性 判断
    public bool isIn = false;
    public bool isLock = false;
    public bool isGuardAttack = false;
    public bool isGather = false;
    // 调试用参数
    public bool isChange = false;
}
