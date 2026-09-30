using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Loading
{
    public sealed class LoadingPanel : MonoBehaviour
    {
        public static bool IsVisible { get; private set; }

        private static readonly string[] Tips =
        {
            "Xếp các món liền kề để phối hợp chiêu thức.",
            "Hóa Kình đúng lúc có thể xoay chuyển thế trận.",
            "Trạng thái tích đủ 50 điểm sẽ bộc phát.",
            "Tâm Pháp tăng sức mạnh cho hệ binh khí phù hợp.",
            "Mỗi ô trong bao phục đều đáng giá — hãy sắp xếp cẩn thận.",
            "Trang bị cùng môn phái sẽ phát huy sở trường của hiệp khách.",
            "Quan sát nhật ký giao đấu để hoàn thiện đội hình."
        };

        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Slider progress;
        [SerializeField] private Text percentage;
        [SerializeField] private Text tip;

        private void Awake()
        {
            group.alpha = 0f;
            group.blocksRaycasts = false;
            panelRoot.SetActive(false);
        }

        public IEnumerator Show()
        {
            IsVisible = true;
            panelRoot.SetActive(true);
            group.blocksRaycasts = true;
            SetProgress(0f);
            tip.text = Tips[Random.Range(0, Tips.Length)];
            yield return Fade(0f, 1f, 0.25f);
        }

        public IEnumerator Hide()
        {
            yield return Fade(1f, 0f, 0.25f);
            group.blocksRaycasts = false;
            panelRoot.SetActive(false);
            IsVisible = false;
        }

        public void SetProgress(float value)
        {
            float clamped = Mathf.Clamp01(value);
            progress.value = clamped;
            percentage.text = Mathf.RoundToInt(clamped * 100f) + "%";
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                group.alpha = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }

            group.alpha = to;
        }
    }
}
