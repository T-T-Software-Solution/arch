namespace TTSS.Infra.Data.Redis;

internal static class Config
{
    public const int TTL = 100;
    public static Task WaitUntilExpired => Task.Delay(TTL + 100);
    public static string RedisConnection => "localhost:6479,password=yourpasswordforredis,ssl=False,abortConnect=False";
}