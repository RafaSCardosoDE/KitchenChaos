using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance {  get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;


    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer = 4f;
    private float spawnRecipeTImerMax = 4f;
    private int waitingRecipesMax = 4;
    private int successfulRecipesAmount;

    private PlateKitchenObject receivedPlateKitchenObject;

    private void Awake()
    {
        Instance = this;

        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
        if(!IsServer) 
        { 
            return;
        }

        spawnRecipeTimer -= Time.deltaTime;
        if(spawnRecipeTimer <= 0f ) 
        {
            spawnRecipeTimer = spawnRecipeTImerMax;

            if(GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax )
            {
                int randomRecipe = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);

                SpawnNewWaitingRecipeClientRpc(randomRecipe);
            }
        }
    }
    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];
        waitingRecipeSOList.Add(waitingRecipeSO);

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliveryRecipe(PlateKitchenObject plateKitchenObject)
    {
        receivedPlateKitchenObject = plateKitchenObject;

        CheckRecipeSOServerRPC();
    }

    [ServerRpc(RequireOwnership =false)]
    private void CheckRecipeSOServerRPC()
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == receivedPlateKitchenObject.GetKitchenObjectSOList().Count)
            {
                //Has the same number of ingredients
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipekitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    //Cycling through all ingredients in the Recipe
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in receivedPlateKitchenObject.GetKitchenObjectSOList())
                    {
                        //Cycling through all ingredients in the Plate
                        if (plateKitchenObjectSO == recipekitchenObjectSO)
                        {
                            //Ingredient matches!
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        //This Recipe ingredient was not found on the Plate
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe)
                {
                    //Player delivered the correct recipe!
                    CorrectRecipeDeliveredClientRpc(i);

                    return;
                }
            }
        }
        //No matches found!
        //Player did not deliver a correct recipe
        IncorrectRecipeDeliveredClientRpc();
    }

    [ClientRpc]
    private void CorrectRecipeDeliveredClientRpc(int waitingRecipeSOIndex)
    {
        waitingRecipeSOList.RemoveAt(waitingRecipeSOIndex);
        successfulRecipesAmount++;
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }

    [ClientRpc]
    private void IncorrectRecipeDeliveredClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount()
    {
        return successfulRecipesAmount;
    }
}
