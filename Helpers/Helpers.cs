using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace inmobiliariaULP.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password, string salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }
    }

    public static class ModelStateHelper
    {
        // Devuelve solo los mensajes de error (como antes)
        public static List<string> GetErrors(ModelStateDictionary modelState)
        {
            return modelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
        }

        // Devuelve una lista de objetos con el nombre del campo y el mensaje de error
        public static List<(string Field, string Error)> GetFieldErrors(ModelStateDictionary modelState)
        {
            return modelState
                .Where(x => x.Value.Errors.Count > 0)
                .SelectMany(x => x.Value.Errors.Select(e => (Field: x.Key, Error: e.ErrorMessage)))
                .ToList();
        }
    }
}