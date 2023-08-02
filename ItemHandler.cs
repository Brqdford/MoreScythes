﻿using System.Collections.Generic;
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
    private static bool _itemsCreated = false;

    private static void CreateScytheItem(int id, int speed, int damage)
    {
		if ((bool)ItemDatabase.GetItemData(id))
		{
			Plugin.logger.LogError((object)$"Cannot create modded item with ID {id} because it is already in use by a different item.");
			return;
		}

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
        
        useItem.gameObject.GetComponent<DamageSource>()._damageRange.Set(dmgx, dmgy);
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
            }else {
            recipe.hoursToCraft = 18f;
            }
                
            rl.craftingRecipes.Add(recipe);
            Plugin.logger.LogDebug($"Added item {id} to {recipeList}");
        }
    }

    public static void CreateScytheItems()
    {
        if (_itemsCreated)
        {
            return;
        }
        
        CreateScytheItem(MithrilScytheId, 14, 30);
        CreateScytheItem(SuniteScytheId, 15, 45);
        
        AddItemToRecipeList(30010, "RecipeList_Anvil", new List<ItemInfo>
        {
            new() { item = ItemDatabase.GetItemData(ItemID.MithrilBar), amount = 8 }
        });
        
        AddItemToRecipeList(30020, "RecipeList_Anvil", new List<ItemInfo>
        {
            new() { item = ItemDatabase.GetItemData(ItemID.SuniteBar), amount = 9 }
        });
        _itemsCreated = true;
    }
}