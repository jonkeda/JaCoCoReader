$here = Split-Path -Parent $MyInvocation.MyCommand.Path
. "$here\Calculate.ps1"

Describe -Tags "Example" "Calculate" {

    It "Calculate addition numbers" {
        Add-Numbers 2 '+' 3  | Should Be 6
    }
}

