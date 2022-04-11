using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 获取模型的顶点数跟面数  ******************挂在你想要查看的模型上运行即可******************
/// </summary>
public class CeShiModel : MonoBehaviour
{
    public int verts;
    public int tris;
    private List<GameObject> sadasd = new List<GameObject>();

    // Use this for initialization
    void Start()
    {

        foreach (var item in this.GetComponentsInChildren<Transform>())
        {
            sadasd.Add(item.gameObject);
        }
        GetAllObjects();
    }

    void GetAllObjects()
    {
        foreach (GameObject obj in sadasd)
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
        int aas = verts / 3;
        string trisdisplay = aas.ToString("#,##0 tris-面数");
        GUILayout.Label(trisdisplay, bb);

    }

}