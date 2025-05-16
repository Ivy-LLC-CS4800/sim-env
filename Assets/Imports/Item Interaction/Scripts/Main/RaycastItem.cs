
/* This script is written by Shubham Vine of Eniv Studios. 
   Item Interaction Kit :- V1.1.3
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnivStudios
{
    public class RaycastItem : MonoBehaviour
    {
        [Header("Raycast")]
        [SerializeField] [Range(1, 10)] float rayDistance = 2f;
        [SerializeField] LayerMask layerMask;

        [Header("Examine Properties")]
        [SerializeField] string examineLayerName;
        [SerializeField] GameObject examineCam;
        [SerializeField] Transform examineItemPos;

        [Header("Post Processing Effects")]
        [SerializeField] PostProcessVolume myVolume;
        [SerializeField] PostProcessProfile examineBlur;
        [SerializeField] PostProcessProfile defaultProfile;

        [Space(15)]
        [SerializeField] ItemUI itemUI;
        [SerializeField] PlayerMovement playerMovement; //You can write the name of your own player script here

        //Private Variables
        AudioSource audioSource;
        float timer;
        ItemController itemController;
        bool examining = false;
        bool inspecting = false;
        GameObject inspected;
        Vector3 originalPos;
        Quaternion originalRot;
        GameObject currentlyFocused;
        int previousLayer;
        bool startAgain = false;
        bool onlyExamine = false;
        Camera _cam;
        float originalFOV;
        float examineOriginalFOV;
        bool faceCam = false;
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            _cam = GetComponent<Camera>();
            //This takes the key specified by the user in inspector and changes it in the itemUI script.
            if (itemUI != null)
            {
                itemUI.examineKeyCode.text = itemUI.examineKey.ToString();
                itemUI.interactKeyCode.text = itemUI.interactKey.ToString();
                itemUI.putBackKey.text = itemUI.examineKey.ToString();
                itemUI.putBackKeyNoZoom.text = itemUI.examineKey.ToString();
            }

            /*This takes the reference of the camera fieldOfView value and stores it so when the item is dropped back to its original position after
            examining it resets camera fieldOfView value to its default value*/
            if (_cam != null)
                originalFOV = _cam.fieldOfView;

            if (examineCam != null && examineCam.GetComponent<Camera>() != null)
                examineOriginalFOV = examineCam.GetComponent<Camera>().fieldOfView;

            /*This takes reference of the player script and since my player script also contains the function of camera rotate 
              so it disables both player movement as well as a camera rotate while examining object but if you have a different camera rotate 
              script just assign it in the inspector and put it below this line and wherever in the script this player script is disabled you
              can disable your camera rotate script too by just writing one additional line of code*/
            //  playerMovement = GetComponentInParent<PlayerMovement>();

            //This just turn off the text animation if that bool is unchecked in ItemUI script
        }
        private void Update()
        {
            bool crosshairColorChange = false;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayDistance, layerMask.value | (1 << LayerMask.NameToLayer(examineLayerName))))
            {               
                itemController = hit.collider.GetComponent<ItemController>();

                if (itemController != null)
                {
                    //These lines handles if the item is set to InteractOnly
                    if (itemController.interactOnly)
                    {
                        
                        Debug.Log("InteractOnly branch");

                        
                        #region InteractOnly
                        crosshairColorChange = true;
                        if (crosshairColorChange) { itemUI.crosshair.color = itemUI.interactCrosshairColor; itemUI.interactKeyBG.SetActive(true); }
                        if (Input.GetKeyDown(itemUI.interactKey))
                        {  
                            OnInteractKeyPressed();
                            TextAnimations();
                        }
                        #endregion
                    }
                    //These lines handles if the item is set to ExamineOnly
                    else if (itemController.examineOnly)
                    {
                        
                        Debug.Log("ExamineOnly branch");

                        
                        #region ExamineOnly
                        crosshairColorChange = true;
                        if (crosshairColorChange) { itemUI.crosshair.color = itemUI.bothCrosshairColor; }
                        if (!examining)
                        {
                            itemUI.examineKeyBG.SetActive(true);
                            if (Input.GetKeyDown(itemUI.examineKey) && !startAgain)
                            {
                                inspected = hit.transform.gameObject;
                                originalPos = hit.transform.position;
                                originalRot = hit.transform.rotation;
                                OnExamineKeyPressed();
                                TextAnimations();
                            }
                        }
                        OnExamining();
                        #endregion
                    }
                    //These lines handles if the item is set to both InteractAndExamine
                    else if (itemController.both)
                    {
                        #region InteractAndExamine
                        crosshairColorChange = true;
                        if (crosshairColorChange) { itemUI.crosshair.color = itemUI.bothCrosshairColor; }
                        if (!examining)
                        {
                            itemUI.interactKeyBG.SetActive(true);
                            itemUI.examineKeyBG.SetActive(true);
                            if (Input.GetKeyDown(itemUI.examineKey) && !startAgain)
                            {
                                inspected = hit.transform.gameObject;
                                originalPos = hit.transform.position;
                                originalRot = hit.transform.rotation;
                                OnExamineKeyPressed();
                                TextAnimations();
                            }
                            else if (Input.GetKeyDown(itemUI.interactKey))
                            {
                                OnInteractKeyPressed();
                                TextAnimations();
                            }
                        }
                        OnExamining();
                        #endregion
                    }
                    //These lines handles if the item is set to NoteSystem
                    else if (itemController.note)
                    {
                        #region NoteSystem
                        crosshairColorChange = true;
                        if (crosshairColorChange) { itemUI.crosshair.color = itemUI.bothCrosshairColor; }
                        if (!examining)
                        {
                            itemUI.examineKeyBG.SetActive(true);
                            if (Input.GetKeyDown(itemUI.examineKey) && !startAgain)
                            {
                                inspected = hit.transform.gameObject;
                                originalPos = hit.transform.position;
                                originalRot = hit.transform.rotation;
                                OnExamineKeyPressed();
                                TextAnimations();
                            }
                        }
                        OnExamining();
                        #endregion
                    }
                }
            }
            else
            {
                crosshairColorChange = false;
                itemUI.crosshair.color = Color.white;
                itemUI.examineKeyBG.SetActive(false);
                itemUI.interactKeyBG.SetActive(false);
                Back();
            }
            
            if (inspecting)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    timer = 0;
                    HideInfo();
                }
            }
        }
        //This function works when InteractKey is pressed
        void OnInteractKeyPressed()
        {
            timer = itemController.timeToDisplay;
            inspecting = true;
        }
        //This function works when ExamineKey is pressed
        void OnExamineKeyPressed()
        {
            if (itemController.playPickUpSound)
            {
                audioSource.clip = itemController.itemPickUpSound;
                audioSource.Play();
            }
            if (itemController.itemFaceCamera) { faceCam = true; }
            inspected.transform.SetParent(examineItemPos);
            inspecting = false;
            onlyExamine = true;
            examining = true;
        }

        /* This function handles Text Animations
        I have used LeanTween Plugin for that which is free on unity asset store you can use your own custom animations
        just replace wherever LeanTween is written with your own animation code
        This shouldn't be difficult I assume you have basic knowledge of C# */
        void TextAnimations()
        {
            if (itemController.interactOnly || itemController.examineOnly || itemController.both)
            {
                LeanTween.scale(itemUI.itemNameBG, new Vector3(0, 0, 0), 0).setEase(LeanTweenType.linear);
                LeanTween.scale(itemUI.itemInfoBG, new Vector3(0, 0, 0), 0).setEase(LeanTweenType.linear);
            }
            if (itemController.examineOnly || itemController.both)
            {
                if (!itemController.enableZoom) { LeanTween.scale(itemUI.putBackBGNoZoom, new Vector3(0, 0, 0), 0).setEase(LeanTweenType.linear); }
                else { LeanTween.scale(itemUI.putBackBG, new Vector3(0, 0, 0), 0).setEase(LeanTweenType.linear); }
            }
            itemController.ObjectDescription();
        }
        void OnExamining()
        {
            if (examining)
            {
                OnInspection();
            }
            else if (inspected != null)
            {
                Back();
            }
        }
        //This function works when item is being examined
        void OnInspection()
        {
            #region Inspection
            //If user checked cursor visible option in the inspector 
            if (itemController.examineCursorVisible)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            playerMovement.enabled = false;
            if (!onlyExamine) { HideInfo();}
            itemUI.crosshair.enabled = false;

            //This provide smooth movement of item from its original position to target position
            inspected.transform.position = Vector3.Lerp(inspected.transform.position, examineItemPos.position, itemController.lerpSpeed * Time.deltaTime);

            //This is used by object to face it towards camera
            if (faceCam)
            {
                inspected.transform.rotation = Quaternion.RotateTowards(inspected.transform.rotation, Quaternion.LookRotation(examineCam.transform.forward, examineCam.transform.up) * Quaternion.Euler(itemController.itemRotation), (itemController.lerpSpeed * 100) * Time.deltaTime);
            }
            SetFocused(inspected);
            myVolume.profile = examineBlur;   
            //These lines handles rotation of item which is being examined
            if (Input.GetMouseButton(0) && !itemController.noRotation)
            {
                float xAxis = Input.GetAxis("Mouse X") * itemController.rotateSpeed;
                float yAxis = Input.GetAxis("Mouse Y") * itemController.rotateSpeed;

                //Rotations are different for items which are facing towards camera 

                if (itemController.horizontal && !faceCam && itemController.itemFaceCamera) {inspected.transform.Rotate(Vector3.down, xAxis, Space.Self); }
                else if(itemController.vertical && !faceCam && itemController.itemFaceCamera) {inspected.transform.Rotate(examineItemPos.transform.right, yAxis, Space.World); }
                else if(itemController.horizontal && !itemController.itemFaceCamera) { inspected.transform.Rotate(Vector3.down, xAxis, Space.World); }
                else if (itemController.vertical && !itemController.itemFaceCamera) { inspected.transform.Rotate(examineItemPos.transform.right, yAxis, Space.World); }
                else if (itemController.bothRotation && !faceCam && itemController.itemFaceCamera)
                {  
                    inspected.transform.Rotate(Vector3.down, xAxis, Space.Self);
                    inspected.transform.Rotate(examineItemPos.transform.right, yAxis, Space.World);
                }
                else if(itemController.bothRotation && !itemController.itemFaceCamera)
                {   
                    inspected.transform.Rotate(Vector3.down, xAxis, Space.World);
                    inspected.transform.Rotate(examineItemPos.transform.right, yAxis, Space.World);
                }
            }
            //This works when the item enable zoom option is checked in the inspector
            if (itemController.enableZoom)
            {
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    examineCam.GetComponent<Camera>().fieldOfView += itemController.zoomScrollSpeed * 70 * Time.deltaTime;
                }
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    examineCam.GetComponent<Camera>().fieldOfView -= itemController.zoomScrollSpeed * 70 * Time.deltaTime;
                }
                examineCam.GetComponent<Camera>().fieldOfView = Mathf.Clamp(examineCam.GetComponent<Camera>().fieldOfView, itemController.minZoomFOV, examineOriginalFOV);
            }
 
            StartCoroutine(DropItem());

            //This disables interact and examine key while examining item
            itemUI.interactKeyBG.SetActive(false);
            itemUI.examineKeyBG.SetActive(false);

            startAgain = true;
            #endregion
        }

        //This function handles background blur while examining item
        void SetFocused(GameObject obj)
        {
            examineCam.SetActive(true);
            if (currentlyFocused) currentlyFocused.layer = previousLayer;
            currentlyFocused = obj;
            if (currentlyFocused)
            {
                previousLayer = currentlyFocused.layer;
                foreach (Transform child in currentlyFocused.GetComponentsInChildren<Transform>(true))
                {
                    child.gameObject.layer = LayerMask.NameToLayer(examineLayerName);  
                }
            }
            else
            {
                foreach (Transform child in inspected.GetComponentsInChildren<Transform>(true))
                {
                    child.gameObject.layer = previousLayer;
                }
            }
        }

        //This coroutine handles dropping item to its original position
        IEnumerator DropItem()
        {
            yield return new WaitForSeconds(0.2f);
            faceCam = false;
            yield return new WaitForSeconds(itemController.antiSpamTime);
            if (Input.GetKeyDown(itemUI.examineKey) && examining)
            {
                ItemDropped();
                yield return new WaitForSeconds(0.3f);
                playerMovement.enabled = true;
                yield return new WaitForSeconds(itemController.antiSpamTime); 
                examineCam.SetActive(false);
                startAgain = false;    
            }
        }
        void ItemDropped()
        {
            if (itemController.playPickUpSound) {audioSource.clip = itemController.itemPickUpSound;
            audioSource.Play(); }
            examineCam.GetComponent<Camera>().fieldOfView = examineOriginalFOV;
            _cam.fieldOfView = originalFOV;
            SetFocused(null);
            examining = false;
            onlyExamine = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            itemUI.crosshair.enabled = true;
            HideInfo();
            myVolume.profile = defaultProfile;
        }
        // This function returns item to its original position with smooth movement after being examined
        void Back()
        {
            if (inspected != null && itemController != null)
            {
                 inspected.transform.SetParent(null);
                 inspected.transform.position= Vector3.Lerp(inspected.transform.position, originalPos, itemController.lerpSpeed * Time.deltaTime);
                 inspected.transform.rotation = Quaternion.Lerp(inspected.transform.rotation, originalRot, itemController.lerpSpeed * Time.deltaTime);
            }
        }

        //This function shows name of the item
        public void ObjectName(string objName)
        {
            if (!itemController.note || itemController.showName)
            {
                itemUI.itemNameText.text = objName;
                itemUI.itemNameBG.SetActive(true);
                LeanTween.scale(itemUI.itemNameBG, new Vector3(1.4f, 1.4f, 1.4f), itemUI.animationSpeed / 10).setEase(LeanTweenType.linear);
            }
        }

        //This function shows extra info of the item
        public void ObjectDescription(string objInfo)
        {
            if (onlyExamine && itemController.enableZoom) { LeanTween.scale(itemUI.putBackBG, new Vector3(1.4f, 1.4f, 1.4f), itemUI.animationSpeed / 10).setEase(LeanTweenType.linear); itemUI.itemInfoBG.SetActive(false); }
            else if (!itemController.enableZoom && !inspecting || onlyExamine) { LeanTween.scale(itemUI.putBackBGNoZoom, new Vector3(1.4f, 1.4f, 1.4f), itemUI.animationSpeed / 10).setEase(LeanTweenType.linear); itemUI.itemInfoBG.SetActive(false); }
            else
            {
                itemUI.itemInfoText.text = objInfo;
                itemUI.itemInfoBG.SetActive(true);
                itemUI.itemInfoBG.SetActive(true);
                LeanTween.scale(itemUI.itemInfoBG, new Vector3(1.4f, 1.4f, 1.4f), itemUI.animationSpeed / 10).setEase(LeanTweenType.linear);
            }
        
        }

        //This function hide item info after some time which can be modified in the inspector
        void HideInfo()
        {
            if (inspecting || !onlyExamine)
            {
                LeanTween.scale(itemUI.itemNameBG, new Vector3(0, 0, 0), itemUI.animationSpeed / 10).setEase(LeanTweenType.linear).setOnComplete(TurningOffUI);
                LeanTween.scale(itemUI.itemInfoBG, new Vector3(0, 0, 0), itemUI.animationSpeed / 10).setEase(LeanTweenType.linear).setOnComplete(TurningOffUI);
                if (!itemController.enableZoom) { LeanTween.scale(itemUI.putBackBGNoZoom, new Vector3(0, 0, 0), itemUI.animationSpeed / 10).setEase(LeanTweenType.linear); }
                else { LeanTween.scale(itemUI.putBackBG, new Vector3(0, 0, 0), itemUI.animationSpeed / 10).setEase(LeanTweenType.linear); }
            }
            timer = itemController.timeToDisplay;
            inspecting = false;
        }
        void TurningOffUI()
        {
            itemUI.itemNameBG.SetActive(false);
            itemUI.itemInfoBG.SetActive(false);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RaycastItem))]
    public class RaycastItemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;


            EditorGUILayout.LabelField("Raycast Item Script", header);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("This script detects objects in the game view.", guiMessageStyle);
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