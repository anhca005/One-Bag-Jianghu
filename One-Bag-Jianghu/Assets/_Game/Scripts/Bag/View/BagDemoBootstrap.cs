using System;
using System.IO;
using OneBagJianghu.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.Bag.View
{
    public sealed class BagDemoBootstrap : MonoBehaviour
    {
        [SerializeField]
        private BagLoadoutPanel loadoutPanel;
        [SerializeField]
        private Button saveButton;
        [SerializeField]
        private Button loadButton;
        [SerializeField]
        private Button clearButton;
        private const string HeroName = "Demo Loadout";
        private string SavePath => Path.Combine(Application.persistentDataPath, "loadout.json");

        private void Awake()
        {
            saveButton.onClick.AddListener(Save);
            loadButton.onClick.AddListener(Load);
            clearButton.onClick.AddListener(Clear);
        }

        private void Start() => Bind(string.Empty);
        private void Save()
        {
            try
            {
                File.WriteAllText(SavePath, loadoutPanel.ToJson(HeroName));
                Debug.Log("Saved bag loadout to: " + SavePath);
            }
            catch (Exception exception)
            {
                Debug.LogError("Could not save bag loadout: " + exception.Message);
            }
        }

        private void Load()
        {
            if (!File.Exists(SavePath))
            {
                Debug.LogWarning("No saved bag loadout at: " + SavePath);
                return;
            }

            try
            {
                Bind(File.ReadAllText(SavePath));
            }
            catch (Exception exception)
            {
                Debug.LogError("Could not load bag loadout: " + exception.Message);
            }
        }

        private void Clear() => Bind(string.Empty);
        private void Bind(string json) => loadoutPanel.Bind(string.Empty, 1, json);
    }
}