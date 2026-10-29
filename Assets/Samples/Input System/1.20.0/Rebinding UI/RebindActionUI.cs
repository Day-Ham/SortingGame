using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEngine.InputSystem.Samples.RebindUI
{
    public class RebindActionUI : MonoBehaviour
    {
        public InputActionReference actionReference
        {
            get => m_Action;
            set { m_Action = value; UpdateActionLabel(); UpdateBindingDisplay(); }
        }

        public string bindingId
        {
            get => m_BindingId;
            set { m_BindingId = value; UpdateBindingDisplay(); }
        }

        public InputBinding.DisplayStringOptions displayStringOptions
        {
            get => m_DisplayStringOptions;
            set { m_DisplayStringOptions = value; UpdateBindingDisplay(); }
        }

        public TMP_Text actionLabel
        {
            get => m_ActionLabel;
            set { m_ActionLabel = value; UpdateActionLabel(); }
        }

        public TMP_Text bindingText
        {
            get => m_BindingText;
            set { m_BindingText = value; UpdateBindingDisplay(); }
        }

        public TMP_Text rebindPrompt { get => m_RebindText; set => m_RebindText = value; }
        public TMP_Text rebindInfo { get => m_RebindInfo; set => m_RebindInfo = value; }
        public Button rebindCancelButton { get => m_RebindCancelButton; set => m_RebindCancelButton = value; }
        public GameObject rebindOverlay { get => m_RebindOverlay; set => m_RebindOverlay = value; }

        public UpdateBindingUIEvent updateBindingUIEvent
        {
            get => m_UpdateBindingUIEvent ??= new UpdateBindingUIEvent();
        }

        public InteractiveRebindEvent startRebindEvent
        {
            get => m_RebindStartEvent ??= new InteractiveRebindEvent();
        }

        public InteractiveRebindEvent stopRebindEvent
        {
            get => m_RebindStopEvent ??= new InteractiveRebindEvent();
        }

        public InputActionRebindingExtensions.RebindingOperation ongoingRebind => m_RebindOperation;

        private static List<RebindActionUI> s_RebindActionUIs;

        private InputActionRebindingExtensions.RebindingOperation m_RebindOperation;
        private RebindActionUI m_DuplicateBinding;
        private InputAction m_PendingAction;
        private int m_PendingBindingIndex;
        private bool m_PendingAllCompositeParts;
        private string m_PendingOriginalPath;
        private bool m_DuplicateRejected;
        private bool m_IsCancellingDuplicate;
        private double m_RebindStartTime = -1;
        private int m_LastRemainingTimeoutSeconds;

        [SerializeField] private InputActionReference m_Action;
        [SerializeField] private string m_BindingId;
        [SerializeField] private InputBinding.DisplayStringOptions m_DisplayStringOptions;
        [SerializeField] private TMP_Text m_ActionLabel;
        [SerializeField] private TMP_Text m_BindingText;
        [SerializeField] private GameObject m_RebindOverlay;
        [SerializeField] private TMP_Text m_RebindText;
        [SerializeField] private GameObject m_RebindDuplicatePromp;
        [SerializeField] private Button m_ConfirmSwapButton;
        [SerializeField] private Button m_CancelSwapButton;
        [SerializeField] private TMP_Text m_RebindInfo;
        [SerializeField] private Button m_RebindCancelButton;
        [SerializeField] private float m_RebindTimeout;
        [SerializeField] private UpdateBindingUIEvent m_UpdateBindingUIEvent;
        [SerializeField] private InteractiveRebindEvent m_RebindStartEvent;
        [SerializeField] private InteractiveRebindEvent m_RebindStopEvent;

        public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
        {
            action = m_Action?.action;
            bindingIndex = action.FindBindingById(m_BindingId);
            if (bindingIndex >= 0) return true;
            if (action != null && !string.IsNullOrEmpty(m_BindingId))
                Debug.LogError($"Cannot find binding with ID '{m_BindingId}' on '{action}'", this);
            return false;
        }

        public void UpdateBindingDisplay()
        {
            string displayString = string.Empty;
            string deviceLayoutName = null;
            string controlPath = null;
            InputAction action = m_Action?.action;

            if (action != null)
            {
                int index = action.bindings.IndexOf(x => x.id.ToString() == m_BindingId);
                if (index != -1)
                    displayString = action.GetBindingDisplayString(index, out deviceLayoutName, out controlPath, m_DisplayStringOptions);
            }

            if (m_BindingText != null) m_BindingText.text = displayString;
            m_UpdateBindingUIEvent?.Invoke(this, displayString, deviceLayoutName, controlPath);
        }

        public void ResetToDefault()
        {
            if (!ResolveActionAndBinding(out InputAction action, out int index)) return;

            if (action.bindings[index].isComposite)
            {
                for (int i = index + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
                    action.RemoveBindingOverride(i);
            }
            else action.RemoveBindingOverride(index);

            UpdateBindingDisplay();
        }

        public void SwapBinding(RebindActionUI other, string originalPath)
        {
            if (this == other) return;
            if (ongoingRebind != null || other.ongoingRebind != null)
                throw new Exception("Cannot swap bindings when interactive rebinding is ongoing");
            if (!ResolveActionAndBinding(out InputAction action, out int index) ||
                !other.ResolveActionAndBinding(out InputAction otherAction, out int otherIndex))
                throw new Exception("Failed to resolve action and binding index");

            if (m_Action.action.actionMap.name != "Player" || other.actionReference.action.actionMap.name != "Player")
                return;

            string otherPath = otherAction.bindings[otherIndex].effectivePath;
            action.ApplyBindingOverride(index, otherPath);
            otherAction.ApplyBindingOverride(otherIndex, originalPath);
            UpdateBindingDisplay();
            other.UpdateBindingDisplay();
            CloseDuplicatePrompt();
        }

        private void ConfirmSwapBinding()
        {
            if (m_DuplicateBinding == null)
            {
                Debug.LogError("Duplicate binding UI is NULL!");
                return;
            }
            SwapBinding(m_DuplicateBinding, m_PendingOriginalPath);
        }

        private void CancelSwapBinding()
        {
            if (m_IsCancellingDuplicate) return;
            m_IsCancellingDuplicate = true;
            CloseDuplicatePrompt();
            UpdateBindingDisplay();
            StartCoroutine(RestartRebindAfterRelease());
        }

        private void CloseDuplicatePrompt()
        {
            if (m_RebindDuplicatePromp != null) m_RebindDuplicatePromp.SetActive(false);
            if (m_RebindOverlay != null) m_RebindOverlay.SetActive(false);
            if (m_ConfirmSwapButton != null) m_ConfirmSwapButton.onClick.RemoveListener(ConfirmSwapBinding);
            if (m_CancelSwapButton != null) m_CancelSwapButton.onClick.RemoveListener(CancelSwapBinding);
        }

        private IEnumerator RestartRebindAfterRelease()
        {
            while (AnyCancelControlPressed()) yield return null;
            yield return null;
            yield return null;

            m_IsCancellingDuplicate = false;
            //if (m_PendingAction != null)
               // PerformInteractiveRebind(m_PendingAction, m_PendingBindingIndex, m_PendingAllCompositeParts);
        }

        private bool AnyCancelControlPressed()
        {
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;
            return (keyboard != null && keyboard.escapeKey.isPressed) ||
                   (mouse != null && (mouse.leftButton.isPressed || mouse.rightButton.isPressed || mouse.middleButton.isPressed));
        }

        public void StartInteractiveRebind()
        {
            if (!ResolveActionAndBinding(out InputAction action, out int index)) return;

            if (action.actionMap.name != "Player") return;

            if (action.bindings[index].isComposite)
            {
                int firstPart = index + 1;
                if (firstPart < action.bindings.Count && action.bindings[firstPart].isPartOfComposite)
                    PerformInteractiveRebind(action, firstPart, true);
            }
            else PerformInteractiveRebind(action, index);
        }

        private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            m_RebindOperation?.Cancel();
            bool wasEnabled = action.enabled;
            if (wasEnabled) action.actionMap.Disable();

            string originalPath = action.bindings[bindingIndex].effectivePath;
            m_DuplicateRejected = false;

            void CleanUp()
            {
                if (m_RebindCancelButton != null) m_RebindCancelButton.onClick.RemoveListener(CancelRebind);
                m_RebindOperation?.Dispose();
                m_RebindOperation = null;
                if (wasEnabled) action.actionMap.Enable();
                if (m_RebindOverlay != null) m_RebindOverlay.SetActive(false);
            }

            m_RebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                .WithCancelingThrough("<Keyboard>/escape")
                .WithControlsExcluding("<Keyboard>/escape")
                .WithActionEventNotificationsBeingSuppressed()
                .WithTimeout(m_RebindTimeout)
                .OnPotentialMatch(operation =>
                {
                    InputControl candidateControl = operation.selectedControl;
                    if (candidateControl == null)
                    {
                        Debug.LogWarning("Rebind matched, but selectedControl was null.", this);
                        operation.Cancel();
                        return;
                    }

                    if (CheckDuplicateBindings(action, bindingIndex, candidateControl, out RebindActionUI duplicate))
                    {
                        Debug.Log($"Rejected duplicate control: {candidateControl.path}", this);
                        m_PendingAction = action;
                        m_PendingBindingIndex = bindingIndex;
                        m_PendingOriginalPath = originalPath;
                        m_PendingAllCompositeParts = allCompositeParts;
                        m_DuplicateBinding = duplicate;
                        m_DuplicateRejected = true;
                        operation.Cancel();
                        return;
                    }

                    Debug.Log($"Accepted rebind candidate: {candidateControl.path}", this);
                    operation.Complete();
                })
                .OnCancel(operation =>
                {
                    bool showDuplicate = m_DuplicateRejected;
                    m_RebindStopEvent?.Invoke(this, operation);
                    UpdateBindingDisplay();
                    CleanUp();

                    if (showDuplicate)
                    {
                        m_DuplicateRejected = false;
                        ShowDuplicatePrompt(m_PendingAction, m_PendingBindingIndex, m_DuplicateBinding,
                            m_PendingOriginalPath, m_PendingAllCompositeParts);
                    }
                })
                .OnComplete(operation =>
                {
                    m_RebindStopEvent?.Invoke(this, operation);
                    UpdateBindingDisplay();
                    CleanUp();

                    if (allCompositeParts)
                    {
                        int next = bindingIndex + 1;
                        if (next < action.bindings.Count && action.bindings[next].isPartOfComposite)
                            PerformInteractiveRebind(action, next, true);
                    }
                });

            string partName = action.bindings[bindingIndex].isPartOfComposite
                ? $"Binding '{action.bindings[bindingIndex].name}'. "
                : string.Empty;

            if (m_RebindOverlay != null) m_RebindOverlay.SetActive(true);
            if (m_RebindText != null)
            {
                string expected = !string.IsNullOrEmpty(m_RebindOperation.expectedControlType)
                    ? $"Waiting for {m_RebindOperation.expectedControlType} input..."
                    : "Waiting for input...";
                m_RebindText.text = partName + expected;
            }

            if (m_RebindCancelButton != null)
            {
                m_RebindCancelButton.onClick.RemoveListener(CancelRebind);
                m_RebindCancelButton.onClick.AddListener(CancelRebind);
            }

            if (m_RebindInfo != null)
            {
                m_RebindStartTime = Time.realtimeSinceStartup;
                UpdateRebindInfo(m_RebindStartTime);
            }

            if (m_RebindOverlay == null && m_RebindText == null && m_RebindStartEvent == null && m_BindingText != null)
                m_BindingText.text = "<Waiting...>";

            m_RebindStartEvent?.Invoke(this, m_RebindOperation);
            m_RebindOperation.Start();
        }

        private void ShowDuplicatePrompt(InputAction action, int bindingIndex, RebindActionUI duplicateBinding,
            string originalPath, bool allCompositeParts = false)
        {
            m_PendingAction = action;
            m_PendingBindingIndex = bindingIndex;
            m_PendingOriginalPath = originalPath;
            m_PendingAllCompositeParts = allCompositeParts;
            m_DuplicateBinding = duplicateBinding;

            if (m_RebindDuplicatePromp != null) m_RebindDuplicatePromp.SetActive(true);
            if (m_ConfirmSwapButton != null)
            {
                m_ConfirmSwapButton.onClick.RemoveListener(ConfirmSwapBinding);
                m_ConfirmSwapButton.onClick.AddListener(ConfirmSwapBinding);
            }
            if (m_CancelSwapButton != null)
            {
                m_CancelSwapButton.onClick.RemoveListener(CancelSwapBinding);
                m_CancelSwapButton.onClick.AddListener(CancelSwapBinding);
            }
        }

        private bool CheckDuplicateBindings(InputAction action, int bindingIndex, InputControl candidateControl, out RebindActionUI duplicateBinding)
        {
            duplicateBinding = null;
            if (candidateControl == null || action?.actionMap?.asset == null) return false;

            if (action.actionMap.name != "Player") return false;

            string targetBindingId = action.bindings[bindingIndex].id.ToString();
            InputActionAsset asset = action.actionMap.asset;

            InputActionMap playerMap = asset.FindActionMap("Player", true);
            

            foreach (InputAction otherAction in playerMap.actions)
            {
                for (int i = 0; i < otherAction.bindings.Count; i++)
                    {
                        InputBinding otherBinding = otherAction.bindings[i];
                        if (otherAction == action && otherBinding.id.ToString() == targetBindingId)
                            continue;
                        if (otherBinding.isComposite || string.IsNullOrEmpty(otherBinding.effectivePath))
                            continue;
                        if (!InputControlPath.Matches(otherBinding.effectivePath, candidateControl))
                            continue;

                        foreach (RebindActionUI ui in s_RebindActionUIs)
                        {
                            if (ui != null && ui.ResolveActionAndBinding(out InputAction uiAction, out int uiIndex) &&
                                uiAction == otherAction && uiIndex == i)
                            {
                                duplicateBinding = ui;
                                Debug.Log($"Duplicate binding found: {candidateControl.path} is already used by {otherAction.name} ({otherBinding.effectivePath}).", this);
                                return true;
                            }
                        }

                        Debug.LogWarning($"Duplicate binding found at {otherAction.name}/{otherBinding.name} ({otherBinding.effectivePath}), but no RebindActionUI exists for that binding, so it cannot be swapped from this prompt.", this);
                        return true;
                    }
                }
            

            return false;
        }

        private void Update()
        {
            if (m_RebindInfo != null) UpdateRebindInfo(Time.realtimeSinceStartupAsDouble);

            if (m_RebindDuplicatePromp != null && m_RebindDuplicatePromp.activeInHierarchy &&
                Keyboard.current != null && Keyboard.current.escapeKey.wasReleasedThisFrame)
                CancelSwapBinding();
        }

        private void UpdateRebindInfo(double now)
        {
            if (m_RebindOperation == null) return;
            int remaining = (int)Math.Floor(m_RebindOperation.timeout - (now - m_RebindStartTime));
            if (remaining == m_LastRemainingTimeoutSeconds) return;

            m_RebindInfo.text = m_RebindOperation.timeout > 0
                ? $"Cancels in <b>{remaining}</b> seconds if no matching input is provided."
                : string.Empty;
            m_LastRemainingTimeoutSeconds = remaining;
        }

        private void CancelRebind() => m_RebindOperation?.Cancel();

        private void OnEnable()
        {
            s_RebindActionUIs ??= new List<RebindActionUI>();
            if (!s_RebindActionUIs.Contains(this)) s_RebindActionUIs.Add(this);
            if (s_RebindActionUIs.Count == 1) InputSystem.onActionChange += OnActionChange;
            UpdateBindingDisplay();
        }

        private void OnDisable()
        {
            m_RebindOperation?.Dispose();
            m_RebindOperation = null;
            if (s_RebindActionUIs != null)
            {
                s_RebindActionUIs.Remove(this);
                if (s_RebindActionUIs.Count == 0)
                {
                    s_RebindActionUIs = null;
                    InputSystem.onActionChange -= OnActionChange;
                }
            }
            UpdateBindingDisplay();
        }

        private static void OnActionChange(object obj, InputActionChange change)
        {
            if (change != InputActionChange.BoundControlsChanged || s_RebindActionUIs == null) return;

            InputAction action = obj as InputAction;
            InputActionMap map = action?.actionMap ?? obj as InputActionMap;
            InputActionAsset asset = map?.asset ?? obj as InputActionAsset;

            foreach (RebindActionUI component in s_RebindActionUIs)
            {
                InputAction referenced = component.actionReference?.action;
                if (referenced == null) continue;
                if (referenced == action || referenced.actionMap == map || referenced.actionMap?.asset == asset)
                    component.UpdateBindingDisplay();
            }
        }

        private void UpdateActionLabel()
        {
            if (m_ActionLabel == null) return;
            InputAction action = m_Action?.action;
            m_ActionLabel.text = action != null ? action.name : string.Empty;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateActionLabel();
            UpdateBindingDisplay();
        }
#endif

        [Serializable]
        public class UpdateBindingUIEvent : UnityEvent<RebindActionUI, string, string, string> { }

        [Serializable]
        public class InteractiveRebindEvent : UnityEvent<RebindActionUI, InputActionRebindingExtensions.RebindingOperation> { }
    }
}
