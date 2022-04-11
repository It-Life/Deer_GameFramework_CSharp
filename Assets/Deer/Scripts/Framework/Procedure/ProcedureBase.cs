using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deer 
{
    public abstract class ProcedureBase : GameFramework.Procedure.ProcedureBase
    {
        private bool currIsLuaProcedure = false;

        public bool IsLuaProcedure {
            get
            {
                return currIsLuaProcedure;
            }
            set
            {
                currIsLuaProcedure = value;
            }
        }
        // 获取流程是否使用原生对话框
        // 在一些特殊的流程（如游戏逻辑对话框资源更新完成前的流程）中，可以考虑调用原生对话框进行消息提示行为
        public abstract bool UseNativeDialog
        {
            get;

        }

        public void UnloadAllScene() 
        {
            // 卸载所有场景
            string[] loadedSceneAssetNames = GameEntry.Scene.GetLoadedSceneAssetNames();
            for (int i = 0; i < loadedSceneAssetNames.Length; i++)
            {
                GameEntry.Scene.UnloadScene(loadedSceneAssetNames[i]);
            }
        }

        public void UnloadAllEntity() 
        {
            // 隐藏所有实体
            GameEntry.Entity.HideAllLoadingEntities();
            GameEntry.Entity.HideAllLoadedEntities();
        }

    }
}
