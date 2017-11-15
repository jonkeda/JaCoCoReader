Remove-Module Pester -Force -ErrorAction SilentlyContinue

import-module $PSScriptRoot\Pester.psm1  -Scope Global


#Invoke-Pester **,.\functions\**,.\functions\Assertions\** -CodeCoverage **.ps1,**.psm1,.\functions\*.ps1,.\functions\*.psm1,.\functions\Assertions\*.ps1,.\functions\Assertions\*.psm1 -detailedcodecoverage -CodeCoverageOutputFile c:\temp\pestercc.xml

#Invoke-Pester .\Pester.tests.ps1,.\Functions\Coverage.Tests.ps1 -detailedcodecoverage -CodeCoverageOutputFile pestercc.xml


Invoke-Pester -Path $PSScriptRoot\..\Examples\Validator\Validator.Tests.ps1  -TestName MyValidator -OutputFile c:\temp\pesteroutput.xml  -detailedcodecoverage -CodeCoverageOutputFile pestercc.xml

# -OutputFormat NUnitXml

# -detailedcodecoverage -CodeCoverageOutputFile pestercc.xml


# -detailedcodecoverage $false
