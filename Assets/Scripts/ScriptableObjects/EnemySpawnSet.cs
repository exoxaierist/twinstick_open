using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnSet",menuName = "Enemy Spawn Set")]
public class EnemySpawnSet : ScriptableObject
{
    public static EnemySpawnSet _main;
    public static EnemySpawnSet main
    {
        get {
            if (_main == null) _main = Resources.Load<EnemySpawnSet>("EnemySpawnSet");
            return _main; 
        }
        set { _main = value; }
    }

    public List<RecipeWeight> recipies = new();
    private List<EnemyWeight> currentRecipe;
    private List<string> currentRandomRecipe = new();

    const bool randomSpawn = true;

    public static void SetRecipe()
    {
        if (randomSpawn)
        {
            //random spawn
            main.currentRandomRecipe.Clear();
            List<string> allId = EnemyList.GetAllID(LevelManager.currentRoomNumber);
            for (int i = 0; i < 3; i++)
            {
                main.currentRandomRecipe.Add(allId[UnityEngine.Random.Range(0, allId.Count)]);
            }
        }
        else
        {
            //pre combined spawn
            List<int> weights = new();
            for (int i = 0; i < main.recipies.Count; i++) weights.Add(main.recipies[i].recipeWeight);
            main.currentRecipe = main.recipies[Utility.WeightedRandom(weights.ToArray())].recipe;
        }
    }

    public static string GetID()
    {
        if (randomSpawn)
        {
            //random pick
            return main.currentRandomRecipe[UnityEngine.Random.Range(0, main.currentRandomRecipe.Count)];
        }
        else
        {
            //preassigned weighted random pick
            List<int> weights = new();
            for (int i = 0; i < main.currentRecipe.Count; i++) weights.Add(main.currentRecipe[i].weight);
            return main.currentRecipe[Utility.WeightedRandom(weights.ToArray())].enemyID;
        }

    }

    [Serializable]
    public struct RecipeWeight
    {
        public List<EnemyWeight> recipe;
        public int recipeWeight;
    }

    [Serializable]
    public struct EnemyWeight
    {
        public string enemyID;
        public int weight;
    }
}
