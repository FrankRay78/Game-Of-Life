$env:SPECIFY_FEATURE = '002-ci-build-test'
& "$PSScriptRoot\check-prerequisites.ps1" -Json -RequireTasks -IncludeTasks
