using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellButton : MonoBehaviour
{
    public int index;
    public GameController controller;

    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(() => controller.OnCellClicked(index));
    }
}