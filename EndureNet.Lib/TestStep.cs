namespace EndureNet.Lib;

public class TestStep
{
    private TestStep()
    {
    }

    public Func<HttpResponseMessage?, HttpRequestMessage>? RequestFactory { get; private set; }
    public TestStep? Next { get; set; }

    public static TestStep CreateRequest(Func<HttpResponseMessage?, HttpRequestMessage> requestFactory)
    {
        if (requestFactory == null) throw new ArgumentNullException(nameof(requestFactory));

        var step = new TestStep
        {
            RequestFactory = requestFactory
        };
        return step;
    }
}