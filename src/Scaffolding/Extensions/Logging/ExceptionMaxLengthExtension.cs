namespace Scaffolding.Extensions.Logging;

public static class ExceptionMaxLengthExtension
{
    public readonly static int ErrorMessageLength = 256;

    public readonly static int ErrorExceptionLength = 1024;

    static ExceptionMaxLengthExtension()
    {
        var errorMessageMaxLength = Environment.GetEnvironmentVariable("SERILOG_ERROR_MESSAGE_MAX_LENGTH");
        if (!string.IsNullOrWhiteSpace(errorMessageMaxLength))
        {
            _ = int.TryParse(errorMessageMaxLength, out ErrorMessageLength);
        }

        var errorExceptionMaxLength = Environment.GetEnvironmentVariable("SERILOG_ERROR_EXCEPTION_MAX_LENGTH");
        if (!string.IsNullOrWhiteSpace(errorExceptionMaxLength))
        {
            _ = int.TryParse(errorExceptionMaxLength, out ErrorExceptionLength);
        }
    }
}
