namespace VsTemplar
module internal SampleData=

    [<Literal>]
    let VsProject = 
        """
        <Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
          <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
          <PropertyGroup>
            <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
            <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
            <ProjectGuid>{006BC5D6-E8BF-40C9-9C9C-A55685624E43}</ProjectGuid>
            <OutputType>Library</OutputType>
            <AppDesignerFolder>Properties</AppDesignerFolder>
            <RootNamespace>YourProjectNameGoesHere</RootNamespace>
            <AssemblyName>YourProjectNameGoesHere</AssemblyName>
            <TargetFrameworkVersion>v4.0</TargetFrameworkVersion>
            <FileAlignment>512</FileAlignment>
            <TargetFrameworkProfile />
          </PropertyGroup>
          <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
            <DebugSymbols>true</DebugSymbols>
            <DebugType>full</DebugType>
            <Optimize>false</Optimize>
            <OutputPath>bin\Debug\</OutputPath>
            <DefineConstants>DEBUG;TRACE</DefineConstants>
            <ErrorReport>prompt</ErrorReport>
            <WarningLevel>4</WarningLevel>
          </PropertyGroup>
          <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
            <DebugType>pdbonly</DebugType>
            <Optimize>true</Optimize>
            <OutputPath>bin\Release\</OutputPath>
            <DefineConstants>TRACE</DefineConstants>
            <ErrorReport>prompt</ErrorReport>
            <WarningLevel>4</WarningLevel>
          </PropertyGroup>
          <ItemGroup>
            <Reference Include="AutoMapper">
              <HintPath>..\..\packages\AutoMapper.2.2.1\lib\net40\AutoMapper.dll</HintPath>
            </Reference>
            <Reference Include="Microsoft.Data.Edm, Version=5.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
              <SpecificVersion>False</SpecificVersion>
              <HintPath>..\..\packages\Microsoft.Data.Edm.5.2.0\lib\net40\Microsoft.Data.Edm.dll</HintPath>
            </Reference>
            <Reference Include="Microsoft.Data.OData, Version=5.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
              <SpecificVersion>False</SpecificVersion>
              <HintPath>..\..\packages\Microsoft.Data.OData.5.2.0\lib\net40\Microsoft.Data.OData.dll</HintPath>
            </Reference>
            <Reference Include="Microsoft.Web.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
              <Private>True</Private>
              <HintPath>..\..\packages\Microsoft.Web.Infrastructure.1.0.0.0\lib\net40\Microsoft.Web.Infrastructure.dll</HintPath>
            </Reference>
            <Reference Include="Newtonsoft.Json, Version=4.5.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed, processorArchitecture=MSIL">
              <SpecificVersion>False</SpecificVersion>
              <HintPath>..\..\packages\Newtonsoft.Json.4.5.11\lib\net40\Newtonsoft.Json.dll</HintPath>
            </Reference>
            <Reference Include="System" />
            <Reference Include="System.Core" />
            <Reference Include="System.Net.Http, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL">
              <HintPath>..\..\packages\Microsoft.Net.Http.2.0.20710.0\lib\net40\System.Net.Http.dll</HintPath>
            </Reference>
            <Reference Include="System.Net.Http.Formatting, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
              <HintPath>..\..\packages\Microsoft.AspNet.WebApi.Client.4.0.30506.0\lib\net40\System.Net.Http.Formatting.dll</HintPath>
            </Reference>
            <Reference Include="System.Net.Http.WebRequest">
              <HintPath>..\..\packages\Microsoft.Net.Http.2.0.20710.0\lib\net40\System.Net.Http.WebRequest.dll</HintPath>
              <Private>True</Private>
            </Reference>
            <Reference Include="System.Spatial, Version=5.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
              <SpecificVersion>False</SpecificVersion>
              <HintPath>..\..\packages\System.Spatial.5.2.0\lib\net40\System.Spatial.dll</HintPath>
            </Reference>
            <Reference Include="System.Web.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
              <HintPath>..\..\packages\Microsoft.AspNet.WebApi.Core.4.0.30506.0\lib\net40\System.Web.Http.dll</HintPath>
            </Reference>
            <Reference Include="System.Web.Http.OData, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
              <SpecificVersion>False</SpecificVersion>
              <HintPath>..\..\packages\Microsoft.AspNet.WebApi.OData.4.0.30506\lib\net40\System.Web.Http.OData.dll</HintPath>
            </Reference>
            <Reference Include="System.Web.Http.WebHost, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
              <HintPath>..\..\packages\Microsoft.AspNet.WebApi.WebHost.4.0.30506.0\lib\net40\System.Web.Http.WebHost.dll</HintPath>
            </Reference>
            <Reference Include="System.Xml.Linq" />
            <Reference Include="System.Data.DataSetExtensions" />
            <Reference Include="Microsoft.CSharp" />
            <Reference Include="System.Data" />
            <Reference Include="System.Xml" />
          </ItemGroup>
          <ItemGroup>
            <Compile Include="GenericTransportMapper.cs" />
            <Compile Include="Properties\AssemblyInfo.cs" />
            <Compile Include="Security\NullUserContextProvider.cs" />
            <Compile Include="SampleResourceApiService.cs" />
          </ItemGroup>
	      <ItemGroup>
		    <Compile Include="anotherCompileItem.cs" />
	      </ItemGroup>
	      <ItemGroup>
		    <Content Include="Global.asax" />
		    <Content Include="Web.config">
			    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
		    </Content>
		    <Content Include="Web.Debug.config">
			    <DependentUpon>Web.config</DependentUpon>
			    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
		    </Content>
		    <Content Include="Web.Release.config">
			    <DependentUpon>Web.config</DependentUpon>
			    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
		    </Content>
	      </ItemGroup>
	        <ItemGroup>
		        <Content Include="packages.config" />
	        </ItemGroup>
	        <ItemGroup>
		        <Folder Include="App_Data\" />
		        <Folder Include="Scripts\" />
	        </ItemGroup>
          <ItemGroup>
            <ProjectReference Include="..\ReferencedProject\ReferencedProject.csproj">
              <Project>{8B3EFAFB-02B6-4FD9-85C6-8F7BF9B9A5BA}</Project>
              <Name>ReferencedProject</Name>
            </ProjectReference>
            <ProjectReference Include="..\ReferencedProject2\ReferencedProject2.csproj">
              <Project>{F1BE99C5-041C-4366-9E74-0AD3632A8611}</Project>
              <Name>ReferencedProject2</Name>
            </ProjectReference>
          </ItemGroup>
          <ItemGroup>
            <None Include="packages.config" />
		        <None Include="anotherNoneItem.config" />
	        </ItemGroup>
	        <ItemGroup>
		        <None Include="yetAnotherNoneItem.config" />
	        </ItemGroup>
	        <ItemGroup>
		        <EmbeddedResource Include="css\HelpPage.css" />
		        <EmbeddedResource Include="img\iQWallpaper_16x9_LogoTopRight_Light.jpg" />
	        </ItemGroup>
          <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
          <!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
               Other similar extension points exist, see Microsoft.Common.targets.
          <Target Name="BeforeBuild">
          </Target>
          <Target Name="AfterBuild">
          </Target>
          -->
        </Project>

    """

    [<Literal>]
    let VsTemplate = 
        """
        <VSTemplate Version="3.0.0" xmlns="http://schemas.microsoft.com/developer/vstemplate/2005" Type="Project">
          <TemplateData>
            <Name>YourProjectNameGoesHere</Name>
            <Description>&lt;No description available&gt;</Description>
            <ProjectType>FSharp</ProjectType>
            <ProjectSubType>
            </ProjectSubType>
            <SortOrder>1000</SortOrder>
            <CreateNewFolder>true</CreateNewFolder>
            <DefaultName>YourProjectNameGoesHere</DefaultName>
            <ProvideDefaultName>true</ProvideDefaultName>
            <LocationField>Enabled</LocationField>
            <EnableLocationBrowseButton>true</EnableLocationBrowseButton>
            <Icon>__TemplateIcon.ico</Icon>
            <RequiredFrameworkVersion>4.5.3.1</RequiredFrameworkVersion>
          </TemplateData>
          <TemplateContent>
            <Project TargetFileName="YourProjectNameGoesHere.csproj" File="YourProjectNameGoesHere.csproj" ReplaceParameters="true">
              <ProjectItem ReplaceParameters="true" TargetFileName="GenericTransportMapper.cs">GenericTransportMapper.cs</ProjectItem>
              <ProjectItem ReplaceParameters="true" TargetFileName="packages.config">packages.config</ProjectItem>
              <Folder Name="Properties" TargetFolderName="Properties">
                <ProjectItem ReplaceParameters="true" TargetFileName="AssemblyInfo.cs">AssemblyInfo.cs</ProjectItem>
              </Folder>
              <Folder Name="Security" TargetFolderName="Security">
                <ProjectItem ReplaceParameters="true" TargetFileName="NullUserContextProvider.cs">NullUserContextProvider.cs</ProjectItem>
              </Folder>
              <ProjectItem ReplaceParameters="true" TargetFileName="TemplateResourceApiService.cs">TemplateResourceApiService.cs</ProjectItem>
            </Project>
            <ProjectCollection>
	         <ProjectTemplateLink ProjectName="$safeprojectname$.Model">PutYourApiNameHere.Model\MyTemplate.vstemplate</ProjectTemplateLink>
	         <ProjectTemplateLink ProjectName="$safeprojectname$.ApiServices">PutYourApiNameHere.ApiServices\MyTemplate.vstemplate</ProjectTemplateLink>      
	         <ProjectTemplateLink ProjectName="$safeprojectname$.Documentation">PutYourApiNameHere.Documentation\MyTemplate.vstemplate</ProjectTemplateLink>
	         <ProjectTemplateLink ProjectName="$safeprojectname$.WebApi">PutYourApiNameHere.WebApi\MyTemplate.vstemplate</ProjectTemplateLink>
	         <ProjectTemplateLink ProjectName="$safeprojectname$.IntegrationTests">PutYourApiNameHere.IntegrationTests\MyTemplate.vstemplate</ProjectTemplateLink>
            </ProjectCollection>
          </TemplateContent>
           <WizardExtension>
            <Assembly>TemplateWizardAssemblyName, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c9a76f51a8a9555f</Assembly>
            <FullClassName>TemplateWizardAssemblyName.Wizard.ChildWizard</FullClassName>
          </WizardExtension>
          <WizardData>
          </WizardData>
        </VSTemplate>
        """
