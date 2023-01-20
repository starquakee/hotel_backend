using HotelManagement.DTO;
using Microsoft.AspNetCore.Mvc;
using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;

namespace HotelManagement.Controllers
{   //APPID

    [ApiController, Route("api/[controller]")]
    public class AlipayController : ControllerBase
    {
        public static string app_id = "2021000122602566";
        // 支付宝网关
        public static string gatewayUrl = "https://openapi.alipaydev.com/gateway.do";
        // 商户私钥，您的原始格式RSA私钥
        public static string private_key = "MIIEogIBAAKCAQEAvrtaHZCgajbb1XYmqcTyMzUEPt2c8BzfkS/JDRhGjj/kkIHNOXirHKZA1WSntJVsRswAFPe9BqQmvcqel6tziEcZZn7PcVAwj3RRNp6zoE/b7+ZynN/1hRIYRW8LkuWNPpxk4GGObDFZLRpAxXuE/AaJqt2Or0jqzsxaa/EpmzRqH22AFPzea/beDS+IT411OXeU1PRa6nhnAVsCm/MHFAj7o2SEkF/mJW9kmhiq58tTdVSRe2IYwyVMIzJTQAvdjVPBy91dzhrkfKzO2ms2utLvW8FTL2Klf2b4RWqP+OsuFfo9SpWBbhNp1mxFnt44OWsGlcgOG6pvVaAlCmocRwIDAQABAoIBAEGKL5UpNXZKE5BuYSrFOlTOSv8vN9ZqlYhWW0fcp6IJ4oilkdfF10ao+m5ZgCdVkTdiskSCPLojfgyJq6WCjNivVdMakD3nyEgDOEUEk8TqrDDrh0bQzpVDeoOhfOClcLurEh7oZwBWlMYi108E+tV/iZ7lMzqYW9dj8U5WOG3UJH6Navoldbjp4q39n52rycwTZD348vV6YzZQh7lOlw0ypvvktN1nxcUGxb61KNkiv1thmOQRkhjX8lS5voZoseVG6tPDQ1858Q243dTp1RphN/h9Gi+fT4/Kv7GTavwx2TH6I9U7jQqi7DUhlMAUx3VAMn68v8I7xuivH4IvQoECgYEA8n3H7QQz3+aSbS12pQeyQOMcK8Miw0dP8n3/RTmaTsNQCY6rkiNFnl3gRL/s40vRQAJSsX/XEpHWM7T/JZmJUlX7lHkP5XHWYFyfjagUsozrqUjnus03JfPz+6yttDhP4Tnk0WsDRI8RWnsnO20rmJ05aE+yOAIetKp/f0YQAK8CgYEAyVtrFO9QUQX86KUnC4e/1k40TVElVrka5aLfWrEe99cdTCEjtBt5u5WUo+2CLdjGmPj0ANbGVr0+mVUNw7O0juB8diMUgVXXYh3wNDedypxqkP1rzHBuQitjiFMKj7rSiATg4dTY35X7hn03SUne6surTPv6Bn3+8F02j+0bk+kCgYAcRa/+oWPO8hoWYpuXqCsPR6tsKsctz22xzyDpGEs2ba0yQBEe67/dNALG4T2kTp2qAdtUWJxhzOEVjD/HSXxqPh4j9G13Ceu9tm7f3D31h1qelSJ3dB++7A+BQ2PJRFuD3fUguYJbBvc1/m/XjOXtWD9uOwSDZTPhSpOs6iVJfwKBgBFAauFFS71VZQDNvZWoUNBcrPB+5lMS0vIfzUEo8b1MSe9O248/12MyAqU04TVqYEGU+trji+S8nBNpDR/aUrr3EtLQWJ5oK2b1p7HhnfxRAHRhsg23CEtVClcQvlPoLseGm3nd0aL+Z6lzKvx1fsrhHlEfOaG6w8/2PImCSB2ZAoGAAoXupjWRLBUVdS/+WGLj7Zfp13Co6tKE8tNALP0774YoFgxPPmyA92Jl9anob6WcxpUx5ksRU+Vozir2f+oGQ/xhrBDcIK9iiBIbh6H5RAuMfco3laazdKc7uFHGm6wN+mEUiVQ+UMi/8fYhnMsOj83xxI+ahOVL3XB4ON9v+p4=";
        // 支付宝公钥,查看地址：https://openhome.alipay.com/platform/keyManage.htm 对应APPID下的支付宝公钥。
        public static string alipay_public_key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAyCAqn8GT6YnjjE7MxRwWyqfUimFdwj6iiRzwMIWEqfGiNchduYu5hloc3BIb0ia/eDMN/pe+yf79WfxaQjIiU4tzDShiTgnV4Qnq6C0XiOub8ZQQogClXKvNC41d7lNz3jlovabCa72qpJBcMu1csgc+x4THnOgnI1n69FgaOpXu6fBXZQq1sTYGQJHWV/puyU4QTY9BVuqToO7OvdhO/ctwGNEsKHMMT0nnGF6b5ZpUsEpnmUcbz6kj+nnqVBcAe/lpFoVDRRyGi9KQvakEAZm+Wldik5nVIy3vQJ1khJMloSRyXjRa4dM6hztnlu4hrePo8i2qAUM6mXKJyt4EywIDAQAB";
        // 签名方式
        public static string sign_type = "RSA2";
        // 编码格式
        public static string charset = "UTF-8";

        [HttpPost("[action]")]
        public async Task<AlipayTradeWapPayResponse> Pay([FromBody] AlipayDto alipayDto)
        {
            DefaultAopClient client = new DefaultAopClient(gatewayUrl, app_id, private_key, "json", "2.0", sign_type, alipay_public_key, charset, false);
            string out_trade_no = alipayDto.UUID;
            string subject = "支付";
            string total_amount = alipayDto.money.ToString();
            string body = "房间";
            //支付失败跳转的地址
            string quit_url = "https://www.bilibili.com";
            //支付成功跳转的地址
            string success_url = "https://www.baidu.com";
            //支付完成异步通知接收地址
            //string notify_url = "";

            //组装业务参数model
            AlipayTradeWapPayModel model = new AlipayTradeWapPayModel();
            model.Body = body;
            model.Subject = subject;
            model.TotalAmount = total_amount;
            model.OutTradeNo = out_trade_no;
            model.ProductCode = "QUICK_WAP_WAY";
            model.QuitUrl = quit_url;

            AlipayTradeWapPayRequest request = new AlipayTradeWapPayRequest();
            request.SetReturnUrl(success_url);
            //request.SetNotifyUrl(notify_url);
            request.SetBizModel(model);
            AlipayTradeWapPayResponse response = null;
            /*try
            {*/
            response = client.pageExecute(request, null, "post");
            //Response.WriteAsync(response.Body);
            return response;
            /*}catch(Exception ex)
            {
                throw ex;
            }*/
        }

    }
}