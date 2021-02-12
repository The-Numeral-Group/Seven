using UnityEngine;
using UnityEngine.UI;

//Scripts is used to move ui elements with respect to the activespeaker.
[RequireComponent(typeof(RectTransform))]
public class ChatBubbleTracking : MonoBehaviour
{
    //Reference to the objects rectangular transform
    RectTransform rectTranform;

    //Initialize monobehaviour fields
    void Start()
    {
        rectTranform = GetComponent<RectTransform>();
    }
    //Move the bubble if the actor is talking.
    void FixedUpdate()
    {
        if (ActiveSpeaker.ACTIVE_NPC)
        {
            rectTranform.localPosition = GetWorldPosition();
        }
    }

    //Calculate the chatbubbles position on screen based on the active speakers location.
    Vector2 GetWorldPosition()
    {
        Vector2 viewPortPosition = 
            Camera.main.WorldToViewportPoint(ActiveSpeaker.ACTIVE_NPC.gameObject.transform.position 
            + new Vector3(0f, ActiveSpeaker.ACTIVE_NPC.spriteInfo.size.y / 2, 0f));
        Vector2 proportionalPosition = new Vector2(
            viewPortPosition.x * MenuManager.DIALOGUE_CANVAS_TRANSFORM.sizeDelta.x,
            (viewPortPosition.y * MenuManager.DIALOGUE_CANVAS_TRANSFORM.sizeDelta.y) + rectTranform.rect.height / 2);
        return proportionalPosition;
    }
}
