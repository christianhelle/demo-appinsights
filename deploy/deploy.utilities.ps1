function Throw-WhenError {
  param (
    [string]
    $output
  )

  if ($LastExitCode -gt 0)
  {
    Write-Error $output
    throw
  }
}

function ConvertTo-PlainText {
  param (
    [Parameter(Mandatory=$true)]
    [SecureString]
    $secret
  )
    return [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($secret))
}