using System.Collections.Generic;
using UnityEngine;

namespace OneBagJianghu.Onboarding
{
    public sealed class TutorialTarget : MonoBehaviour
    {
        [SerializeField]
        private string id;
        public string Id => id;

        public void Configure(string value)
        {
            id = value;
        }

        public static TutorialTarget Find(string targetId)
        {
            if (string.IsNullOrWhiteSpace(targetId))
                return null;
            IReadOnlyList<TutorialTarget> targets = FindObjectsOfType<TutorialTarget>(true);
            for (int i = 0; i < targets.Count; i++)
                if (targets[i].id == targetId && targets[i].isActiveAndEnabled)
                    return targets[i];
            return null;
        }
    }
}
