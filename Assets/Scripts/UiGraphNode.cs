using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiGraphNode : MonoBehaviour
{
    public Image nodeImage;
    public TextMeshProUGUI nodeText;

    private GraphNode node;

    public void Reset()
    {
        SetColor(node.CanVisit ? Color.white : Color.gray);
        SetText($"ID: {node.id}\nWeight: {node.weight}");
    }

    public void SetNode(GraphNode node)
    {
        this.node = node;
    }

    public void SetColor(Color color)
    {
        nodeImage.color = color;
    }

    public void SetText(string text)
    {
        nodeText.text = text;
    }
}