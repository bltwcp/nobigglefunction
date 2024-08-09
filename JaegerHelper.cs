namespace yournamespace
{
   internal static class JaegerHelper
{
    public static Activity StartLogActivity(ActivitySource activitySource, HttpRequestHeaders headers, [CallerMemberName] string methodName = "")
    {

        var activity = activitySource.StartActivity(methodName);
        if (headers != null)
        {
            PropagationContext ctx = Propagators.DefaultTextMapPropagator.Extract(default(PropagationContext), headers, extract);
            activity.SetParentId(ctx.ActivityContext.TraceId, ctx.ActivityContext.SpanId, ctx.ActivityContext.TraceFlags);
        }
        return activity;
    }
    private static IEnumerable<string> extract(IEnumerable<KeyValuePair<string, IEnumerable<string>>> arg1, string arg2)
    {
        var rs = arg1.FirstOrDefault(p => p.Key == arg2);
        if (!rs.Equals(default(KeyValuePair<string, IEnumerable<string>>)))
        {
            return rs.Value;
        }
        return null;
    }
}
}
