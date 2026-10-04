using UnityEditor;
using UnityEngine;

namespace OneBagJianghu.EditorTools.Audio
{
    public sealed class MusicAudioImporter : AssetPostprocessor
    {
        private const string MusicRoot = "Assets/_Game/Audio/Music/";

        private void OnPreprocessAudio()
        {
            if (!assetPath.StartsWith(MusicRoot, System.StringComparison.Ordinal))
                return;
            var importer = (AudioImporter)assetImporter;
            AudioImporterSampleSettings settings = importer.defaultSampleSettings;
            settings.loadType = AudioClipLoadType.Streaming;
            settings.compressionFormat = AudioCompressionFormat.Vorbis;
            settings.preloadAudioData = false;
            importer.defaultSampleSettings = settings;
        }
    }
}
