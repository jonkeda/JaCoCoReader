$here = Split-Path -Parent $MyInvocation.MyCommand.Path
. "$here\Calculate.ps1"

Describe -Tags "Example" "Calculate" {

    It "Calculate addition numbers Fail" {
        Calculate-Numbers 2 '+' 3  | Should Be 6
    }

    It "Calculate addition numbers" {
        Calculate-Numbers 2 '+' 3  | Should Be 5
    }
}

