using NUnit.Framework;
using OneBagJianghu.Battle.Replay.View;
using UnityEngine;

namespace OneBagJianghu.Tests.Battle.Replay.VfxLab
{
    public sealed class VfxTuningTests
    {
        private VfxTuning tuning;

        [SetUp]
        public void SetUp() { tuning = ScriptableObject.CreateInstance<VfxTuning>(); }

        [TearDown]
        public void TearDown() { Object.DestroyImmediate(tuning); }

        [Test]
        public void MissingKeyIsIdentity()
        {
            Assert.That(tuning.TryGet("missing", out VfxTuning.Entry entry), Is.False);
            Assert.That(VfxTuning.IsIdentity(entry), Is.True);
            VfxTransform result = VfxTuning.Compose(entry, false);
            Assert.That(result.LocalScale, Is.EqualTo(Vector3.one));
            Assert.That(result.Rotation, Is.Zero);
            Assert.That(result.Offset, Is.EqualTo(Vector2.zero));
        }

        [Test]
        public void MirroringNegatesHorizontalValues()
        {
            var entry = new VfxTuning.Entry { rotation = 20f, scale = 2f, offset = new Vector2(12f, 7f) };
            VfxTransform result = VfxTuning.Compose(entry, true);
            Assert.That(result.LocalScale, Is.EqualTo(new Vector3(-2f, 2f, 1f)));
            Assert.That(result.Rotation, Is.EqualTo(-20f));
            Assert.That(result.Offset, Is.EqualTo(new Vector2(-12f, 7f)));
        }

        [Test]
        public void FlipXAndMirrorMakePositiveScaleX()
        {
            VfxTransform result = VfxTuning.Compose(new VfxTuning.Entry { scale = 1f, flipX = true }, true);
            Assert.That(result.LocalScale.x, Is.EqualTo(1f));
        }

        [TestCase(.01f, .2f)]
        [TestCase(8f, 4f)]
        public void ComposeClampsScale(float input, float expected)
        {
            VfxTransform result = VfxTuning.Compose(new VfxTuning.Entry { scale = input }, false);
            Assert.That(result.LocalScale.x, Is.EqualTo(expected));
        }

        [Test]
        public void ResetRemovesTuning()
        {
            tuning.GetOrCreate("hit").rotation = 10f;
            tuning.Reset("hit");
            Assert.That(tuning.TryGet("hit", out _), Is.False);
        }
    }
}
