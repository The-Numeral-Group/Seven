using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueMenu : BaseUI
{
    //Reference to yarnspinners dialogue runner. Expected to be set through Inspector
    public DialogueRunner dialogueRunner;
    //Set the speakerNameTextBox. Expected to be set through Inspector.
    public Text speakerNameTextBox;
}
