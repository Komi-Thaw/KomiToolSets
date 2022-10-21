using RestSharp;

namespace KomiToolSets.RestPack;

public class RestHelper
{
    public static string _uri = string.Empty;

    private static Uri _uri_fetch => new(_uri);

    public static Dictionary<string, object> param_dic = new();

    public static Dictionary<string, string> header_dic = new();

    private static Func<int, string> content_type_fetch = form =>
    {
        return form switch
        {
            1 => Content_Type.application_json_format.content_type_name,
            2 => Content_Type.form_data_format.content_type_name,
            3 => Content_Type.multipart_form_data_format.content_type_name,
            _ => string.Empty
        };
    };

    public async static Task<string> PostSend(int content_form = 1, int timeout_span = 5000,
        bool configure_await = false)
    {
        using var cli = new RestClient();

        var rest = new RestRequest(_uri_fetch, Method.Post);

        var params_fetch =
            new ParametersCollection(param_dic.Distinct().Select(x =>
                Parameter.CreateParameter(x.Key, x.Value, ParameterType.RequestBody)));

        rest.Timeout = timeout_span;

        rest.AddOrUpdateParameters(params_fetch);

        rest.AddHeader("content-type", content_type_fetch.Invoke(content_form));
        rest.AddOrUpdateHeaders(header_dic);

        var result = await cli.ExecuteAsync(rest).ConfigureAwait(configure_await);

        var content = result.IsSuccessful ? result.Content! : default;

        return content!;
    }
}

public record Content_Type
{
    public string content_type_name { get; }

    public static Content_Type application_json_format => new("application/json");

    public static Content_Type form_data_format => new("application/x-www-form-urlencoded");

    public static Content_Type multipart_form_data_format = new("multipart/form-data");

    private Content_Type(string name)
    {
        content_type_name = name;
    }
}