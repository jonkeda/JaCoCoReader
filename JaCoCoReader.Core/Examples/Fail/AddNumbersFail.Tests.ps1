$here = Split-Path -Parent $MyInvocation.MyCommand.Path
. "$here\AddNumbersFail.ps1"

Describe -Tags "Example" "Add-Numbers" {

    It "adds positive numbers" {
        Add-Numbers 2 3 | Should Be 6
    }
}

Describe -Tags "Example" "Add-Numbers2" {

    It "adds positive numbers2" {
        Add-Numbers 2 3 | Should Be 6
    }
}
