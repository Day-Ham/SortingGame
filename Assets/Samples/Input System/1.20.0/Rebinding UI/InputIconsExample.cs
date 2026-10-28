using UnityEngine.UI;
using static UnityEngine.InputSystem.Samples.RebindUI.GamepadIconsExample;
using static UnityEngine.InputSystem.Samples.RebindUI.KeyboardAndMouseIconsExample;

namespace UnityEngine.InputSystem.Samples.RebindUI
{
    public class InputIconsExample : MonoBehaviour
    {
        [Header("Keyboard and Mouse")]
        public KeyboardMouseIcons keyboardMouse;

        [Header("Gamepad")]
        public GamepadIcons xbox;
        public GamepadIcons ps4;

        protected void OnEnable()
        {
            var components = transform.GetComponentsInChildren<RebindActionUI>(true);

            foreach (var component in components)
            {
                component.updateBindingUIEvent.AddListener(OnUpdateBindingDisplay);
                component.UpdateBindingDisplay();
            }
        }

        protected void OnDisable()
        {
            var components = transform.GetComponentsInChildren<RebindActionUI>(true);

            foreach (var component in components)
                component.updateBindingUIEvent.RemoveListener(OnUpdateBindingDisplay);
        }

        protected void OnUpdateBindingDisplay(
            RebindActionUI component,
            string bindingDisplayString,
            string deviceLayoutName,
            string controlPath)
        {
            if (string.IsNullOrEmpty(deviceLayoutName) ||
                string.IsNullOrEmpty(controlPath))
                return;

            Sprite icon = null;

            if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "DualShockGamepad"))
            {
                icon = ps4.GetSprite(controlPath);
            }
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Gamepad"))
            {
                icon = xbox.GetSprite(controlPath);
            }
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Keyboard") ||
                     InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Mouse"))
            {
                icon = keyboardMouse.GetSprite(controlPath);
            }

            var text = component.bindingText;
            var imageTransform = text.transform.parent.Find("ActionBindingIcon");

            if (imageTransform == null) return;

            var image = imageTransform.GetComponent<Image>();

            if (icon != null)
            {
                text.gameObject.SetActive(false);
                image.sprite = icon;
                image.gameObject.SetActive(true);
            }
            else
            {
                text.gameObject.SetActive(true);
                image.gameObject.SetActive(false);
            }
        }
    }
}
