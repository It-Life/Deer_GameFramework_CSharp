// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2022-03-15 14-21-56  
//修改作者 : 杜鑫 
//修改时间 : 2022-03-15 14-21-56  
//版 本 : 0.1 
// ===============================================
using MetaUser;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

public class ProcedureLogin : Deer.ProcedureBase
{
    private ProcedureOwner m_ProcedureOwner;
    public override bool UseNativeDialog
    {
        get
        {
            return false;
        }
    }
    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);
        m_ProcedureOwner = procedureOwner;
        InitForm.Ins.Close();
        GameEntry.Sound.PlayMusic(Constant.SoundId.LOGIN_BGM, null);
        GameEntry.UI.CreateForm(UINameConfig.UILoginPanel);


        GameEntry.Messenger.RegisterEvent(EventName.EVENT_CS_NET_RECEIVE, RspLogin);
    }
    protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown) 
    {
        base.OnLeave(procedureOwner, isShutdown);
        GameEntry.Messenger.UnRegisterEvent(EventName.EVENT_CS_NET_RECEIVE, RspLogin);
    }

    public void OnChangeProcedure(ProcedureConfig procedureConfig) 
    {
        if (procedureConfig == ProcedureConfig.ProcedureMainHall)
        {
            m_ProcedureOwner.SetData<VarInt>("nextProcedureId", (int)ProcedureConfig.ProcedureMainHall);
            ChangeState<ProcedureChangeScene>(m_ProcedureOwner);
        }
    }


    public void Send() 
    {
        LoginUserAuthRequest dPUserLoginInfoReq = new LoginUserAuthRequest();
        dPUserLoginInfoReq.Authorization = "12w";
        dPUserLoginInfoReq.Mac = "111";
        dPUserLoginInfoReq.Version = 1111;
        dPUserLoginInfoReq.ServerId = 2222;
        GameEntry.NetConnector.Send(MetaGameBase.Protocol.C2SLoginReq, dPUserLoginInfoReq);
    }
    private object RspLogin(object pSender)
    {
        MessengerInfo messengerInfo = (MessengerInfo)pSender;
        short protoId = (short)messengerInfo.param1;
        if (protoId!=(short)MetaGameBase.Protocol.S2CLoginRes && protoId != (short)MetaGameBase.Protocol.S2CUserOperate)
        {
            return null;
        }
        if (protoId == (short)MetaGameBase.Protocol.S2CLoginRes)
        {
            LoginUserAuthResponse msg = ProtobufUtils.Deserialize<LoginUserAuthResponse>(messengerInfo.param2);
            if (msg.Status != LoginUserResult.Success)
            {
                Log.Info("LoginUserResult:" + msg.Status.ToString());
            }
            Log.Info("SzAccount:" + msg.Nickname);
        }
        else 
        {
            UserOperateNotify msg = ProtobufUtils.Deserialize<UserOperateNotify>(messengerInfo.param2);
            Log.Info("userId:" + msg.User.UserId);
            Log.Info("type:" + msg.Type.ToString());
        }

        return null;
    }
}
