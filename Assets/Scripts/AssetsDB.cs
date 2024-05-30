using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

public class AssetsDB : MonoBehaviour
{
    public static AssetsDB Instance { get; private set; }
    
    public Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> projectileSpriteDictionary = new Dictionary<string, Sprite>();
    public Dictionary<string, Animator> effectAnimatorDictionary = new Dictionary<string, Animator>();
    public Dictionary<string, DropTable> dropTableDictionary = new Dictionary<string, DropTable>();
    public Dictionary<string, BaseInfo> baseInfoDictionary = new Dictionary<string, BaseInfo>();
    public Dictionary<string, BaseInfo> corpseBaseInfoDictionary = new Dictionary<string, BaseInfo>();
    
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensures that the instance persists across scenes
            LoadAllAssets();

        }
    }
    
    public void LoadAllAssets() {
        LoadSprites("Sprites", spriteDictionary);
        LoadSprites("ProjectileSprites", projectileSpriteDictionary);
        // LoadEffectAnimators("");
        LoadDropTables("Assets/Resources/DropTables");
        LoadBaseInfos("Assets/Resources/ItemBaseInfos", baseInfoDictionary);
        LoadBaseInfos("Assets/Resources/CorpseBaseInfos", corpseBaseInfoDictionary);
    }

    void LoadSprites(string path, Dictionary<string, Sprite> dictionary)
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>(path);
        foreach (Sprite sprite in allSprites)
        {
            if (sprite != null && !dictionary.ContainsKey(sprite.name))
            {
                dictionary.Add(sprite.name, sprite);
            }
        }
    }

    void LoadEffectAnimators(string path) {
        Animator[] animators = Resources.LoadAll<Animator>(path);
        foreach (Animator animator in animators) {
            effectAnimatorDictionary.Add(animator.name, animator);
        }
    }

    void LoadDropTables(string path) {
        string[] jsonFiles = Directory.GetFiles(path, "*.json");
        foreach (string file in jsonFiles) {
            DropTable dropTable = JsonUtils.LoadJsonAsSO<DropTable>(file);
            dropTableDictionary.Add(Path.GetFileNameWithoutExtension(file), dropTable);
        }
    }

    void LoadBaseInfos(string path, Dictionary<string, BaseInfo> dictionary) {
        string[] jsonFiles = Directory.GetFiles(path, "*.json");
        foreach (string file in jsonFiles) {
            BaseInfo baseInfo = JsonUtils.LoadJsonAsSO<BaseInfo>(file);
            dictionary.Add(baseInfo.itemName, baseInfo);
        }
    }

    


}