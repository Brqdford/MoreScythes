using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Morthy.Util;
using UnityEngine;
using Wish;
using Object = UnityEngine.Object;
using HarmonyLib;
namespace MoreScythes;

public static class ItemHandler
{
    private const int OriginalScytheId = 3000;
    private const int MithrilScytheId = 30010;
    private const int SuniteScytheId = 30020;
    private const int GloriteScytheId = 30030;
    private static bool _itemsCreated = false;

    private static void CreateScytheItem(int id, int speed, int damage)
    {

        var original = ItemDatabase.GetItemData(OriginalScytheId);

        var dmgx = damage - 8;
        var dmgy = damage;
        var item = ScriptableObject.CreateInstance<ItemData>();
        JsonUtility.FromJsonOverwrite(FileLoader.LoadFile(Assembly.GetExecutingAssembly(), $"data.{id}.json"), item);
        item.icon = SpriteUtil.CreateSprite(FileLoader.LoadFileBytes(Assembly.GetExecutingAssembly(), $"img.{id}.png"), $"Modded item icon {id}");

        var useItem = Object.Instantiate(original.useItem);
        if (!useItem)
        {
            Plugin.logger.LogError("Original scythe has no useItem");
            return;
        }
        item.useItem = useItem;

        var frameRate = useItem.gameObject.GetComponent<Weapon>();
        Traverse.Create(frameRate).Field("_frameRate").SetValue(speed);

        useItem.gameObject.GetComponent<DamageSource>()._damageRange.Set(dmgx, dmgy);
        
        useItem.gameObject.SetActive(false);

        Object.DontDestroyOnLoad(useItem);
        ItemDatabase.items[item.id] = item;
        ItemDatabase.ids[item.name.RemoveWhitespace().ToLower()] = item.id;
        Plugin.logger.LogDebug($"Created item {item.id} with name {item.name}");
    }
    private static void AddItemToRecipeList(int id, string recipeList, List<ItemInfo> input)
    {
        
        foreach (var rl in Resources.FindObjectsOfTypeAll<RecipeList>())
        {
            if (!rl.name.Equals(recipeList)) continue;
            
            if (rl.craftingRecipes.Any(r => r.output.item.id == id))
            {
                return;
            }
            var recipe = ScriptableObject.CreateInstance<Recipe>();
            recipe.output = new ItemInfo { item = ItemDatabase.GetItemData(id), amount = 1 };
            recipe.input = input;
            recipe.worldProgressTokens = new List<Progress>();
            recipe.characterProgressTokens = new List<Progress>();
            recipe.questProgressTokens = new List<QuestAsset>();
            if (id==30010){
            recipe.hoursToCraft = 12f;
            }else if (id == 30020){
            recipe.hoursToCraft = 18f;
            }else {
            recipe.hoursToCraft = 24f;
            }
                
            rl.craftingRecipes.Add(recipe);
            Plugin.logger.LogDebug($"Added item {id} to {recipeList}");
        }
    }

    public static void CreateScytheItems()
    {
        
        CreateScytheItem(MithrilScytheId, 14, 18);
        CreateScytheItem(SuniteScytheId, 15, 22);
        CreateScytheItem(GloriteScytheId, 16, 26);
        AddItemToRecipeList(30010, "RecipeList_Anvil", new List<ItemInfo>
        {
            new() { item = ItemDatabase.GetItemData(ItemID.MithrilBar), amount = 10 }
        });
        
        AddItemToRecipeList(30020, "RecipeList_Anvil", new List<ItemInfo>
        {
            new() { item = ItemDatabase.GetItemData(ItemID.SuniteBar), amount = 10 }
        });
        AddItemToRecipeList(30030, "RecipeList_Monster Anvil", new List<ItemInfo>
        {
            new() { item = ItemDatabase.GetItemData(ItemID.GloriteBar), amount = 10 }
        });
    }
}