using System.ComponentModel.DataAnnotations;

namespace catalogo_filmes_previsao_tempo.Validators;

public class LatitudeLongitudeValidator
{
    public static ValidationResult? ValidateLatitude(double? latitude)
    {
        if (!latitude.HasValue)
            return ValidationResult.Success;

        if (latitude < -90 || latitude > 90)
            return new ValidationResult("Latitude deve estar entre -90 e 90 graus.");

        return ValidationResult.Success;
    }

    public static ValidationResult? ValidateLongitude(double? longitude)
    {
        if (!longitude.HasValue)
            return ValidationResult.Success;

        if (longitude < -180 || longitude > 180)
            return new ValidationResult("Longitude deve estar entre -180 e 180 graus.");

        return ValidationResult.Success;
    }
}