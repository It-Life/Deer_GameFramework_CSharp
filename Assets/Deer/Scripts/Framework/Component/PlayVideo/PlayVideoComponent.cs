/* ================================================
 * Introduction：视频播放组件 
 * Creator：XinDu 
 * CreationTime：2022-03-29 18-21-57
 * ChangeCreator：XinDu 
 * ChangeTime：2022-03-29 18-21-57
 * CreateVersion：0.1
 *  =============================================== */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameFramework;
using GameFramework.Event;
using GameFramework.FileSystem;
using GameFramework.ObjectPool;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.Video;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

/// <summary>
/// 视频播放组件 
/// TODO 扩展多视频同时播放
/// </summary>
[RequireComponent(typeof(VideoPlayer))]
[DisallowMultipleComponent]
public class PlayVideoComponent : GameFrameworkComponent
{
    private VideoPlayer m_VideoPlayer;
    /// <summary>
    /// 对象池自动释放时间间隔
    /// </summary>
    [SerializeField] private float m_AutoReleaseInterval = 60f;
    /// <summary>
    /// 视频对象池
    /// </summary>
    private IObjectPool<VideoItemObject> m_VideoPool;

    private LoadAssetCallbacks m_LoadAssetCallbacks;

    private Dictionary<int,Dictionary<string,object>> m_DicLoadSuccessObject;

    private List<string> m_ListLoading = new List<string>();

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        m_VideoPlayer = GetComponent<VideoPlayer>();
        m_VideoPool = GameEntry.ObjectPool.CreateMultiSpawnObjectPool<VideoItemObject>("VideoPool", m_AutoReleaseInterval, 16, 60, 0);
        m_VideoPlayer.targetTexture = null;
        m_VideoPlayer.clip = null;
        m_LoadAssetCallbacks = new LoadAssetCallbacks(OnLoadAssetSuccess, OnLoadAssetFailure);
        m_DicLoadSuccessObject = new Dictionary<int, Dictionary<string, object>>();
    }
    /// <summary>
    /// 通过资源系统设置图片
    /// </summary>
    /// <param name="setTexture2dObject">需要设置图片的对象</param>
    public void SetVideoByResources(SetVideoObject setVideoObject)
    {
        if (m_VideoPool.CanSpawn(setVideoObject.VideoFilePath))
        {
            VideoItemObject videoItemObject = m_VideoPool.Spawn(setVideoObject.VideoFilePath);
            var video = (VideoClip)videoItemObject.Target;
            var rTexture = (RenderTexture)videoItemObject.Target1;
            if (video != null && rTexture != null)
            {
                SetVideo(true, setVideoObject, video, rTexture);
                return;
            }
            else 
            {
                m_VideoPool.Unspawn(videoItemObject);
            }
        }
        if (m_ListLoading.Contains(setVideoObject.VideoFilePath))
        {
            Log.Info("The current object is being loaded");
            return;
        }
        m_ListLoading.Add(setVideoObject.VideoFilePath);
        Dictionary<string, object> loadSuccess;
        if (m_DicLoadSuccessObject.TryGetValue(setVideoObject.GetHashCode(), out loadSuccess))
        {
            Log.Info("The current object is being loaded");
            return;
        }
        loadSuccess = new Dictionary<string, object>();
        m_DicLoadSuccessObject.Add(setVideoObject.GetHashCode(), loadSuccess);
        GameEntry.Resource.LoadAsset(setVideoObject.VideoFilePath, typeof(VideoClip), m_LoadAssetCallbacks, setVideoObject);
        GameEntry.Resource.LoadAsset(setVideoObject.RTFilePath, typeof(RenderTexture), m_LoadAssetCallbacks, setVideoObject);
    }
    public void SetStopVideo() 
    {
        m_VideoPlayer.Stop();
    }
    private void SetVideo(bool success, SetVideoObject setVideoObject, object videoClip, object renderTexture)
    {
        if (success)
        {
            m_VideoPlayer.clip = (VideoClip)videoClip;
            m_VideoPlayer.targetTexture = (RenderTexture)renderTexture;
            setVideoObject.SetRawImage(success, (RenderTexture)renderTexture);
            m_VideoPlayer.Play();
        }
        else 
        {
            setVideoObject.SetRawImage(success,null);
        }
    }

    private void OnLoadAssetFailure(string assetName, LoadResourceStatus status, string errorMessage, object userData)
    {
        SetVideoObject setVideoObject = userData as SetVideoObject;
        if (m_ListLoading.Contains(setVideoObject.VideoFilePath))
        {
            m_ListLoading.Remove(setVideoObject.VideoFilePath);
        }
        SetVideo(false, setVideoObject, null,null);
        Log.Error("Can not load Video from '{1}' with error message '{2}'.",assetName, errorMessage);
    }

    private void OnLoadAssetSuccess(string assetName, object asset, float duration, object userData)
    {
        VideoClip video = asset as VideoClip;
        RenderTexture renderTexture = asset as RenderTexture;
        SetVideoObject setVideoObject = userData as SetVideoObject;
        if (video != null || renderTexture !=null)
        {
            Dictionary<string, object> loadSuccess;
            if (m_DicLoadSuccessObject.TryGetValue(userData.GetHashCode(), out loadSuccess))
            {
                if (loadSuccess.Count > 2)
                {
                    loadSuccess.Clear();
                    SetVideo(false, setVideoObject, null, null);
                    Log.Error($" Have more object by hashcode: {userData.GetHashCode()}");
                    return;
                }
                else 
                {
                    if (video != null)
                    {
                        loadSuccess.Add(setVideoObject.VideoFilePath, video);
                    }
                    else 
                    {
                        loadSuccess.Add(setVideoObject.RTFilePath, renderTexture);
                    }
                    if (loadSuccess.Count==2)
                    {
                        m_VideoPool.Register(VideoItemObject.Create(setVideoObject.VideoFilePath, loadSuccess[setVideoObject.VideoFilePath], loadSuccess[setVideoObject.RTFilePath]), true);
                        SetVideo(true,setVideoObject, loadSuccess[setVideoObject.VideoFilePath], loadSuccess[setVideoObject.RTFilePath]);
                        if (m_ListLoading.Contains(setVideoObject.VideoFilePath))
                        {
                            m_ListLoading.Remove(setVideoObject.VideoFilePath);
                        }
                    }
                }
            }
            else 
            {
                SetVideo(false, setVideoObject, null,null);
                Log.Error($"Find not object by hashcode: {userData.GetHashCode()}");
            }
        }
        else
        {
            SetVideo(false, setVideoObject, null,null);
            Log.Error($"Load failure asset type is {asset.GetType()}.");
        }
    }
}