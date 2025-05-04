using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;

/// <summary>
/// A central class containing static utility functions for Lua interaction with in-game elements.
/// </summary>
public class LuaCodebase : MonoBehaviour
{
    private static readonly string rootPath = "InteractableObjects/";
    public static Action OnSpawnObject;
    private static AudioSource sound;

    private void Start()
    {
        sound = this.AddComponent<AudioSource>();
        sound.playOnAwake = false;
        sound.loop = false;
    }

    /***** Game Object Methods *****/

    // Gets the Transform of an interactable object by name
    public static Transform GetObjectTransform(string objectName)
    {
        GameObject obj = GameObject.Find(rootPath + objectName);
        if (obj == null) return null;
        return obj.transform;
    }
    
    // Moves an object to a new position, optionally with animation
    public static async Task MoveObject(string objectName, float x, float y, float z, bool progress)
    {
        GameObject obj = GameObject.Find(rootPath + objectName);
        if (obj == null) throw new Exception("MoveObject: cannot find target object");
        
        if (progress) await obj.GetComponent<Interactable>().MoveToPosition(new Vector3(x, y, z));
        else obj.transform.position = new Vector3(x, y, z);
    }

    // Rotates an object to a new angle, optionally with animation
    public static async Task RotateObject(string objectName, float x, float y, float z, bool progress)
    {
        GameObject obj = GameObject.Find(rootPath + objectName);
        if (obj == null) throw new Exception("RotateObject: cannot find target object");
        
        if (progress) await obj.GetComponent<Interactable>().RotateToAngle(new Vector3(x, y, z));
        else obj.transform.eulerAngles = new Vector3(x, y, z);
    }

    // Scales an object to a new size, optionally with animation
    public static async Task ScaleObject(string objectName, float x, float y, float z, bool progress)
    {
        GameObject obj = GameObject.Find(rootPath + objectName);
        if (obj == null) throw new Exception("ScaleObject: cannot find target object");
        
        if (progress) await obj.GetComponent<Interactable>().ScaleToSize(new Vector3(x, y, z));
        else obj.transform.localScale = new Vector3(x, y, z);
    }
    
    // Spawns a new object prefab at a location with optional eyes
    public static GameObject SpawnObject(string prefabName, string objectName, float x, float y, float z, bool eyes)
    {
        GameObject prefab = Resources.Load<GameObject>("SpawnPrefab/" + prefabName);
        if (prefab == null) throw new Exception("SpawnObject: cannot find target prefab");
        
        GameObject obj = Instantiate(prefab, GameObject.Find("InteractableObjects").transform);
        obj.transform.position = new Vector3(x, y, z);
        obj.name = objectName;
        obj.GetComponent<Interactable>().SetEyes(eyes);
        OnSpawnObject?.Invoke();
        if (LuaManager.instance.currentMode == EditorMode.PLAYGROUND)
        {
            GameObject appearVFX = Resources.Load<GameObject>("Interaction/Appear");
            GameObject vfx = Instantiate(appearVFX);
            vfx.transform.position = new Vector3(x, y, z);
        }
        return obj;
    }

    // Replaces an existing object with a new prefab, preserving transform
    public static void UpdateSpawnObject(string prefabName, string objectName, float x, float y, float z, bool eyes)
    {
        GameObject previousObject = GameObject.Find("InteractableObjects/" + objectName);
        if (previousObject == null) throw new Exception("UpdateSpawnObject: cannot find the previous object");
        
        Transform previousTransform = previousObject.transform;
        Destroy(previousObject);
        GameObject obj = SpawnObject(prefabName, objectName, x, y, z, eyes);
        obj.transform.position = previousTransform.position;
        obj.transform.rotation = previousTransform.rotation;
        obj.transform.localScale = previousTransform.localScale;
        obj.GetComponent<Interactable>().SetEyes(eyes);
    }
    
    // Renames an existing object
    public static void UpdateSpawnObject(string previousName, string newName)
    {
        GameObject previousObject = GameObject.Find("InteractableObjects/" + previousName);
        if (previousObject == null) throw new Exception("UpdateSpawnObject: cannot find the previous object");
        previousObject.gameObject.name = newName;
    }

    // Sets whether an object has eyes
    public static void ChangeEyes(string objectName, bool hasEyes)
    {
        GameObject obj = GameObject.Find("InteractableObjects/" + objectName);
        if (obj == null) throw new Exception("ChangeEyes: invalid object name");
        obj.GetComponent<Interactable>().SetEyes(hasEyes);
    }
    
    // Updates the position of a spawned object
    public static void UpdateSpawnObject(string objectName, float x, float y, float z)
    {
        GameObject previousObject = GameObject.Find("InteractableObjects/" + objectName);
        if (previousObject == null) throw new Exception("UpdateSpawnObject: cannot find the previous object");
        previousObject.transform.position = new Vector3(x, y, z);
    }

    // Destroys a given object
    public static void DestroyObject(string objectName)
    {
        GameObject obj = GameObject.Find(rootPath + objectName);
        if (obj == null) throw new Exception("DestroyObject: cannot find target object");
        if (LuaManager.instance.currentMode == EditorMode.PLAYGROUND)
        {
            GameObject disappearVFX = Resources.Load<GameObject>("Interaction/Disappear");
            GameObject vfx = Instantiate(disappearVFX);
            vfx.transform.position = obj.transform.position;
        }
        Destroy(obj);
    }
    
    /***** Material *****/

