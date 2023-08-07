using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Sign : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI textComponent;
    public TextMeshPro readPrompt;
    public CanvasGroup dialogueBox;

    [Header("Text")]
    public string[] lines;
    public float textSpeed;

    private int index;
    private bool readable = false;
    private bool reading = false;
    private bool playerColliding = false;

    private void Start(){
        textComponent.text = string.Empty;
    }

    private void Update(){
        if(readable && !reading && Input.GetKeyDown(KeyCode.E)){
            StartDialogue();
        } else if(!readable && reading && Input.GetKeyDown(KeyCode.E)){
            if(textComponent.text == lines[index]){
                NextLine();
            } else {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    private void StartDialogue(){
        textComponent.text = string.Empty;
        readable = false;
        reading = true;
        readPrompt.enabled = false;
        dialogueBox.alpha = 1f;
        index = 0;
        StartCoroutine(TypeLine());
    }

    private void EndDialogue(){
        textComponent.text = string.Empty;
        reading = false;
        dialogueBox.alpha = 0f;
        if(playerColliding) {
            readable = true;
            readPrompt.enabled = true;
        }
    }

    private IEnumerator TypeLine(){
        foreach (char c in lines[index].ToCharArray()){
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void NextLine(){
        if(index < lines.Length - 1){
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        } else {
            textComponent.text = lines[index];
            EndDialogue();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player") && !reading){
            readable = true;
            playerColliding = true;
            readPrompt.enabled = true;
        }  
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            readable = false;
            playerColliding = false;
            readPrompt.enabled = false;
        }
    }
}
