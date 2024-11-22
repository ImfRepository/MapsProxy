namespace MapsProxyApi.Extensions
{
    public static class HttpContextExtension
    {
        public static async Task CopyResponseFromAsync(this HttpContext context, HttpResponseMessage responseMessage)
        {
            context.Response.StatusCode = (int)responseMessage.StatusCode;
            foreach (var header in responseMessage.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            // SendAsync removes chunking from the response. This removes the header so it doesn't expect a chunked response.
            context.Response.Headers.Remove("transfer-encoding");

            using (var responseStream = await responseMessage.Content.ReadAsStreamAsync())
            {
                await responseStream.CopyToAsync(context.Response.Body, 81920, context.RequestAborted);
            }
        }
    }
}
