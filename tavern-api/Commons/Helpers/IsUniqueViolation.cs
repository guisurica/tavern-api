using Npgsql;

namespace tavern_api.Commons.Helpers;

public static class IsUniqueViolation 
{ 
    public static bool Execute(Exception ex)
    {
        if (ex.InnerException is PostgresException pg && pg.SqlState == PostgresErrorCodes.UniqueViolation)
            return true;

        return false;
    }
}
