using System.Collections.Generic;
using OneBagJianghu.Economy;
using OneBagJianghu.Pve;

namespace OneBagJianghu.Onboarding
{
    public static class StoryProgress
    {
        public static bool TryQueueBossClear(PlayerProfile profile, PveStage stage, bool firstClear)
        {
            if (!firstClear || stage?.Chapter == null || stage.Index != 10 || stage.Kind != "boss")
                return false;
            int chapter = stage.Chapter.Index;
            if (profile.SeenStoryChapters.Contains(chapter) || profile.PendingStoryChapters.Contains(chapter))
                return false;
            profile.PendingStoryChapters.Add(chapter);
            return true;
        }

        public static void MarkSeen(PlayerProfile profile, int chapter)
        {
            profile.PendingStoryChapters.Remove(chapter);
            if (!profile.SeenStoryChapters.Contains(chapter))
                profile.SeenStoryChapters.Add(chapter);
        }

        public static List<StoryChapter> UnlockedChapters(
            StoryCatalog stories,
            PveCatalog pve,
            PlayerProfile profile)
        {
            var unlocked = new List<StoryChapter>();
            StoryChapter intro = stories.Get(0);
            if (intro != null)
                unlocked.Add(intro);
            foreach (PveChapter chapter in pve.Chapters)
            {
                PveStage boss = chapter.Stages.Find(stage => stage.Index == 10 && stage.Kind == "boss");
                StoryChapter story = stories.Get(chapter.Index);
                if (story != null && boss != null && PveRules.IsCleared(profile, boss.Id))
                    unlocked.Add(story);
            }

            return unlocked;
        }
    }
}
