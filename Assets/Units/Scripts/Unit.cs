using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

[CreateAssetMenu]
public class Unit : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject prefab;
    [SerializeField] [TextArea] private List<string> intros;
    [SerializeField] private List<Dialogue> chatDialogue;
    [SerializeField] private List<Dialogue> giveDialogue;

    public string Name => name;
    public Sprite Sprite => sprite;
    public GameObject Prefab => prefab;
    public List<string> Intros => intros;
    public List<Dialogue> GiveDialogue => giveDialogue;
    public List<Dialogue> ChatDialogue => chatDialogue;

    public void Initialize(JObject data)
    {
        Debug.Log(data);
        // Greetings → intros
        intros = new List<string>();
        if (data["greetings"] is JArray greetingsArray)
        {
            foreach (var g in greetingsArray)
            {
                intros.Add(g.ToString());
            }
        }

        // Chat dialogue
        chatDialogue = new List<Dialogue>();
        if (data["chat"] is JArray chatArray)
        {
            foreach (var c in chatArray)
            {
                Dialogue d = new Dialogue(c.ToString());
                chatDialogue.Add(d);
            }
        }

        // Gift dialogue
        giveDialogue = new List<Dialogue>();
        if (data["gift"] is JArray giftArray)
        {
            foreach (var g in giftArray)
            {
                if (g is JObject giftObj)
                {
                    var text = giftObj.Value<string>("text") ?? string.Empty;
                    var actionStr = giftObj.Value<string>("action");

                    var action = actionStr switch
                    {
                        "spore" => DialogueAction.PLAY_SPORE,
                        _ => DialogueAction.DEFAULT,
                    };

                    Dialogue d = new Dialogue(text, action);
                    giveDialogue.Add(d);
                }
                else
                {
                    Dialogue d = new Dialogue(g.ToString());
                    giveDialogue.Add(d);
                }
            }
        }
    }

}
