using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using VDT.Core.Operators.Examples;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

await builder.Build().RunAsync();
