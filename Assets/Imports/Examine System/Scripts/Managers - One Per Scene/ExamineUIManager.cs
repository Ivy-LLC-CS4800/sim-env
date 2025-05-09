using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ExamineSystem
{
    public class ExamineUIManager : MonoBehaviour
    {
        [Header("No UI Close Button")]
        [SerializeField] private GameObject noUICloseButton = null;

        [Header("Basic Example UI References")]
        [SerializeField] private TMP_Text basicItemNameUI = null;
        [SerializeField] private TMP_Text basicItemDescUI = null;
        [SerializeField] private GameObject basicExamineUI = null;

        [Header("Right Side Example UI References")]
        [SerializeField] private TMP_Text rightItemNameUI = null;
        [SerializeField] private TMP_Text rightItemDescUI = null;
        [SerializeField] private GameObject rightExamineUI = null;

        [Header("Highlight Name UI")]
        [SerializeField] private CanvasGroup highlightNameCanvas = null;
        [SerializeField] private TMP_Text highlightItemNameUI = null;
        [SerializeField] private GameObject highlightNameHelpPrompt = null;

        [Header("Interest Point UI's")]
        [SerializeField] private TMP_Text interestPointText = null;
        [SerializeField] private GameObject interestPointParentUI = null;

        [Header("Help Panel Visibility")]
        [SerializeField] private bool showHelp = false;
        [SerializeField] private GameObject examineHelpUI = null;

        [Header("Crosshair")]
        [SerializeField] private Image uiCrosshair = null;

        [Header("Should persist?")]
        [SerializeField] private bool persistAcrossScenes = true;

        private ExaminableItem _examinableItem;

        public ExaminableItem examinableItem
        {
            get { return _examinableItem; }
            set { _examinableItem = value; }
        }

        public static ExamineUIManager instance;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                if (persistAcrossScenes)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            FieldNullCheck();
        }

        public void SetInspectPointParent(bool on, Vector3 position)
        {
            interestPointParentUI.SetActive(on);
            interestPointParentUI.transform.position = position;
        }

        public void SetInspectPointText(string inspectText)
        {
            interestPointText.text = inspectText;
        }

        public void SetBasicUIText(string itemName, string itemDescription, bool on)
        {
            basicItemNameUI.text = itemName;
            basicItemDescUI.text = itemDescription;
            basicExamineUI.SetActive(on);
        }

        public void SetBasicUITextSettings(int textSize, TMP_FontAsset fontType, Color fontColor, int textSizeDesc, TMP_FontAsset fontTypeDesc, Color fontColorDesc)
        {
            basicItemNameUI.fontSize = textSize;
            basicItemNameUI.font = fontType;
            basicItemNameUI.color = fontColor;

            basicItemDescUI.fontSize = textSizeDesc;
            basicItemDescUI.font = fontTypeDesc;
            basicItemDescUI.color = fontColorDesc;
        }

        public void SetRightSideUIText(string itemName, string itemDescription, bool on)
        {
            rightItemNameUI.text = itemName;
            rightItemDescUI.text = itemDescription;
            rightExamineUI.SetActive(on);
        }

        public void SetRightUITextSettings(int textSize, TMP_FontAsset fontType, Color fontColor, int textSizeDesc, TMP_FontAsset fontTypeDesc, Color fontColorDesc)
        {
            rightItemNameUI.fontSize = textSize;
            rightItemNameUI.font = fontType;
            rightItemNameUI.color = fontColor;

            rightItemDescUI.fontSize = textSizeDesc;
            rightItemDescUI.font = fontTypeDesc;
            rightItemDescUI.color = fontColorDesc;
        }

        public void SetHighlightName(string itemName, bool on, bool showPrompt)
        {
            highlightItemNameUI.text = itemName;
            highlightNameCanvas.alpha = on ? 1 : 0; // Fully visible if 'on' is true, otherwise fully transparent
            highlightNameHelpPrompt.SetActive(showPrompt);
        }

        public void ShowCloseButton(bool on)
        {
            noUICloseButton.SetActive(on);
        }

        public void CloseButton()
        {
            examinableItem.DropObject(true);
        }

        public void ShowHelpPrompt(bool on)
        {
            if (showHelp)
            {
                examineHelpUI.SetActive(on);
            }
        }

        public void EnableCrosshair(bool on)
        {
            uiCrosshair.enabled = on;
            Cursor.lockState = on ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !on;
        }

        public void HighlightCrosshair(bool on)
        {
            uiCrosshair.color = on ? Color.red : Color.white;
        }

        void FieldNullCheck()
        {
            // Checking each field and logging an error if it is null
            CheckField(noUICloseButton, "NoUICloseButton");

            CheckField(basicItemNameUI, "BasicItemNameUI");
            CheckField(basicItemDescUI, "BasicItemDescUI");
            CheckField(basicExamineUI, "BasicExamineUI");

            CheckField(rightItemNameUI, "RightItemNameUI");
            CheckField(rightItemDescUI, "RightItemDescUI");
            CheckField(rightExamineUI, "RightExamineUI");

            CheckField(highlightNameCanvas, "HighlightNameCanvas");
            CheckField(highlightItemNameUI, "HighlightItemNameUI");
            CheckField(highlightNameHelpPrompt, "HighlightNameHelpPrompt");

            CheckField(interestPointText, "InterestPointText");
            CheckField(interestPointParentUI, "InterestPointParentUI");

            CheckField(examineHelpUI, "ExamineHelpUI");

            CheckField(uiCrosshair, "UICrosshair");
        }

        void CheckField(Object field, string fieldName)
        {
            if (field == null)
            {
                Debug.LogError($"FieldNullCheck: {fieldName} is not set in the inspector!");
            }
        }
    }
}
