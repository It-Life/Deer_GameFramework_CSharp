// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2022-03-15 15-02-53  
//修改作者 : 杜鑫 
//修改时间 : 2022-03-15 15-02-53  
//版 本 : 0.1 
// ===============================================
using UnityGameFramework.Runtime;
using GameFramework.Event;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using System;

public class ProcedureChangeScene : Deer.ProcedureBase
{
    private int m_nextProcedureId;
    public override bool UseNativeDialog
    {
        get
        {
            return false;
        }
    }

    private bool m_IsChangeSceneComplete;

    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);

        m_IsChangeSceneComplete = false;

        GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
        GameEntry.Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
        GameEntry.Event.Subscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneUpdate);
        GameEntry.Event.Subscribe(LoadSceneDependencyAssetEventArgs.EventId, OnLoadSceneDependencyAsset);

        m_nextProcedureId = procedureOwner.GetData<VarInt>("nextProcedureId");
        GameEntry.UI.CreateForm(UINameConfig.UILoadingPanel);
        GameEntry.Scene.LoadScene(AssetUtility.Scene.GetSceneAsset("Demo_Town"), Constant.AssetPriority.SceneAsset, this);
    }

    protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
    {
        GameEntry.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
        GameEntry.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
        GameEntry.Event.Unsubscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneUpdate);
        GameEntry.Event.Unsubscribe(LoadSceneDependencyAssetEventArgs.EventId, OnLoadSceneDependencyAsset);

        base.OnLeave(procedureOwner, isShutdown);
    }

    protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

        if (!m_IsChangeSceneComplete)
        {
            return;
        }

        if (m_nextProcedureId == (int)ProcedureConfig.ProcedureMainHall)
        {
            ChangeState<ProcedureMainHall>(procedureOwner);
        }
    }

    private void OnLoadSceneDependencyAsset(object sender, GameEventArgs e)
    {
        Log.Info("OnLoadSceneDependencyAsset");
    }

    private void OnLoadSceneUpdate(object sender, GameEventArgs e)
    {
        Log.Info("OnLoadSceneUpdate");
    }

    private void OnLoadSceneFailure(object sender, GameEventArgs e)
    {
        Log.Info("OnLoadSceneFailure");
    }

    private void OnLoadSceneSuccess(object sender, GameEventArgs e)
    {
        m_IsChangeSceneComplete = true;
        GameEntry.Messenger.SendEvent(EventName.EVENT_LOAD_SCENE_SUCCESS);
    }
}