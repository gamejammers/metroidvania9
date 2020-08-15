//
// (c) BLACKTRIANGLES 2020
// http://www.blacktriangles.com
//

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace blacktriangles
{
    public class PaletteMaker
        : EditorWindow
    {
        //
        // constants //////////////////////////////////////////////////////////
        //

        public const string kMenuPath                           = "Tools/blacktriangles/Common/Palette Maker";
        public const string kTitle                              = "Palette Maker";

        //
        // members ////////////////////////////////////////////////////////////
        //

        private Texture2D texture;

        //
        // constructor ////////////////////////////////////////////////////////
        //

        [MenuItem(kMenuPath)]
        private static void OpenWindow()
        {
            PaletteMaker window = GetWindow<PaletteMaker>();
            window.titleContent = new GUIContent(kTitle);
            window.Initialize();
        }

        //
        // --------------------------------------------------------------------
        //

        private void Initialize()
        {
        }
        
        //
        // unity callbacks ////////////////////////////////////////////////////
        //

        protected virtual void OnEnable()
        {
            Initialize();
        }

        //
        // --------------------------------------------------------------------
        //

        protected virtual void OnGUI()
        {
            texture = EditorGUILayout.ObjectField("Read Palette From", texture, typeof(Texture2D), false) as Texture2D;

            if(GUILayout.Button("Generate"))
            {
                HashSet<Color> colors = new HashSet<Color>(texture.GetPixels(0));

                string resultFile = kColorFileHeader;
                foreach(Color color in colors)
                {
                    resultFile += System.String.Format(kColorFileEntry, color.r, color.g, color.b, color.a);
                }

                System.IO.File.WriteAllText("Assets/GeneratedPalette.colors", resultFile);
                AssetDatabase.Refresh();
            }
        } 

        //
        // private methods ////////////////////////////////////////////////////
        //

        //
        // more constants ////////////////////////////////////////////////////////////////////////
        //
        private const string kColorFileHeader =
@"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &1
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 12323, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Presets:
";

    private const string kColorFileEntry =
@"  - m_Name: 
    m_Color: {{ r: {0}, g: {1}, b: {2}, a: {3} }}
";


    }
}
