using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

[ExecuteInEditMode]
public class DirectiveDefiner : ScriptableObject
{
    public LookUpCode[] lookUpCode;
    private bool needRecompile = false;

    [InitializeOnLoadMethod]
    static void Bootstrap()
    {
        Debug.Log("DirectiveDefiner Bootstrap");
        var directiveDefiner = UnityEditor.AssetDatabase.LoadAssetAtPath<DirectiveDefiner>("Assets/YJack/Scripts/Editor/PDM/PreprocessorDirectiveManager.asset");
        directiveDefiner.OnEnable();
    }
    void OnEnable()
    {
        string[] currentLines;
        List<string> newLines = new List<string>();
        List<string> linesToRemove = new List<string>();
        List<string> resultLines = new List<string>();

        #region What
        for (int i = 0; i < lookUpCode.Length; i++)
        {
            string directiveToDefine = "-define:" + lookUpCode[i].define;

            switch (lookUpCode[i].domainType)
            {
                case DomainType.Class:
                    if (CheckIfClassExists(lookUpCode[i].ifExist))//Ex: "UnityStandardAssets.ImageEffects"
                        newLines.Add(directiveToDefine);//Ex: "-define:STANDARD_IMAGE_EFFECTS_EXIST"
                    else
                        linesToRemove.Add(directiveToDefine);
                    break;
                case DomainType.Namespace:
                    if (CheckIfNamespaceExists(lookUpCode[i].ifExist))//OVRManager
                        newLines.Add(directiveToDefine);//-define:USING_OVR
                    else
                        linesToRemove.Add(directiveToDefine);
                    break;
            }
        }
        #endregion

        string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Assets";
        if (!Directory.Exists(path))
        {
            Debug.Log("Something goes wrong and optimizer couldn't create the compiler defines needed. RTO should work but with limitations. Please report back to Yves \"Jack\" (AKA: RTO creator) so we can solve this issue");
            return;
        }

#if NET_4_6
        path += "\\csc.rsp";
#else
        path += "\\mcs.rsp"; //.NET 3.5 (Deprecated)
#endif

        if (!File.Exists(path))
        {
            resultLines.AddRange(newLines);

            if (resultLines.Count > 0)
                File.WriteAllLines(path, resultLines.ToArray());
        }
        else
        {
            currentLines = File.ReadAllLines(path);
            resultLines = currentLines.ToList();

            foreach (string line in newLines)
            {
                if (!resultLines.Contains(line))
                {
                    Debug.Log("define " + line);
                    resultLines.Add(line);
                    needRecompile = true;
                }
            }

            foreach (string line in linesToRemove)
            {
                if (resultLines.Contains(line))
                {
                    Debug.Log("Undef " + line);
                    resultLines.Remove(line);
                    needRecompile = true;
                }
            }

            if (resultLines.Count > 0)
                File.WriteAllLines(path, resultLines.ToArray());
            else
            {
                Debug.Log("No Directives defined by PDM. Removing csc.rsp file.");
                File.Delete(path);
            }

            if (needRecompile)
                CompilationPipeline.RequestScriptCompilation();
        }
    }

    static bool CheckIfNamespaceExists(string namespaceName)
    {
        bool namespaceFound = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                               from type in assembly.GetTypes()
                               where type.Namespace == namespaceName
                               select type).Any();

        return namespaceFound;
    }

    static bool CheckIfClassExists(string className)
    {
        var myType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                      from type in assembly.GetTypes()
                      where type.Name == className
                      select type).FirstOrDefault();

        if (myType == null)
            return false;
        else
            return true;
    }

    [Serializable]
    public struct LookUpCode
    {
        public DomainType domainType;
        public string ifExist;
        public string define;
    }

    public enum DomainType
    {
        Class,
        Namespace
    }
}