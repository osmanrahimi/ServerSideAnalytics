# ServerSideAnalytics
AspNet Core component for server side analytics SSA

Live demo running at [https://matteofabbri.org/stat](https://matteofabbri.org/stat)

To use it invoke UseServerSideAnalytics in app startup

```csharp
// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
public void Configure(IApplicationBuilder app)
{
   app.UseServerSideAnalytics(new EventSourcingRepository("mydb://localhost"));
}
```

To store request implement the IWebRequestStore

```csharp
public interface IWebRequestStore<T> where T : IWebRequest
{
   T GetNew();
   Task AddAsync(T request);
}
```

There are some already implemented stores but at the moment they quite experimental.
Any help into design and implementation of stores is welcome 💗
