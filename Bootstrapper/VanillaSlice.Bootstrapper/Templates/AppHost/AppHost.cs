var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.{{ProjectNameUnderScored}}_WebPortal>("{{ProjectNameHyphened}}-webportal");

builder.AddProject<Projects.{{ProjectNameUnderScored}}_WebAPI>("{{ProjectNameHyphened}}-webapi");

builder.Build().Run();
