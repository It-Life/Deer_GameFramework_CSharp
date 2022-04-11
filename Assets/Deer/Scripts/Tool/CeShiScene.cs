using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 获取场景的顶点数跟面数  ******************挂在场景上运行即可******************
/// </summary>
public class CeShiScene : MonoBehaviour
{
    public static int verts;
    public static int tris;
    // Use this for initialization
    void Start()
    {
        GetAllObjects();
    }
    /// <summary>
    /// 得到场景中所有的GameObject
    /// </summary>
    void GetAllObjects()
    {
        verts = 0;
        tris = 0;
        GameObject[] ob = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject obj in ob)
        {
            GetAllVertsAndTris(obj);
        }
    }
    //得到三角面和顶点数
    void GetAllVertsAndTris(GameObject obj)
    {
        Component[] filters;
        filters = obj.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter f in filters)
        {
            tris += f.sharedMesh.triangles.Length / 3;
            verts += f.sharedMesh.vertexCount;
        }
    }
    void OnGUI()
    {
        GUIStyle bb = new GUIStyle();
        bb.normal.background = null;    //这是设置背景填充的
        bb.normal.textColor = new Color(1.0f, 0.5f, 0.0f);   //设置字体颜色的
        bb.fontSize = 40;       //当然，这是字体大小
        string vertsdisplay = verts.ToString("#,##0 verts-顶点数");
        GUILayout.Label(vertsdisplay, bb);
        string trisdisplay = tris.ToString("#,##0 tris-面数");
        GUILayout.Label(trisdisplay, bb);

    }

}