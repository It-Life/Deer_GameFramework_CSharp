using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;

namespace UnityEditorInternal
{
    /*
     * Introduction：
     * Creator： Xiaohei Wang
    */
    [InitializeOnLoad()]
    public class CustomToolbar
    {
        private static string SceneName1 = "DeerLauncher", SceneName2 = "TestCity";
       
        
        private static readonly Type kToolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static ScriptableObject m_CurrentToolbar;

        static CustomToolbar()
        {
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            if (m_CurrentToolbar == null)
            {
                UnityEngine.Object[] toolbars = Resources.FindObjectsOfTypeAll(kToolbarType);
                m_CurrentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
                if (m_CurrentToolbar != null)
                {
                    FieldInfo root = m_CurrentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                    VisualElement concreteRoot = root.GetValue(m_CurrentToolbar) as VisualElement;

                    VisualElement toolbarZone = concreteRoot.Q("ToolbarZoneLeftAlign");
                    VisualElement parent = new VisualElement()
                    {
                        style = {
                                flexGrow = 1,
                                flexDirection = FlexDirection.Row,
                        }
                    };
                    IMGUIContainer container = new IMGUIContainer();
                    container.onGUIHandler += OnGuiBody;
                    parent.Add(container);
                    toolbarZone.Add(parent);
                }
            }
        }


        static string ButtonStyleName = "Tab middle";

        static GUIStyle ButtonGuiStyle;
        private static void OnGuiBody()
        {
            float startPos = (SceneName1.Length + SceneName2.Length) * 6f;
            if (null == ButtonGuiStyle)
            {
                ButtonGuiStyle = new GUIStyle(ButtonStyleName)
                {
                    padding = new RectOffset(2, 8, 2, 2)
                };
            }
            //自定义按钮加在此处
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIUtility.currentViewWidth / 2f - startPos - 270f);
            if (GUILayout.Button(new GUIContent(SceneName1, EditorGUIUtility.FindTexture("PlayButton"), $"Start {SceneName1} Scene"), ButtonGuiStyle))
            {
                SceneHelper.StartScene("DeerLauncher");
            }

            GUILayout.Space(10);

            if (GUILayout.Button(new GUIContent(SceneName2, EditorGUIUtility.FindTexture("PlayButton"), $"Start {SceneName2} Scene"), ButtonGuiStyle))
            {
                SceneHelper.StartScene("TestCity");
            }
            GUILayout.EndHorizontal();
        }
    }
    static class SceneHelper
    {
        static string sceneToOpen;

        public static void StartScene(string sceneName)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            sceneToOpen = sceneName;
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            if (sceneToOpen == null ||
                EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            EditorApplication.update -= OnUpdate;

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string[] guids = AssetDatabase.FindAssets("t:scene " + sceneToOpen, null);
                if (guids.Length == 0)
                {
                    Debug.LogWarning("Couldn't find scene file");
                }
                else
                {
                    string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    EditorSceneManager.OpenScene(scenePath);
                    EditorApplication.isPlaying = true;
                }
            }
            sceneToOpen = null;
        }
    }
}
