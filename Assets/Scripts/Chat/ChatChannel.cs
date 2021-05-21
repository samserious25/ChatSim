using System;
using UnityEngine;
using UnityEngine.UI;

public class ChatChannel : MonoBehaviour
{
    public static event Action<string> GoChannel;

    [SerializeField]
    private Image channelImage;
    [SerializeField]
    private Text channelName;
    [SerializeField]
    private Button channelButton;

    private void Start()
    {
        channelButton.onClick.AddListener(() => GoChannel?.Invoke(channelName.text));
    }

    public void FillContent(string channelName)
    {
        this.channelName.text = channelName;
        channelImage.color = GetRandomColor();
    }

    private Color GetRandomColor()
    {
        return new Color(UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f), 1f);
    }
}
