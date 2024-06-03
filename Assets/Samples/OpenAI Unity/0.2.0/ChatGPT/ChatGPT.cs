using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using OpenAI;
using Samples.Whisper;

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private ScrollRect scroll;
        
        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;
        [SerializeField] private GameObject whisperObject; //NEED TO REFERENCE OBJECT SCRIPT, not just the object
        [SerializeField] private TTSManager managerScript;
        //public Whisper whisper;

        private float height;
        private OpenAIApi openai = new OpenAIApi();

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "You are an instructor named Elizabeth and your goals is to provide guidance to the user on how to create a Peanut Butter and Jelly Sandwich." +
            "This will take place in a Virtual Reality environment, where the user has two controllers and a headset." +
            "Users can interact with objects using the triggers of the controller." +
            "Answer questions that can help the user figure out what step to make next in their sandwich-making process." +
            "Never break character, or mention you are an AI model." +
            "The user will be provided a table. At the top left of the table is a bag of bread they can interact with to get a slice of bread" +
            "At the top right of the table are two jars of peanut butter and jelly. They can only obtain the two ingredients with a knife" +
            "To apply the peanut butter and jelly, the user only needs to have the knife with the ingredient touch a slice of bread" +
            "";

        private void Start()
        {
            //button.onClick.AddListener(SendReply); dont need this FOR NOW
        }

        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        public async void SendReply(string text)
        {
            //NOTE: import res.txt from Whisper.cs. Put it into prompt + newMessage.content == prompt + "\n"
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };
            
            AppendMessage(newMessage);
            //newMessage.Content = whisper.message.text; //.text might not work

            Debug.Log(text);
            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text + text; //NOTE: Test this
            
            messages.Add(newMessage);
            
            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;
            
            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-4o",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                //feed this message into TTS
                message.Content = message.Content.Trim();
                
                messages.Add(message);
                //Part2: Call SynthesizeAndPlay() in TTSManager.cs
                AppendMessage(message);
                managerScript.SynthesizeAndPlay(message.Content);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            button.enabled = true;
            inputField.enabled = true;
        }
    }
}
