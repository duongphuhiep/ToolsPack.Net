msbuild /p:Configuration=Release
nuget pack ToolsPack.Log4net\ToolsPack.Log4net.csproj -IncludeReferencedProjects -Prop Configuration=Release
nuget pack ToolsPack.Displayer\ToolsPack.Displayer.csproj -IncludeReferencedProjects -Prop Configuration=Release
nuget pack ToolsPack.Sql\ToolsPack.Sql.csproj -IncludeReferencedProjects -Prop Configuration=Release
nuget pack ToolsPack.Thread\ToolsPack.Thread.csproj -IncludeReferencedProjects -Prop Configuration=Release
nuget pack ToolsPack.Config\ToolsPack.Config.csproj -IncludeReferencedProjects -Prop Configuration=Release