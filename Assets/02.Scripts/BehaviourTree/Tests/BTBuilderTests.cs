using System;
using NUnit.Framework;
using Status = ClientSystem.BTCore.BTNode.Status;

namespace ClientSystem.BTCore.Tests
{
    public class BTBuilderTests
    {
        [Test]
        public void 중첩된_Composite가_부모에_연결된다()
        {
            var inner = new BTProbe(Status.Success);
            var root = new BTBuilder()
                .Selector()
                    .Action(() => Status.Failure)
                    .Sequence()
                        .Action(inner.Tick)
                    .End()
                .End()
                .Build();

            Assert.AreEqual(Status.Success, root.Tick());
            Assert.AreEqual(1, inner.TickCount);
        }

        [Test]
        public void Leaf_하나만으로_트리를_만들_수_있다()
        {
            var root = new BTBuilder()
                .Action(() => Status.Running)
                .Build();

            Assert.AreEqual(Status.Running, root.Tick());
        }

        [Test]
        public void 열린_Composite_없이_End를_호출하면_예외()
        {
            Assert.Throws<InvalidOperationException>(() => new BTBuilder().End());
        }

        [Test]
        public void End를_빠뜨리고_Build하면_예외()
        {
            var builder = new BTBuilder()
                .Selector()
                    .Action(() => Status.Success);

            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Test]
        public void 노드_없이_Build하면_예외()
        {
            Assert.Throws<InvalidOperationException>(() => new BTBuilder().Build());
        }

        [Test]
        public void 루트를_닫은_뒤_Composite를_추가하면_예외()
        {
            var builder = new BTBuilder().Selector().End();

            Assert.Throws<InvalidOperationException>(() => builder.Sequence());
        }

        [Test]
        public void 루트를_닫은_뒤_Leaf를_추가하면_예외()
        {
            var builder = new BTBuilder().Selector().End();

            Assert.Throws<InvalidOperationException>(() => builder.Action(() => Status.Success));
            Assert.Throws<InvalidOperationException>(() => builder.Condition(() => true));
        }

        [Test]
        public void Leaf_루트_뒤에_노드를_추가하면_예외()
        {
            var builder = new BTBuilder().Action(() => Status.Success);

            Assert.Throws<InvalidOperationException>(() => builder.Selector());
        }

        [Test]
        public void null_델리게이트는_빌더에서도_예외()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new BTBuilder().Selector().Action(null));
        }
    }
}
