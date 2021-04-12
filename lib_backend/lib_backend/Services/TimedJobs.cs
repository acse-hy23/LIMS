using Pomelo.AspNetCore.TimedJob;

namespace lib_backend.Services
{
    public class TimedJobs : Job
    {
        /// <summary>
        ///     每 10 分钟刷新一次数据库中的借阅超期情况
        /// </summary>
        [Invoke(Begin = "2020-09-01 00:00:00", Interval = 1000 * 600, SkipWhileExecuting = true)]
        public void Do()
        {
            GlobalFunc.RefreshBookState();
        }
    }
}