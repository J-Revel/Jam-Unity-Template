using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

// Create a new type of Settings Asset.
class SpriteAnimEditorSettings : ScriptableObject
{
    public const string k_MyCustomSettingsPath = "Assets/Editor/SpriteAnimEditorSettings.asset";

    public Color[] colors;

    internal static SpriteAnimEditorSettings GetOrCreateSettings()
    {
        var settings = AssetDatabase.LoadAssetAtPath<SpriteAnimEditorSettings>(k_MyCustomSettingsPath);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<SpriteAnimEditorSettings>();
            settings.colors = new Color[]{Color.red, Color.green, Color.yellow};
            AssetDatabase.CreateAsset(settings, k_MyCustomSettingsPath);
            AssetDatabase.SaveAssets();
        }
        return settings;
    }

    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }
}

// Register a SettingsProvider using IMGUI for the drawing framework:
static class SpriteAnimEditorSettingsIMGUIRegister
{
    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider()
    {
        var provider = new SettingsProvider("Project/Sprite Anim Config", SettingsScope.Project)
        {
            label = "Sprite Animator",
            guiHandler = (searchContext) =>
            {
                var settings = SpriteAnimEditorSettings.GetSerializedSettings();
                EditorGUILayout.PropertyField(settings.FindProperty("colors"), new GUIContent("Colors"));
                settings.ApplyModifiedPropertiesWithoutUndo();
            },

            // Populate the search keywords to enable smart search filtering and label highlighting:
            keywords = new HashSet<string>(new[] { "Number", "Some String" })
        };

        return provider;
    }
}

