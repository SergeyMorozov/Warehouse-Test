using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GAME
{
    public class CreateSystemWindow : EditorWindow
    {
        static EditorWindow _window;

        private string _sourcePath;
        private string _systemName;
        private ScriptableObject _objSettings;

        private bool _scriptableObjectCreating;

//        private PlatformData _platformData;
//        private PlatformRefs _platformRefs;

        private GameObject _selected;

        [MenuItem("Tools/Create System")]
        public static void ShowWindow()
        {
            _window = GetWindow(typeof(CreateSystemWindow));
            _window.minSize = new Vector2(200, 200);
            _window.titleContent = new GUIContent("Create System");

        }

        void OnGUI()
        {
            _systemName = EditorGUILayout.TextField("Name System", _systemName);

            _sourcePath = "Assets/_Game Base/Tools/Create System/Empty System";


            string targetPath = "_Game Engine";
            string pretag = "- ";
            string folder = "Assets/" + targetPath + "/" + pretag + _systemName;


            if (GUILayout.Button("Create System", GUILayout.Width(200), GUILayout.Height(25)))
            {
                if (_systemName == "")
                    return;

                if (AssetDatabase.IsValidFolder(folder))
                {
                    Debug.Log("System <" + _systemName + "> уже создана!");
                    return;
                }

                string guid1 = AssetDatabase.CreateFolder("Assets/" + targetPath, pretag + _systemName);
                string guid2 = AssetDatabase.CreateFolder("Assets/" + targetPath + "/" + pretag + _systemName, "Logics");
                string guid3 = AssetDatabase.CreateFolder("Assets/" + targetPath + "/" + pretag + _systemName, "Components");
                
                CreateScript(folder, _systemName, "SystemSettings");
                CreateScript(folder, _systemName, "SystemData");
                CreateScript(folder, _systemName, "SystemEvents");
                CreateScript(folder, _systemName, "System");
                CreateScript(folder, _systemName, "Logic", "/Logics");
                CreateScript(folder, _systemName, "Object", "/Components");
                CreateScript(folder, _systemName, "Preset", "/Components");
                CreateScript(folder, _systemName, "Ref", "/Components");

                _scriptableObjectCreating = true;
            }

            if (GUILayout.Button("Create ScriptableObject", GUILayout.Width(200), GUILayout.Height(25)))
            {
                if (!_scriptableObjectCreating)
                    return;
                
                CreatePrefabObject(folder, _systemName, "System");
                CreateScriptableObject(folder, _systemName, "SystemSettings");
                CreateScriptableObject(folder + "/Components", _systemName, "Preset");
            }

            if (_window) _window.Repaint();
        }

        private void CreateScript(string sriptPath, string sriptName, string sriptType, string folder = "")
        {
            string sourcePath = _sourcePath + folder + "/Empty" + sriptType + ".cs";
            string targetPath = sriptPath + folder + "/" + sriptName + sriptType + ".cs";

//            Debug.Log("Source Classfile: " + sourcePath);
//            Debug.Log("Creating Classfile: " + targetPath);

            StreamReader streamReader = new StreamReader(sourcePath);
            
            string content = streamReader.ReadToEnd();
            content = content.Replace("Empty", sriptName);
            
            using (StreamWriter streamWriter = new StreamWriter(targetPath))
            {
                streamWriter.WriteLine(content);
            }

            AssetDatabase.Refresh();
        }

        private void CreateScriptableObject(string sriptPath, string scriptName, string sriptType)
        {
            _objSettings = CreateInstance(scriptName + sriptType);
            if (_objSettings == null)
                return;

            _scriptableObjectCreating = false;

            AssetDatabase.CreateAsset(_objSettings, sriptPath + "/" + scriptName + " " + sriptType + ".asset");
            AssetDatabase.SaveAssets();
        }

        private void CreatePrefabObject(string sriptPath, string scriptName, string sriptType)
        {
            GameObject obj = new GameObject();
            Component component = UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(obj, "", scriptName + sriptType);
            GameObject obj2 = PrefabUtility.SaveAsPrefabAsset(obj, sriptPath + "/" + scriptName + " " + sriptType + ".prefab");
            DestroyImmediate(obj);
        }

    }
}