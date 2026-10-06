using System.Globalization;

namespace Domain.Shared.Extensions
{
    public static class ConverterValidatorsExtensions
    {
        public static string StringLengthValidation<TEntity>(TEntity entity, int length)
        {
            var value = entity?.ToString();

            if (String.IsNullOrEmpty(value))
                return String.Empty;

            return value.Length > length ? value[..length] : value;
        }

        public static DateTime ConvertToDateTimeValidation<TEntity>(TEntity entity)
        {
            if (entity != null && typeof(TEntity) == typeof(String))
            {
                if (String.IsNullOrEmpty(entity?.ToString()) || !DateTime.TryParse(entity.ToString(), out var parsed) || parsed < new DateTime(1753, 1, 1))
                    return new DateTime(1990, 01, 01, 00, 00, 00, new CultureInfo("en-US").Calendar);

                return parsed;
            }

            if (entity != null && (typeof(TEntity) == typeof(DateTime) || typeof(TEntity) == typeof(DateTime?)))
                return Convert.ToDateTime(entity?.ToString());

            return new DateTime(1990, 01, 01, 00, 00, 00, new CultureInfo("en-US").Calendar);
        }

        public static Int32 ConvertToInt32Validation<TEntity>(TEntity entity)
        {
            if (entity != null && typeof(TEntity) == typeof(String))
            {
                if (String.IsNullOrEmpty(entity?.ToString()) || !Int32.TryParse(entity.ToString(), out var parsed))
                    return 0;

                return parsed;
            }

            if (entity != null && (typeof(TEntity) == typeof(Int32) || typeof(TEntity) == typeof(Int32?)))
                return Convert.ToInt32(entity?.ToString());

            return 0;
        }

        public static Int64 ConvertToInt64Validation<TEntity>(TEntity entity)
        {
            if (entity != null && typeof(TEntity) == typeof(String))
            {
                if (String.IsNullOrEmpty(entity?.ToString()) || !Int64.TryParse(entity.ToString(), out var parsed))
                    return 0;

                return parsed;
            }

            if (entity != null && (typeof(TEntity) == typeof(Int64) || typeof(TEntity) == typeof(Int64?)))
                return Convert.ToInt64(entity?.ToString());

            return 0;
        }

        public static decimal ConvertToDecimalValidation<TEntity>(TEntity entity)
        {
            if (entity != null && typeof(TEntity) == typeof(String))
            {
                if (String.IsNullOrEmpty(entity?.ToString()) || !Decimal.TryParse(entity.ToString(), NumberStyles.Any, new CultureInfo("en-US"), out var parsed))
                    return 0;

                return parsed;
            }

            if (entity != null && (typeof(TEntity) == typeof(decimal) || typeof(TEntity) == typeof(decimal?)))
                return Convert.ToDecimal(entity?.ToString(), new CultureInfo("en-US"));

            return 0;
        }

        public static Guid ConvertToGuidValidation<TEntity>(TEntity entity)
        {
            if (entity != null && typeof(TEntity) == typeof(String))
            {
                if (String.IsNullOrEmpty(entity?.ToString()) || !Guid.TryParse(entity.ToString(), out var parsed))
                    return Guid.Empty;

                return parsed;
            }

            if (entity != null && (typeof(TEntity) == typeof(Guid) || typeof(TEntity) == typeof(Guid?)))
                return Guid.Parse(entity?.ToString());

            return Guid.Empty;
        }

        public static bool ConvertToBooleanValidation<TEntity>(TEntity entity)
        {
            if (entity != null && typeof(TEntity) == typeof(String))
            {
                if (String.IsNullOrEmpty(entity?.ToString()) || !Boolean.TryParse(entity.ToString(), out var parsed))
                    return false;

                return parsed;
            }

            if (entity != null && (typeof(TEntity) == typeof(Boolean) || typeof(TEntity) == typeof(Boolean?)))
                return Convert.ToBoolean(entity?.ToString());

            return false;
        }
    }
}
