using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using VDT.Core.Operators.Examples;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

await builder.Build().RunAsync();

// TODO value generator method in stream ctor?
// TODO - with option to replay method or values only
// TODO error handling?
// TODO complete?
