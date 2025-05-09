
/* This script is written by Shubham Vine of Eniv Studios. 
   Item Interaction Kit :- V1.1.3
*/


using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnivStudios
{
    //This just passes UIElements to RaycastItem script
    public class ItemUI : MonoBehaviour
    {
        [Space()]
        public Image crosshair;
        [Header("Interact UI")]
        public KeyCode interactKey;
        public Color interactCrosshairColor;

        [Space()]
        public GameObject interactKeyBG;
        public Text interactKeyCode;

        [Header("Examine UI")]
        public KeyCode examineKey;
        [Tooltip("Crosshair color is same for ExamineOnly objects as well as for both Interact and Examine objects")]
        public Color bothCrosshairColor;
        [Space()]

        public GameObject examineKeyBG;
        public Text examineKeyCode;

        [Header("Items UI")]
        public GameObject itemNameBG;
        public Text itemNameText;

        [Space()]

        public GameObject itemInfoBG;
        public Text itemInfoText;

        [Space()]
        public GameObject putBackBG;
        public Text putBackKey;

        [Space()]
        public GameObject putBackBGNoZoom;
        public Text putBackKeyNoZoom;


        [Header("Text Animation")]
        [Tooltip("Lower the value faster the text scale animation")]
        public float animationSpeed = 1;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ItemUI))]
    public class ItemUIEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;

            EditorGUILayout.LabelField("Item UI Script", header);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("This script handles all the UI elements.", guiMessageStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            DrawDefaultInspector();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

        }
        GUIStyle guiMessageStyle
        {
            get
            {
                var messageStyle = new GUIStyle(GUI.skin.label);
                messageStyle.wordWrap = true;
                return messageStyle;
            }
        }
        GUIStyle header
        {
            get
            {
                var messageStyle = new GUIStyle(GUI.skin.label);
                messageStyle.wordWrap = true;
                messageStyle.fontStyle = FontStyle.BoldAndItalic;
                messageStyle.fontSize = 18;
                return messageStyle;
            }
        }
    }
#endif
}
