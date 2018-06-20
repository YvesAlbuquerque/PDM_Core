using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
[CreateAssetMenu(menuName = "Scriptable Objects/DirectiveDefiner")]
public class DirectiveDefiner : ScriptableObject
{
    public LookUpCode[] lookUpCode;

    void Awake()
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
                    if (CheckIfNamespaceExists(lookUpCode[i].ifExist))//Ex: "UnityStandardAssets.ImageEffects"
                        newLines.Add(directiveToDefine);//Ex: "-define:STANDARD_IMAGE_EFFECTS_EXIST"
                    else
                        linesToRemove.Add(directiveToDefine);
                    break;
                case DomainType.Namespace:
                    if (CheckIfClassExists(lookUpCode[i].ifExist))//OVRManager
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
        path += "\\mcs.rsp";

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
                    resultLines.Add(line);
            }

            foreach (string line in linesToRemove)
            {
                if (!resultLines.Contains(line))
                    resultLines.Remove(line);
            }

            if (resultLines.Count > 0)
                File.WriteAllLines(path, resultLines.ToArray());
            else
                File.Delete(path);
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