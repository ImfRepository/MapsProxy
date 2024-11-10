using MapsProxyApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace MapsProxyApi.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class ProxyController : ControllerBase
    //{
    //    private readonly ProxyTestsService _service;

    //    public ProxyController(ProxyTestsService service)
    //    {
    //        _service = service;
    //    }

    //    [HttpGet("testservices/{service}/{*path}")]
    //    public async Task<IActionResult> GetProxy(
    //        string service,
    //        string path,
    //        [FromQuery] string token)
    //    {
    //        var query = Request.QueryString.ToUriComponent();
    //        var response = await _service.Proxy(token, service, path, query);
    //        return Ok(response);
    //    }
    //}
}
