using System.Text.Json;

namespace AicaDocsApi.Tests.Utils;

public static class JsonSerializerOp
{
       public static JsonSerializerOptions GetOptions()
       {
              return new JsonSerializerOptions
              {
                     PropertyNameCaseInsensitive = true
              };
       }
}