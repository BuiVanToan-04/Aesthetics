namespace StaticLogger.cs
{
	public static class StaticLogger
{
    private static ILoggerFactory? _loggerFactory;

    // Hàm này dùng để khởi tạo logger từ Program.cs
    public static void InitLogger(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    // Hàm này trả về ILogger cho class bất kỳ
    public static ILogger CreateLogger<T>()
    {
        if (_loggerFactory == null)
            throw new InvalidOperationException("LoggerFactory chưa được khởi tạo!");

        return _loggerFactory.CreateLogger<T>();
    }
}

}
