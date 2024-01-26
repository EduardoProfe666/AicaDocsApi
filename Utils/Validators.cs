using System.ComponentModel.DataAnnotations;

namespace AicaDocsApi.Utils;

public static class Validators
{
    public static Tuple<bool, IEnumerable<string?>> ValidateModel(object model)
    {
        try
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model);
            var isValid = Validator.TryValidateObject(model, ctx, validationResults, true);
            var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();
            return new Tuple<bool, IEnumerable<string?>>(isValid, errors);
        }
        catch (Exception e)
        {
            return new Tuple<bool, IEnumerable<string?>>(false, ["Bad Format Request"]);
        }
        
    }
}