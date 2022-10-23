using Newtonsoft.Json.Linq;
using RestSharp;

namespace KomiToolSets.RestPack;

public class RestHelper
{
    public static string _uri = string.Empty;

    private static Uri _uri_fetch => new(_uri);

    public static Dictionary<string, object> param_dic = new();

    public static Dictionary<string, string> header_dic = new();

    public static string json_body_value = string.Empty;

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

    /// <summary>
    /// Post请求
    /// </summary>
    /// <param name="content_form">content-type类型 1-application/json 2-form-data 3-multipart-form-data</param>
    /// <param name="timeout_span">请求超时时间 默认为5s</param>
    /// <param name="configure_await">用于UI界面的等待 默认为false</param>
    /// <param name="is_json_format">是否有json格式的body 默认为false</param>
    /// <returns></returns>
    public async static Task<string> PostSend(int content_form = 1, int timeout_span = 5000,
        bool configure_await = false, bool is_json_format = false)
    {
        using var cli = new RestClient();

        var rest = new RestRequest(_uri_fetch, Method.Post);

        var params_fetch =
            new ParametersCollection(param_dic.Distinct().Select(x =>
                Parameter.CreateParameter(x.Key, x.Value, ParameterType.RequestBody)));

        rest.Timeout = timeout_span;

        var rest_json_parameter = is_json_format ? JObject.Parse(json_body_value) : new JObject();

        if (is_json_format) rest.AddJsonBody(rest_json_parameter);

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