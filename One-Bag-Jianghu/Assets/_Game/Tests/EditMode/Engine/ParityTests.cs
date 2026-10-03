using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OneBagJianghu.Engine;
using UnityEngine;
using Rect = OneBagJianghu.Engine.Rect;

namespace OneBagJianghu.Tests.Engine
{
    public sealed class ParityTests
    {
        public static IEnumerable<string> Cases()
        {
            string folder = Path.Combine(Application.dataPath, "_Game/Tests/EditMode/Data/Parity");
            foreach (string file in Directory.GetFiles(folder, "case_*.json"))
            {
                yield return file;
            }
        }

        [TestCaseSource(nameof(Cases))]
        public void GoldenCaseMatchesEveryField(string file)
        {
            string data = Path.Combine(Application.dataPath, "_Game/Resources/Data");
            ItemCatalog items = ItemCatalog.Load(File.ReadAllText(Path.Combine(data, "items.json")));
            SectCatalog sects = SectCatalog.Load(File.ReadAllText(Path.Combine(data, "sects.json")));
            JObject golden = JObject.Parse(File.ReadAllText(file));
            Loadout a = golden["a"].ToObject<Loadout>();
            Loadout b = golden["b"].ToObject<Loadout>();
            JObject expected = (JObject)golden["expected"];
            expected.Remove("draw");
            JObject actual =
new BattleSimulator(items,
sects,
BattleConfig.Defaults()).Run(a,
b,
golden.Value<long>("seed")).ToJObject();
            foreach (string field in new[]
            {
                "winner",
                "durationMs",
                "finalHp",
                "bags",
                "items"
            }

            )
            {
                Assert.That(JToken.DeepEquals(expected[field],
actual[field]),
Is.True,
Path.GetFileName(file) +
" field " +
field);
            }

            JArray expectedEvents = (JArray)expected["events"];
            JArray actualEvents = (JArray)actual["events"];
            Assert.That(actualEvents.Count,
Is.EqualTo(expectedEvents.Count),
Path.GetFileName(file) +
" event count");
            for (int i = 0; i < expectedEvents.Count; i++)
            {
                if (JToken.DeepEquals(expectedEvents[i], actualEvents[i]))
                    continue;
                int start = System.Math.Max(0, i - 3);
                JArray context = new JArray();
                for (int j = start; j < i; j++)
                {
                    context.Add(actualEvents[j]);
                }

                Assert.Fail(Path.GetFileName(file) +
" event " +
i +
"\nexpected: " +
expectedEvents[i] +
"\nactual: " +
actualEvents[i] +
"\nprevious actual: " +
context);
            }
        }

        [Test]
        public void BagGridFindsOverlapAndOutside()
        {
            List<string> errors =
new BagGrid(2,
2).Validate(new[] { new Rect(0, 0, 2, 1),
new Rect(1, 0, 2, 1) });
            Assert.That(errors, Has.Count.EqualTo(2));
        }

        [Test]
        public void SplitMix64IsStable()
        {
            Rng rng = new Rng(0);
            Assert.That(rng.NextLong(),
Is.EqualTo(unchecked((long)0xE220A8397B1DCDAFUL)));
        }

        [Test]
        public void CatalogRejectsUnknownJsonFields()
        {
            Assert.Throws<DataException>(() => ItemCatalog.Load("[{\"id\":\"x\",\"unknown\":1}]"));
        }
    }
}