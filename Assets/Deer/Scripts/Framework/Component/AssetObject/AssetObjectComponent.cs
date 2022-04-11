// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2022-03-09 19-28-53  
//修改作者 : 杜鑫 
//修改时间 : 2022-03-09 19-28-53  
//版 本 : 0.1 
// ===============================================
using GameFramework;
using GameFramework.ObjectPool;
using GameFramework.Resource;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Deer
{
    public class AssetObjectComponent : GameFrameworkComponent
    {
        private LoadAssetCallbacks m_LoadAssetCallbacks; //AssetObject加载回调
        private Dictionary<int, AssetObjectInfo> m_AssetObjectBeingLoaded; //正在加载的AssetObject列表      
        private HashSet<int> m_AssetObjectToReleaseOnLoad; //加载完毕要卸载的AssetObject  
        private string m_luaModuleHelperName = "AssetObjectManagerHelper";
        private IObjectPool<AssetInstanceObject> m_InstancePool; //AssetObject资源池   

        protected override void Awake()
        {
            base.Awake();
            m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadAssetObjectSuccessCallback, LoadAssetObjectFailureCallback, LoadAssetObjectUpdateCallback, LoadAssetObjectDependencyAssetCallback);
        }

        private void Start()
        {
            m_InstancePool = GameEntry.ObjectPool.CreateMultiSpawnObjectPool<AssetInstanceObject>("Asset Object Pool", 10, 16, 2, 0);
        }
        public void LoadAssetAsync(int nLoadSerial, string strPath, string strShowName)
        {
            AssetInstanceObject assetObject = m_InstancePool.Spawn(strPath);
            if (assetObject == null)
            {
                AssetObjectInfo assetObjectInfo = AssetObjectInfo.Create(nLoadSerial, strPath, strShowName);
                m_AssetObjectBeingLoaded.Add(nLoadSerial, assetObjectInfo);
                GameEntry.Resource.LoadAsset(strPath, typeof(GameObject), Constant.AssetPriority.SceneUnit, m_LoadAssetCallbacks, assetObjectInfo);
            }
            else
            {
                //CallFunction("LoadAssetObjectSuccessCallback", (GameObject)assetObject.Target, nLoadSerial);
            }
        }
        /// <summary>
        /// 回收资源(不要调用 只用于AssetObjectBase destroy的时候)
        /// </summary>
        /// <param name="asset"></param>
        public void Unspwn(object asset)
        {
            if (m_InstancePool == null)
            {
                Log.Error("AssetObjectComponent Unspwn m_InstancePool null");
                return;
            }
            m_InstancePool.Unspawn(asset);
        }
        /// <summary>
        /// 是否正在加载界面
        /// </summary>
        /// <param name="assetObjectName">资源名称</param>
        /// <returns>是否正在加载界面</returns>
        public bool IsLoadingAssetObject(string assetObjectName)
        {
            if (string.IsNullOrEmpty(assetObjectName))
            {
                throw new GameFrameworkException("AssetObject name is invalid.");
            }

            foreach (KeyValuePair<int, AssetObjectInfo> assetObjectBeingLoaded in m_AssetObjectBeingLoaded)
            {
                if (assetObjectBeingLoaded.Value.ShowAssetObjectName.Equals(assetObjectName))
                {
                    return true;
                }
            }
            return false;
        }
        private void LoadAssetObjectDependencyAssetCallback(string assetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            AssetObjectInfo assetObjectInfo = (AssetObjectInfo)userData;
            if (assetObjectInfo == null)
            {
                Log.Error("Open AssetObject info is invalid.");
            }
        }

        private void LoadAssetObjectUpdateCallback(string assetName, float progress, object userData)
        {
            AssetObjectInfo assetObjectInfo = (AssetObjectInfo)userData;
            if (assetObjectInfo == null)
            {
                Log.Error("Open AssetObject info is invalid.");
            }
        }

        private void LoadAssetObjectFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            AssetObjectInfo assetObjectInfo = (AssetObjectInfo)userData;
            if (assetObjectInfo == null)
            {
                throw new GameFrameworkException("Open AssetObject info is invalid.");
            }

            if (m_AssetObjectToReleaseOnLoad.Contains(assetObjectInfo.SerialId))
            {
                m_AssetObjectToReleaseOnLoad.Remove(assetObjectInfo.SerialId);
                ReferencePool.Release(assetObjectInfo);
                return;
            }

            m_AssetObjectBeingLoaded.Remove(assetObjectInfo.SerialId);

            string appendErrorMessage = Utility.Text.Format("Load assetObject failure, asset name '{0}', status '{1}' , error message '{2}'.", assetName, status.ToString(), errorMessage);

            //CallFunction("LoadAssetObjectFailureCallback", assetObjectInfo.SerialId);

            ReferencePool.Release(assetObjectInfo);
            Log.Error(appendErrorMessage);
        }

        private void LoadAssetObjectSuccessCallback(string assetName, object asset, float duration, object userData)
        {
            AssetObjectInfo assetObjectInfo = (AssetObjectInfo)userData;
            if (assetObjectInfo == null)
            {
                throw new Exception("Open AssetObject info is invalid.");
            }
            m_AssetObjectBeingLoaded.Remove(assetObjectInfo.SerialId);
            if (m_AssetObjectToReleaseOnLoad.Contains(assetObjectInfo.SerialId))
            {
                m_AssetObjectToReleaseOnLoad.Remove(assetObjectInfo.SerialId);
                GameEntry.Resource.UnloadAsset(asset);
                return;
            }
            AssetInstanceObject assetObject = m_InstancePool.Spawn(assetName);
            if (assetObject == null)
            {
                assetObject = AssetInstanceObject.Create(assetName, asset);
                m_InstancePool.Register(assetObject, true);
            }
            else
            {
                GameEntry.Resource.UnloadAsset(asset);
            }
            //CallFunction("LoadAssetObjectSuccessCallback", (GameObject)assetObject.Target, assetObjectInfo.SerialId);
            ReferencePool.Release(assetObjectInfo);

        }
/*        private void CallFunction(string func, int serialId)
        {
            GameEntry.Lua.CallFunction(m_luaModuleHelperName + "." + func, serialId);
        }
        private void CallFunction(string func, GameObject gameObject, int serialId)
        {
            GameEntry.Lua.CallFunction(m_luaModuleHelperName + "." + func, gameObject, serialId);
        }*/
    }
}