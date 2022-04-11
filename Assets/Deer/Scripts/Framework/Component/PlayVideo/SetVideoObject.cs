/* ================================================
 * Introduction：xxx 
 * Creator：XinDu 
 * CreationTime：2022-03-30 10-52-39
 * ChangeCreator：XinDu 
 * ChangeTime：2022-03-30 10-52-39
 * CreateVersion：0.1
 *  =============================================== */
using GameFramework;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Please modify the description.
/// </summary>
[Serializable]
public class SetVideoObject : IReference
{
#if ODIN_INSPECTOR
    [ShowInInspector]
#endif
    private RawImage m_RawImage;
    //private VideoClip m_VideoClip;
    //private RenderTexture m_RenderTexture;
    private string m_VideoFilePath;
    public string VideoFilePath { get { return m_VideoFilePath; }set { m_VideoFilePath = value; } }
    private string m_RTFilePath;
    public string RTFilePath { get { return m_RTFilePath; }set { m_RTFilePath = value; } }
    private GameFrameworkAction<bool, RawImage> m_CompleteCallBack;
    public void SetRawImage(bool success, RenderTexture renderTexture) 
    {
        if (success)
        {
            m_RawImage.texture = renderTexture;
        }
        m_CompleteCallBack?.Invoke(success, m_RawImage);
    }
    public static SetVideoObject Create(RawImage rawImage,string fileName, GameFrameworkAction<bool, RawImage> completeCallBack = null) 
    {
        SetVideoObject setVideoObject = new SetVideoObject();
        setVideoObject.m_RawImage = rawImage;
        setVideoObject.m_VideoFilePath = AssetUtility.Video.GetVideoAsset(fileName);
        setVideoObject.m_RTFilePath = AssetUtility.Video.GetRenderTexterAsset(fileName);
        setVideoObject.m_CompleteCallBack = completeCallBack;
        return setVideoObject;
    }
    public void Clear()
    {
        m_RawImage = null;
        m_VideoFilePath = null;
        m_RTFilePath = null;
        VideoFilePath = null;
        RTFilePath = null;
    }
}