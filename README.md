# AntiXssUF
跨站脚本攻击（XSS）过滤器，以白名单的过滤策略，支持多种过滤策略，可以根据业务场景选择适合的过滤策略，或者根据用户角色动态绑定过滤策略，支持OwaspAntisamy项目的配置，支持json格式的配置；
使用方法：
       1. 在启动类Startup.cs上添加依赖注入
        public void ConfigureServices(IServiceCollection services)
        {
            //添加策略和设置默认策略
            services.AddXssFilter(opt=>opt.DefaultSchemeName= "DefaultPolicy")
                .AddScheme<AntisamyPolicy>("antisamy", () => File.ReadAllTextAsync(Path.Combine(HostEnvironment.ContentRootPath, "resources/antisamy.xml")))
                .AddScheme<AntisamyPolicy>("anythinggoes", () => File.ReadAllTextAsync(Path.Combine(HostEnvironment.ContentRootPath, "resources/antisamy-anythinggoes.xml")))
                .AddScheme<AntisamyPolicy>("ebay", () => File.ReadAllTextAsync(Path.Combine(HostEnvironment.ContentRootPath, "resources/antisamy-ebay.xml")))
                .AddScheme<AntisamyPolicy>("myspace", () => File.ReadAllTextAsync(Path.Combine(HostEnvironment.ContentRootPath, "resources/antisamy-myspace.xml")))
                .AddScheme<AntisamyPolicy>("slashdot", () => File.ReadAllTextAsync(Path.Combine(HostEnvironment.ContentRootPath, "resources/antisamy-slashdot.xml")))
                .AddScheme<AntisamyPolicy>("test", () => File.ReadAllTextAsync(Path.Combine(HostEnvironment.ContentRootPath, "resources/antisamy-test.xml")))
                .AddScheme<JsonFilterPolicy>("DefaultPolicy", () => File.ReadAllTextAsync(Path.Combine(HostEnvironment.ContentRootPath, "resources/DefaultPolicy.json")));
            ;
            //添加模型绑定器
            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new RichTextBinderProvider());
            });
            services.AddControllersWithViews();
        }
        2.在构造函数注入依赖
        //依赖注入
        public HomeController(IFilterPolicyFactory policyFactory)
        {
            this.policyFactory = policyFactory;
        }
        public async Task<IActionResult> Test(string source) {
            var policyName="ebay"//策略名称
            var filter=await policyFactory.CreateHtmlFilter(policyName);//创建过滤器
            var clean = filter.Filters(source);//过滤危险代码
            return Content(clean);
        }
        //模型绑定过滤策略
        public class TestModel
        {
           public string Name { get; set; }
            [XssSchemeName("ebay")]
            public RichText RichText { get; set; }
        }
        public IActionResult Test(TestModel model)
        {
            string clean = model?.RichText;//这里自动过滤危险代码
            return Content(clean??string.Empty);
        }
        //使用参数绑定过滤策略
        public IActionResult Test([XssSchemeName("ebay")] RichText richText)
        {
            string clean = richText;//这里自动过滤危险代码
            return Content(clean??string.Empty);
        }
        
        
