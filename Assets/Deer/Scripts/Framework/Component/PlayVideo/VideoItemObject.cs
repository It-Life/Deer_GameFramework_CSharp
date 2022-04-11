/* ================================================
 * Introduction：xxx 
 * Creator：XinDu 
 * CreationTime：2022-03-29 18-27-57
 * ChangeCreator：XinDu 
 * ChangeTime：2022-03-29 18-27-57
 * CreateVersion：0.1
 *  =============================================== */
using GameFramework;
using GameFramework.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Please modify the description.
/// </summary>
public class VideoItemObject : ObjectBase
{
    private object m_Target1;
    public object Target1 { get { return m_Target1; }set { m_Target1 = value; } }
    public static VideoItemObject Create(string path, object target, object target1)
    {
        VideoItemObject item = ReferencePool.Acquire<VideoItemObject>();
        item.Initialize(path, target);
        item.m_Target1 = target1;
        return item;
    }
    protected override void Release(bool isShutdown)
    {
        m_Target1 = null;
        Target1 = null;
    }
}