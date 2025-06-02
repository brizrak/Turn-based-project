using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceItemsManager : MonoBehaviour
{
    [SerializeField] private TMP_Text turns;
    [SerializeField] private GameObject win;
    [SerializeField] private GameObject lose;
    [SerializeField] private GameObject draw;
    [SerializeField] private GameObject superButton;

    public void SetTurn(int turn)
    {
        turns.text = "Ход " + turn.ToString();
    }

    public void SetEnd(float playerHealth, float enemyHealth)
    {
        switch (playerHealth.CompareTo(enemyHealth))
        {
            case 1:
                win.SetActive(true);
                break;
            case -1:
                lose.SetActive(true);
                break;
            case 0:
                draw.SetActive(true);
                break;
            default:
                return;
        }
    }

    public void SetSuperButton(bool active)
    {
        superButton.GetComponent<Button>().interactable = active;
    }
}
