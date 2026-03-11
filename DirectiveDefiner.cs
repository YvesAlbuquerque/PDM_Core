#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

/// <summary>
/// Preprocessor Directive Manager (PDM) - Automatically defines compiler preprocessor
/// directives based on the presence of namespaces or classes in the project.
/// </summary>
[ExecuteInEditMode]
public class DirectiveDefiner : ScriptableObject
{
    private const string LogPrefix = "[PDM] ";
    private const string AssetSearchName = "PreprocessorDirectiveDefiner";
    private const string EditorPrefsKey = "DirectiveDefiner";
    private const string RspFileName = "csc.rsp";

    public LookUpCode[] lookUpCode;
    private bool needRecompile = false;

    [InitializeOnLoadMethod]
    static void Bootstrap()
    {
        string[] guids = AssetDatabase.FindAssets(AssetSearchName);
        if (guids.Length == 0)
        {
            Debug.LogWarning(LogPrefix + "PreprocessorDirectiveDefiner asset not found. " +
                "Please create a DirectiveDefiner ScriptableObject asset named 'PreprocessorDirectiveDefiner'.");
            return;
        }

        string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        DirectiveDefiner directiveDefiner = AssetDatabase.LoadAssetAtPath<DirectiveDefiner>(assetPath);
        if (directiveDefiner == null)
        {
            Debug.LogWarning(LogPrefix + "Failed to load DirectiveDefiner asset at: " + assetPath);
            return;
        }

        directiveDefiner.ApplyDirectives();

        UnityEditor.PackageManager.Events.registeredPackages -= OnRegisteredPackages;
        UnityEditor.PackageManager.Events.registeredPackages += OnRegisteredPackages;
    }

    private static void OnRegisteredPackages(UnityEditor.PackageManager.PackageRegistrationEventArgs obj)
    {
        Bootstrap();
    }

#if ODIN_INSPECTOR
    [Button("Apply")]
#endif
    [ContextMenu("Apply Directives")]
    public void ApplyDirectives()
    {
        if (lookUpCode == null || lookUpCode.Length == 0)
        {
            Debug.LogWarning(LogPrefix + "No lookup entries configured. Nothing to do.");
            return;
        }

        List<string> newLines = new List<string>();
        List<string> linesToRemove = new List<string>();
        List<string> resultLines = new List<string>();

        EditorPrefs.SetString(EditorPrefsKey, AssetDatabase.GetAssetPath(this));
        needRecompile = false;

        for (int i = 0; i < lookUpCode.Length; i++)
        {
            string directiveToDefine = "-define:" + lookUpCode[i].define;

            switch (lookUpCode[i].domainType)
            {
                case DomainType.Class:
                    if (CheckIfClassExists(lookUpCode[i].ifExist))
                        newLines.Add(directiveToDefine);
                    else
                        linesToRemove.Add(directiveToDefine);
                    break;
                case DomainType.Namespace:
                    if (CheckIfNamespaceExists(lookUpCode[i].ifExist))
                        newLines.Add(directiveToDefine);
                    else
                        linesToRemove.Add(directiveToDefine);
                    break;
            }
        }

        string assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
        if (!Directory.Exists(assetsPath))
        {
            Debug.LogError(LogPrefix + "Assets directory not found at: " + assetsPath +
                ". Cannot write compiler response file.");
            return;
        }

        string rspPath = Path.Combine(assetsPath, RspFileName);

        try
        {
            UpdateRspFile(rspPath, newLines, linesToRemove, resultLines);
        }
        catch (IOException ex)
        {
            Debug.LogError(LogPrefix + "Failed to update " + RspFileName + ": " + ex.Message);
            return;
        }

        if (needRecompile)
            CompilationPipeline.RequestScriptCompilation();
    }

    private void UpdateRspFile(string rspPath, List<string> newLines, List<string> linesToRemove, List<string> resultLines)
    {
        if (!File.Exists(rspPath))
        {
            resultLines.AddRange(newLines);
            needRecompile = newLines.Count > 0;

            if (resultLines.Count > 0)
                File.WriteAllLines(rspPath, resultLines.ToArray());
        }
        else
        {
            string[] currentLines = File.ReadAllLines(rspPath);
            resultLines.AddRange(currentLines);

            foreach (string line in newLines)
            {
                if (!resultLines.Contains(line))
                {
                    resultLines.Add(line);
                    needRecompile = true;
                }
            }

            foreach (string line in linesToRemove)
            {
                if (resultLines.Contains(line))
                {
                    resultLines.Remove(line);
                    needRecompile = true;
                }
            }

            if (resultLines.Count > 0)
                File.WriteAllLines(rspPath, resultLines.ToArray());
            else
            {
                Debug.Log(LogPrefix + "No directives defined. Removing " + RspFileName + ".");
                File.Delete(rspPath);
            }
        }
    }

    static bool CheckIfNamespaceExists(string namespaceName)
    {
        try
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(GetTypesSafe)
                .Any(type => type.Namespace == namespaceName);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(LogPrefix + "Error checking namespace '" + namespaceName + "': " + ex.Message);
            return false;
        }
    }

    static bool CheckIfClassExists(string className)
    {
        try
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(GetTypesSafe)
                .Any(type => type.Name == className);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(LogPrefix + "Error checking class '" + className + "': " + ex.Message);
            return false;
        }
    }

    private static IEnumerable<Type> GetTypesSafe(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null);
        }
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