# AntiXssUF
跨站脚本攻击（XSS）过滤器，以白名单的过滤策略，支持多种过滤策略，可以根据业务场景选择适合的过滤策略，或者根据用户角色动态绑定过滤策略，支持OwaspAntisamy项目的配置，支持json格式的配置；
使用方法：

### . 在启动类Startup.cs上添加依赖注入
    

```
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
        
```
        
### . 在构造函数注入依赖
      

```
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
        
```
### . 使用模型绑定器


```
        //模型绑定过滤策略
        public class TestModel
        {
           public string Name { get; set; }
            [XssSchemeName("ebay")]
            public RichText RichText { get; set; }
        }        
        
```

### . 在控制器上直接使用


```
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
        
```

### 不使用依赖注入，直接使用

1. 使用内置的默认策略

```
        //使用参数绑定过滤策略，这里需要添加模型绑定器
        public IActionResult Test(RichText richText)
        {
            string clean = richText;//这里自动过滤危险代码
            return Content(clean??string.Empty);
        }
        //这里不需要添加模型绑定器
        public IActionResult Test(string source)
        {
            RichText richText=source;
            string clean = richText;//这里自动过滤危险代码
            return Content(clean??string.Empty);
        }
        
```
2. 指定策略

```

        public IActionResult Test(string source)
        {
            var policy = new AntisamyPolicy();//json格式用JsonFilterPolicy类
            policy.Init(File.ReadAllText("c:/www/resources/antisamy-ebay.xml"),"ebay");
            var filter=new HtmlFilter(policy);
            var clean = filter.Filters(source);//过滤危险代码
            return Content(clean??string.Empty);
        }
        
```
        
