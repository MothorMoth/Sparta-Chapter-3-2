using UnityEngine;

public interface IInteractable
{
    public string GetPrompt();
    public void Interact();
}

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public string GetPrompt()
    {
        string str = $"{data.itemName}\n\n{data.description}";
        return str;
    }

    public void Interact()
    {
        CharacterManager.Instance.Player.itemData = data;
        CharacterManager.Instance.Player.addItem?.Invoke();
        Destroy(gameObject);
    }
}
