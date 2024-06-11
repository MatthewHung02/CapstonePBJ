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
        [SerializeField] private GameObject whisperObject;
        [SerializeField] private TTSManager managerScript;
        [SerializeField] private SceneSwitcher sceneReset;

        private float height;
        private OpenAIApi openai = new OpenAIApi();

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt =
            "Never break character, or mention you are an AI model." +
            "Only write responses with two sentences or less. Keep responses as short as possible but be sure to still sound friendly meanwhile." +
            "You are an instructor named Elizabeth and your goal is to provide guidance to the user on how to create a Peanut Butter and Jelly Sandwich." +
            "This will take place in a Virtual Reality environment, where the user has two controllers and a headset." +
            "Users can interact with objects using the triggers of the controller." +
            "Answer questions that can help the user figure out what step to make next in their sandwich-making process." +
            "The user will be provided a table. At the top left of the table is a bag of bread they can interact with to get a slice of bread" +
            "At the top right of the table are two jars of peanut butter and jelly. They can only obtain the two ingredients with a knife" +
            "To apply the peanut butter and jelly, the user only needs to have the knife with the ingredient touch a slice of bread" +
            "Keep responses short and concise. About two sentences maximum." +
            "If the user asks for step-by-step instructions, go as follow:" +
                "1) Grab the knife with either controller" +
                "2) Use knife to obtain peanut butter or jelly by making the knife touch the respective jar at the top left" +
                "3) apply the ingredient to the piece of bread at the center" +
                "4) do the same for the other ingredient" +
                "5) when both ingredients are applied, grab a piece of bread from the bag of bread and place it on top to complete the sandwich." +
                "Be sure to congratulate the player for doing so." +
            "If the user says they've lost the knife or they've made a mistake, offer the user to reset the scene so they can start over. They should respond with yes or no first." +
            "If they say yes, then say that you will restart the scene and append RESET_SCENE phrase";


        private void Start()
        {
            //button.onClick.AddListener(SendReply); <= Don't need this, but keeping it just in case
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
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };
            
            AppendMessage(newMessage);

            Debug.Log(text);
            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text + text;
            newMessage.Content += "\n" + text;

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

            //Resets the scene when the user confirms they want to do so
            if (completionResponse.Choices[0].Message.Content.Contains("RESET_SCENE"))
            {
                sceneReset.LoadScene("MainPBJScene");
            }

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                //feed this message into TTS
                message.Content = message.Content.Trim();
                
                messages.Add(message);
                //PART 2: When response is received, sends results over to TTSManager.cs
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
