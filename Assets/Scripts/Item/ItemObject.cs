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
        PlayerManager.Instance.Player.itemData = data;
        PlayerManager.Instance.Player.addItem?.Invoke();
        Destroy(gameObject);
    }
}
