using NUnit.Framework;
using Status = ClientSystem.BTCore.BTNode.Status;

namespace ClientSystem.BTCore.Tests
{
    public class BTSequenceTests
    {
        [TestCase(Status.Success, Status.Success, Status.Success)]
        [TestCase(Status.Success, Status.Failure, Status.Failure)]
        [TestCase(Status.Failure, Status.Success, Status.Failure)]
        [TestCase(Status.Success, Status.Running, Status.Running)]
        [TestCase(Status.Running, Status.Success, Status.Running)]
        public void 자식_결과_조합에_따른_반환값(Status first, Status second, Status expected)
        {
            var root = new BTBuilder()
                .Sequence()
                    .Action(() => first)
                    .Action(() => second)
                .End()
                .Build();

            Assert.AreEqual(expected, root.Tick());
        }

        [Test]
        public void 자식이_없으면_Success()
        {
            var root = new BTBuilder().Sequence().End().Build();

            Assert.AreEqual(Status.Success, root.Tick());
        }

        [Test]
        public void 각_자식은_한_틱에_한_번만_호출된다()
        {
            var a = new BTProbe(Status.Success);
            var b = new BTProbe(Status.Success);
            var root = new BTBuilder()
                .Sequence()
                    .Action(a.Tick)
                    .Action(b.Tick)
                .End()
                .Build();

            root.Tick();

            Assert.AreEqual(1, a.TickCount);
            Assert.AreEqual(1, b.TickCount);
        }

        [Test]
        public void Failure가_나오면_뒤의_자식은_호출되지_않는다()
        {
            var after = new BTProbe(Status.Success);
            var root = new BTBuilder()
                .Sequence()
                    .Action(() => Status.Failure)
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
                .Sequence()
                    .Action(() => Status.Running)
                    .Action(after.Tick)
                .End()
                .Build();

            root.Tick();

            Assert.AreEqual(0, after.TickCount);
        }

        [Test]
        public void Running_다음_틱에_앞쪽_자식이_다시_호출된다()
        {
            var first = new BTProbe(Status.Success);
            var running = new BTProbe(Status.Running);
            var root = new BTBuilder()
                .Sequence()
                    .Action(first.Tick)
                    .Action(running.Tick)
                .End()
                .Build();

            root.Tick();
            root.Tick();

            // 처음부터 다시 보기: 이미 Success였던 앞쪽 자식도 매 틱 다시 호출
            Assert.AreEqual(2, first.TickCount);
            Assert.AreEqual(2, running.TickCount);
        }

        [Test]
        public void Running_중_앞쪽_Condition이_거짓이_되면_Failure()
        {
            var inRange = new BTConditionProbe(true);
            var attack = new BTProbe(Status.Running);
            var root = new BTBuilder()
                .Sequence()
                    .Condition(inRange.Check)
                    .Action(attack.Tick)
                .End()
                .Build();

            Assert.AreEqual(Status.Running, root.Tick());

            inRange.Result = false;
            attack.ResetCount();

            Assert.AreEqual(Status.Failure, root.Tick());
            Assert.AreEqual(0, attack.TickCount);
        }
    }
}
