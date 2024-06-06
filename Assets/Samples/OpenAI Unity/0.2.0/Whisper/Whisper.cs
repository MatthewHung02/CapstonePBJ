using OpenAI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Samples.Whisper
{
    public class Whisper : MonoBehaviour
    {
        private int microphoneIndex = 0;
        [SerializeField] public InputActionProperty promptButton;   //button for beginning the prompt
        [SerializeField] private Button recordButton;
        [SerializeField] private Image progressBar;
        [SerializeField] public Text message;
        [SerializeField] private Dropdown dropdown;
        [SerializeField] private ChatGPT gtpScript;
        
        private readonly string fileName = "output.wav";
        private readonly int duration = 5;
        
        private AudioClip clip;
        private bool isRecording;
        private float time;
        private OpenAIApi openai = new OpenAIApi();

        private void Start()
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            dropdown.options.Add(new Dropdown.OptionData("Microphone not supported on WebGL"));
            #else
            foreach (var device in Microphone.devices)
            {
                dropdown.options.Add(new Dropdown.OptionData(device));
            }
            recordButton.onClick.AddListener(StartRecording);
            dropdown.onValueChanged.AddListener(ChangeMicrophone);
            
            var index = PlayerPrefs.GetInt("user-mic-device-index");
            dropdown.SetValueWithoutNotify(index);
            #endif
        }

        private void ChangeMicrophone(int index)
        {
            PlayerPrefs.SetInt("user-mic-device-index", index);
        }
        
        public void StartRecording()
        {
            isRecording = true;
            recordButton.enabled = false;

            //var index = PlayerPrefs.GetInt("user-mic-device-index");

            #if !UNITY_WEBGL
            //clip = Microphone.Start(dropdown.options[index].text, false, duration, 44100);
            clip = Microphone.Start(Microphone.devices[microphoneIndex], false, duration, 44100);
            #endif
        }

        private async void EndRecording()
        {
            message.text = "Transcripting...";
            
            #if !UNITY_WEBGL
            Microphone.End(null);
            #endif
            
            byte[] data = SaveWav.Save(fileName, clip);
            
            var req = new CreateAudioTranslationRequest
            {
                FileData = new FileData() {Data = data, Name = "audio.wav"},
                // File = Application.persistentDataPath + "/" + fileName,
                Model = "whisper-1",
                
            };
            var res = await openai.CreateAudioTranslation(req);

            
            message.text = res.Text; //This is what's displayed. Use this to feed into GPT
            //Part1: Call the SendReply() from ChatGPT.cs
            gtpScript.SendReply(message.text);


        }

        private void Update()
        {
            if (promptButton.action.WasPressedThisFrame())
            { //Starts recording when button is pressed(?)
                Debug.Log("XR Button Works");
                StartRecording();
            }

            if (isRecording)
            {
                time += Time.deltaTime;
                progressBar.fillAmount = time / duration;
                
                if (time >= duration)
                {
                    time = 0;
                    isRecording = false;
                    EndRecording();
                }
            }
        }
    }
}
