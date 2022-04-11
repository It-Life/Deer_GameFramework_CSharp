// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2022-03-15 14-48-16  
//修改作者 : 杜鑫 
//修改时间 : 2022-03-15 14-48-16  
//版 本 : 0.1 
// ===============================================
using UnityEngine;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

public class ProcedureMainHall : Deer.ProcedureBase
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
        GameEntry.Sound.PlayMusic(Constant.SoundId.MAIN_BGM, null);
        GameEntry.UI.CreateForm(UINameConfig.UIMainHallPanel);
        int entityId = GameEntry.Entity.GenerateSerialId();
        CharacterPlayerData entityData = new CharacterPlayerData(entityId, Constant.AssetType.Character, "Character/Blade_girl/Blade_Girl_Prefab");
        entityData.Position = new Vector3(12f, 0.59f, 1.8f);
        entityData.IsOwner = true;
        GameEntry.Entity.ShowEntity(typeof(CharacterPlayer), Constant.EntityGroup.Character, Constant.AssetPriority.RolePlayerAsset, entityData);

        int entityId1 = GameEntry.Entity.GenerateSerialId();
        CharacterPetData entityData1 = new CharacterPetData(entityId1, Constant.AssetType.Pet, "Pet/DragonSD_00");
        entityData1.Position = new Vector3(13f, 0.59f, 1.8f);
        entityData1.IsOwner = false;
        entityData1.AIStates.Add(AIStateType.AIStatePet);
        entityData1.MasterId = entityId;
        GameEntry.Entity.ShowEntity(typeof(CharacterPet), Constant.EntityGroup.CharacterPet, Constant.AssetPriority.RolePlayerAsset, entityData1);
    }

    public void OnChangeProcedure(ProcedureConfig procedureConfig)
    {
        if (procedureConfig == ProcedureConfig.ProcedureLogin)
        {
            UnloadAllScene();
            UnloadAllEntity();
            ChangeState<ProcedureLogin>(m_ProcedureOwner);
        }
    }

}