using System;
using UnityEngine;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lua.Codebase;
using Lua;
using Playground;

public enum EditorMode
{
    GAME = 0,
    INTERACTION = 1,
    PLAYGROUND = 2
}

public class LuaManager : MonoBehaviour
{
    public static LuaManager instance;
    public event Action OnLuaScriptCompleted, OnLuaExecutionStart, OnLuaExecutionEnd;
    public EditorMode currentMode = EditorMode.GAME;
    
    private Script luaExecutor;
    private ScriptList<ILuaMethod> _luaMethods;
    private readonly Queue<Func<Task>> methodQueue = new();

    private bool isExecuting = false;

    public bool IsExecuting
    {
        get => isExecuting;
        set
        {
            if (isExecuting == value) return;
            if (value) OnLuaExecutionStart?.Invoke();
            else OnLuaExecutionEnd?.Invoke();
            isExecuting = value;
        }
    }
    
    public ScriptList<ILuaMethod> luaMethods
    {
        get => _luaMethods;
    }

    void Awake()
    {
        if (instance == null) instance = this;
        luaExecutor = new Script();
        _luaMethods = new ScriptList<ILuaMethod>();

        // Register Unity types
        UserData.RegisterAssembly();
        UserData.RegisterType<GameObject>();
        UserData.RegisterType<Transform>();
        UserData.RegisterType<Vector3>();
        UserData.RegisterType<Color>();
        UserData.RegisterType<Renderer>();

        // Expose GameObject functions
        luaExecutor.Globals["MoveObject"] = (System.Action<string, float, float, float, bool>)MoveObject;
        luaExecutor.Globals["RotateObject"] = (System.Action<string, float, float, float, bool>)RotateObject;
        luaExecutor.Globals["ScaleObject"] = (System.Action<string, float, float, float, bool>)ScaleObject;
        luaExecutor.Globals["SpawnObject"] = (System.Action<string, string, float, float, float, bool>)SpawnObject;
        luaExecutor.Globals["DestroyObject"] = (System.Action<string>)DestroyObject;
        luaExecutor.Globals["ShowDialog"] = (System.Action<string, string>)ShowDialog;
        luaExecutor.Globals["ChangeMaterialColor"] = (System.Action<string, int, int, int>)ChangeMaterialColor;
        luaExecutor.Globals["SwitchScene"] = (System.Action<string>)SwitchScene;
        luaExecutor.Globals["PlaySound"] = (System.Action<string>)PlaySound;
        luaExecutor.Globals["PlayVisualEffect"] = (System.Action<string, float, float, float, string>)PlayVisualEffect;
        luaExecutor.Globals["DestroyVisualEffect"] = (System.Action<string>)DestroyVisualEffect;
        luaExecutor.Globals["PlayAnimation"] = (System.Action<string>)PlayAnimation;
        luaExecutor.Globals["ChangeEyes"] = (System.Action<string, bool>)ChangeEyes;
        luaExecutor.Globals["ChangeCharacterSize"] = (System.Action<float>)ChangeCharacterSize;
        luaExecutor.Globals["ChangeCharacterSpeed"] = (System.Action<float>)ChangeCharacterSpeed;
        luaExecutor.Globals["Delay"] = (System.Action<float>)Delay;
        luaExecutor.Globals["Log"] = (System.Action<string>)Debug.Log;
    }

    public void SetLuaMode(EditorMode newMode)
    {
        currentMode = newMode;
    }
    
    public async Task RunLuaScript(string script)
    {
        ResetMethodList();
        methodQueue.Clear();
        IsExecuting = true;
        
        luaExecutor.DoString(script);
        while (methodQueue.Count > 0)
        {
            var next = methodQueue.Dequeue();
            await next();
        }
        OnLuaScriptCompleted?.Invoke();
        IsExecuting = false;
    }

    public void ResetMethodList()
    {
        _luaMethods.ClearUnnotify();
    }
    
    // ===== Shared Functions ===== //

    public async Task ExecuteFunction(ILuaMethod method)
    {
        try
        {
            await method.executeFunction();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed in execution: " + e.Message);
            return;
        }
        
        if (currentMode == EditorMode.PLAYGROUND) _luaMethods.Add(method);
    }
    
    // ===== Object Methods ===== //
    
    public void MoveObject(string objectName, float x, float y, float z, bool progress)
    {
        methodQueue.Enqueue(() => ExecuteFunction(new MoveObjectMethod(objectName, x, y, z, progress)));
    }

    public void RotateObject(string objectName, float x, float y, float z, bool progress)
    {
        methodQueue.Enqueue(() => ExecuteFunction(new RotateObjectMethod(objectName, x, y, z, progress)));
    }

    public void ScaleObject(string objectName, float x, float y, float z, bool progress)
    {
        methodQueue.Enqueue(() => ExecuteFunction(new ScaleObjectMethod(objectName, x, y, z, progress)));
    }

    public void SpawnObject(string prefabName, string objectName, float x, float y, float z, bool eyes)
    {
        methodQueue.Enqueue(() => ExecuteFunction(new SpawnObjectMethod(prefabName, objectName, x, y, z, eyes)));
    }
    
    public void DestroyObject(string objectName)
    {
        methodQueue.Enqueue(() => ExecuteFunction(new DestroyObjectMethod(objectName)));
    }
    
    // ===== Dialog Interaction ===== //

    public void ShowDialog(string character, string content)
    {
        methodQueue.Enqueue(() => ExecuteFunction(new ShowDialogMethod(character, content)));
    }
    
    // ===== Material Change ===== //

    public void ChangeMaterialColor(string objectName, int r, int g, int b)
    {
        methodQueue.Enqueue(() => ExecuteFunction(new ChangeMaterialColor(objectName, r, g, b)));
    }
    
    // ===== Scene Load and Unload ===== //
    public void SwitchScene(string sceneName)
    {
        methodQueue.Enqueue(() => ExecuteFunction(new SwitchSceneMethod(sceneName)));
    }
    
    // ===== Game Feel ===== //
    public void PlaySound(string clipName)
    {
        methodQueue.Enqueue(() => ExecuteFunction(new PlaySoundMethod(clipName)));
    }

    public void PlayVisualEffect(string vfxName, float x, float y, float z, string objectName)
    {
        methodQueue.Enqueue(() => ExecuteFunction(new PlayVisualEffectMethod(vfxName, x, y, z, objectName)));
    }
    
    public void DestroyVisualEffect(string vfxName)
    {
        methodQueue.Enqueue(() => ExecuteFunction(new DestroyVFXMethod(vfxName)));
    }

    public void PlayAnimation(string animName)
    {
        methodQueue.Enqueue(() => ExecuteFunction(new PlayAnimationMethod(animName)));
    }

    public void ChangeEyes(string objectName, bool hasEyes)
    {
        methodQueue.Enqueue(() => ExecuteFunction(new ChangeEyesMethod(objectName, hasEyes)));
    }

    public void ChangeCharacterSize(float scale)
    {
        methodQueue.Enqueue(() => ExecuteFunction(new ChangeCharacterSizeMethod(scale)));
    }
    
    public void ChangeCharacterSpeed(float speed)
    {
        methodQueue.Enqueue(() => ExecuteFunction(new ChangeCharacterSpeedMethod(speed)));
    }
    
    public void Delay(float delayTime)
    {
        methodQueue.Enqueue(() => ExecuteFunction(new DelayMethod(delayTime)));
    }
}