    // Changes the material color of an object
    public static void ChangeMaterialColor(string objectName, float r, float g, float b)
    {
        GameObject obj = GameObject.Find(rootPath + objectName);
        if (obj == null) throw new Exception("ChangeMaterialColor: cannot find target object");

        Color newColor = new Color(r / 255, g / 255, b / 255);
        List<Renderer> renderers = obj.GetComponent<Interactable>().GetColorRenderers();
        if (renderers == null) throw new Exception("ChangeMaterialColor: cannot find Renderer");
        
        foreach (var renderer in renderers)
        {
            if (renderer.material.shader == Shader.Find("Toon"))
                renderer.material.SetColor("_BaseColor", newColor);
            else renderer.material.color = newColor;
        }
    }
    
    /***** Dialog Interaction *****/

    // Shows a dialog box with a character and message
    public static void ShowDialog(string character, string content)
    {
        GameObject prefab = Resources.Load<GameObject>("Interaction/" + "Dialog");
        if (prefab == null) throw new Exception("ShowDialog: cannot find target prefab");
        
        DialogHandler handler = GameObject.Find("Canvas/InteractableUI").GetComponentInChildren<DialogHandler>();
        if (handler == null)
        {
            GameObject obj = Instantiate(prefab, GameObject.Find("Canvas/InteractableUI").transform);
            handler = obj.GetComponent<DialogHandler>();
        }
        handler.AddDialog(content, character);
    }
    
    // Destroys and recreates the dialog to show a new message
    public static void UpdateShowDialog(string character, string content)
    {
        GameObject prefab = Resources.Load<GameObject>("Interaction/" + "Dialog");
        if (prefab == null) throw new Exception("ShowDialog: cannot find target prefab");
        
        DialogHandler exHandler = GameObject.Find("Canvas/InteractableUI").GetComponentInChildren<DialogHandler>();
        if (exHandler != null)
        {
            Destroy(exHandler.gameObject);
        }
        GameObject obj = Instantiate(prefab, GameObject.Find("Canvas/InteractableUI").transform);
        obj.GetComponent<DialogHandler>().AddDialog(content, character);
    }
    
    /***** Scene Load and Unload *****/

    // Switches the game to a new scene
    public static async Task SwitchScene(string sceneName)
    {
        try
        {
            await SceneLoadManager.instance.SwitchScene(sceneName);
        }
        catch (Exception e)
        {
            throw new Exception("SwitchScene: " + e.Message);
        }
    }
    
    /***** Game Feel *****/

    // Plays a sound effect by clip name
    public static void PlaySound(string clipName)
    {
        AudioClip clip = Resources.Load<AudioClip>("SFX/" + clipName);
        if (clip == null) throw new Exception("PlaySoundEffect: cannot find target audio clip");
        sound.Stop();
        sound.clip = clip;
        sound.Play();
    }

    // Spawns a visual effect at a given location and names it
    public static void PlayVisualEffect(string vfxName, float x, float y, float z, string objectName)
    {
        GameObject vfxPrefab = Resources.Load<GameObject>("VFX/" + vfxName);
        if (vfxPrefab == null) throw new Exception("PlayVisualEffect: cannot find target VFX prefab");
        
        GameObject vfxParent = GameObject.Find("VFXs");
        GameObject vfx = Instantiate(vfxPrefab, vfxParent.transform);
        vfx.name = objectName;
        vfx.transform.localPosition = new Vector3(x, y, z);
    }
    
    // Replaces an existing visual effect with a new one
    public static void UpdateVisualEffect(string vfxName, float x, float y, float z, string objectName)
    {
        GameObject obj = GameObject.Find("VFXs/" + objectName);
        if (obj == null) throw new Exception("UpdateVisualEffect: cannot find target vfx");

        GameObject vfxPrefab = Resources.Load<GameObject>("VFX/" + vfxName);
        if (vfxPrefab == null) throw new Exception("PlayVisualEffect: cannot find target VFX prefab");
        
        Destroy(obj);
        GameObject vfxParent = GameObject.Find("VFXs");
        GameObject vfx = Instantiate(vfxPrefab, vfxParent.transform);
        vfx.name = objectName;
        vfx.transform.localPosition = new Vector3(x, y, z);
    }
    
    // Destroys a named visual effect
    public static void DestroyVFX(string vfxName)
    {
        GameObject obj = GameObject.Find("VFXs/" + vfxName);
        if (obj == null) throw new Exception("DestroyVFX: cannot find target object");
        Destroy(obj);
    }

    // Plays a named animation on the player character
    public static void PlayAnimation(string animName)
    {
        GameObject character = GameObject.FindGameObjectWithTag("PlayerModel");
        Animator animator = character.GetComponent<Animator>();
        try
        {
            animator.CrossFadeInFixedTime(animName, 0.15f, 0);
        }
        catch
        {
            throw new Exception("PlayAnimation: invalid animation name");
        }
    }
    
    /***** Character Related *****/
    // Changes the size of the player character
    public static void ChangeCharacterSize(float scale)
    {
        GameObject character = GameObject.FindGameObjectWithTag("CharacterSetup");
        try
        {
            character.GetComponent<CharacterSetup>().ChangeCharacterSize(new Vector3(scale, scale, scale));
        }
        catch
        {
            throw new Exception("ChangeCharacterSize: failed to change the size");
        }
    }
    
    // Changes the movement speed of the player character
    public static void ChangeCharacterSpeed(float speed)
    {
        GameObject character = GameObject.FindGameObjectWithTag("CharacterSetup");
        try
        {
            character.GetComponent<CharacterSetup>().ChangeCharacterSpeed(speed);
        }
        catch
        {
            throw new Exception("ChangeCharacterSpeed: failed to change the speed");
        }
    }
    
    /***** Time Delay *****/
    // Delays further code execution for a specified number of seconds
    public static Task Delay(float delayTime)
    {
        return Task.Delay((int)(delayTime * 1000));
    }
}
