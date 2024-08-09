namespace yournamespace
{
    public class JaegerHelper
    {
        private ActivitySource Activity = new ActivitySource("Shipping");
        public static JaegerHelper CreateInstance()
        {
            return new JaegerHelper();
        }
        public static JaegerHelper CreateInstance(string activitySource)
        {
            return new JaegerHelper()
            {
                Activity = new ActivitySource(activitySource)
            };
        }

        public Activity StartLogActivity([CallerMemberName] string methodName = "")
        {
            var activity = Activity.StartActivity(methodName);
            PropagationContext ctx = Propagators.DefaultTextMapPropagator.Extract(default(PropagationContext), HttpContext.Current.Request.Headers, extract);
            activity.SetParentId(ctx.ActivityContext.TraceId, ctx.ActivityContext.SpanId, ctx.ActivityContext.TraceFlags);
            return activity;
        }
        private IEnumerable<string> extract(System.Collections.Specialized.NameValueCollection arg1, string arg2)
        {
            return arg1.GetValues(arg2);
        }

        /// <summary>
        /// 把使用到的變數跟方法名稱加到最上層(Root)的Trace
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <param name="methodName"></param>
        public static void SetTagAndMethodNameToRootTrace(string tag, string value, [CallerMemberName] string methodName = "")
        {
            var tagAndValues = new Dictionary<string, string>()
            {
                {"method", methodName },
                {tag, value }
            };

            SetTagsToRootTrace(tagAndValues);
        }
        /// <summary>
        /// 在Root Trace設置Tags
        /// </summary>
        /// <param name="keyValues"></param>
        public static void SetTagsToRootTrace(Dictionary<string, string> keyValues)
        {
            var rootActivity = GetRootParentActivity();
            if (rootActivity != null)
            {
                foreach (var keyValue in keyValues)
                {
                    rootActivity.SetTag(keyValue.Key, keyValue.Value);
                }
            }
        }

        /// <summary>
        /// 取得最上層的Activity，如果沒有Current Activity則會回傳null
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public static Activity GetRootParentActivity()
        {
            var currentActivity = System.Diagnostics.Activity.Current;
            return GetRootParentActivity(currentActivity) ?? currentActivity;
        }
        /// <summary>
        /// 遞迴取得最上層的Root Trace
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        private static Activity GetRootParentActivity(Activity activity)
        {
            var parentActivity = activity is null ? null : activity.Parent;
            return parentActivity is null ? activity : GetRootParentActivity(parentActivity);
        }
    }
}
