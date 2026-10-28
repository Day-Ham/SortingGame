
using System;
using UnityEngine;

[Serializable]
public struct KeyboardMouseIcons
{
    [Header("Letters")]
    public Sprite a;
    public Sprite b;
    public Sprite c;
    public Sprite d;
    public Sprite e;
    public Sprite f;
    public Sprite g;
    public Sprite h;
    public Sprite i;
    public Sprite j;
    public Sprite k;
    public Sprite l;
    public Sprite m;
    public Sprite n;
    public Sprite o;
    public Sprite p;
    public Sprite q;
    public Sprite r;
    public Sprite s;
    public Sprite t;
    public Sprite u;
    public Sprite v;
    public Sprite w;
    public Sprite x;
    public Sprite y;
    public Sprite z;

    [Header("Number Row")]
    public Sprite digit0;
    public Sprite digit1;
    public Sprite digit2;
    public Sprite digit3;
    public Sprite digit4;
    public Sprite digit5;
    public Sprite digit6;
    public Sprite digit7;
    public Sprite digit8;
    public Sprite digit9;

    [Header("Function Keys")]
    public Sprite f1;
    public Sprite f2;
    public Sprite f3;
    public Sprite f4;
    public Sprite f5;
    public Sprite f6;
    public Sprite f7;
    public Sprite f8;
    public Sprite f9;
    public Sprite f10;
    public Sprite f11;
    public Sprite f12;
    public Sprite f13;
    public Sprite f14;
    public Sprite f15;
    public Sprite f16;
    public Sprite f17;
    public Sprite f18;
    public Sprite f19;
    public Sprite f20;
    public Sprite f21;
    public Sprite f22;
    public Sprite f23;
    public Sprite f24;

    [Header("Main Keys")]
    public Sprite space;
    public Sprite enter;
    public Sprite tab;
    public Sprite escape;
    public Sprite backspace;
    public Sprite insert;
    public Sprite delete;
    public Sprite home;
    public Sprite end;
    public Sprite pageUp;
    public Sprite pageDown;
    public Sprite printScreen;
    public Sprite pause;
    public Sprite contextMenu;
    public Sprite anyKey;

    [Header("Punctuation and Symbols")]
    public Sprite backquote;
    public Sprite minus;
    public Sprite equals;
    public Sprite leftBracket;
    public Sprite rightBracket;
    public Sprite backslash;
    public Sprite semicolon;
    public Sprite quote;
    public Sprite comma;
    public Sprite period;
    public Sprite slash;

    [Header("Modifier Keys")]
    public Sprite leftShift;
    public Sprite rightShift;
    public Sprite leftCtrl;
    public Sprite rightCtrl;
    public Sprite leftAlt;
    public Sprite rightAlt;
    public Sprite leftMeta;
    public Sprite rightMeta;
    public Sprite shift;
    public Sprite ctrl;
    public Sprite alt;
    public Sprite meta;

    [Header("Arrow Keys")]
    public Sprite upArrow;
    public Sprite downArrow;
    public Sprite leftArrow;
    public Sprite rightArrow;

    [Header("Locks")]
    public Sprite capsLock;
    public Sprite numLock;
    public Sprite scrollLock;

    [Header("Numpad Numbers")]
    public Sprite numpad0;
    public Sprite numpad1;
    public Sprite numpad2;
    public Sprite numpad3;
    public Sprite numpad4;
    public Sprite numpad5;
    public Sprite numpad6;
    public Sprite numpad7;
    public Sprite numpad8;
    public Sprite numpad9;

    [Header("Numpad Operations")]
    public Sprite numpadEnter;
    public Sprite numpadDivide;
    public Sprite numpadMultiply;
    public Sprite numpadPlus;
    public Sprite numpadMinus;
    public Sprite numpadPeriod;
    public Sprite numpadEquals;
    public Sprite numpadComma;

    [Header("Mouse Buttons")]
    public Sprite leftMouse;
    public Sprite rightMouse;
    public Sprite middleMouse;
    public Sprite forwardMouse;
    public Sprite backMouse;

    [Header("Mouse Movement and Scroll")]
    public Sprite mousePosition;
    public Sprite mouseDelta;
    public Sprite scrollWheel;
    public Sprite scrollUp;
    public Sprite scrollDown;

