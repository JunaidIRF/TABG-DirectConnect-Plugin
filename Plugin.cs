using BepInEx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Landfall.Network;
using Landfall.TABG.UI;
using Landfall.TABG;
using BepInEx.Logging;
using System.Net;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

namespace TABGDirectConnectPlugin
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {

        TMP_InputField ipAddressField;
        TMP_InputField portField;
        TMP_InputField passwordField;
        Button connectButton;
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            SceneManager.sceneLoaded += OnGameLoaded;
        }
        private void OnGameLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == 0) // 0 = main menu
            {
                StartCoroutine("LoadDirectConnectUI_delayed");
            }
        }

        private IEnumerator LoadDirectConnectUI_delayed()
        {
            yield return new WaitForSeconds(2f);
            LoadDirectConnectUI();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if(SceneManager.GetActiveScene().buildIndex == 0) //if in server browser (main menu scene buildindex)
                {
                    if (GameObject.Find("/MainMenuCamPivot/MainMenuCam/UICAM/Canvas/Server Browser/directConnectHeader")==null) //if its not already loaded*
                    {
                        LoadDirectConnectUI();
                        Logger.LogInfo("Loaded Direct Connect UI!");
                    }
                }
            }
        }

        private void LoadDirectConnectUI()
        {
            GameObject CSheader = GameObject.Find("/MainMenuCamPivot/MainMenuCam/UICAM/Canvas/Server Browser/Header"); //community server header (to dupe and change to direct connect header)
            GameObject SNFinputField = GameObject.Find("/MainMenuCamPivot/MainMenuCam/UICAM/Canvas/Server Browser/ListHeader/Filters/InputField (TMP)"); // search name filter input field
            GameObject RButton = GameObject.Find("/MainMenuCamPivot/MainMenuCam/UICAM/Canvas/Server Browser/ListHeader/Filters/Refresh"); // refresh button

            GameObject directConnectHeader = Instantiate(CSheader, CSheader.transform.parent);
            GameObject ipAddressObj = Instantiate(SNFinputField, SNFinputField.transform.parent);
            GameObject portObj = Instantiate(SNFinputField, SNFinputField.transform.parent);
            GameObject passwordObj = Instantiate(SNFinputField, SNFinputField.transform.parent);
            GameObject connectObj = Instantiate(RButton, RButton.transform.parent);

            ipAddressField = ipAddressObj.GetComponent<TMP_InputField>();
            portField = portObj.GetComponent<TMP_InputField>();
            passwordField = passwordObj.GetComponent<TMP_InputField>();
            connectButton = connectObj.GetComponent<Button>();

            ipAddressObj.name = "ipAddressField";
            portObj.name = "portField";
            passwordObj.name = "passwordField";
            connectObj.name = "connectButton";
            directConnectHeader.name = "directConnectHeader";

            RectTransform ipAddressRectTransform = ipAddressObj.GetComponent<RectTransform>();
            RectTransform portRectTransform = portObj.GetComponent<RectTransform>();
            RectTransform passwordRectTransform = passwordObj.GetComponent<RectTransform>();

            ipAddressRectTransform.sizeDelta = new Vector2(200f, ipAddressRectTransform.sizeDelta.y);
            portRectTransform.sizeDelta = new Vector2(100f, portRectTransform.sizeDelta.y);
            passwordRectTransform.sizeDelta = new Vector2(200f, passwordRectTransform.sizeDelta.y);


            ipAddressObj.transform.position = new Vector3(ipAddressObj.transform.position.x+600f, ipAddressObj.transform.position.y, ipAddressObj.transform.position.z);
            directConnectHeader.transform.position = new Vector3(directConnectHeader.transform.position.x+679f, directConnectHeader.transform.position.y, directConnectHeader.transform.position.z);
            
            
            GameObject.Find("/MainMenuCamPivot/MainMenuCam/UICAM/Canvas/Server Browser/ListHeader/Filters/ipAddressField/Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = "IP Address";
            GameObject.Find("/MainMenuCamPivot/MainMenuCam/UICAM/Canvas/Server Browser/ListHeader/Filters/portField/Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = "Port";
            GameObject.Find("/MainMenuCamPivot/MainMenuCam/UICAM/Canvas/Server Browser/ListHeader/Filters/passwordField/Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = "Password";
            connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Connect";

            directConnectHeader.GetComponent<TextMeshProUGUI>().text = "DIRECT CONNECT";
            portField.contentType = TMP_InputField.ContentType.IntegerNumber;
            passwordField.contentType = TMP_InputField.ContentType.Password;
            connectButton.onClick.RemoveAllListeners();
            connectButton.onClick.AddListener(() => DirectConnect());

        }

        private void DirectConnect()
        {
            if (!string.IsNullOrEmpty(passwordField.text))
            {
                ServerConnector.LastPassword = passwordField.text;
            }

            ServerConnector.Instance.ConnectToServerIP(ipAddressField.text, ushort.Parse(portField.text), "");
        }

    }
}
