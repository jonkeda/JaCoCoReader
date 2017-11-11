$here = Split-Path -Parent $MyInvocation.MyCommand.Path
. "$here\AddNumbersFail.ps1"

Describe -Tags "Example" "Add-Numbers" {

    It "adds positive numbers" {
        Add-Numbers 2 3 | Should Be 6
    }


}

