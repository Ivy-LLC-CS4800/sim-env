
/* This script is written by Shubham Vine of Eniv Studios. 
   Item Interaction Kit :- V1.1.3
*/

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnivStudios
{
    public class ItemController : MonoBehaviour
    {
        #region Variable Names
        public enum states { InteractOnly, ExamineOnly, Both, NoteSystem }
        [SerializeField] states interactionType;
        [SerializeField] string itemName;
        [SerializeField][TextArea] string itemInfo;
        public float timeToDisplay = 3;
        public bool rotateObject;
        public bool playPickUpSound;
        [SerializeField] RaycastItem raycastItem;
        public AudioClip itemPickUpSound;
        public bool itemFaceCamera;
        public Vector3 itemRotation;
        public enum rotation { Horizontal, Vertical, Both }
        public rotation rotationType;
        public int rotateSpeed = 2;
        public float antiSpamTime = 0.5f;
        public bool examineCursorVisible;
        public int lerpSpeed = 10;
        public bool enableZoom;
        public float zoomScrollSpeed = 5;
        public int minZoomFOV = 40;
        public bool showName;
        [SerializeField] string noteName;

        public bool interactOnly = false, examineOnly = false, both = false, note = false;
        public bool horizontal = false, vertical = false, bothRotation = false, noRotation = false;
        #endregion

        void Start()
        {
            if (interactionType == states.InteractOnly)
            {
                interactOnly = true;
            }
            else if (interactionType == states.ExamineOnly)
            {
                examineOnly = true;
                if (rotationType == rotation.Horizontal) { horizontal = true; }
                else if (rotationType == rotation.Vertical) { vertical = true; }
                else if (rotationType == rotation.Both) { bothRotation = true; }
                if (!rotateObject) { noRotation = true; }
            }
            else if (interactionType == states.Both)
            {
                both = true;
                if (rotationType == rotation.Horizontal) { horizontal = true; }
                else if (rotationType == rotation.Vertical) { vertical = true; }
                else if (rotationType == rotation.Both) { bothRotation = true; }
                if (!rotateObject) { noRotation = true; }
            }
            else if (interactionType == states.NoteSystem)
            {
                note = true;
                if (rotationType == rotation.Horizontal) { horizontal = true; }
                else if (rotationType == rotation.Vertical) { vertical = true; }
                else if (rotationType == rotation.Both) { bothRotation = true; }
                if (!rotateObject) { noRotation = true; }
            }
            if (itemPickUpSound == null) { return; }
        }


        //This function passes name and info of the item to the RaycastItem Script
        public void ObjectDescription()
        {
            if (showName) { raycastItem.ObjectName(noteName); raycastItem.ObjectDescription(itemInfo); }
            else
            {
                raycastItem.ObjectName(itemName);
                raycastItem.ObjectDescription(itemInfo);
            }
        }

    }

    // This is a custom editor script which makes Inspector Dynamic (You can check out my FREE Stylize C# Script asset on unity asset store to make your own dynamic inspector scripts)

#if UNITY_EDITOR
    [CustomEditor(typeof(ItemController))]
    public class ItemControllerEditor : Editor
    {
        #region Item Names
        SerializedProperty States;
        SerializedProperty ItemName;
        SerializedProperty ItemInfo;
        SerializedProperty TimeToDisplay;
        SerializedProperty RaycastItemScript;
        SerializedProperty RotateObject;
        SerializedProperty PlayPickUpSound;
        SerializedProperty ItemPickUpSound;
        SerializedProperty ItemFaceCamera;
        SerializedProperty ItemRotation;
        SerializedProperty Rotations;
        SerializedProperty RotateSpeed;
        SerializedProperty AntiSpamTime;
        SerializedProperty ExamineCursorVisible;
        SerializedProperty LerpSpeed;
        SerializedProperty EnableZoom;
        SerializedProperty ZoomScrollSpeed;
        SerializedProperty MinZoomFOV;
        SerializedProperty ShowName;
        SerializedProperty NoteName;
        #endregion
        private void OnEnable()
        {
            #region Item Initialization
            States = serializedObject.FindProperty("interactionType");
            ItemName = serializedObject.FindProperty("itemName");
            ItemInfo = serializedObject.FindProperty("itemInfo");
            TimeToDisplay = serializedObject.FindProperty("timeToDisplay");
            RaycastItemScript = serializedObject.FindProperty("raycastItem");
            PlayPickUpSound = serializedObject.FindProperty("playPickUpSound");
            ItemPickUpSound = serializedObject.FindProperty("itemPickUpSound");
            ItemFaceCamera = serializedObject.FindProperty("itemFaceCamera");
            ItemRotation = serializedObject.FindProperty("itemRotation");
            RotateObject = serializedObject.FindProperty("rotateObject");
            Rotations = serializedObject.FindProperty("rotationType");
            RotateSpeed = serializedObject.FindProperty("rotateSpeed");
            AntiSpamTime = serializedObject.FindProperty("antiSpamTime");
            ExamineCursorVisible = serializedObject.FindProperty("examineCursorVisible");
            LerpSpeed = serializedObject.FindProperty("lerpSpeed");
            EnableZoom = serializedObject.FindProperty("enableZoom");
            ZoomScrollSpeed = serializedObject.FindProperty("zoomScrollSpeed");
            MinZoomFOV = serializedObject.FindProperty("minZoomFOV");
            ShowName = serializedObject.FindProperty("showName");
            NoteName = serializedObject.FindProperty("noteName");
            #endregion
        }
        public override void OnInspectorGUI()
        {
            var centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;

            EditorGUILayout.LabelField("Item Controller Script", header);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("This script handles all the interaction types and is fully dynamic in inspector.", guiMessageStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            serializedObject.Update();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(States);
            EditorGUILayout.Space();

            var stateType = (ItemController.states)States.enumValueIndex;

            switch (stateType)
            {
                case ItemController.states.InteractOnly:
                    GUILayout.Label("Item UI", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(ItemName);
                    EditorGUILayout.PropertyField(ItemInfo);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(TimeToDisplay);
                    break;

                case ItemController.states.ExamineOnly:
                    GUILayout.Label("Sound Effects", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(PlayPickUpSound);
                    if (PlayPickUpSound.boolValue)
                    {
                        EditorGUILayout.PropertyField(ItemPickUpSound, new GUIContent("Item Pick Up Sound"));
                    }
                    EditorGUILayout.Space();
                    GUILayout.Label("Examine Direction", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(ItemFaceCamera);
                    if (ItemFaceCamera.boolValue)
                    {
                        EditorGUILayout.PropertyField(ItemRotation, new GUIContent("Item Rotation"));
                    }

                    EditorGUILayout.Space();

                    GUILayout.Label("Rotation Handling", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(RotateObject);
                    if (RotateObject.boolValue)
                    {
                        EditorGUILayout.PropertyField(Rotations);
                        EditorGUILayout.IntSlider(RotateSpeed, 1, 8, new GUIContent("Rotate Speed"));
                    }
                    EditorGUILayout.Space();
                    GUILayout.Label("Other Item Handling", EditorStyles.boldLabel);
                    EditorGUILayout.Slider(AntiSpamTime, 0.3f, 1, new GUIContent("Anti Spam Time"));
                    EditorGUILayout.PropertyField(ExamineCursorVisible);
                    EditorGUILayout.IntSlider(LerpSpeed, 6, 20, new GUIContent("Lerp Speed"));

                    EditorGUILayout.Space();

                    GUILayout.Label("Zoom Handling", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(EnableZoom);
                    if (EnableZoom.boolValue)
                    {
                        EditorGUILayout.Slider(ZoomScrollSpeed, 1, 10, new GUIContent("Zoom Scroll Speed"));
                        EditorGUILayout.IntSlider(MinZoomFOV, 20, 60, new GUIContent("Min Zoom FOV"));
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(ItemName);
                    break;

                case ItemController.states.Both:
                    GUILayout.Label("Sound Effects", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(PlayPickUpSound);
                    if (PlayPickUpSound.boolValue)
                    {
                        EditorGUILayout.PropertyField(ItemPickUpSound, new GUIContent("Item Pick Up Sound"));
                    }
                    EditorGUILayout.Space();
                    GUILayout.Label("Examine Direction", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(ItemFaceCamera);
                    if (ItemFaceCamera.boolValue)
                    {
                        EditorGUILayout.PropertyField(ItemRotation, new GUIContent("Item Rotation"));
                    }

                    EditorGUILayout.Space();

                    GUILayout.Label("Rotation Handling", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(RotateObject);
                    if (RotateObject.boolValue)
                    {
                        EditorGUILayout.PropertyField(Rotations);
                        EditorGUILayout.IntSlider(RotateSpeed, 1, 8, new GUIContent("Rotate Speed"));
                    }
                    EditorGUILayout.Space();
                    GUILayout.Label("Other Item Handling", EditorStyles.boldLabel);
                    EditorGUILayout.Slider(AntiSpamTime, 0.3f, 1, new GUIContent("Anti Spam Time"));
                    EditorGUILayout.PropertyField(ExamineCursorVisible);
                    EditorGUILayout.IntSlider(LerpSpeed, 6, 20, new GUIContent("Lerp Speed"));

                    EditorGUILayout.Space();

                    GUILayout.Label("Zoom Handling", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(EnableZoom);
                    if (EnableZoom.boolValue)
                    {
                        EditorGUILayout.Slider(ZoomScrollSpeed, 1, 10, new GUIContent("Zoom Scroll Speed"));
                        EditorGUILayout.IntSlider(MinZoomFOV, 20, 60, new GUIContent("Min Zoom FOV"));
                    }

                    EditorGUILayout.Space();
                    GUILayout.Label("Item UI", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(ItemName);
                    EditorGUILayout.PropertyField(ItemInfo);

                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(TimeToDisplay);
                    break;

                case ItemController.states.NoteSystem:
                    GUILayout.Label("Sound Effects", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(PlayPickUpSound);
                    if (PlayPickUpSound.boolValue)
                    {
                        EditorGUILayout.PropertyField(ItemPickUpSound, new GUIContent("Item Pick Up Sound"));
                    }
                    EditorGUILayout.Space();
                    GUILayout.Label("Examine Direction", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(ItemFaceCamera);
                    if (ItemFaceCamera.boolValue)
                    {
                        EditorGUILayout.PropertyField(ItemRotation, new GUIContent("Item Rotation"));
                    }

                    EditorGUILayout.Space();

                    GUILayout.Label("Rotation Handling", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(RotateObject);
                    if (RotateObject.boolValue)
                    {
                        EditorGUILayout.PropertyField(Rotations);
                        EditorGUILayout.IntSlider(RotateSpeed, 1, 8, new GUIContent("Rotate Speed"));
                    }
                    EditorGUILayout.Space();
                    GUILayout.Label("Other Item Handling", EditorStyles.boldLabel);
                    EditorGUILayout.Slider(AntiSpamTime, 0.3f, 1, new GUIContent("Anti Spam Time"));
                    EditorGUILayout.PropertyField(ExamineCursorVisible);
                    EditorGUILayout.IntSlider(LerpSpeed, 6, 20, new GUIContent("Lerp Speed"));

                    EditorGUILayout.Space();

                    GUILayout.Label("Zoom Handling", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(EnableZoom);
                    if (EnableZoom.boolValue)
                    {
                        EditorGUILayout.Slider(ZoomScrollSpeed, 1, 10, new GUIContent("Zoom Scroll Speed"));
                        EditorGUILayout.IntSlider(MinZoomFOV, 20, 60, new GUIContent("Min Zoom FOV"));
                    }

                    EditorGUILayout.Space();
                    GUILayout.Label("Item UI", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(ShowName);
                    if (ShowName.boolValue)
                    {
                        EditorGUILayout.PropertyField(NoteName);
                    }
                    break;
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(RaycastItemScript);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

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



