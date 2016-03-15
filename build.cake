var target = Argument("target", "Default");
var projects = new String[]{"NET","Portable","Win8","WP8","UWP"};
//"NET","Portable","Win8","WP8","UWP"
var platforms = new Dictionary<String, String[]>();
platforms.Add("NET", new String[]{"Any CPU"});
platforms.Add("Portable", new String[]{"Any CPU"});
platforms.Add("Win8", new String[]{"Any CPU"});
platforms.Add("WP8", new String[]{"Any CPU"});
platforms.Add("UWP", new String[]{"Any CPU"});

var outputDirectory = "Build";
var buildVersion = "1.2.3-beta-4";

Task("AppVeyorUpdate")
	.Does(() =>
{
	if(AppVeyor.IsRunningOnAppVeyor)
	{
		Information("Building on AppVeyor");
		buildVersion = AppVeyor.Environment.Build.Version;
		Information("Build Version is {0}", buildVersion);
	}
	else
	{
		Information("Not building on AppVeyor");
	}
});

Task("CleanUp")
	.Does(()=>
{
	CleanDirectories(outputDirectory);
});

Task("UpdateAssemblyVersion")
	.Does(() =>
{
	var fixedVersionString = buildVersion.Replace("-beta-", ".").Replace("-alpha-",".");
	
	if(fixedVersionString.Split('.').Length == 3){
		fixedVersionString += ".0";
	}
	
	CreateAssemblyInfo("NET\\Particle.NET\\Properties\\AssemblyVersion.cs", new AssemblyInfoSettings
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
	var fs = new FileSystem();
	foreach(var project in projects)
	{
		var filePath = new FilePath(String.Format("{0}\\ParticleSDK.{0}.sln", project));
		var outputDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(outputDirectory, project));
		Information("Start Building {0}", project);
		Information("\tRestoring");
		var argument = String.Format("restore {0}", filePath.FullPath); 
		Information("\t\tRunning {0}", argument);
		var result = StartProcess(".nuget/nuget.exe", argument);
		Information("\tBuilding");
		if(platforms.ContainsKey(project))
		{
			foreach(var plat in platforms[project])
			{
				result = StartProcess("C:\\Program Files (x86)\\MSBuild\\14.0\\bin\\msbuild.exe", String.Format("{0} /p:Configuration=Release /p:Platform=\"{1}\" /p:OutDir=\"{2}\\{1}\"", filePath, plat, outputDir));
			}
		}
	}
});

Task("SignAssembly")
	.IsDependentOn("Build")
	.Does(()=>
{
	if(AppVeyor.IsRunningOnAppVeyor)
	{
		Sign(String.Format("{0}\\NET\\Any CPU\\Particle.dll", outputDirectory), new SignToolSignSettings()
		{
			TimeStampUri = new Uri("http://timestamp.digicert.com"),
			CertPath = "pfx\\ParticleNET.pfx",
			Password = EnvironmentVariable("PFXPassword")
		});
		Sign(String.Format("{0}\\Portable\\Any CPU\\Particle.Portable.dll", outputDirectory), new SignToolSignSettings()
		{
			TimeStampUri = new Uri("http://timestamp.digicert.com"),
			CertPath = "pfx\\ParticleNET.pfx",
			Password = EnvironmentVariable("PFXPassword")
		});
	}
	else
	{
		Sign(String.Format("{0}\\NET\\Any CPU\\Particle.dll", outputDirectory), new SignToolSignSettings()
		{
			TimeStampUri = new Uri("http://timestamp.digicert.com"),
			CertPath = "pfx\\local.pfx",
			Password = "localtest"
		});
		Sign(String.Format("{0}\\Portable\\Any CPU\\Particle.Portable.dll", outputDirectory), new SignToolSignSettings()
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
	NUnit3(GetFiles(String.Format("{0}\\*Tests*.dll", outputDirectory)), new NUnit3Settings()
	{
		Results = String.Format("{0}\\TestResults.xml", outputDirectory)
	});
});

Task("NuGetPack")
	.IsDependentOn("AppVeyorUpdate")
	.Does(() =>
{
	var local = buildVersion;
	var index = 0;
	if((index = local.IndexOf("-beta-")) > -1)
	{
		var version = long.Parse(local.Substring(index + 6));
		local = String.Format("{0}{1:0000}", local.Substring(0, index+6), version);
	}
	if((index = local.IndexOf("-alpha-")) > -1)
	{
		var version = long.Parse(local.Substring(index + 7));
		local = String.Format("{0}{1:0000}", local.Substring(0, index+7), version);
	}
	StringBuilder releaseNotes = new StringBuilder();
	releaseNotes.AppendLine(local);
	releaseNotes.AppendLine(System.IO.File.ReadAllText("CurrentReleaseNotes.txt"));
	releaseNotes.AppendLine();
	releaseNotes.AppendLine(System.IO.File.ReadAllText("PreviousReleaseNotes.txt"));
	NuGetPack("nuspec\\ParticleNET.ParticleSDK.nuspec", new NuGetPackSettings()
	{
		Version = local,
		OutputDirectory = outputDirectory,
		ReleaseNotes = new []{releaseNotes.ToString()}
	});
	
	NuGetPack("nuspec\\ParticleNET.ParticleSDK.Portable.nuspec", new NuGetPackSettings()
	{
		Version = local,
		OutputDirectory = outputDirectory,
		ReleaseNotes = new []{releaseNotes.ToString()}
	});
});

Task("AppVeyorArtifact")
	.Does(() =>
{
	if(AppVeyor.IsRunningOnAppVeyor)
	{
		var files = GetFiles(String.Format("{0}\\**\\ParticleNET.*.nupkg"));
		foreach(var f in files)
		{
			AppVeyor.UploadArtifact(f);
		}
	}
});

Task("Default")
	.IsDependentOn("Build")
	.IsDependentOn("SignAssembly")
	.IsDependentOn("NuGetPack")
	.IsDependentOn("AppVeyorArtifact");
RunTarget(target);