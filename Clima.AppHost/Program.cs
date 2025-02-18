var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Clima>("clima");

builder.Build().Run();
