// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2022-03-15 16-34-05  
//修改作者 : 杜鑫 
//修改时间 : 2022-03-15 16-34-06  
//版 本 : 0.1 
// ===============================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UILoadingPanel : UIBaseForms
{
    private void Start()
    {
        GameEntry.Messenger.RegisterEvent(EventName.EVENT_LOAD_SCENE_SUCCESS, OnHandleLoadSceneSuccess);
    }

    private void OnDestroy()
    {
        GameEntry.Messenger.UnRegisterEvent(EventName.EVENT_LOAD_SCENE_SUCCESS, OnHandleLoadSceneSuccess);
    }

    private object OnHandleLoadSceneSuccess(object pSender)
    {
        Close();
        return null;
    }
}