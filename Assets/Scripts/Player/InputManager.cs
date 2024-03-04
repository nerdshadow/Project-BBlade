using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public static MainControls inputActions;
    public static event Action RebindComplete;
    public static event Action RebindCanceled;
    public static event Action<InputAction, int> rebindStarted;
    public PlayerInput playerInput;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitManager();
        if (inputActions == null)
        {
            //Debug.Log("created new MControls");
            inputActions = new MainControls();
        }
        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();
        TryFindUIInputModule();
    }
    void InitManager()
    {
    }
    bool TryFindUIInputModule()
    {
        if (playerInput.uiInputModule == null)
        {
            playerInput.uiInputModule = FindObjectOfType<InputSystemUIInputModule>();
        }
        if (playerInput.uiInputModule == null)
            return false;
        else
            return true;
    }
    public static void ChangeControlsMappingToGameplay()
    {
        inputActions.Gameplay.Enable();
        inputActions.UI.Disable();
    }
    public static void ChangeControlsMappingToMenu()
    {
        inputActions.Gameplay.Disable();
        inputActions.UI.Enable();
    }
    public static void StartRebind(string actionName, int bindingIndex, TMP_Text statusText, bool excludeMouse)
    {
        InputAction action = inputActions.asset.FindAction(actionName);
        if (action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.LogError("Couldnt find action or binding");
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex = bindingIndex + 1;
            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isComposite)
            {
                DoRebind(action, bindingIndex, statusText, true, excludeMouse);
            }
        }
        else
        {
            DoRebind(action, bindingIndex, statusText, false, excludeMouse);
        }
    }
    static void DoRebind(InputAction actionToRebind, int bindingIndex, TMP_Text statusText, bool allCompositeParts, bool excludeMouse)
    {
        if (actionToRebind == null || bindingIndex < 0)
        {
            return;
        }
        statusText.text = $"Press a {actionToRebind.expectedControlType}";
        actionToRebind.Disable();
        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);
        rebind.OnComplete(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();

            if (allCompositeParts)
            {
                var nextBindingIndex = bindingIndex + 1;
                if (nextBindingIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isComposite)
                {
                    DoRebind(actionToRebind, nextBindingIndex, statusText, allCompositeParts, excludeMouse);
                }
            }
            SaveBinding(actionToRebind);
            RebindComplete?.Invoke();
        });

        rebind.OnCancel(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();

            RebindCanceled?.Invoke();
        });

        rebind.WithCancelingThrough("<Keyboard>/escape");

        if (excludeMouse)
            rebind.WithControlsExcluding("Mouse");

        rebindStarted?.Invoke(actionToRebind, bindingIndex);
        rebind.Start();
    }
    public static string GetBindingName(string _actionBindingName, int _bindingIndex)
    {
        if (inputActions == null)
            inputActions = new MainControls();

        InputAction action = inputActions.asset.FindAction(_actionBindingName);
        return action.GetBindingDisplayString(_bindingIndex);
    }
    public static void SaveBinding(InputAction action)
    {
        //TODO Save bindings
    }
    public static void LoadBinding(string actionName)
    {
        //TODO Load bindings
    }
    public static void ResetBinding(string actionName, int bindingIndex)
    {
        InputAction action = inputActions.asset.FindAction(actionName);

        if (action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Couldnt find the binding");
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            for (int i = bindingIndex; i < action.bindings.Count && action.bindings[i].isComposite; i++)
            {
                action.RemoveBindingOverride(i);
            }
        }
        else
            action.RemoveBindingOverride(bindingIndex);
        SaveBinding(action);
    }
}

