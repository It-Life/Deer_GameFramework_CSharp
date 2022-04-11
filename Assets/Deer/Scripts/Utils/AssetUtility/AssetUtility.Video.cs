using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class AssetUtility
{
    /// <summary>
    /// 视频相关相关
    /// </summary>
    public static class Video
    {
        /// <summary>
        /// 获取视频资源路径
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <returns></returns>
        public static string GetVideoAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Deer/Asset/Video/{0}/{0}.mp4", assetName);
        }

        /// <summary>
        /// 获取RenderTexter资源路径
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <returns></returns>
        public static string GetRenderTexterAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Deer/Asset/Video/{0}/{0}.renderTexture", assetName);
        }
    }
}
