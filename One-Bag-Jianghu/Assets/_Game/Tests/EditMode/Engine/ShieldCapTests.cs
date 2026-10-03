using System.Linq;
using NUnit.Framework;
using OneBagJianghu.Engine;

namespace OneBagJianghu.Tests.Engine
{
    public sealed class ShieldCapTests
    {
        [Test]
        public void ShieldGainIsCappedAtCurrentMaxHpAndReportsActualGain()
        {
            const string items =
"[{\"id\":\"armor\",\"name\":\"Armor\",\"type\":\"armor\",\"size\":[1,1]," +
"\"stats\":{\"hp\":900},\"triggers\":[{\"on\":\"battle_start\"," +
"\"effects\":[{\"op\":\"shield\",\"value\":800}," +
"{\"op\":\"shield\",\"value\":500},{\"op\":\"shield\",\"value\":100}]}]}]";
            Loadout a =
Loadout.FromJson("{\"name\":\"a\",\"level\":1,\"items\":[{\"id\":\"armor\",\"x\":0,\"y\":0}]}");
            Loadout b = new Loadout
            {
                name = "b",
                level = 181
            };
            BattleResult result = new BattleSimulator(ItemCatalog.Load(items)).Run(a, b, 1);
            BattleEvent[] shields =
result.events.Where(e => e.type == EventType.SHIELD && e.side == 0).ToArray();
            Assert.That(shields.Select(e => e.value).ToArray(),
Is.EqualTo(new int?[] { 800,
200,
0 }));
            Assert.That(shields.Select(e => e.shield).ToArray(),
Is.EqualTo(new int?[] { 800,
1000,
1000 }));
        }
    }
}