    public Sprite GetSprite(string controlPath)
    {
        string key = controlPath.ToLowerInvariant();

        if (key.EndsWith("/scroll/y"))
            return scrollWheel;

        int slashIndex = key.LastIndexOf('/');
        if (slashIndex >= 0)
            key = key.Substring(slashIndex + 1);

        switch (key)
        {
            case "a": return a;
            case "b": return b;
            case "c": return c;
            case "d": return d;
            case "e": return e;
            case "f": return f;
            case "g": return g;
            case "h": return h;
            case "i": return i;
            case "j": return j;
            case "k": return k;
            case "l": return l;
            case "m": return m;
            case "n": return n;
            case "o": return o;
            case "p": return p;
            case "q": return q;
            case "r": return r;
            case "s": return s;
            case "t": return t;
            case "u": return u;
            case "v": return v;
            case "w": return w;
            case "x": return x;
            case "y": return y;
            case "z": return z;

            case "0": return digit0;
            case "1": return digit1;
            case "2": return digit2;
            case "3": return digit3;
            case "4": return digit4;
            case "5": return digit5;
            case "6": return digit6;
            case "7": return digit7;
            case "8": return digit8;
            case "9": return digit9;

            case "f1": return f1;
            case "f2": return f2;
            case "f3": return f3;
            case "f4": return f4;
            case "f5": return f5;
            case "f6": return f6;
            case "f7": return f7;
            case "f8": return f8;
            case "f9": return f9;
            case "f10": return f10;
            case "f11": return f11;
            case "f12": return f12;
            case "f13": return f13;
            case "f14": return f14;
            case "f15": return f15;
            case "f16": return f16;
            case "f17": return f17;
            case "f18": return f18;
            case "f19": return f19;
            case "f20": return f20;
            case "f21": return f21;
            case "f22": return f22;
            case "f23": return f23;
            case "f24": return f24;

            case "space": return space;
            case "enter": return enter;
            case "tab": return tab;
            case "escape": return escape;
            case "backspace": return backspace;
            case "insert": return insert;
            case "delete": return delete;
            case "home": return home;
            case "end": return end;
            case "pageup": return pageUp;
            case "pagedown": return pageDown;
            case "printscreen": return printScreen;
            case "pause": return pause;
            case "contextmenu": return contextMenu;
            case "anykey": return anyKey;

            case "backquote": return backquote;
            case "minus": return minus;
            case "equals": return equals;
            case "leftbracket": return leftBracket;
            case "rightbracket": return rightBracket;
            case "backslash": return backslash;
            case "semicolon": return semicolon;
            case "quote": return quote;
            case "comma": return comma;
            case "period": return period;
            case "slash": return slash;

            case "leftshift": return leftShift != null ? leftShift : shift;
            case "rightshift": return rightShift != null ? rightShift : shift;
            case "shift": return shift;
            case "leftctrl": return leftCtrl != null ? leftCtrl : ctrl;
            case "rightctrl": return rightCtrl != null ? rightCtrl : ctrl;
            case "leftalt": return leftAlt != null ? leftAlt : alt;
            case "rightalt": return rightAlt != null ? rightAlt : alt;
            case "leftmeta": return leftMeta != null ? leftMeta : meta;
            case "rightmeta": return rightMeta != null ? rightMeta : meta;

            case "uparrow": return upArrow;
            case "downarrow": return downArrow;
            case "leftarrow": return leftArrow;
            case "rightarrow": return rightArrow;

            case "capslock": return capsLock;
            case "numlock": return numLock;
            case "scrolllock": return scrollLock;

            case "numpad0": return numpad0;
            case "numpad1": return numpad1;
            case "numpad2": return numpad2;
            case "numpad3": return numpad3;
            case "numpad4": return numpad4;
            case "numpad5": return numpad5;
            case "numpad6": return numpad6;
            case "numpad7": return numpad7;
            case "numpad8": return numpad8;
            case "numpad9": return numpad9;

            case "numpadenter": return numpadEnter;
            case "numpaddivide": return numpadDivide;
            case "numpadmultiply": return numpadMultiply;
            case "numpadplus": return numpadPlus;
            case "numpadminus": return numpadMinus;
            case "numpadperiod": return numpadPeriod;
            case "numpadequals": return numpadEquals;
            case "numpadcomma": return numpadComma;

            case "leftbutton": return leftMouse;
            case "rightbutton": return rightMouse;
            case "middlebutton": return middleMouse;
            case "forwardbutton": return forwardMouse;
            case "backbutton": return backMouse;

            case "position": return mousePosition;
            case "delta": return mouseDelta;
            case "scroll": return scrollWheel;
            case "scroll/y": return scrollWheel;
            case "scrollup": return scrollUp;
            case "scrolldown": return scrollDown;
        }

        return null;
    }
}

[Serializable]
public struct GamepadIcons
{
    public Sprite buttonSouth;
    public Sprite buttonNorth;
    public Sprite buttonEast;
    public Sprite buttonWest;
    public Sprite startButton;
    public Sprite selectButton;
    public Sprite leftTrigger;
    public Sprite rightTrigger;
    public Sprite leftShoulder;
    public Sprite rightShoulder;
    public Sprite dpad;
    public Sprite dpadUp;
    public Sprite dpadDown;
    public Sprite dpadLeft;
    public Sprite dpadRight;
    public Sprite leftStick;
    public Sprite rightStick;
    public Sprite leftStickPress;
    public Sprite rightStickPress;

    public Sprite GetSprite(string controlPath)
    {
        string key = controlPath.ToLowerInvariant();
        int slashIndex = key.LastIndexOf('/');

        if (slashIndex >= 0)
            key = key.Substring(slashIndex + 1);

        switch (key)
        {
            case "buttonsouth": return buttonSouth;
            case "buttonnorth": return buttonNorth;
            case "buttoneast": return buttonEast;
            case "buttonwest": return buttonWest;
            case "start": return startButton;
            case "select": return selectButton;
            case "lefttrigger":
            case "lefttriggerbutton": return leftTrigger;
            case "righttrigger":
            case "righttriggerbutton": return rightTrigger;
            case "leftshoulder": return leftShoulder;
            case "rightshoulder": return rightShoulder;
            case "dpad": return dpad;
            case "up": return dpadUp;
            case "down": return dpadDown;
            case "left": return dpadLeft;
            case "right": return dpadRight;
            case "leftstick": return leftStick;
            case "rightstick": return rightStick;
            case "leftstickpress": return leftStickPress;
            case "rightstickpress": return rightStickPress;
            default: return null;
        }
    }
}
