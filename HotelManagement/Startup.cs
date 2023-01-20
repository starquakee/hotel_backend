using Essensoft.Paylink.Alipay;


namespace HotelManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 添加Paylink依赖注入
            services.AddAlipay();
            
            services.Configure<AlipayOptions>(Configuration.GetSection("Alipay"));


           // services.AddControllersWithViews();
        }

     
    }


}
