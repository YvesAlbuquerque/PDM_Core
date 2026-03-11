#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

/// <summary>
/// Preprocessor Directive Manager (PDM) - Automatically defines compiler preprocessor
/// directives based on the presence of namespaces, classes, or packages in the project.
/// Uses Unity's PlayerSettings scripting define symbols API with NamedBuildTarget.
/// </summary>
[ExecuteInEditMode]
public class DirectiveDefiner : ScriptableObject
{
    private const string LogPrefix = "[PDM] ";
    private const string AssetSearchName = "PreprocessorDirectiveDefiner";
    private const string EditorPrefsKey = "DirectiveDefiner";

    public LookUpCode[] lookUpCode;

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

        EditorPrefs.SetString(EditorPrefsKey, AssetDatabase.GetAssetPath(this));

        List<string> definesToAdd = new List<string>();
        List<string> definesToRemove = new List<string>();

        for (int i = 0; i < lookUpCode.Length; i++)
        {
            string define = lookUpCode[i].define;
            bool exists = false;

            switch (lookUpCode[i].domainType)
            {
                case DomainType.Class:
                    exists = CheckIfClassExists(lookUpCode[i].ifExist);
                    break;
                case DomainType.Namespace:
                    exists = CheckIfNamespaceExists(lookUpCode[i].ifExist);
                    break;
                case DomainType.Package:
                    exists = CheckIfPackageExists(lookUpCode[i].ifExist);
                    break;
            }

            if (exists)
                definesToAdd.Add(define);
            else
                definesToRemove.Add(define);
        }

        try
        {
            UpdateScriptingDefineSymbols(definesToAdd, definesToRemove);
        }
        catch (Exception ex)
        {
            Debug.LogError(LogPrefix + "Failed to update scripting define symbols: " + ex.Message);
        }
    }

    private static void UpdateScriptingDefineSymbols(List<string> definesToAdd, List<string> definesToRemove)
    {
        BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
        if (group == BuildTargetGroup.Unknown)
        {
            Debug.LogWarning(LogPrefix + "Unknown build target group. Cannot update scripting defines.");
            return;
        }

        NamedBuildTarget target = NamedBuildTarget.FromBuildTargetGroup(group);
        string currentDefinesStr = PlayerSettings.GetScriptingDefineSymbols(target);
        HashSet<string> defines = new HashSet<string>(
            currentDefinesStr.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

        bool changed = false;

        foreach (string define in definesToAdd)
        {
            if (defines.Add(define))
                changed = true;
        }

        foreach (string define in definesToRemove)
        {
            if (defines.Remove(define))
                changed = true;
        }

        if (changed)
        {
            PlayerSettings.SetScriptingDefineSymbols(target, string.Join(";", defines));
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

    static bool CheckIfPackageExists(string packageName)
    {
        try
        {
            string assetPath = "Packages/" + packageName + "/package.json";
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(assetPath);
            return packageInfo != null && packageInfo.name == packageName;
        }
        catch (Exception ex)
        {
            Debug.LogWarning(LogPrefix + "Error checking package '" + packageName + "': " + ex.Message);
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
        Namespace,
        Package
    }
}