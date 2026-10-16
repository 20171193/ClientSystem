using ClientSystem.BTCore;

namespace ClientSystem.BTCore.Tests
{
    // 테스트용 가짜 Leaf 동작
    //  : 반환할 결과를 틱 사이에 바꿀 수 있고, 호출 횟수를 기록한다
    public class BTProbe
    {
        public BTNode.Status Result;
        public int TickCount { get; private set; }

        public BTProbe(BTNode.Status result)
        {
            Result = result;
        }

        public BTNode.Status Tick()
        {
            TickCount++;
            return Result;
        }

        public void ResetCount()
        {
            TickCount = 0;
        }
    }

    // 테스트용 가짜 Condition
    public class BTConditionProbe
    {
        public bool Result;
        public int TickCount { get; private set; }

        public BTConditionProbe(bool result)
        {
            Result = result;
        }

        public bool Check()
        {
            TickCount++;
            return Result;
        }
    }
}
