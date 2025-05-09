using UnityEditor;
using UnityEngine;

namespace ExamineSystem
{
    [CustomEditor(typeof(ExaminableItem))]
    public class ExamineItemCustomEditor : Editor
    {
        #region SerializeProperties
        SerializedProperty isEmptyParent;
        SerializedProperty _hasChildren;
        SerializedProperty childObjects;

        SerializedProperty initialRotationOffset;
        SerializedProperty horizontalOffset;
        SerializedProperty verticalOffset;

        SerializedProperty smoothExamineSpeed;
        SerializedProperty initialZoom;
        SerializedProperty zoomRange;
        SerializedProperty zoomSensitivity;

        SerializedProperty rotationSpeed;
        SerializedProperty invertRotation;

        SerializedProperty showEmissionHighlight;
        SerializedProperty showNameHighlight;

        SerializedProperty _hasInspectPoints;
        SerializedProperty inspectPoints;

        SerializedProperty pickupSound;
        SerializedProperty dropSound;

        SerializedProperty _UIType;

        SerializedProperty itemName;

        SerializedProperty textSize;
        SerializedProperty fontType;
        SerializedProperty fontColor;

        SerializedProperty itemDescription;

        SerializedProperty textSizeDesc;
        SerializedProperty fontTypeDesc;
        SerializedProperty fontColorDesc;
        #endregion

        bool itemNameGroup, itemDescriptionGroup;

        void OnEnable()
        {
            #region SerializedObject References
            isEmptyParent = serializedObject.FindProperty(nameof(isEmptyParent));
            _hasChildren = serializedObject.FindProperty(nameof(_hasChildren));
            childObjects = serializedObject.FindProperty(nameof(childObjects));

            initialRotationOffset = serializedObject.FindProperty(nameof(initialRotationOffset));
            horizontalOffset = serializedObject.FindProperty(nameof(horizontalOffset));
            verticalOffset = serializedObject.FindProperty(nameof(verticalOffset));

            smoothExamineSpeed = serializedObject.FindProperty(nameof(smoothExamineSpeed));
            initialZoom = serializedObject.FindProperty(nameof(initialZoom));
            zoomRange = serializedObject.FindProperty(nameof(zoomRange));
            zoomSensitivity = serializedObject.FindProperty(nameof(zoomSensitivity));

            rotationSpeed = serializedObject.FindProperty(nameof(rotationSpeed));
            invertRotation = serializedObject.FindProperty(nameof(invertRotation));

            showEmissionHighlight = serializedObject.FindProperty(nameof(showEmissionHighlight));
            showNameHighlight = serializedObject.FindProperty(nameof(showNameHighlight));

            _hasInspectPoints = serializedObject.FindProperty(nameof(_hasInspectPoints));
            inspectPoints = serializedObject.FindProperty(nameof(inspectPoints));

            pickupSound = serializedObject.FindProperty(nameof(pickupSound));
            dropSound = serializedObject.FindProperty(nameof(dropSound));

            _UIType = serializedObject.FindProperty(nameof(_UIType));

            itemName = serializedObject.FindProperty(nameof(itemName));

            textSize = serializedObject.FindProperty(nameof(textSize));
            fontType = serializedObject.FindProperty(nameof(fontType));
            fontColor = serializedObject.FindProperty(nameof(fontColor));

            itemDescription = serializedObject.FindProperty(nameof(itemDescription));


            textSizeDesc = serializedObject.FindProperty(nameof(textSizeDesc));
            fontTypeDesc = serializedObject.FindProperty(nameof(fontTypeDesc));
            fontColorDesc = serializedObject.FindProperty(nameof(fontColorDesc));
            #endregion
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((ExaminableItem)target), typeof(ExaminableItem), false);
            GUI.enabled = true;
            EditorGUILayout.Space(5);

            ExaminableItem _examineItemController = (ExaminableItem)target;

            EditorGUILayout.LabelField("Parent Settings", EditorStyles.toolbarTextField);

            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(isEmptyParent);

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Children Settings", EditorStyles.toolbarTextField);

            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(_hasChildren);

            if (_examineItemController.hasChildren)
            {
                EditorGUILayout.PropertyField(childObjects);
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Inspect Point Settings", EditorStyles.toolbarTextField);

            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(_hasInspectPoints);

            if (_examineItemController.hasInspectPoints)
            {
                EditorGUILayout.PropertyField(inspectPoints);
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Highlight Settings", EditorStyles.toolbarTextField);

            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(showEmissionHighlight);
            EditorGUILayout.PropertyField(showNameHighlight);

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Rotation Settings", EditorStyles.toolbarTextField);

            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(rotationSpeed);
            EditorGUILayout.PropertyField(invertRotation);

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Offset Settings", EditorStyles.toolbarTextField);

            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(initialRotationOffset);
            EditorGUILayout.PropertyField(horizontalOffset);
            EditorGUILayout.PropertyField(verticalOffset);

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Zoom Settings", EditorStyles.toolbarTextField);

            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(smoothExamineSpeed);
            EditorGUILayout.PropertyField(initialZoom);
            EditorGUILayout.PropertyField(zoomRange);
            EditorGUILayout.PropertyField(zoomSensitivity);

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Text Settings", EditorStyles.toolbarTextField);

            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(_UIType);

            itemNameGroup = EditorGUILayout.BeginFoldoutHeaderGroup(itemNameGroup, "Item Name Settings");
            if (itemNameGroup)   
            {
                EditorGUILayout.PropertyField(itemName);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(textSize);
                EditorGUILayout.Space(2);
                EditorGUILayout.PropertyField(fontType);
                EditorGUILayout.Space(2);
                EditorGUILayout.PropertyField(fontColor);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            itemDescriptionGroup = EditorGUILayout.BeginFoldoutHeaderGroup(itemDescriptionGroup, "Item Description Settings");
            if (itemDescriptionGroup)
            {
                EditorGUILayout.PropertyField(itemDescription);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(textSizeDesc);
                EditorGUILayout.Space(2);
                EditorGUILayout.PropertyField(fontTypeDesc);
                EditorGUILayout.Space(2);
                EditorGUILayout.PropertyField(fontColorDesc);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Audio Settings", EditorStyles.toolbarTextField);

            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(pickupSound);
            EditorGUILayout.PropertyField(dropSound);

            OpenEditorScript();

            serializedObject.ApplyModifiedProperties();
        }

        void OpenEditorScript()
        {
            EditorGUILayout.Space(5);

            if (GUILayout.Button("Open Editor Script"))
            {
                string scriptFilePath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(scriptFilePath));
            }
        }
    }
}

