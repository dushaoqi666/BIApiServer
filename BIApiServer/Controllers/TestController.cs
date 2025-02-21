using BIApiServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace BIApiServer.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly RedisService _redis;

    public TestController(RedisService redis)
    {
        _redis = redis;
    }

    [HttpGet("redis-test")]
    public async Task<IActionResult> TestRedis()
    {
        // 存储字符串
        await _redis.SetValueAsync("test:key", "Hello Redis!");
        
        // 获取字符串
        var value = await _redis.GetValueAsync("test:key");

        // 存储对象
        var user = new { Name = "John", Age = 30 };
        await _redis.SetObjectAsync("test:user", user, TimeSpan.FromMinutes(30));

        // 获取对象
        var cachedUser = await _redis.GetObjectAsync<dynamic>("test:user");

        // 删除键
        await _redis.RemoveKeyAsync("test:key");

        return Ok(new { value, cachedUser });
    }
}