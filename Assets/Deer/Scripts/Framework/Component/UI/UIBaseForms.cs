// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2022-03-15 11-10-43  
//修改作者 : 杜鑫 
//修改时间 : 2022-03-15 11-10-43  
//版 本 : 0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UIBaseForms : MonoBehaviour
{
	private int m_Serialld;
	private UIFormGroupType m_enumGroup;
	private string m_AssetName;

	public int Serialld
	{
		get { return m_Serialld; }
		set { m_Serialld = value; }
	}
	public UIFormGroupType UIGroup
	{
		get { return m_enumGroup; }
		set { m_enumGroup = value; }
	}
/*	public string AssetName
	{
		get { return m_AssetName; }
		set { m_AssetName = value; }
	}*/

	public void OnInit()
	{
		
	}
	public void OnClose() 
	{
	
	}

	public void Close() 
	{
		GameEntry.UI.CloseForm(Serialld);
	}
}