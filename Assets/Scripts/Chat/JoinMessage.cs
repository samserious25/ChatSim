using UnityEngine;
using UnityEngine.UI;

public class JoinMessage : MonoBehaviour
{
    private Text joinText;

    private void Awake() => joinText = transform.GetChild(0).GetComponent<Text>();

    public void SetJoinText(string clientName, bool join)
    {
        string joinStatus = join == true ? " присоединился к каналу" : " покинул канал";
        joinText.text = clientName + joinStatus;
    }
}
