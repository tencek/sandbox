namespace klubovna.api

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Swashbuckle.AspNetCore
open Swashbuckle.AspNetCore.Swagger

type Startup private () =
    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection) =
        // Add framework services.
        services.AddMvc() |> ignore
        services.AddSwaggerGen(fun c -> 
            c.SwaggerDoc("v1", new Info(Title="Klubovna API", Version="v1"))) |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IHostingEnvironment) =
        app.UseMvc() |> ignore
        app.UseSwagger() |> ignore
        app.UseSwaggerUI(fun c -> 
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Klubovna API V1")) |> ignore

    member val Configuration : IConfiguration = null with get, set