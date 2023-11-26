# HttpClientExtensions

The **HttpClientExtensions** is a collection of extension methods that makes sending HTTP requests with HttpClient more convenient and readable.

### How to use:

Here is an example that covers most extension methods:
```
OneOf<Option<ResponseModel>, HttpResponseError> result = await new HttpClient()
    .CreateRequest()
    .SetUrl("url")
    .WithBasicAuthentication("username", "password") // OR .WithBearerToken("token")
    .WithBearerToken("token")
    .WithHeader("hKey1", "hValue1")
    .WithHeaders(new[] { new KeyValuePair<string, string?>("hKey2", "hValue2") })
    .WithQueryParam("qpKey1", "qpValue1")
    .WithQueryParams(new[] { new KeyValuePair<string, string?>("qpKey2", "qpKey2") })
    .WithJsonContent(new RequestModel()) // OR WithFormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>>) OR WithContent(HttpContent)
    .WithTimeout(TimeSpan.FromSeconds(5))
    .GetAsync()
    .OnSuccess(responseResult => Console.WriteLine("success"))
    .OnFailure(error => Console.WriteLine("failure"))
    .DeserializeJsonResultAsync<ResponseModel>(allowEmptyContent: true);

var responseModel = result.Match(
    r => r.ExtractOrDefault(),
    e => null);
```

The `OneOf` library provides discriminated unions for C#, allowing to return a result or an error.

The `Option` class offers an explicit way to indicate whether a return value is present or not.
