using System;
using NUnit.Framework;
using Status = ClientSystem.BTCore.BTNode.Status;

namespace ClientSystem.BTCore.Tests
{
    public class BTLeafTests
    {
        [TestCase(Status.Success)]
        [TestCase(Status.Failure)]
        [TestCase(Status.Running)]
        public void Action은_델리게이트_결과를_그대로_반환한다(Status result)
        {
            var action = new BTAction(() => result);

            Assert.AreEqual(result, action.Tick());
        }

        [TestCase(true, Status.Success)]
        [TestCase(false, Status.Failure)]
        public void Condition은_참이면_Success_거짓이면_Failure(bool check, Status expected)
        {
            var condition = new BTCondition(() => check);

            Assert.AreEqual(expected, condition.Tick());
        }

        [Test]
        public void Action에_null_델리게이트를_넘기면_예외()
        {
            Assert.Throws<ArgumentNullException>(() => new BTAction(null));
        }

        [Test]
        public void Condition에_null_델리게이트를_넘기면_예외()
        {
            Assert.Throws<ArgumentNullException>(() => new BTCondition(null));
        }
    }
}
