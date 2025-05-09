using UnityEngine;

namespace ExamineSystem
{
    [RequireComponent(typeof(Camera))]
    public class ExamineInteractor : MonoBehaviour
    {
        [SerializeField] private float interactDistance = 5;

        private ExaminableItem examinableItem;
        private Camera _camera;

        void Start()
        {
            if (!TryGetComponent<Camera>(out _camera))
            {
                Debug.LogError("Camera component not found on the GameObject.");
            }
        }

        void Update()
        {
            if (Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)), transform.forward, out RaycastHit hit, interactDistance))
            {
                var examineItem = hit.collider.GetComponent<ExaminableItem>();
                if (examineItem != null)
                {
                    examinableItem = examineItem;
                    examinableItem.ItemHighlight(true);
                    HighlightCrosshair(true);
                }
                else
                {
                    ClearExaminable();
                }
            }
            else
            {
                ClearExaminable();
            }

            if (examinableItem != null)
            {
                if (Input.GetKeyDown(ExamineInputManager.instance.interactKey))
                {
                    examinableItem.ExamineObject();
                }
            }
        }

        private void ClearExaminable()
        {
            if (examinableItem != null)
            {
                examinableItem.ItemHighlight(false);
                HighlightCrosshair(false);
                examinableItem = null;
            }
        }

        void HighlightCrosshair(bool on)
        {
            ExamineUIManager.instance.HighlightCrosshair(on);
        }
    }
}
