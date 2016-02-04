var target = Argument("target", "Default");
var solutionFiles = GetFiles("ParticleSDK.sln");
var outputDirectory = "Build";
var buildVersion = "1.2.3-beta-4";

Task("AppVeyorUpdate")
	.Does(() =>
{
	if(AppVeyor.IsRunningOnAppVeyor)
	{
		Information("Building on AppVeyor");
		buildVersion = AppVeyor.Environment.Build.Version;
	}
	else
	{
		Information("Not building on AppVeyor");
	}
});

Task("CleanUp")
	.Does(()=>
{
	if(System.IO.Directory.Exists(outputDirectory))
	{
		System.IO.Directory.Delete(outputDirectory, true);
	}
	
	System.IO.Directory.CreateDirectory(outputDirectory);
});

Task("UpdateAssemblyVersion")
	.Does(() =>
{
	var fixedVersionString = buildVersion.Replace("-beta-", ".");
	
	if(fixedVersionString.Split('.').Length == 3){
		fixedVersionString += ".0";
	}
	
	CreateAssemblyInfo("Particle\\Properties\\AssemblyVersion.cs", new AssemblyInfoSettings
	{
		Version = fixedVersionString,
		FileVersion = fixedVersionString
	});
});

Task("Build")
	.IsDependentOn("CleanUp")
	.IsDependentOn("AppVeyorUpdate")
	.IsDependentOn("UpdateAssemblyVersion")
	.Does(()=>
{
	foreach(var file in solutionFiles)
	{
		Information("Restoring {0}", file);
		NuGetRestore(file);	
		Information("Building {0}", file);
		MSBuild(file, settings => settings
			.WithProperty("OutputPath", String.Format("..\\{0}\\", outputDirectory))
			.SetPlatformTarget(PlatformTarget.MSIL)
			.SetConfiguration("Release"));
	}
});

Task("SignAssembly")
	.IsDependentOn("Build")
	.Does(()=>
{
	if(AppVeyor.IsRunningOnAppVeyor)
	{
		Sign(String.Format("{0}\\Particle.dll", outputDirectory), new SignToolSignSettings()
		{
			TimeStampUri = new Uri("http://timestamp.digicert.com"),
			CertPath = "pfx\\ParticleNET.pfx",
			Password = EnvironmentVariable("PFXPassword")
		});
	}
	else
	{
		Sign(String.Format("{0}\\Particle.dll", outputDirectory), new SignToolSignSettings()
		{
			TimeStampUri = new Uri("http://timestamp.digicert.com"),
			CertPath = "pfx\\local.pfx",
			Password = "localtest"
		});
	}
});

Task("NUnitTests")
	.Does(() =>
{
	NUnit3(GetFiles(String.Format("{0}\\*Tests*.dll", outputDirectory)));
});

Task("NuGetPack")
	.IsDependentOn("AppVeyorUpdate")
	.Does(() =>
{
	StringBuilder releaseNotes = new StringBuilder();
	releaseNotes.AppendLine(buildVersion);
	releaseNotes.AppendLine(System.IO.File.ReadAllText("CurrentReleaseNotes.txt"));
	releaseNotes.AppendLine();
	releaseNotes.AppendLine(System.IO.File.ReadAllText("PreviousReleaseNotes.txt"));
	NuGetPack("nuspec\\ParticleNET.ParticleSDK.nuspec", new NuGetPackSettings()
	{
		Version = buildVersion,
		OutputDirectory = outputDirectory,
		ReleaseNotes = new []{releaseNotes.ToString()}
	});
});

Task("AppVeyorArtifact")
	.Does(() =>
{
	if(AppVeyor.IsRunningOnAppVeyor)
	{
		AppVeyor.UploadArtifact(String.Format("{0}\\ParticleNET.*.nupkg"));
	}
});

Task("Default")
	.IsDependentOn("Build")
	.IsDependentOn("SignAssembly")
	.IsDependentOn("NUnitTests")
	.IsDependentOn("NuGetPack")
	.IsDependentOn("AppVeyorArtifact");

RunTarget(target);