// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-07-11 23-29-11  
//修改作者 : 杜鑫 
//修改时间 : 2021-07-11 23-29-11  
//版 本 : 0.1 
// ===============================================
using GameFramework;
using GameFramework.ObjectPool;
using GameFramework.Resource;
using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Deer
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Deer/DeerUI")]
    public class DeerUIComponent : GameFrameworkComponent
    {
        private IObjectPool<AssetInstanceObject> m_InstancePool; //UI资源池   
        private LoadAssetCallbacks m_LoadAssetCallbacks; //UI加载回调                    
        private Dictionary<int, OpenUIInfo> m_UIFormsBeingLoaded; //正在加载的UI列表      
        private HashSet<int> m_UIFormsToReleaseOnLoad; //加载完毕要卸载的UI   
        private string m_luaModuleHelperName = "UIManagerHelper";
        private Dictionary<UIFormGroupType, GameObject> m_listUIGroups;
        private Dictionary<UIFormGroupType, Dictionary<int,UIBaseForms>> m_listUIForms;
        private int m_nLoadSerial = 0;

        protected override void Awake()
        {
            base.Awake();
            m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadUIFormSuccessCallback, LoadUIFormFailureCallback, LoadUIFormUpdateCallback, LoadUIFormDependencyAssetCallback);
            m_UIFormsBeingLoaded = new Dictionary<int, OpenUIInfo>();
            m_UIFormsToReleaseOnLoad = new HashSet<int>();
            m_listUIGroups = new Dictionary<UIFormGroupType, GameObject>();
            m_listUIForms = new Dictionary<UIFormGroupType, Dictionary<int, UIBaseForms>>();

        }

        private void Start()
        {
        }

        private IObjectPool<AssetInstanceObject> GetInstancePool() 
        {
            if (m_InstancePool == null)
            {
                m_InstancePool = GameEntry.ObjectPool.CreateMultiSpawnObjectPool<AssetInstanceObject>("UI Asset Pool", 10, 16, 2, 0);
            }
            return m_InstancePool;
        }

        protected void OnDestroy()
        {
        }

        protected void OnApplicationQuit()
        {

        }

        public void CreateForm(string strUIName) 
        {
            InstantiatedUI(strUIName);
        }

        public void CloseForm(int nLoadSerial) 
        {
            foreach (var item in m_listUIForms)
            {
                UIBaseForms uiPanel;
                if (item.Value.TryGetValue(nLoadSerial, out uiPanel))
                {
                    uiPanel.OnClose();
                    Destroy(uiPanel.gameObject);
                    item.Value.Remove(nLoadSerial);
                }
            }
        }

        private void InstantiatedUI(string strUIName) 
        {
            string[] arr = strUIName.Split('|');
            string _strUIPrefabPath = AssetUtility.UI.GetUIPrefabPath(arr[1]);
            m_nLoadSerial += 1;
            LoadAssetAsync(m_nLoadSerial, _strUIPrefabPath, arr[0]);
        }

        public void LoadAssetAsync(int nLoadSerial, string strUIPath, string strShowName)
        {
            AssetInstanceObject uiFormAsset = GetInstancePool().Spawn(strUIPath);
            if (uiFormAsset == null)
            {
                OpenUIInfo openUIInfo = OpenUIInfo.Create(nLoadSerial, strUIPath, strShowName);
                m_UIFormsBeingLoaded.Add(nLoadSerial, openUIInfo);
                GameEntry.Resource.LoadAsset(strUIPath, typeof(GameObject), Constant.AssetPriority.UIFormAsset, m_LoadAssetCallbacks, openUIInfo);
            }
            else
            {
                //ProcessAfterFinishLoadAssetSuccess((GameObject)uiFormAsset.Target, nLoadSerial);
                //CallFunction("LoadUIFormSuccessCallback", (GameObject)uiFormAsset.Target, nLoadSerial);
                CreateObject((GameObject)uiFormAsset.Target, nLoadSerial);
            }
        }

        /// <summary>
        /// 回收资源(不要调用 只用于UIBase destroy的时候)
        /// </summary>
        /// <param name="asset"></param>
        public void Unspwn(object asset)
        {
            if (m_InstancePool == null)
            {
                Log.Error("UIComponent Unspwn m_InstancePool null");
                return;
            }
            m_InstancePool.Unspawn(asset);
        }

        /// <summary>
        /// 是否正在加载界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>是否正在加载界面</returns>
        public bool IsLoadingUIForm(string uiname)
        {
            if (string.IsNullOrEmpty(uiname))
            {
                throw new GameFrameworkException("UI name is invalid.");
            }

            foreach (KeyValuePair<int, OpenUIInfo> uiFormBeingLoaded in m_UIFormsBeingLoaded)
            {
                if (uiFormBeingLoaded.Value.ShowUIName.Equals(uiname))
                {
                    return true;
                }
            }
            return false;
        }

        private void LoadUIFormDependencyAssetCallback(string assetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            OpenUIInfo openUIInfo = (OpenUIInfo)userData;
            if (openUIInfo == null)
            {
                Log.Error("Open UI info is invalid.");
            }
        }

        private void LoadUIFormUpdateCallback(string assetName, float progress, object userData)
        {
            OpenUIInfo openUIInfo = (OpenUIInfo)userData;
            if (openUIInfo == null)
            {
                Log.Error("Open UI info is invalid.");
            }
        }

        private void LoadUIFormFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            OpenUIInfo openUIInfo = (OpenUIInfo)userData;
            if (openUIInfo == null)
            {
                throw new GameFrameworkException("Open UI info is invalid.");
            }

            if (m_UIFormsToReleaseOnLoad.Contains(openUIInfo.SerialId))
            {
                m_UIFormsToReleaseOnLoad.Remove(openUIInfo.SerialId);
                ReferencePool.Release(openUIInfo);
                return;
            }

            m_UIFormsBeingLoaded.Remove(openUIInfo.SerialId);

            string appendErrorMessage = Utility.Text.Format("Load UI failure, asset name '{0}', status '{1}' , error message '{2}'.", assetName, status.ToString(), errorMessage);

            //ProcessAfterFinishLoadAssetFailed(openUIFormInfo.SerialId);
            //CallFunction("LoadUIFormFailureCallback", openUIInfo.SerialId);

            ReferencePool.Release(openUIInfo);
            Log.Error(appendErrorMessage);
        }

        private void LoadUIFormSuccessCallback(string assetName, object asset, float duration, object userData)
        {
            OpenUIInfo openUIInfo = (OpenUIInfo)userData;
            if (openUIInfo == null)
            {
                throw new Exception("Open UI info is invalid.");
            }
            m_UIFormsBeingLoaded.Remove(openUIInfo.SerialId);
            if (m_UIFormsToReleaseOnLoad.Contains(openUIInfo.SerialId))
            {
                m_UIFormsToReleaseOnLoad.Remove(openUIInfo.SerialId);
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
            //CallFunction("LoadUIFormSuccessCallback", (GameObject)assetObject.Target, openUIInfo.SerialId);
            CreateObject((GameObject)assetObject.Target, openUIInfo.SerialId);
            ReferencePool.Release(openUIInfo);
        }



        private void CreateObject(GameObject goPrefab,int nLoadSerial) 
        {
            GameObject transDeerGFRoot = GameObject.Find("DeerGF");
            if (transDeerGFRoot == null)
                return;
            Transform uiNode = transDeerGFRoot.transform.Find("Customs/UI/UIRoot");
            if (uiNode == null)
                return;
            UIFormGroupType enumGroup = UIFormGroupType.Group1;
            GameObject transUIGroup = CreateGroup(enumGroup, uiNode);
            Dictionary<int, UIBaseForms> _listFroms;
            if (m_listUIForms.TryGetValue(enumGroup, out _listFroms))
            {

                UIBaseForms uIBaseForms = CreateForms(goPrefab, transUIGroup.transform, _listFroms.Count);
                uIBaseForms.Serialld = nLoadSerial;
                uIBaseForms.UIGroup = enumGroup;
                _listFroms.Add(nLoadSerial, uIBaseForms);
            }
            else
            {
                _listFroms = new Dictionary<int, UIBaseForms>();
                UIBaseForms uIBaseForms = CreateForms(goPrefab, transUIGroup.transform, _listFroms.Count);
                uIBaseForms.Serialld = nLoadSerial;
                uIBaseForms.UIGroup = enumGroup;
                _listFroms.Add(nLoadSerial, uIBaseForms);
                m_listUIForms.Add(enumGroup, _listFroms);
            }
            return;
        }

        private UIBaseForms CreateForms(GameObject goPrefab,Transform transUIGroup,int count) 
        {
            GameObject gameObject = Instantiate(goPrefab, transUIGroup);
            UIBaseForms uIBaseForms = gameObject.GetComponent<UIBaseForms>();
            if (uIBaseForms != null)
            {
                uIBaseForms.OnInit();
            }
            Canvas canvas = uIBaseForms.gameObject.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = count;
            }
            uIBaseForms.gameObject.GetOrAddComponent<GraphicRaycaster>();
            return uIBaseForms;
        }

        private GameObject CreateGroup(UIFormGroupType enumGroup,Transform uiNode) 
        {
            GameObject goGroup;
            if (m_listUIGroups.TryGetValue(enumGroup, out goGroup))
            {
                return goGroup;
            }
            else 
            {
                string strGroupName = "Group"+ enumGroup;
                goGroup = new GameObject(strGroupName);
                goGroup.transform.SetParent(uiNode);
                goGroup.transform.localPosition = new Vector3(0, 0, 0);
                Canvas canvasComp = goGroup.GetOrAddComponent<Canvas>();
                canvasComp.overrideSorting = true;
                canvasComp.sortingOrder = (int)enumGroup * 100;
                goGroup.GetOrAddComponent<GraphicRaycaster>();
                RectTransform rect = goGroup.GetComponent<RectTransform>();
                rect.localPosition = Vector3.zero;
                rect.localScale = Vector3.one;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                m_listUIGroups.Add(enumGroup, goGroup);
            }
            return goGroup;
        }

/*        private void CallFunction(string func, int serialId)
        {
            GameEntry.Lua.CallFunction(m_luaModuleHelperName + "." + func, serialId);
        }
        private void CallFunction(string func,GameObject gameObject,int serialId) 
        {
            GameEntry.Lua.CallFunction(m_luaModuleHelperName + "." + func, gameObject, serialId);
        }*/
    }

   

}