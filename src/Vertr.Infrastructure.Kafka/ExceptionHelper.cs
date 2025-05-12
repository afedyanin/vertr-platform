namespace Vertr.Infrastructure.Kafka;
internal static class ExceptionHelper
{
    public static bool IsCritical(this Exception exception)
    {
        if (exception is OutOfMemoryException)
        {
            return true;
        }

        return false;
    }
}
