# ServerSideAnalytics
AspNet Core component for server side analytics SSA

Live demo running at [https://matteofabbri.org/stat](https://matteofabbri.org/stat)

To use it invoke UseServerSideAnalytics in app startup

```csharp
// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
public void Configure(IApplicationBuilder app)
{
    app.UseServerSideAnalytics(new MongoRequestStore())
            .ExcludePath("/content")
            .ExcludeExtension(".js")
            .ExcludeExtension(".css")
            .ExcludeLoopBack()
            .UseGeoIp(IpInfo.Resolve);
}
```

To store request implement the IWebRequestStore.
There are some already implemented stores available on Nuget:

[https://www.nuget.org/packages/ServerSideAnalytics.Mongo](https://www.nuget.org/packages/ServerSideAnalytics.Mongo)
[https://www.nuget.org/packages/ServerSideAnalytics.SqlServer](https://www.nuget.org/packages/ServerSideAnalytics.SqlServer)
[https://www.nuget.org/packages/ServerSideAnalytics.Sqlite](https://www.nuget.org/packages/ServerSideAnalytics.Sqlite)


Any help into design and implementation of stores is welcome 💗

```csharp
public interface IWebRequestStore<T> where T : IWebRequest
{
   T GetNew();
   Task AddAsync(T request);
}
```


