using System.Collections;
using System.Collections.Generic;
using OneBagJianghu.Bag;
using OneBagJianghu.Pve;
using OneBagJianghu.UI.Common;
using OneBagJianghu.UI.Popups;
using UnityEngine;
using UnityEngine.UI;
using OneBagJianghu.Audio;

namespace OneBagJianghu.UI.Pve
{
    public sealed class DialoguePopupData
    {
        public DialoguePopupData(PveCatalog catalog, List<PveDialogueLine> lines)
        {
            Catalog = catalog;
            Lines = lines;
        }

        public PveCatalog Catalog;
        public List<PveDialogueLine> Lines;
    }

    public sealed class DialoguePopup : BasePopup<DialoguePopupData>
    {
        [SerializeField]
        private Image portrait, dialogueFrame;
        [SerializeField]
        private RectTransform portraitLeft, portraitRight;
        [SerializeField]
        private Text speakerText, bodyText;
        [SerializeField]
        private Button advanceButton;
        [SerializeField]
        private UiButton skipButton;
        private DialoguePopupData data;
        private int index;
        private Coroutine typing;
        private bool complete;
        private PveArtLibrary art;
        private ItemIconLibrary itemArt;
        protected override void Awake()
        {
            base.Awake();
            advanceButton.onClick.AddListener(Advance);
            skipButton.onClick.AddListener(Close);
        }

        // PopupManager calls Setup before the popup is active, so the typewriter starts in OnShown.
        protected override void OnSetup(DialoguePopupData value)
        {
            data = value;
            index = 0;
            art = PveArtLibrary.Load();
            itemArt = ItemIconLibrary.Load();
            speakerText.text = string.Empty;
            bodyText.text = string.Empty;
            complete = false;
            Sprite frame = Get("frame_dialogue");
            if (dialogueFrame != null && frame != null)
            {
                dialogueFrame.sprite = frame;
                dialogueFrame.type = Image.Type.Simple;
                dialogueFrame.preserveAspect = false;
            }
        } // the box is authored at the scroll's 3:1 aspect dialogueFrame.color = Color.white; } }

        protected override void OnShown()
        {
            base.OnShown();
            if (data != null && index < data.Lines.Count)
                ShowLine();
        }

        public override bool OnBackPressed()
        {
            Close();
            return true;
        }

        private void Advance()
        {
            AudioManager.Instance?.Play("dialogue_next");
            if (!complete)
            {
                if (typing != null)
                    StopCoroutine(typing);
                bodyText.text = data.Lines[index].Text;
                complete = true;
                return;
            }

            index++;
            if (index >= data.Lines.Count)
                Close();
            else
                ShowLine();
        }

        private void ShowLine()
        {
            PveDialogueLine line = data.Lines[index];
            PveCharacter character = data.Catalog.GetCharacter(line.Speaker);
            speakerText.text = character?.Name ?? line.Speaker;
            portrait.transform.SetParent(line.Speaker == "hero" ? portraitLeft : portraitRight, false);
            portrait.rectTransform.anchorMin = Vector2.zero;
            portrait.rectTransform.anchorMax = Vector2.one;
            portrait.rectTransform.offsetMin = portrait.rectTransform.offsetMax = Vector2.zero;
            portrait.sprite = Get(character?.Portrait);
            portrait.color = portrait.sprite == null ? new Color(.2f, .2f, .2f, .25f) : Color.white;
            if (typing != null)
                StopCoroutine(typing);
            typing = null;
            if (!isActiveAndEnabled)
            {
                bodyText.text = line.Text;
                complete = true;
                return;
            }

            typing = StartCoroutine(Type(line.Text));
        }

        private IEnumerator Type(string value)
        {
            complete = false;
            bodyText.text = string.Empty;
            foreach (char c in value)
            {
                bodyText.text += c;
                yield return new WaitForSecondsRealtime(.025f);
            }

            complete = true;
            typing = null;
        }

        private Sprite Get(string key)
        {
            if (art != null && art.TryGet(key, out Sprite sprite))
                return sprite;
            return itemArt != null && itemArt.TryGet(key, out sprite) ? sprite : null;
        }
    }
}
