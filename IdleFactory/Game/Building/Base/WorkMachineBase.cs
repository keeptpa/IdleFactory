using IdleFactory.ContainerSystem;
using IdleFactory.Game.DataBase;
using IdleFactory.Game.Modules;
using IdleFactory.RecipeSystem;
using IdleFactory.Util;
using Newtonsoft.Json;

namespace IdleFactory.Game.Building.Base;
[Serializable]
public class WorkMachineBase : BuildingBase, IItemContainer, ITickable
{
    [JsonProperty]
    private Recipe? selectedRecipe;
    private List<Recipe>? availableRecipes;
    [JsonProperty]
    private Container machineContainer;
    [JsonProperty]
    private int cookProgress = 0;

    public void ApplyContainerSetting(BuildingSetting buildSetting)
    {
        if (buildSetting.ContainerSetting.HasValue)
        {
            var setting = buildSetting.ContainerSetting.Value;
            machineContainer = new(setting.InputSlotsCount, setting.OutputSlotsCount);
            if (setting.SlotsTagFilter != null)
            {
                foreach (var filterSetting in setting.SlotsTagFilter)
                {
                    machineContainer.GetInputSlots()[filterSetting.Key].TagFilter = filterSetting.Value;
                }
            }
            
            if (setting.SlotsTag != null)
            {
                foreach (var tagSetting in setting.SlotsTag)
                {
                    machineContainer.GetInputSlots()[tagSetting.Key].Tag = tagSetting.Value;
                }
            }
        }
    }

    public virtual void Tick()
    {
        CookTick();
    }

    public virtual bool CanCookRecipe()
    {
        if (selectedRecipe == null) return false;
        return machineContainer.CheckRecipeValid(selectedRecipe);
    }

    public void CookTick()
    {
        if (!CanCookRecipe()) return;
        cookProgress++;
        if (cookProgress >= selectedRecipe.TimeToCook)
        {
            cookProgress = 0;
            CookOnce();
        }
    }

    public override bool Retrieve()
    {
        machineContainer.Clear();
        return base.Retrieve();
    }

    public void ForceRemoveTimer()
    {
        Utils.GetModule<UpdateModule>().Update -= Tick;
    }

    public List<Recipe> GetAllAvailableRecipes()
    {
        availableRecipes ??= Utils.GetData<RecipeData>().GetRecipes(ID);
        return availableRecipes;
    }

    public Recipe? SearchRecipe(string id)
    {
        availableRecipes ??= Utils.GetData<RecipeData>().GetRecipes(ID);
        return availableRecipes.Find(r => r.ID == id);
    }
    
    public void SetRecipe(Recipe recipe = null)
    {
        selectedRecipe = recipe;
        cookProgress = 0;
    }

    public Recipe? GetNowRecipe()
    {
        return selectedRecipe;
    }
    public void CookOnce()
    {
        foreach (var ingredient in selectedRecipe.Ingredients)
        {
            machineContainer.TryRemoveItem(new ResourceItemBase()
            {
                ID = ingredient.Key,
                Quantity = ingredient.Value
            });
        }

        foreach (var product  in selectedRecipe.Outputs)
        {
            machineContainer.TryAddItem(new ResourceItemBase()
            {
                ID = product.Key,
                Quantity = product.Value
            }, false);
        }
    }

    public Container GetMachineContainer()
    {
        return machineContainer;
    }

    public float GetProgress()
    {
        if (selectedRecipe == null) return 0;
        return (float)cookProgress / selectedRecipe.TimeToCook;
    }
    
    public BuildingBase GetBuilding()
    {
        return  this;
    }
}