/* ================================================
 * Introduction：xxx 
 * Creator：XinDu 
 * CreationTime：2022-03-26 15-40-48
 * ChangeCreator：XinDu 
 * ChangeTime：2022-03-26 15-40-48
 * CreateVersion：0.1
 *  =============================================== */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Please modify the description.
/// </summary>
public class CharacterPetData : CharacterData
{
    public int MasterId;
    public CharacterPetData(int entityId, int typeId, string assetName) : base(entityId, typeId, assetName)
    {
    }
}