using NUnit.Framework;
using Status = ClientSystem.BTCore.BTNode.Status;

namespace ClientSystem.BTCore.Tests
{
    public class BTSelectorTests
    {
        [TestCase(Status.Success, Status.Failure, Status.Success)]
        [TestCase(Status.Failure, Status.Success, Status.Success)]
        [TestCase(Status.Failure, Status.Failure, Status.Failure)]
        [TestCase(Status.Failure, Status.Running, Status.Running)]
        [TestCase(Status.Running, Status.Success, Status.Running)]
        public void 자식_결과_조합에_따른_반환값(Status first, Status second, Status expected)
        {
            var root = new BTBuilder()
                .Selector()
                    .Action(() => first)
                    .Action(() => second)
                .End()
                .Build();

            Assert.AreEqual(expected, root.Tick());
        }

        [Test]
        public void 자식이_없으면_Failure()
        {
            var root = new BTBuilder().Selector().End().Build();

            Assert.AreEqual(Status.Failure, root.Tick());
        }

        [Test]
        public void 각_자식은_한_틱에_한_번만_호출된다()
        {
            var a = new BTProbe(Status.Failure);
            var b = new BTProbe(Status.Failure);
            var root = new BTBuilder()
                .Selector()
                    .Action(a.Tick)
                    .Action(b.Tick)
                .End()
                .Build();

            root.Tick();

            Assert.AreEqual(1, a.TickCount);
            Assert.AreEqual(1, b.TickCount);
        }

        [Test]
        public void Success가_나오면_뒤의_자식은_호출되지_않는다()
        {
            var after = new BTProbe(Status.Success);
            var root = new BTBuilder()
                .Selector()
                    .Action(() => Status.Success)
                    .Action(after.Tick)
                .End()
                .Build();

            root.Tick();

            Assert.AreEqual(0, after.TickCount);
        }

        [Test]
        public void Running이_나오면_뒤의_자식은_호출되지_않는다()
        {
            var after = new BTProbe(Status.Success);
            var root = new BTBuilder()
                .Selector()
                    .Action(() => Status.Running)
                    .Action(after.Tick)
                .End()
                .Build();

            root.Tick();

            Assert.AreEqual(0, after.TickCount);
        }

        [Test]
        public void 하위_분기가_Running이어도_다음_틱에_상위_분기를_다시_평가한다()
        {
            var seePlayer = new BTConditionProbe(false);
            var chase = new BTProbe(Status.Running);
            var patrol = new BTProbe(Status.Running);
            var root = new BTBuilder()
                .Selector()
                    .Sequence()
                        .Condition(seePlayer.Check)
                        .Action(chase.Tick)
                    .End()
                    .Action(patrol.Tick)
                .End()
                .Build();

            // 순찰 중
            root.Tick();
            Assert.AreEqual(1, patrol.TickCount);
            Assert.AreEqual(0, chase.TickCount);

            // 플레이어 발견 → 다음 틱에 바로 추격으로 전환
            seePlayer.Result = true;
            root.Tick();
            Assert.AreEqual(1, chase.TickCount);
            Assert.AreEqual(1, patrol.TickCount);
        }
    }
}
