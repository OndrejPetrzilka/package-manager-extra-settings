using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SettingsManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace PackageManagerExtraSettings
{
    public class ExtraSettingsProvider : SettingsProvider
    {
        public static bool ShowHiddenPackages
        {
            get { return Settings.Get("ShowHiddenPackages.Enabled", SettingsScope.Project, true); }
            set
            {
                Settings.Set("ShowHiddenPackages.Enabled", value);
                DiscoverExtraPackages.EnableHook(ShowHiddenPackages);
                Settings.Save();
            }
        }

        public static bool DisableWarning
        {
            get { return Settings.Get("ShowHiddenPackages.DisableWarning", SettingsScope.Project, true); }
            set
            {
                Settings.Set("ShowHiddenPackages.DisableWarning", value);
                DisablePreviewPackageWarning.DisableWarning(value);
                Settings.Save();
            }
        }

        const string PackageName = "com.unity.show-hidden-packages";

        private static GUIContent m_showHiddenPackages = new GUIContent("Show hidden packages", "Shows hidden packages in package manager\r\nRefresh package manager after changing this option");
        private static GUIContent m_disableWarning = new GUIContent("Disable warning", "Disables warning on toolbar:\r\nPreview packages in use");

        private static IEnumerable<string> m_keywords = new string[] { m_showHiddenPackages.text, m_showHiddenPackages.tooltip, m_disableWarning.text, m_showHiddenPackages.tooltip };
        private static Settings m_settings;

        public static Settings Settings
        {
            get
            {
                if (m_settings == null)
                {
                    m_settings = new Settings(PackageName);
                }
                return m_settings;
            }
        }

        public ExtraSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope)
        {
        }

        [InitializeOnLoadMethod]
        static void Init()
        {
            if (ShowHiddenPackages)
            {
                // Wrapper to prevent static constructor call when not used
                ShowHiddenPackagesWrapper();
            }
            if (DisableWarning)
            {
                // Wrapper to prevent static constructor call when not used
                DisableWarningWrapper();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void DisableWarningWrapper()
        {
            DisablePreviewPackageWarning.DisableWarning(true);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ShowHiddenPackagesWrapper()
        {
            DiscoverExtraPackages.EnableHook(ShowHiddenPackages);
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUI.indentLevel++;
            ShowHiddenPackages = EditorGUILayout.Toggle(m_showHiddenPackages, ShowHiddenPackages);
            DisableWarning = EditorGUILayout.Toggle(m_disableWarning, DisableWarning);
            EditorGUI.indentLevel--;
        }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new ExtraSettingsProvider("Project/Package Manager/Extra Settings", SettingsScope.Project) { keywords = m_keywords };
        }
    }
}