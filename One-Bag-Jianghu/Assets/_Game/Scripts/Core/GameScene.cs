namespace OneBagJianghu.Core
{
    public enum GameScene
    {
        Login,
        Home,
        Gameplay
    }

    public static class GameScenePaths
    {
        public static string GetPath(GameScene scene)
        {
            return scene switch
            {
                GameScene.Login => "Assets/_Game/Scenes/LoginScene.unity",
                GameScene.Home => "Assets/_Game/Scenes/HomeScene.unity",
                GameScene.Gameplay => "Assets/_Game/Scenes/GameplayScene.unity",
                _ => throw new System.ArgumentOutOfRangeException(nameof(scene), scene, null)
            };
        }

        public static string GetName(GameScene scene)
        {
            string path = GetPath(scene);
            return System.IO.Path.GetFileNameWithoutExtension(path);
        }
    }
}