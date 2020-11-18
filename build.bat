
"C:\Users\frank\Documents\Professional\Software Development\Jenkins\nuget.exe" restore Game-Of-Life.sln
if %errorlevel% neq 0 exit /b %errorlevel%

"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild.exe" Game-Of-Life.sln /p:Configuration=Debug "/p:Platform=Any CPU" /t:clean;build
if %errorlevel% neq 0 exit /b %errorlevel%

"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\Extensions\TestPlatform\vstest.console.exe" "Game-Of-Life_TESTS\bin\Debug\Game-Of-Life_TESTS.dll" /TestAdapterPath:"packages\MSTest.TestAdapter.1.3.2" /ResultsDirectory:TestResults /logger:trx 
if %errorlevel% neq 0 exit /b %errorlevel%

packages\OpenCover.4.7.922\tools\OpenCover.Console.exe -register:user -target:"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\Extensions\TestPlatform\vstest.console.exe" -targetargs:"Game-Of-Life_TESTS\bin\Debug\Game-Of-Life_TESTS.dll /TestAdapterPath:packages\MSTest.TestAdapter.1.3.2" -filter:"+[*]* -[*TESTS]*" -output:TestResults\OpenCover.XML -skipautoprops
if %errorlevel% neq 0 exit /b %errorlevel%

REM packages\OpenCoverToCoberturaConverter.0.3.4\tools\OpenCoverToCoberturaConverter.exe -input:TestResults\coverage.xml -output:TestResults\Cobertura.xml
REM if %errorlevel% neq 0 exit /b %errorlevel%

packages\ReportGenerator.4.8.0\tools\net47\ReportGenerator.exe -reports:TestResults\OpenCover.xml -targetdir:TestResults\ReportGenerator -reportTypes:"Html;HtmlChart;HtmlSummary;Cobertura;"
